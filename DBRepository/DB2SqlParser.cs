using System;
using System.Text.RegularExpressions;
using Constant;
using Helper;

namespace DBRepository
{
	public class DB2SqlParser : DBSqlParser
	{
		private static DB2SqlParser _instance;
		public static DB2SqlParser Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new DB2SqlParser ();
				}
				return _instance;
			}
		}

		private DB2SqlParser ()
		{
			// prevent new object outside
		}
		public override string ReadSqlParser (string jsonQuery)
		{
			//reset query
			this.ResetReadQuery ();

			jsonQuery = StringHelper.Simpler (jsonQuery, Pattern.PATTERN_SPECIAL_CHARS);

			Match m = Regex.Match (jsonQuery, DBPatternHelper.GetQueryPattern ());
			if (m.Success == true)
			{
				Group objGroup = m.Groups ["OBJECT"];
				Match objMatch = this.ReadObjParser (objGroup.Value);
				if (objMatch == null)
				{
					return null;
				}

				CaptureCollection attrCaps = m.Groups ["ATTRIBUTES"].Captures;
				foreach (Capture cap in attrCaps)
				{
					string attr = cap.Value;
					ReadAttributeParser (attr, objMatch);

				}

				CaptureCollection joinCaps = m.Groups ["JOIN_OBJECTS"].Captures;
				foreach (Capture cap in joinCaps)
				{
					this.ReadJoinParser (cap.Value);
				}

			}
			else
			{
				Console.WriteLine ("Match failed.");
			}
			return StringHelper.Simpler ($@"
					{this.Select}
					{this.From}
					{this.Join}
					{this.Where}
					{this.GroupBy}
					{this.Having}
					{this.OrderBy}
					{this.Limit}
					{this.Offset}
					");
		}

		public Match ReadObjParser (string objStr)
		{
			Match objMatch = Regex.Match (objStr, DBPatternHelper.GetQueryObjPattern ());
			if (objMatch.Success == true)
			{
				// 1. Parser table Identifier
				string objName = objMatch.Groups ["OBJ_NAME"].Value;
				if (string.IsNullOrEmpty (objName))
				{
					return null;
				}

				this.ObjName = objName;

				string tableName = objMatch.Groups ["OBJ_TABLE_NAME"].Value;
				tableName = string.IsNullOrEmpty (tableName) ? objName : tableName + " AS " + objName;
				this.From = $"FROM {tableName}";

				// 2. Parser table condition
				string condition = objMatch.Groups ["OBJ_CONDITION"].Value;
				if (!string.IsNullOrEmpty (condition))
				{
					this.Where = $"WHERE {condition}";
				}

				// 3. Parser pack
				string pack = objMatch.Groups ["OBJ_PACK"].Value;
				pack = (string.IsNullOrEmpty (pack) ? "ALL" : pack).ToUpper ();
				if (!string.IsNullOrEmpty (pack))
				{
					if (pack != "ALL")
					{
						Match packByPageMatch = Regex.Match (pack, DBPatternHelper.GetQueryObjPackPagePattern ());
						Match packByAmount = Regex.Match (pack, "^(?<AMOUNT>\\d+)$");
						if (packByPageMatch.Success == true)
						{
							// 1. pack by Page
							int pageIndex = Convert.ToInt32 (packByPageMatch.Groups ["PAGE_INDEX"].Value);
							int amount = Convert.ToInt32 (packByPageMatch.Groups ["AMOUNT"].Value);
							if (amount != 0)
							{
								this.Limit = $"LIMIT {amount}";
								this.Offset = $"OFFSET {(pageIndex - 1) * amount}";
							}
						}
						else if (packByAmount.Success == true)
						{
							// 2. pack by amount
							int amount = Convert.ToInt32 (packByAmount.Groups ["AMOUNT"].Value);
							if (amount != 0)
							{
								this.Limit = $"LIMIT {amount}";

								if (amount == 1)
								{
									this.IsReadToList = false;
								}
							}
						}

					}
				}
				return objMatch;
			}
			else
			{
				Console.WriteLine ("OBJ not match!");
				return null;
			}
		}

		public void ReadAttributeParser (string attrStr, Match objMatch)
		{
			// Get object value
			string objName = objMatch.Groups ["OBJ_NAME"].Value;
			string objTableName = objMatch.Groups ["OBJ_TABLE_NAME"].Value;
			objTableName = string.IsNullOrEmpty (objTableName) ? objName : objTableName;

			// Parser Attribute
			Match attrMatch = Regex.Match (attrStr, DBPatternHelper.GetQueryAttrPattern ());

			if (attrMatch.Success == true)
			{
				string attrName = attrMatch.Groups ["ATTR_NAME"].Value;
				string colDeclare = attrMatch.Groups ["COL_NAME"].Value;
				colDeclare = string.IsNullOrEmpty (colDeclare) ? attrName : colDeclare;
				string colName = colDeclare;
				bool mustGroup = true;

				if (!string.IsNullOrEmpty (attrName))
				{
					// 1. Anotaion parser
					CaptureCollection anoCaps = attrMatch.Groups ["ANOTATIONS"].Captures;
					foreach (Capture anoCap in anoCaps)
					{
						Match anoMatch = Regex.Match (anoCap.Value, DBPatternHelper.GetQueryAttrAnotationPattern ());
						if (anoMatch.Success == true)
						{
							string anoName = anoMatch.Groups ["ANOTATION_NAME"].Value.ToUpper ();
							string anoMeta = anoMatch.Groups ["ANOTATION_META"].Value.ToUpper ();

							switch (anoName)
							{
								case "COUNT":
								case "SUM":
								case "MIN":
								case "MAX":
								case "AVG":
									colName = anoName + "(" + objName + "." + colName + ")";
									if (!string.IsNullOrEmpty (anoMeta))
									{
										anoMeta = $"{colName}{anoMeta}";
										if (string.IsNullOrEmpty (this.Having))
										{
											this.Having = $"HAVING {anoMeta}";
										}
										else
										{
											this.Having += $" AND {anoMeta}";
										}
									}
									// Calculator Funcs don't GROUP BY
									mustGroup = false;
									break;
								case "DISTINCT":
									colName = anoName + " " + objName + "." + colName;
									break;
								case "ORDER":
									string orderVal = attrName;
									switch (anoMeta)
									{
										case "DESC":
											orderVal += $" {anoMeta}";
											break;
										default:
											break;
									}

									if (string.IsNullOrEmpty (this.OrderBy))
									{
										this.OrderBy = $"ORDER BY {orderVal}";
									}
									else
									{
										this.OrderBy += $", {orderVal}";
									}
									break;
								default:
									break;

							}
						}
					}

					//check must GROUP
					if (mustGroup)
					{
						if (string.IsNullOrEmpty (this.GroupBy))
						{
							this.GroupBy = $"GROUP BY {objName}.{colDeclare}";
						}
						else
						{
							this.GroupBy += $", {objName}.{colDeclare}";
						}

					}

					// 2. Name Parser
					attrName = (colName != colDeclare ? colName : objName + "." + colName) + " AS " + (this.IsJoinView ? colDeclare : attrName);
					if (string.IsNullOrEmpty (this.Select))
					{
						this.Select = $"SELECT {attrName}";
					}
					else
					{
						this.Select += $", {attrName}";
					}
				}
			}
			else
			{
				Console.WriteLine ("Attributes not match!");
			}
		}

		public void ReadJoinParser (string joinStr)
		{
			Match joinMatch = Regex.Match (joinStr, DBPatternHelper.GetQueryJoinPattern ());
			if (joinMatch.Success == true)
			{

				string joinType = joinMatch.Groups ["JOIN_TYPE"].Value.ToUpper ();
				string joinCondition = joinMatch.Groups ["JOIN_CONDITION"].Value;

				string joinAttrName = joinMatch.Groups ["JOIN_ATTR_NAME"].Value;
				string joinTable = joinMatch.Groups ["JOIN_TABLE_NAME"].Value;
				joinTable = string.IsNullOrEmpty (joinTable) ? joinAttrName : joinTable;

				string joinTableCondition = string.Empty;

				string joinPack = joinMatch.Groups ["JOIN_PACK"].Value;

				//check IsReadJoinByView
				if (this.IsReadJoinByView (joinMatch))
				{
					// JOIN by VIEW
					string joinViewQuery = joinMatch.Groups ["JOIN_TABLE"].Value;
					DB2SqlParser joinViewParser = new DB2SqlParser ();
					joinViewParser.IsJoinView = true;
					joinTable = "(" + joinViewParser.ReadSqlParser (joinViewQuery) + ")";
				}
				else
				{
					// JOIN by TABLE
					joinTableCondition = joinMatch.Groups ["JOIN_TABLE_CONDITION"].Value;
				}

				// check join type
				switch (joinType)
				{
					case "LEFT":
					case "RIGHT":
					case "INNER":
					case "OUTER":
						break;
					default:
						joinType = "INNER";
						break;
				}

				// JOIN
				this.Join += $" {joinType} JOIN {joinTable} AS {joinAttrName} ON {joinCondition}";
				if (!string.IsNullOrEmpty (joinTableCondition))
				{
					this.Join += $" AND ({joinTableCondition})";
				}

				// GENERATE SELECT LISTAGG
				string joinAgg = this.ReadJoinAGG (joinMatch);
				if (joinAgg != null)
				{
					if (string.IsNullOrEmpty (this.Select))
					{
						this.Select = $"SELECT {joinAgg}";
					}
					else
					{
						this.Select += $", {joinAgg}";
					}
				}
			}
			else
			{
				Console.WriteLine ("Join not match!");
			}
		}

		// detect JOIN by VIEW (SELECT) or JOIN by TABLE
		public bool IsReadJoinByView (Match joinMatch)
		{
			//check JOIN PACK
			string joinPack = joinMatch.Groups ["JOIN_PACK"].Value;
			//defaul join pack is all
			joinPack = string.IsNullOrEmpty (joinPack) ? "ALL" : joinPack;
			if (joinPack.ToUpper () != "ALL")
			{
				return true;
			}
			//check Join Values has functions
			CaptureCollection joinValCaps = joinMatch.Groups ["JOIN_VALUES"].Captures;
			foreach (Capture cap in joinValCaps)
			{
				Match attrMatch = Regex.Match (cap.Value, DBPatternHelper.GetQueryAttrPattern ());
				if (attrMatch.Success == true)
				{
					CaptureCollection anoCaps = attrMatch.Groups ["ANOTATIONS"].Captures;
					foreach (Capture anoCap in anoCaps)
					{
						Match anoMatch = Regex.Match (anoCap.Value, DBPatternHelper.GetQueryAttrAnotationPattern ());
						if (anoMatch.Success == true)
						{
							string anoName = anoMatch.Groups ["ANOTATION_NAME"].Value;
							switch (anoName.ToUpper ())
							{
								case "MIN":
								case "MAX":
								case "COUNT":
								case "SUM":
								case "AVG":
									return true;
								default:
									return false;
							}
						}
					}
				}
			}
			return false;
		}

		public string ReadJoinAGG (Match joinMatch)
		{
			string listAgg = string.Empty;

			// get join meta
			string joinPack = joinMatch.Groups ["JOIN_PACK"].Value;
			string joinAttrName = joinMatch.Groups ["JOIN_ATTR_NAME"].Value;
			string joinTableName = joinMatch.Groups ["JOIN_TABLE_NAME"].Value;
			joinTableName = string.IsNullOrEmpty (joinTableName) ? joinAttrName : joinTableName;

			CaptureCollection joinValCaps = joinMatch.Groups ["JOIN_VALUES"].Captures;
			foreach (Capture cap in joinValCaps)
			{
				Match attrMatch = Regex.Match (cap.Value, DBPatternHelper.GetQueryAttrPattern ());
				if (attrMatch.Success == true)
				{
					string attrName = attrMatch.Groups ["ATTR_NAME"].Value;
					string colName = attrMatch.Groups ["COL_NAME"].Value;
					if (string.IsNullOrEmpty (listAgg))
					{
						listAgg = $"'\"{attrName}\":' || '\"' || {joinAttrName}.{colName} || '\"'";
					}
					else
					{
						listAgg += $" || ',' || '\"{attrName}\":' || '\"' || {joinAttrName}.{colName} || '\"'";
					}
				}
			}
			listAgg = string.IsNullOrEmpty (listAgg) ? listAgg : "LISTAGG(('{' || " + listAgg + " || '}'), ',')";
			return string.IsNullOrEmpty (listAgg) ? null : ((joinPack == "1") ? listAgg : $"('[' || {listAgg} || ']')") + " AS " + "JOIN_" + joinAttrName;
		}

        public override bool InsertSqlParser(string jsonObject, string value)
        {
            throw new NotImplementedException();
        }

        public override bool UpdateSqlParser(string jsonObject, string value)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteSqlParser(string jsonObject)
        {
            throw new NotImplementedException();
        }
    }
}
using System;
using System.Text.RegularExpressions;
using Constant;
using Helper;
using IBM.Data.DB2.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DBRepository
{
	public class DB2Repository : IRepository
	{
		private DB2Connection _connection;

		private DBSqlParser _sqlParser;

		public DB2Repository (string connectionString)
		{
			this._connection = new DB2Connection (connectionString);
			this._sqlParser = DB2SqlParser.Instance;
		}

		public void OpenConnection ()
		{
			if (this._connection != null)
			{
				this._connection.Open ();
			}
		}

		public void CloseConnection ()
		{
			if (this._connection != null)
			{
				this._connection.Close ();
			}
		}

		public string Read (string jsonQuery)
		{
			string selectSQL = this._sqlParser.ReadSqlParser (jsonQuery);
			string objName = this._sqlParser.ObjName;
			bool isReadToList = this._sqlParser.IsReadToList;

			string jsonDataSet = string.Empty;

			if (!string.IsNullOrEmpty (selectSQL))
			{
				DB2Command dbCmd = new DB2Command (selectSQL, this._connection);
				this.OpenConnection ();
				DB2DataReader dbReader = dbCmd.ExecuteReader ();
				while (dbReader.Read ())
				{
					// add LIST seperator jsonDataSet 
					if (isReadToList && !string.IsNullOrEmpty (jsonDataSet))
					{
						jsonDataSet += ",";
					}

					int fieldCount = dbReader.FieldCount;

					// open json row
					jsonDataSet += "{";
					for (int fieldIndex = 0; fieldIndex < fieldCount; fieldIndex++)
					{
						// GET NAME
						string fieldName = dbReader.GetName (fieldIndex);
						string JOIN_PREFIX = "JOIN_";

						bool isJoinField = false;
						if (fieldName.ToUpper ().StartsWith ("JOIN_"))
						{
							isJoinField = true;
							fieldName = fieldName.Substring (fieldName.IndexOf (JOIN_PREFIX) + JOIN_PREFIX.Length);
						}
						fieldName = $"\"{fieldName}\"";

						// GET VALUE
						string fieldValue = dbReader.IsDBNull (fieldIndex) ? "\"\"" : dbReader.GetString (fieldIndex);
						if (!isJoinField)
						{
							fieldValue = $"\"{fieldValue}\"";
						}

						jsonDataSet += $"{fieldName}:{fieldValue}";
						if (fieldIndex < fieldCount - 1)
						{
							jsonDataSet += ",";
						}
					}
					// close json row
					jsonDataSet += "}";
				}

				//close
				dbReader.Close ();
				dbReader.Dispose ();
				this.CloseConnection ();
			}
			return StringHelper.Simpler (string.IsNullOrEmpty (jsonDataSet) ? null : ("{" + $"\"{objName}\"" + ":" + ((isReadToList) ? "[" + jsonDataSet + "]" : jsonDataSet) + "}"), Pattern.PATTERN_SPECIAL_CHARS);
		}

		public void Delete (string jsonQuery)
		{

		}

		public string Update (string jsonObject, string value)
		{
			throw new NotImplementedException ();
		}

		public string Insert (string jsonObject, string value)
		{
			throw new NotImplementedException ();
		}
	}
}

namespace DBRepository
{
	public abstract class DBSqlParser : ISqlParser
	{
		public string ObjName { get; set; }

		public bool IsReadToList { get; set; } = true;

		public string Select { get; set; }

		public string From { get; set; }

		public string Join { get; set; }

		public string Where { get; set; }

		public string OrderBy { get; set; }

		public string GroupBy { get; set; }

		public string Having { get; set; }

		public string Limit { get; set; }

		public string Offset { get; set; }

		public bool IsJoinView { get; set; }

		//reset query value
		public void ResetReadQuery ()
		{
			this.ObjName = string.Empty;
			this.IsReadToList = true;

			this.Select = string.Empty;
			this.Select = string.Empty;
			this.From = string.Empty;
			this.Join = string.Empty;
			this.Where = string.Empty;
			this.OrderBy = string.Empty;
			this.GroupBy = string.Empty;
			this.Having = string.Empty;
			this.Limit = string.Empty;
			this.Offset = string.Empty;
		}

		public abstract string ReadSqlParser (string jsonQuery);

		public abstract bool InsertSqlParser(string jsonObject, string value);

		public abstract bool UpdateSqlParser(string jsonObject, string value);

		public abstract bool DeleteSqlParser(string jsonObject);
	}
}
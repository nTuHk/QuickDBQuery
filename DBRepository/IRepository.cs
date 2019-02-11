namespace DBRepository
{
	public interface IRepository
	{
		void OpenConnection ();

		void CloseConnection ();


		string Read (string jsonQuery);

		string Update(string jsonObject, string value);

		string Insert(string jsonObject, string value);

		void Delete (string jsonQuery);
	}
}
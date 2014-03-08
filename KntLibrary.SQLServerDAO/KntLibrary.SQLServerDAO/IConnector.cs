
namespace KntLibrary.DAO.Interface
{
	/// <summary>
	/// DataBase接続インターフェース
	/// </summary>
	public interface IConnector
	{
		void Open();

		void Close();
	}
}

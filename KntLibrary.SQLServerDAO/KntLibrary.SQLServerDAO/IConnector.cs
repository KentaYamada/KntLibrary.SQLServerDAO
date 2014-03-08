
namespace KntLibrary.SQLServerDAO
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

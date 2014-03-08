using System.Data;

namespace KntLibrary.DAO.Interface
{
	/// <summary>
	/// DBパラメータインターフェース
	/// </summary>
	public interface IParamCreator
	{
		object IsDBNull(object value);

		void Add(string paramName, DbType type, object value);
	}
}

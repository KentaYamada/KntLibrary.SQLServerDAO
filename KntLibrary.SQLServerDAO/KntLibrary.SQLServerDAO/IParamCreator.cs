using System.Data;

namespace KntLibrary.SQLServerDAO
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

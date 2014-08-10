using System.Data;
using System.Data.SqlClient;

namespace KntLibrary.SQLServerDAO.Interfaces
{
	public interface IParamCreation
	{
		void Add(string paramName, SqlDbType type, object value);

		void Add(string paramName, DataTable value);

		void Add<T>(T args) where T: class;

        void Clear();

        object ToDBNull(object value);
	}
}

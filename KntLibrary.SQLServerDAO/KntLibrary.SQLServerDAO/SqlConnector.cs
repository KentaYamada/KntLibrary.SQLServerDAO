using System;
using System.Configuration;
using System.Data.SqlClient;

using KntLibrary.SQLServerDAO.Interfaces;

namespace KntLibrary.SQLServerDAO
{
    internal class SqlConnector : IConnection, IDisposable
	{
        private SqlConnection _connection = null;

        internal SqlConnection GetConnection { get { return this._connection; }}

        internal SqlConnector() 
        {
	        this._connection = new SqlConnection();
            this._connection.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultDatabase"].ConnectionString;
        }

        internal SqlConnector(string connectionName)
        {
            if(string.IsNullOrWhiteSpace(connectionName))
            {
                throw new ArgumentNullException();
            }

            this._connection = new SqlConnection();
            this._connection.ConnectionString = ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;
        }

        public void Open()
        {
	        if (null != this._connection)
	        {
		        this._connection.Open();
	        }
        }

        public void Close()
        {
            this.Dispose();
        }

        public void Dispose()
        {
	        if (null != this._connection)
	        {
		        this._connection.Dispose();
	        }
        }

	}
}

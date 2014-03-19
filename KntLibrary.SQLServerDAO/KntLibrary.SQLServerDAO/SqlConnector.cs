using System;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;

namespace KntLibrary.SQLServerDAO
{
    /// <summary>
    /// MS SQL-Server connector class
    /// </summary>
	internal sealed class SqlConnector : IConnector, IDisposable
	{
		#region Fields

		/// <summary>Inner connection object</summary>
		private SqlConnection _InnerConnection = null;

		/// <summary>Get Connection Object</summary>
		internal SqlConnection Connection
		{
			get
			{
				return this._InnerConnection;
			}
		}

		#endregion

		#region Constructors

		internal SqlConnector() 
		{
			this._InnerConnection = new SqlConnection();
			this._InnerConnection.ConnectionString = ConfigurationManager.AppSettings["ConnectString"].ToString();
		}

        #endregion

		#region Public Methods

        /// <summary>
        /// Open Database
        /// </summary>
        public void Open()
		{
			if ( (null != this._InnerConnection) && (ConnectionState.Open != this._InnerConnection.State) )
			{
				this._InnerConnection.Open();
			}
		}

		/// <summary>
		/// Close up DataBase
		/// </summary>
		public void Close()
		{
			if ( (null != this._InnerConnection) && (ConnectionState.Closed != this._InnerConnection.State) )
			{
				this._InnerConnection.Close();
			}
		}

		/// <summary>
		/// Clean up Resource 
		/// </summary>
        public void Dispose()
		{
			if (ConnectionState.Closed != this._InnerConnection.State)
			{
				this._InnerConnection.Close();
			}

			if (null != this._InnerConnection)
			{
				this._InnerConnection.Dispose();
				this._InnerConnection = null;
			}
		}

		#endregion
	}
}

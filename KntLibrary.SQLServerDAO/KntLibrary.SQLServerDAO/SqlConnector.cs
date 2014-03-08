using System;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;

using KntLibrary.DAO.Interface;

namespace KntLibrary.DAO.SqlServer
{
    /// <summary>
    /// SQL Server接続クラス
    /// </summary>
	internal sealed class SqlConnector : IConnector, IDisposable
	{
		#region Fields

		/// <summary>DB接続内部保持</summary>
		private SqlConnection _InnerConnection = null;

		/// <summary>DB接続情報提供</summary>
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
		/// DataBase接続
		/// </summary>
        public void Open()
		{
			if ( (null != this._InnerConnection) && (ConnectionState.Open != this._InnerConnection.State) )
			{
				this._InnerConnection.Open();
			}
		}

		/// <summary>
		/// DataBase接続切断
		/// </summary>
		public void Close()
		{
			if ( (null != this._InnerConnection) && (ConnectionState.Closed != this._InnerConnection.State) )
			{
				this._InnerConnection.Close();
			}
		}

		/// <summary>
		/// リソース解放
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

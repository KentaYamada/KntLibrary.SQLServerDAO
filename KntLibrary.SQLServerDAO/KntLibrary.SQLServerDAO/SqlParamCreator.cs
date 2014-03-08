using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

using KntLibrary.DAO.Interface;

namespace KntLibrary.DAO.SqlServer
{
    /// <summary>
    /// SQL パラメータ生成クラス
    /// </summary>
    public sealed class SqlParamCreator : IParamCreator
	{
		#region Fields

		/// <summary>内部SQLパラメータリスト</summary>
		private List<SqlParameter> _InnerParameters = null;

		/// <summary>SQLパラメータ配列</summary>
		public SqlParameter[] Parameters
		{
			get
			{ 
				return this._InnerParameters.ToArray(); 
			}
		}

		#endregion

		#region Constructors

		public SqlParamCreator()
        {
            this._InnerParameters = new List<SqlParameter>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// DBNull値変換
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public object IsDBNull(object value)
        {
            if (string.IsNullOrWhiteSpace(Convert.ToString(value)))
            {
                return DBNull.Value;
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// パラメータ追加
        /// </summary>
        /// <param name="paramName">パラメータ名</param>
        /// <param name="dbType">マッピングタイプ</param>
        /// <param name="value">値</param>
        public void Add(string paramName, DbType dbType, object value)
        {
			var param = new SqlParameter();

			param.ParameterName = paramName;
			param.DbType = dbType;
			param.Value = this.IsDBNull(value);
			param.Direction = ParameterDirection.Input;

			this._InnerParameters.Add(param);
        }

		/// <summary>
		/// パラメータ追加
		/// </summary>
		/// <param name="paramName">パラメータ名</param>
		/// <param name="table">データテーブル</param>
		public void Add(string paramName, DataTable table)
		{
			var param = new SqlParameter();

			param.ParameterName = paramName;
			param.SqlDbType = SqlDbType.Structured;
			param.Value = table;
			param.Direction = ParameterDirection.Input;

			this._InnerParameters.Add(param);
		}

        #endregion
    }
}

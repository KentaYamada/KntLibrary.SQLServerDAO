using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace KntLibrary.SQLServerDAO
{
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
        /// Convert to DBNull.value
        /// </summary>
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
       
        public void Add(string paramName, DbType dbType, object value)
        {
			var param = new SqlParameter();

			param.ParameterName = paramName;
			param.DbType = dbType;
			param.Value = this.IsDBNull(value);
			param.Direction = ParameterDirection.Input;

			this._InnerParameters.Add(param);
        }

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

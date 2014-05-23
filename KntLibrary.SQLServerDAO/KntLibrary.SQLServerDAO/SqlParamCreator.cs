using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace KntLibrary.SQLServerDAO
{
    public sealed class SqlParamCreator : IParamCreator
	{
		#region Fields
		
		private List<SqlParameter> _innerSqlParameters = null;

        private List<object> _innerParameters = null;

		public SqlParameter[] SqlParameters
		{
			get
			{ 
				return this._innerSqlParameters.ToArray(); 
			}
		}

        public object[] Parameters
        {
            get
            {
                return this._innerParameters.ToArray();
            }
        }

		#endregion

		#region Constructors

		public SqlParamCreator()
        {
            this._innerSqlParameters = new List<SqlParameter>();
            this._innerParameters = new List<object>();
        }

        #endregion

        #region Private Mehtods
        #endregion

        #region Public Methods

        public object IsDBNull(object value)
        {
            return value ?? DBNull.Value;
        }

        public void Add(object value)
        {
            this._innerParameters.Add(value);
        }

        public void Add(string paramName, DbType dbType, object value)
        {
			var param = new SqlParameter();

			param.ParameterName = paramName;
			param.Value = this.IsDBNull(value);
            param.DbType = dbType;
			param.Direction = ParameterDirection.Input;

			this._innerSqlParameters.Add(param);
        }

		public void Add(string paramName, DataTable table)
		{
			var param = new SqlParameter();

			param.ParameterName = paramName;
            param.Value = table;
            param.SqlDbType = SqlDbType.Structured;
			param.Direction = ParameterDirection.Input;

			this._innerSqlParameters.Add(param);
		}

        #endregion
    }
}

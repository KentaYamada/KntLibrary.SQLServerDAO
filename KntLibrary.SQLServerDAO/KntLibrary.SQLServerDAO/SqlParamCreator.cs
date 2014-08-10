using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

using KntLibrary.SQLServerDAO.Interfaces;

namespace KntLibrary.SQLServerDAO
{
    public class SqlParamCreator : IParamCreation
    {
        #region Fields
		
        private List<SqlParameter> _sqlParameters;

        public SqlParameter[] SqlParameters
        {
	        get
	        { 
		        return this._sqlParameters.ToArray(); 
	        }
        }

        #endregion

        #region Constructors

        public SqlParamCreator()
        {
            this._sqlParameters = new List<SqlParameter>();
        }

        #endregion

        private string AddPrefix(string paramName)
        {
            if (string.IsNullOrWhiteSpace(paramName))
            {
                throw new ArgumentNullException("パラメータ名を空白にすることはできません。");
            }
            
            return paramName.IndexOf('@') < 0 ? string.Format("@{0}", paramName) : paramName;
        }

        #region Public Methods

        public void Add(string paramName, DataTable table)
        {
	        var param = new SqlParameter();
			
	        param.Direction = ParameterDirection.Input;
	        param.ParameterName = this.AddPrefix(paramName);
	        param.SqlDbType = SqlDbType.Structured;
	        param.Value = table;

	        this._sqlParameters.Add(param);
        }

        public void Add(string paramName, SqlDbType type, object value)
        {
	        var param = new SqlParameter();

	        param.Direction = ParameterDirection.Input;
	        param.SqlDbType = type;
            param.ParameterName = this.AddPrefix(paramName);
	        param.Value = this.ToDBNull(value);

	        this._sqlParameters.Add(param);
        }

        public void Add<T>(T args) where T : class
        {
            if (args == null)
            {
                throw new ArgumentNullException();
            }

            var param = new SqlParameter();

            foreach (PropertyInfo arg in args.GetType().GetProperties())
            {
                param.Direction = ParameterDirection.Input;
                param.ParameterName = string.Format("@{0}",arg.Name);
                param.Value = this.ToDBNull(arg.GetValue(args, null));

                this._sqlParameters.Add(param);
            }
        }

        public void Clear()
        {
            this._sqlParameters.Clear();
        }

        public object ToDBNull(object value)
        {
            return Convert.ToString(value) == string.Empty ? DBNull.Value : value;
        }

        #endregion
    }
}

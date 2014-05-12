using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace KntLibrary.SQLServerDAO
{
    public sealed class SqlParamCreator : IParamCreator
	{
		#region Fields

		/// <summary>内部SQLパラメータリスト</summary>
		private List<SqlParameter> _InnerParameters = null;

        /// <summary>SQL DBType辞書</summary>
        private Dictionary<DbType, SqlDbType> _innerDbType = null;

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
            this._innerDbType = new Dictionary<DbType, SqlDbType>()
            {
                { DbType.AnsiString, SqlDbType.VarChar },
                { DbType.AnsiStringFixedLength, SqlDbType.Char },
                { DbType.Binary, SqlDbType.VarBinary },
                { DbType.Boolean, SqlDbType.Bit },
                { DbType.Byte, SqlDbType.TinyInt },
                { DbType.Currency, SqlDbType.Money },
                { DbType.Date, SqlDbType.Date },
                { DbType.DateTime, SqlDbType.DateTime },
                { DbType.DateTime2, SqlDbType.DateTime2 },
                { DbType.DateTimeOffset, SqlDbType.DateTimeOffset },
                { DbType.Decimal, SqlDbType.Decimal },
                { DbType.Double, SqlDbType.Float },
                { DbType.Guid, SqlDbType.UniqueIdentifier },
                { DbType.Int16, SqlDbType.SmallInt },
                { DbType.Int32, SqlDbType.Int },
                { DbType.Int64, SqlDbType.BigInt },
                { DbType.Object, SqlDbType.Variant },
                { DbType.Single, SqlDbType.Real },
                { DbType.String, SqlDbType.NVarChar },
                { DbType.StringFixedLength, SqlDbType.NChar },
                { DbType.Time, SqlDbType.Time },
                { DbType.Xml, SqlDbType.Xml },
            };
        }

        #endregion

        #region Private Mehtods

        /// <summary>
        /// Convert DbType to MS SQL-Server data type
        /// </summary>
        /// <param name="dbType">DbType</param>
        /// <returns>SqlDbType</returns>
        private SqlDbType CvtDbTypeToSqlType(DbType dbType)
        {
            if (this._innerDbType.ContainsKey(dbType))
            {
                throw new ArgumentException(dbType.ToString() + "is not suporeted.");
            }

            return this._innerDbType[dbType];
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Convert to DBNull.value
        /// </summary>
        public object IsDBNull(object value)
        {
            return value ?? DBNull.Value;
        }

        public void Add<T>(T arg)
        {
            if (null == arg)
            {
                throw new NullReferenceException();
            }

            var type = arg.GetType();
            
            var param = new SqlParameter();

            foreach (var t in type.GetProperties())
            {
                param.ParameterName = string.Format("@{0}", t.Name);
                param.Value = this.IsDBNull(t.GetValue(arg, null));
                param.SqlDbType = this.CvtDbTypeToSqlType(param.DbType);
                param.Direction = ParameterDirection.Input;
                
                this._InnerParameters.Add(param);
            }
        }

        public void Add(string paramName, DbType dbType, object value)
        {
			var param = new SqlParameter();

			param.ParameterName = paramName;
			param.Value = this.IsDBNull(value);
            param.SqlDbType = this.CvtDbTypeToSqlType(dbType);
			param.Direction = ParameterDirection.Input;

			this._InnerParameters.Add(param);
        }

		public void Add(string paramName, DataTable table)
		{
			var param = new SqlParameter();

			param.ParameterName = paramName;
            param.Value = table;
            param.SqlDbType = SqlDbType.Structured;
			param.Direction = ParameterDirection.Input;

			this._InnerParameters.Add(param);
		}

        #endregion
    }
}

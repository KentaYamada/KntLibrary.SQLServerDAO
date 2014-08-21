using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Linq;

namespace KntLibrary.SQLServerDAO
{
	public static class SqlCommander
	{
		#region Private static methods

        private static SqlCommand BuildCommand(string commandText, SqlParamCreator param)
        {
            var comm = new SqlCommand(commandText, new SqlConnector().GetConnection);

            if (null != param && 0 < param.SqlParameters.Length)
            {
                comm.Parameters.AddRange(param.SqlParameters);
            }

            return comm;
        }

		#endregion

		#region Public static methods
        
        public static List<T> SelectToList<T>(string commandText, params object[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException();
            }

            using (var connect = new SqlConnector())
            using (var context = new DataContext(connect.GetConnection))
            {
                return context.ExecuteQuery<T>(commandText, args).ToList();
            }
        }

        public static DataSet SelectToDataSet(string commandText, SqlParamCreator param)
        {
            using (var command = SqlCommander.BuildCommand(commandText, param))
            using (var adapt = new SqlDataAdapter(command))
            {
                var ds = new DataSet();

                try
                {
                    adapt.Fill(ds);
                }
                finally
                {
                    command.Parameters.Clear();
                }

                return ds;
            }
        }

        public static DataTable SelectToDataTable(string commandText, SqlParamCreator param)
		{
            return SqlCommander.SelectToDataSet(commandText, param).Tables[0];
		}

        public static int ExecuteNonQuery(string commandText, SqlParamCreator param)
		{
			using (var command = SqlCommander.BuildCommand(commandText, param))
			{
                try
                {
                    command.Connection.Open();

                    return command.ExecuteNonQuery();
                }
                finally
                {
                    command.Parameters.Clear();
                }
			}
		}

        public static int ExecuteStoredProcedure(string commandText, SqlParamCreator param)
        {
            using (var command = SqlCommander.BuildCommand(commandText, param))
            {
                command.CommandType = CommandType.StoredProcedure;

                try
                {
                    command.Connection.Open();

                    return command.ExecuteNonQuery();
                }
                finally
                {
                    command.Parameters.Clear();
                }
            }
        }

        public static object Scalor(string commandText, SqlParamCreator param)
		{
			using (var command = SqlCommander.BuildCommand(commandText, param))
			{
                try
                {
                    command.Connection.Open();

                    return command.ExecuteScalar();
                }
                finally
                {
                    command.Parameters.Clear();
                }
			}
		}

        public void BeginTransaction()
        {
 
        }
		#endregion
	}
}

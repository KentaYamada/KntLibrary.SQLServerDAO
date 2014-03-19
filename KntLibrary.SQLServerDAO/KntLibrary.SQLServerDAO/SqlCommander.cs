﻿using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;

namespace KntLibrary.SQLServerDAO
{
    /// <summary>
    /// MS SQL-Server Execute command class
    /// </summary>
	public static class SqlCommander
	{
		#region Private static methods

		/// <summary>
        /// Set parameters to SqlCommand object
        /// </summary>
        private static void SetParameters(SqlCommand command, SqlParameter[] args)
        {
            command.Parameters.Clear();

            if ((null != args) && (0 < args.Length))
            {
                command.Parameters.AddRange(args);
            }
        }

		#endregion

		#region Public static methods

        /// <summary>
        /// Execute select query
        /// </summary>
        public static List<T> Select<T>(string query, params object[] args)
        {
            using (var connector = new SqlConnector())
            using (var context = new DataContext(connector.Connection))
            {
                return context.ExecuteQuery<T>(query, args).ToList();
            }
        }

        /// <summary>
        /// Execute select query
        /// </summary>
		public static DataTable Select(string query, params SqlParameter[] args)
		{
			using (var connector = new SqlConnector())
            using (var command = new SqlCommand(query, connector.Connection))
			{
                SqlCommander.SetParameters(command, args);

                using (var adapt = new SqlDataAdapter(command))
                {
                    var table = new DataTable();

					adapt.Fill(table);

                    return table;
                }
			}
		}

		/// <summary>
		/// Sqve data
		/// </summary>
		public static int Save(string query, params SqlParameter[] args)
		{
            using (var connector = new SqlConnector())
			using (var command = new SqlCommand(query))
			{
                command.Connection = connector.Connection;

				SqlCommander.SetParameters(command, args);
                
                connector.Open();

                int count = 0;

                using (var tran = new TransactionScope())
                {
                    count = command.ExecuteNonQuery();

                    if (0 < count)
                    {
                        tran.Complete();
                    }
                }

                connector.Close();

                return count;
			}
		}

        /// <summary>
        /// Sqve data
        /// </summary>
        public static int Save(string query)
        {
            return SqlCommander.Save(query);
        }

        /// <summary>
        /// Sqve data
        /// </summary>
        public static int Save(List<string> queries)
        {
            int count = 0;

            foreach (string query in queries)
            {
                count += SqlCommander.Save(query);
            }

            return count;
        }

		/// <summary>
		/// Execute delete query
		/// </summary>
		public static int Delete(string query, params SqlParameter[] args)
		{
			return SqlCommander.Save(query, args);
		}

        /// <summary>
        /// Execute delete query
        /// </summary>
        public static int Delete(List<string> queries)
		{
			return SqlCommander.Save(queries);
		}

        /// <summary>
		/// Get Scalor value
        /// </summary>
		public static int Scalor(string query, params SqlParameter[] args)
		{
			using (var connector = new SqlConnector())
			using (var command = new SqlCommand(query, connector.Connection))
			{
				SqlCommander.SetParameters(command, args);

				connector.Open();

				int count = (int)command.ExecuteScalar();

				connector.Close();

				return count;
			}
		}

        /// <summary>
        /// Execute stored procedure
        /// </summary>
		public static int ExecStoredProcedure(string procName, params SqlParameter[] args)
		{
            using (var connector = new SqlConnector())
			using (var command = new SqlCommand(procName))
			{
				SqlCommander.SetParameters(command, args);
                command.CommandType = CommandType.StoredProcedure;

                return command.ExecuteNonQuery();
			}
		}

		#endregion
	}
}

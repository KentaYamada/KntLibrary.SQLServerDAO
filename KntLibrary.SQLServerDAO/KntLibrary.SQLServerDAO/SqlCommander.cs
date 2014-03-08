using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;

namespace KntLibrary.SQLServerDAO
{
    /// <summary>
    /// SQL実行クラス
    /// </summary>
	public static class SqlCommander
	{
		#region Private static methods

		/// <summary>
        /// コマンドクラスにパラメータセット
        /// </summary>
        /// <param name="command">実行コマンドクラス</param>
        /// <param name="args">パラメータ配列</param>
        private static void SetParameters(SqlCommand command, SqlParameter[] args)
        {
            command.Parameters.Clear();

            if ((null != args) && (0 < args.Length))
            {
                command.Parameters.AddRange(args);
            }
        }

		/// <summary>
		/// データ登録・更新・削除実行
		/// </summary>
		/// <param name="command">実行コマンドクラス</param>
		/// <returns>影響行数</returns>
		private static int ExecuteNonQuery(SqlCommand command)
		{
			using (var connector = new SqlConnector())
			{
				command.Connection = connector.Connection;
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

		#endregion

		#region Public static methods

        public static List<T> Select<T>(string query, params object[] args)
        {
            using (var connector = new SqlConnector())
            using (var context = new DataContext(connector.Connection))
            {
                var result = context.ExecuteQuery<T>(query, args).ToList();

                return result;
            }
        }

		/// <summary>
        /// SELECT文実行
        /// </summary>
        /// <param name="query">SELECT文</param>
        /// <param name="args">検索条件</param>
        /// <returns>抽出結果</returns>
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
		/// データ登録
		/// </summary>
		/// <param name="queries">SQL生成文字列</param>
		/// <returns>True:正常 / False:エラー</returns>
		public static bool Save(List<string> queries)
		{
			foreach (string query in queries)
			{
				if (!Save(query))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// データ登録
		/// </summary>
		/// <param name="query">SQL生成文字列</param>
		/// <param name="args">DBパラメータ</param>
		/// <returns>True:正常 / False:エラー</returns>
		public static bool Save(string query, params SqlParameter[] args)
		{
			using (var command = new SqlCommand(query))
			{
				SqlCommander.SetParameters(command, args);

				bool result = 0 < ExecuteNonQuery(command) ? true : false;

				return result;
			}
		}

		/// <summary>
		/// データ削除
		/// </summary>
		/// <param name="query">SQL生成文字列</param>
		/// <param name="args">DBパラメータ</param>
		/// <returns></returns>
		public static bool Delete(string query, params SqlParameter[] args)
		{
			return SqlCommander.Save(query, args);
		}

		/// <summary>
		/// データ削除
		/// </summary>
        /// <param name="queries"></param>
		/// <returns></returns>
        public static bool Delete(List<string> queries)
		{
			return SqlCommander.Save(queries);
		}

        /// <summary>
		/// 単一行、単一列の結果を返します
        /// </summary>
        /// <param name="query"></param>
        /// <param name="args"></param>
        /// <returns></returns>
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
        /// ストアドプロシージャ呼び出し
        /// </summary>
        /// <param name="procName">ストアドプロシージャ名</param>
        /// <param name="args">DBパラメータ</param>
        /// <returns>True:成功 / False:失敗</returns>
		public static bool ExecStoredProcedure(string procName, params SqlParameter[] args)
		{
			using (var command = new SqlCommand(procName))
			{
				command.CommandType = CommandType.StoredProcedure;

				SqlCommander.SetParameters(command, args);

				bool result = 0 < SqlCommander.ExecuteNonQuery(command) ? true : false;

				return result;
			}
		}

		#endregion
	}
}

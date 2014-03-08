using System;

namespace KntLibrary.SQLServerDAO
{
    /// <summary>
    /// SQL Serverパラメータ作成クラス
    /// </summary>
    public static class SqlFormatter
    {
        /// <summary>
        /// シングルクォーテーション付与（文字列型、日付型で使用）
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
		public static string CvtSqlLiteral(object value)
        {
            return string.Format("'{0}'", value);
        }
    }
}

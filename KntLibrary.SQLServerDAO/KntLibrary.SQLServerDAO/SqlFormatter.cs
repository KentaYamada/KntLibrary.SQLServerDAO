using System;

namespace KntLibrary.SQLServerDAO
{
    /// <summary>
    /// MS SQL-Server parameter formatter class
    /// </summary>
    public static class SqlFormatter
    {
        /// <summary>
        /// Format parameter
        /// </summary>
		public static string CvtSqlLiteral(object value)
        {
            return string.Format("'{0}'", value);
        }
    }
}

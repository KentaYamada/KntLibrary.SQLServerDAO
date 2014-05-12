using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using KntLibrary.SQLServerDAO;
using NUnit.Framework;

namespace KntLibrary.SQLServerDAO.UnitTest
{
	[TestFixture]
	public class SqlCommanderTest
	{
		[Test]
		public void SaveWhenMultiDataTest()
		{
			string sql1 = "insert into emp values(16, 'test', 100000, null, 10)";
			string sql2 = "insert into emp values(17, 'test2', 100000, null, 10)";
			string sql3 = "insert into emp values(18, 'test3', 100000, null, 10)";

			List<string> sqls = new List<string>();
			sqls.Add(sql1);
			sqls.Add(sql2);
			sqls.Add(sql3);

			bool result = SqlCommander.Save(sqls);

			Assert.AreEqual(true, result);
		}

		[Test]
		public void SaveWhenMultiDataFalse()
		{
			string sql1 = "insert into emp values(19, 'test4', 100000, null, 10)";
			string sql2 = "insert into emp values(20, 'test5', 200000, null, 10)";
			string sql3 = "insert into emp values(19, 'test6', 300000, null, 10)";

			List<string> sqls = new List<string>();
			sqls.Add(sql1);
			sqls.Add(sql2);
			sqls.Add(sql3);

			Assert.That(() => SqlCommander.Save(sqls), Throws.Exception.TypeOf<SqlException>());
		}
	}
}

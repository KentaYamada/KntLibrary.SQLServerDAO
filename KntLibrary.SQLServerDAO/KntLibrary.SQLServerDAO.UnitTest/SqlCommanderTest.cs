using System;
using System.Collections.Generic;
using System.Data;
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
			string sql1 = "insert into emp values(10, 'test1', 100000, null, 10)";
			string sql2 = "insert into emp values(11, 'test2', 100000, null, 10)";
			string sql3 = "insert into emp values(12, 'test3', 100000, null, 10)";

			List<string> sqls = new List<string>();
			sqls.Add(sql1);
			sqls.Add(sql2);
			sqls.Add(sql3);

			int result = SqlCommander.Save(sqls);

			Assert.AreEqual(3, result);
		}

        [Test]
        public void DeleteQueryTest()
        {
            string delSql = "delete from emp where empno between 10 and 12";

            int result = SqlCommander.Delete(delSql);

            Assert.AreEqual(0 < result ? true : false, true);
        }

		[Test]
		public void SaveWhenMultiDataFalse()
		{
			string sql = "insert into emp values(9, 'test4', 100000, null, 10)";
			Assert.That(() => SqlCommander.Save(sql), Throws.Exception.TypeOf<SqlException>());
		}

        [Test]
        public void SelectTest()
        {
            string sql = "select * from emp where empno between {0} and {1}";

            var param = new SqlParamCreator();
            param.Add(3);
            param.Add(7);

            List<Employee> emp = SqlCommander.Select<Employee>(sql, param.Parameters);

            Assert.AreEqual(emp.Count, 5);
        }

        [Test]
        public void SelectTest2()
        {
            string sql = "select * from emp where empno between @empnoStart and @empnoEnd";

            var param = new SqlParamCreator();
            param.Add("@empnoStart", DbType.Int32, 3);
            param.Add("@empnoEnd", DbType.Int32, 7);

            DataTable emp = SqlCommander.Select(sql, param.SqlParameters);

            Assert.AreEqual(emp.Rows.Count, 5);
        }

        [Test]
        public void SelectTestWithoutParameters()
        {
            string sql = "select * from emp where empno between 3 and 7";
            DataTable emp = SqlCommander.Select(sql);

            Assert.AreEqual(emp.Rows.Count, 5);
        }

        [Test]
        public void SelectTestWithoutParameters2()
        {
            string sql = "select * from emp where empno between 3 and 7";
            List<Employee> emp = SqlCommander.Select<Employee>(sql);

            Assert.AreEqual(emp.Count, 5);
        }

        [Test]
        public void SelectWhenLikeInParameterTest()
        {
            string sql = "select * from emp where empname like '田村%'";
            DataTable emp = SqlCommander.Select(sql);

            Assert.AreEqual(emp.Rows.Count, 1);
        }

        [Test]
        public void SelectWhenLikeInParameterTest2()
        {
            string sql = "select * from emp where empname like '田村%'";
            List<Employee> emp = SqlCommander.Select<Employee>(sql);

            Assert.AreEqual(emp.Count, 1);
        }

        [Test]
        public void ScalorTest()
        {
            string sql = "select max(sal) from emp where deptno = @deptno";

            var param = new SqlParamCreator();
            param.Add("@deptno", DbType.Int32, 10);

            int result = SqlCommander.Scalor(sql,param.SqlParameters);

            Assert.AreEqual(result, 700000);
        }

        [Test]
        public void ScalorTestWithoutSqlParameters()
        {
            string sql = "select max(sal) from emp where deptno = 10";
            int result = SqlCommander.Scalor(sql);

            Assert.AreEqual(result, 700000);
        }

        public class Employee
        {
            public Employee() 
            { }

            public int empno { get; set; }

            public string empname { get; set; }

            public int? sal { get; set; }

            public DateTime? hiredate { get; set; }

            public int? deptno { get; set; }
        }
	}
}

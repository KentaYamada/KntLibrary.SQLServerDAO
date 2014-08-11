using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Transactions;

using KntLibrary.SQLServerDAO;
using NUnit.Framework;

namespace KntLibrary.SQLServerDAO.UnitTest
{
	[TestFixture]
	public class SqlCommanderTest
	{
        /// <summary>
        /// Execute command when query is not use parameters
        /// </summary>
        [Test]
        public void SelectToListTest_Pattern1()
        {
            string sql = "select * from emp";

            var result = SqlCommander.SelectToList<Employee>(sql);

            Assert.IsNotNull(result);
        }

        /// <summary>
        /// If args is null, throw ArgumentNullException object
        /// </summary>
        [Test]
        public void SelectToListTest_Pattern2()
        {
            string sql = "select * from emp";

            Assert.Throws<ArgumentNullException>(()=> SqlCommander.SelectToList<Employee>(sql, null));
        }

        /// <summary>
        /// Execute command If args array length = 0
        /// </summary>
        [Test]
        public void SelectToListTest_Pattern3()
        {
            string sql = "select * from emp";

            var result = SqlCommander.SelectToList<Employee>(sql, new object[]{}).Count;

            Assert.AreNotEqual(0, result);
        }

        [Test]
        public void SelectToListTest_Pattern4()
        {
            string sql = "select * from emp where emp.empno between {0} and {1}";

            var result = SqlCommander.SelectToList<Employee>(sql, new object[] {1, 7}).Count;

            Assert.AreEqual(7, result);
        }

        [Test]
        public void SelectToListTest_Pattern5()
        {
            string sql = "select * from emp where emp.empno between {0} and {1}";

            Assert.Throws<FormatException>(()=> SqlCommander.SelectToList<Employee>(sql, new object[] { 1 }));
        }

        [Test]
        public void BuildCommandTest()
        {
            string sql = "select * from emp ";

            var result = typeof(SqlCommander).InvokeMember("BuildCommand", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, new object[] { sql, null }) as SqlCommand;

            Assert.IsNotNull(result.Connection);

            Assert.IsNotNullOrEmpty(result.Connection.ConnectionString);
        }

        [Test]
        public void SelectToDataSetTest_Pattern1()
        {
            string sql = "select * from emp ";

            var result = SqlCommander.SelectToDataSet(sql, null);

            Assert.IsNotNull(result);

            Assert.AreEqual(typeof(DataTable), result.Tables[0].GetType());
        }

        [Test]
        public void SelectToDataSetTest_Pattern2()
        {
            string sql = "select * from emp where empno between @empNoStart and @empnoEnd";

            var p = new SqlParamCreator();

            p.Add("@empNoStart", SqlDbType.Int, 1);
            p.Add("@empnoEnd", SqlDbType.Int, 7);

            var result = SqlCommander.SelectToDataSet(sql, p);

            Assert.AreEqual(7, result.Tables[0].Rows.Count);
        }

        [Test]
        public void SelectToDataSetTest_Pattern3()
        {
            string sql = "select * from emp where empno between @empNoStart and @empnoEnd";

            var p = new SqlParamCreator();

            p.Add("@empNoStart", SqlDbType.Int, 1);

            Assert.Throws<SqlException>(() => SqlCommander.SelectToDataSet(sql, p));
        }

        /// <summary>
        /// Success pattern
        /// </summary>
        [Test]
        public void SaveDataTest_Pattern1()
        {
            string sql = "insert into emp values(10, 'test', null, null, 10)";

            Assert.Throws<SqlException>(() =>SqlCommander.ExecuteNonQuery(sql, null));
        }

        /// <summary>
        /// Rollback check
        /// </summary>
        [Test]
        public void SaveDataTest_Pattern2()
        {
            //Error sql. "emps" is not exist database.
            string sql = "insert into emps values(10, 'test', null, null, 10)";

            bool result = true;
            using (var tran = new TransactionScope())
            {
                try
                {
                    SqlCommander.ExecuteNonQuery(sql, null);
                    tran.Complete();
                }
                catch
                {
                    result = false;
                }
            }

            Assert.AreNotEqual(true, result);
        }

        [Test]
        public void SaveDataTest_Pattern3()
        {
            string sql = "insert into emp values(@empno, @empname, @sal, @hiredate, @deptno)";

            var p = new SqlParamCreator();

            p.Add("@empno", SqlDbType.Int, 11);
            p.Add("@empname", SqlDbType.Char, "Test");
            p.Add("@sal", SqlDbType.Int, 100000);
            p.Add("@hiredate", SqlDbType.DateTime, DateTime.Now);
            p.Add("@deptno", SqlDbType.Int, 20);


            bool result = true;

            using (var tran = new TransactionScope())
            {
                try
                {
                    SqlCommander.ExecuteNonQuery(sql, p);

                    tran.Complete();
                }
                catch
                {
                    result = false;
                }
            }

            Assert.AreEqual(true, result);
        }

        [Test]
        public void SaveDataTest_Pattern4()
        {
            string sql = "insert into emp values(@empno, @empname, @sal, @hiredate, @deptno)";

            var e = new Employee();
            e.empno = 12;
            e.empname = "Test";
            e.sal = 100000;
            e.hiredate = null;
            e.deptno = 10;

            var p = new SqlParamCreator();
            p.Add<Employee>(e);

            using (var tran = new TransactionScope())
            {
                bool result = true;

                try
                {
                    SqlCommander.ExecuteNonQuery(sql, p);

                    tran.Complete();
                }
                catch
                {
                    result = false;
                }

                Assert.AreEqual(true, result);
            }
        }

        [Test]
        public void SaveDataTest_Pattern5()
        {
            var sql = new StringBuilder();

            sql.AppendLine("begin try");
            sql.AppendLine("  begin tran");
            sql.AppendLine("    insert into emp values (@empno, @empname, @sal, @hiredate, @deptno)");
            sql.AppendLine("  commit tran");
            sql.AppendLine("end try");
            sql.AppendLine("begin catch");
            sql.AppendLine("    rollback tran");
            sql.AppendLine("end catch");

            var e = new Employee();
            e.empno = 13;
            e.empname = "Test";
            e.sal = 100000;
            e.hiredate = null;
            e.deptno = 10;

            var p = new SqlParamCreator();
            p.Add<Employee>(e);

            var result = SqlCommander.ExecuteNonQuery(sql.ToString(), p);

            Assert.AreEqual(1, result);
        }

        [Test]
        public void ExecTSqlTest_Pattern1()
        {
            var e = new Employee();
            e.empno = 13;
            e.empname = "T-SqlTest";
            e.sal = 100000;
            e.hiredate = null;
            e.deptno = 10;

            var p = new SqlParamCreator();
            p.Add<Employee>(e);

            var result = SqlCommander.ExecuteStoredProcedure("SaveEmployee", p);

            Assert.AreEqual(1, result);
        }

        [Test]
        public void ScalorTest()
        {
            string sql = "select MAX(sal) as sal from emp";

            int result = (int)SqlCommander.Scalor(sql, null);

            Assert.AreEqual(700000, result);
        }

        public class Employee
        {
            public int empno { get; set; }

            public string empname { get; set; }

            public int? sal { get; set; }

            public DateTime? hiredate { get; set; }

            public int? deptno { get; set; }
        }
	}
}

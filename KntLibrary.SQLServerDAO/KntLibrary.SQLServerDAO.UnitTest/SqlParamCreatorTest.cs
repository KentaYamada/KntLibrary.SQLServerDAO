using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Linq;
using System.Text;

using NUnit.Framework;
using KntLibrary.SQLServerDAO;

namespace KntLibrary.SQLServerDAO.UnitTest
{
    [TestFixture]
    public class SqlParamCreatorTest
    {
        /// <summary>
        /// Add parameter method check
        /// </summary>
        [Test]
        public void AddTest_Pattern1()
        {
            var p = new SqlParamCreator();

            p.Add("@param1", SqlDbType.Char, "test");
            p.Add("@param2", SqlDbType.Int, 100);
            p.Add("@param3", SqlDbType.DateTime, DateTime.Now);

            Assert.AreEqual(3, p.SqlParameters.Length);
        }

        /// <summary>
        /// If value is null or empty string, SqlParameter.Value convert DBNull.Value??
        /// </summary>
        [Test]
        public void AddTest_Pattern2()
        {
            var p = new SqlParamCreator();

            p.Add("@param1", SqlDbType.Char, "");
            p.Add("@param2", SqlDbType.DateTime, null);

            Assert.AreEqual(DBNull.Value, p.SqlParameters[0].Value);
            Assert.AreEqual(DBNull.Value, p.SqlParameters[1].Value);
        }

        [Test]
        public void AddTest_Pattern3()
        {
            var table = new DataTable();
            
            table.Columns.Add("param1");
            table.Columns.Add("param2");
            table.Columns.Add("param3");

            table.Rows.Add(1, "test", "row");
            table.Rows.Add(2, "test", "row2");

            var p = new SqlParamCreator();

            p.Add("@table", table);

            Assert.AreEqual(1, p.SqlParameters.Length);

            table = p.SqlParameters[0].Value as DataTable;

            Assert.AreEqual(typeof(DataTable), table.GetType());
        }

        /// <summary>
        /// If argument object is null, throw ArgumentNullException
        /// </summary>
        [Test]
        public void AddTest_Pattern4()
        {
            Products products = null;
            var p = new SqlParamCreator();

            Assert.Throws<ArgumentNullException>(()=> p.Add<Products>(products));
        }


        [Test]
        public void AddTest_Pattern5()
        {
            var products = new Products();

            products.ProductID = "100";
            products.ProductName = "test";
            products.Price = 10000;
            products.UpdateDate = null;

            var p = new SqlParamCreator();

            p.Add<Products>(products);

            Assert.AreEqual(4, p.SqlParameters.Length);

            var name = p.SqlParameters[0].ParameterName;

            Assert.AreEqual(true, name.Contains('@'));
        }

        [Test]
        public void AddTest_Pattern6()
        {
            var products = new Products();

            products.ProductID = "100";
            products.ProductName = "";
            products.Price = 10000;
            products.UpdateDate = null;

            var p = new SqlParamCreator();

            p.Add<Products>(products);

            var target = p.SqlParameters.OfType<SqlParameter>().Where(x => x.ParameterName == "@UpdateDate").First();

            Assert.AreEqual(target.Value, DBNull.Value);
        }

        [TestCase("param")]
        [TestCase("@param")]
        public void AddPrefixTest_Pattern1(string paramName)
        {
            var a = typeof(SqlParamCreator).InvokeMember("AddPrefix",
                                                          BindingFlags.NonPublic |BindingFlags.Instance | BindingFlags.InvokeMethod,
                                                          null, new SqlParamCreator(), new object[] { paramName });

            Assert.AreEqual("@param", a);
        }

        [TestCase("")]
        [TestCase(" ")]
        public void AddPrefixTest_Pattern2(string paramName)
        {
            Assert.Throws<TargetInvocationException>(() => typeof(SqlParamCreator).InvokeMember("AddPrefix",
                                                       BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod,
                                                       null, new SqlParamCreator(), new object[] { paramName }));
        }

        [Test]
        public void ClearTest_Pattern1()
        {
            var p = new SqlParamCreator();

            p.Add("@param1", SqlDbType.Char, "test");
            p.Add("@param2", SqlDbType.Int, 100);
            p.Add("@param3", SqlDbType.DateTime, DateTime.Now);

            Assert.AreEqual(3, p.SqlParameters.Length);

            p.Clear();

            Assert.AreEqual(0, p.SqlParameters.Length);
        }

        [Test]
        public void ClearTest_Pattern2()
        {
            var table = new DataTable();

            table.Columns.Add("param1");
            table.Columns.Add("param2");
            table.Columns.Add("param3");

            table.Rows.Add(1, "test", "row");
            table.Rows.Add(2, "test", "row2");

            var p = new SqlParamCreator();

            p.Add("@table", table);

            Assert.AreEqual(1, p.SqlParameters.Length);

            p.Clear();

            Assert.AreEqual(0, p.SqlParameters.Length);
        }

        [Test]
        public void ClearTest_Pattern3()
        {
            var p = new SqlParamCreator();

            Assert.AreEqual(0, p.SqlParameters.Length);

            p.Clear();

            Assert.AreEqual(0, p.SqlParameters.Length);
        }

        /// <summary>
        /// When argument is not null case
        /// </summary>
        [TestCase("Test")]
        [TestCase(100)]
        public void ToDBNullTest_Pattern1(object arg)
        {
            var p = new SqlParamCreator();
            var result = p.ToDBNull(arg);

            Assert.IsNotNull(result);
        }

        /// <summary>
        /// When argument is null or empty string case
        /// </summary>
        [TestCase(null)]
        [TestCase("")]
        public void ToDBNullTest_Pattern2(object arg)
        {
            var p = new SqlParamCreator();
            var result = p.ToDBNull(arg);

            Assert.AreEqual(DBNull.Value, result);
        }

        public class Products
        {
            public string ProductID { get; set; }

            public string ProductName { get; set; }

            public decimal Price { get; set; }

            public DateTime? UpdateDate { get; set; }
        }
    }
}

// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Implementations;
using HunterIndustriesAPI.PersistenceTests.API.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.PersistenceTests.API
{
    [TestClass]
    [DoNotParallelize]
    public class DatabaseWrapperTest
    {
        private static string _ConnectionString;
        private static string _DatabaseName;

        /// <summary>
        /// Creates the test database and schema.
        /// </summary>
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            (_ConnectionString, _DatabaseName) = LocalDbTestHelper.CreateDatabase();
            LocalDbTestHelper.CreateSchema(_ConnectionString);
        }

        /// <summary>
        /// Drops the test database.
        /// </summary>
        [ClassCleanup]
        public static void ClassCleanup()
        {
            LocalDbTestHelper.DropDatabase(_DatabaseName);
        }

        /// <summary>
        /// Clears the test data between tests.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            LocalDbTestHelper.ClearData(_ConnectionString);
        }

        /// <summary>
        /// Creates a DatabaseWrapper instance with the test connection string.
        /// </summary>
        private DatabaseWrapper CreateWrapper()
        {
            Mock<IDatabaseOptions> mockOptions = new();
            mockOptions.Setup(o => o.ConnectionString)
                .Returns(_ConnectionString);

            return new DatabaseWrapper(mockOptions.Object);
        }
        /// <summary>
        /// Checks whether the Query method returns multiple rows from the database.
        /// </summary>
        [TestMethod]
        public async Task TestQuery()
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO APIUser (Username, [Password], IsDeleted) VALUES ('admin', 'pass1', 0), ('user', 'pass2', 0)",
                    conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            DatabaseWrapper wrapper = CreateWrapper();

            (List<string> results, Exception ex) = await wrapper.Query(
                "SELECT Username FROM APIUser ORDER BY UserId",
                reader => reader.GetString(0));

            Assert.IsNull(ex);
            Assert.AreEqual(
                2,
                results.Count);
            Assert.AreEqual(
                "admin",
                results[0]);
            Assert.AreEqual(
                "user",
                results[1]);
        }

        /// <summary>
        /// Checks whether the Query method returns an empty list when no rows exist.
        /// </summary>
        [TestMethod]
        public async Task TestQueryEmpty()
        {
            DatabaseWrapper wrapper = CreateWrapper();

            (List<string> results, Exception ex) = await wrapper.Query(
                "SELECT Username FROM APIUser",
                reader => reader.GetString(0));

            Assert.IsNull(ex);
            Assert.AreEqual(
                0,
                results.Count);
        }

        /// <summary>
        /// Checks whether the Query method returns multiple columns as tuples.
        /// </summary>
        [TestMethod]
        public async Task TestQueryMultipleColumns()
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO APIUser (Username, [Password], IsDeleted) VALUES ('admin', 'hashed1', 0), ('user', 'hashed2', 0)",
                    conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            DatabaseWrapper wrapper = CreateWrapper();

            (List<(string, string)> results, Exception ex) = await wrapper.Query(
                "SELECT Username, [Password] FROM APIUser ORDER BY UserId",
                reader => (reader.GetString(0), reader.GetString(1)));

            Assert.IsNull(ex);
            Assert.AreEqual(
                2,
                results.Count);
            Assert.AreEqual(
                "admin",
                results[0].Item1);
            Assert.AreEqual(
                "hashed1",
                results[0].Item2);
        }

        /// <summary>
        /// Checks whether the Query method handles parameterised queries.
        /// </summary>
        [TestMethod]
        public async Task TestQueryWithParameters()
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO APIUser (Username, [Password], IsDeleted) VALUES ('admin', 'pass1', 0), ('deleted', 'pass2', 1)",
                    conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            DatabaseWrapper wrapper = CreateWrapper();
            SqlParameter[] parameters =
            [
                new SqlParameter("@isDeleted", System.Data.SqlDbType.Bit) { Value = 0 }
            ];

            (List<string> results, Exception ex) = await wrapper.Query(
                "SELECT Username FROM APIUser WHERE IsDeleted = @isDeleted",
                reader => reader.GetString(0),
                parameters);

            Assert.IsNull(ex);
            Assert.AreEqual(
                1,
                results.Count);
            Assert.AreEqual(
                "admin",
                results[0]);
        }

        /// <summary>
        /// Checks whether the QuerySingle method returns a single row from the database.
        /// </summary>
        [TestMethod]
        public async Task TestQuerySingle()
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO APIUser (Username, [Password], IsDeleted) VALUES ('admin', 'pass1', 0)",
                    conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            DatabaseWrapper wrapper = CreateWrapper();

            (string result, Exception ex) = await wrapper.QuerySingle(
                "SELECT Username FROM APIUser WHERE Username = 'admin'",
                reader => reader.GetString(0));

            Assert.IsNull(ex);
            Assert.AreEqual(
                "admin",
                result);
        }

        /// <summary>
        /// Checks whether the QuerySingle method returns the default value when no rows exist.
        /// </summary>
        [TestMethod]
        public async Task TestQuerySingleEmpty()
        {
            DatabaseWrapper wrapper = CreateWrapper();

            (string result, Exception ex) = await wrapper.QuerySingle(
                "SELECT Username FROM APIUser WHERE Username = 'nonexistent'",
                reader => reader.GetString(0));

            Assert.IsNull(ex);
            Assert.IsNull(result);
        }

        /// <summary>
        /// Checks whether the QuerySingle method handles parameterised queries.
        /// </summary>
        [TestMethod]
        public async Task TestQuerySingleWithParameters()
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "SET IDENTITY_INSERT Authorisation ON; INSERT INTO Authorisation (PhraseId, Phrase, IsDeleted) VALUES (1, 'testphrase', 0); SET IDENTITY_INSERT Authorisation OFF;" +
                    "SET IDENTITY_INSERT Application ON; INSERT INTO Application (ApplicationId, PhraseId, [Name], IsDeleted) VALUES (1, 1, 'TestApp', 0); SET IDENTITY_INSERT Application OFF;",
                    conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            DatabaseWrapper wrapper = CreateWrapper();
            SqlParameter[] parameters =
            [
                new SqlParameter("@phrase", System.Data.SqlDbType.VarChar) { Value = "testphrase" }
            ];

            (string result, Exception ex) = await wrapper.QuerySingle(
                "SELECT [Name] FROM [Application] JOIN Authorisation ON [Application].PhraseId = Authorisation.PhraseId WHERE Phrase = @phrase",
                reader => reader.GetString(0),
                parameters);

            Assert.IsNull(ex);
            Assert.AreEqual(
                "TestApp",
                result);
        }

        /// <summary>
        /// Checks whether the Execute method returns the correct row count for an insert.
        /// </summary>
        [TestMethod]
        public async Task TestExecuteInsert()
        {
            DatabaseWrapper wrapper = CreateWrapper();

            (int rowCount, Exception ex) = await wrapper.Execute(
                "INSERT INTO APIUser (Username, [Password], IsDeleted) VALUES ('admin', 'pass1', 0)");

            Assert.IsNull(ex);
            Assert.AreEqual(
                1,
                rowCount);
        }

        /// <summary>
        /// Checks whether the Execute method returns the correct row count for an update.
        /// </summary>
        [TestMethod]
        public async Task TestExecuteUpdate()
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO APIUser (Username, [Password], IsDeleted) VALUES ('admin', 'pass1', 0)",
                    conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            DatabaseWrapper wrapper = CreateWrapper();

            (int rowCount, Exception ex) = await wrapper.Execute(
                "UPDATE APIUser SET IsDeleted = 1 WHERE Username = 'admin'");

            Assert.IsNull(ex);
            Assert.AreEqual(
                1,
                rowCount);
        }

        /// <summary>
        /// Checks whether the Execute method returns zero when no rows are affected.
        /// </summary>
        [TestMethod]
        public async Task TestExecuteNoRows()
        {
            DatabaseWrapper wrapper = CreateWrapper();

            (int rowCount, Exception ex) = await wrapper.Execute(
                "UPDATE APIUser SET IsDeleted = 1 WHERE Username = 'nonexistent'");

            Assert.IsNull(ex);
            Assert.AreEqual(
                0,
                rowCount);
        }

        /// <summary>
        /// Checks whether the Execute method handles parameterised queries.
        /// </summary>
        [TestMethod]
        public async Task TestExecuteWithParameters()
        {
            DatabaseWrapper wrapper = CreateWrapper();
            SqlParameter[] parameters =
            [
                new SqlParameter("@username", System.Data.SqlDbType.VarChar) { Value = "admin" },
                new SqlParameter("@password", System.Data.SqlDbType.VarChar) { Value = "pass1" },
                new SqlParameter("@isDeleted", System.Data.SqlDbType.Bit) { Value = false }
            ];

            (int rowCount, Exception ex) = await wrapper.Execute(
                "INSERT INTO APIUser (Username, [Password], IsDeleted) VALUES (@username, @password, @isDeleted)",
                parameters);

            Assert.IsNull(ex);
            Assert.AreEqual(
                1,
                rowCount);

            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "SELECT Username FROM APIUser",
                    conn))
                {
                    object result = cmd.ExecuteScalar();
                    Assert.AreEqual(
                        "admin",
                        result);
                }
            }
        }

        /// <summary>
        /// Checks whether the ExecuteScalar method returns the correct scalar value.
        /// </summary>
        [TestMethod]
        public async Task TestExecuteScalar()
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO APIUser (Username, [Password], IsDeleted) VALUES ('admin', 'pass1', 0), ('user', 'pass2', 0)",
                    conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            DatabaseWrapper wrapper = CreateWrapper();

            (object result, Exception ex) = await wrapper.ExecuteScalar(
                "SELECT COUNT(*) FROM APIUser");

            Assert.IsNull(ex);
            Assert.AreEqual(
                2,
                result);
        }

        /// <summary>
        /// Checks whether the ExecuteScalar method returns zero when no rows exist.
        /// </summary>
        [TestMethod]
        public async Task TestExecuteScalarEmpty()
        {
            DatabaseWrapper wrapper = CreateWrapper();

            (object result, Exception ex) = await wrapper.ExecuteScalar(
                "SELECT COUNT(*) FROM APIUser");

            Assert.IsNull(ex);
            Assert.AreEqual(
                0,
                result);
        }

    }
}

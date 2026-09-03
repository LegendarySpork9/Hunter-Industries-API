// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Implementations;
using HunterIndustriesAPI.PersistenceTests.API.Helpers;
using HunterIndustriesAPI.Services;
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.PersistenceTests.API.Services
{
    [TestClass]
    [DoNotParallelize]
    public class TokenServiceTest
    {
        private static string _ConnectionString;
        private static string _DatabaseName;
        private static string _SqlFilesPath;

        private readonly Mock<ILoggerService> _MockLogger = new();
        private readonly IFileSystem _FileSystem = new FileSystemWrapper();

        /// <summary>
        /// Creates the test database and schema.
        /// </summary>
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            (_ConnectionString, _DatabaseName) = LocalDbTestHelper.CreateDatabase();
            LocalDbTestHelper.CreateSchema(_ConnectionString);
            _SqlFilesPath = LocalDbTestHelper.GetSqlFilesPath();
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
        /// Creates a service instance with real database dependencies for testing.
        /// </summary>
        private (TokenService service, DatabaseWrapper database) CreateService(string phrase)
        {
            Mock<IDatabaseOptions> mockOptions = new();
            mockOptions.Setup(o => o.ConnectionString)
                .Returns(_ConnectionString);
            mockOptions.Setup(o => o.SQLFiles)
                .Returns(_SqlFilesPath);

            DatabaseWrapper database = new(mockOptions.Object);

            TokenService service = new(
                _MockLogger.Object,
                _FileSystem,
                mockOptions.Object,
                database,
                phrase);

            return (service, database);
        }

        /// <summary>
        /// Inserts an authorisation and application record into the database.
        /// </summary>
        private void InsertAuthorisationAndApplication(
            string phrase,
            string applicationName)
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO Authorisation (Phrase, IsDeleted) VALUES (@phrase, 0); " +
                    "DECLARE @phraseId INT = SCOPE_IDENTITY(); " +
                    "INSERT INTO Application (PhraseId, [Name], IsDeleted) VALUES (@phraseId, @name, 0);",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@phrase",
                        phrase);
                    cmd.Parameters.AddWithValue(
                        "@name",
                        applicationName);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        /// <summary>
        /// Checks whether the ApplicationName method returns the application name from the database.
        /// </summary>
        [TestMethod]
        public async Task TestApplicationName()
        {
            string expectedPhrase = "testphrase";
            string expectedName = "TestApplication";

            InsertAuthorisationAndApplication(
                expectedPhrase,
                expectedName);

            (TokenService service, _) = CreateService(expectedPhrase);

            string actual = await service.ApplicationName();

            Assert.AreEqual(
                expectedName,
                actual);
        }

        /// <summary>
        /// Checks whether the ApplicationName method returns an empty string when the phrase does not exist.
        /// </summary>
        [TestMethod]
        public async Task TestApplicationNameEmpty()
        {
            (TokenService service, _) = CreateService("nonexistent");

            string actual = await service.ApplicationName();

            Assert.IsNull(actual);
        }

        /// <summary>
        /// Checks whether the GetUsers method returns the correct usernames and passwords.
        /// </summary>
        [TestMethod]
        public async Task TestGetUsers()
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO APIUser (Username, [Password], IsDeleted) VALUES ('admin', 'password1', 0), ('user', 'password2', 0)",
                    conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            (TokenService service, _) = CreateService("testphrase");

            (string[] usernames, string[] passwords) = await service.GetUsers();

            Assert.AreEqual(
                2,
                usernames.Length);
            Assert.AreEqual(
                2,
                passwords.Length);
            Assert.AreEqual(
                "admin",
                usernames[0]);
            Assert.AreEqual(
                "password1",
                passwords[0]);
            Assert.AreEqual(
                "user",
                usernames[1]);
            Assert.AreEqual(
                "password2",
                passwords[1]);
        }

        /// <summary>
        /// Checks whether the GetUsers method returns empty arrays when no active users exist.
        /// </summary>
        [TestMethod]
        public async Task TestGetUsersEmpty()
        {
            (TokenService service, _) = CreateService("testphrase");

            (string[] usernames, string[] passwords) = await service.GetUsers();

            Assert.AreEqual(
                0,
                usernames.Length);
            Assert.AreEqual(
                0,
                passwords.Length);
        }

        /// <summary>
        /// Checks whether the GetUsers method excludes deleted users.
        /// </summary>
        [TestMethod]
        public async Task TestGetUsersExcludesDeleted()
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO APIUser (Username, [Password], IsDeleted) VALUES ('active', 'pass1', 0), ('deleted', 'pass2', 1)",
                    conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            (TokenService service, _) = CreateService("testphrase");

            (string[] usernames, string[] passwords) = await service.GetUsers();

            Assert.AreEqual(
                1,
                usernames.Length);
            Assert.AreEqual(
                "active",
                usernames[0]);
        }

        /// <summary>
        /// Checks whether the GetAuthorisationPhrases method returns the correct phrases.
        /// </summary>
        [TestMethod]
        public async Task TestGetAuthorisationPhrases()
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO Authorisation (Phrase, IsDeleted) VALUES ('phrase1', 0), ('phrase2', 0)",
                    conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            (TokenService service, _) = CreateService("testphrase");

            string[] actual = await service.GetAuthorisationPhrases();

            Assert.AreEqual(
                2,
                actual.Length);
            Assert.AreEqual(
                "phrase1",
                actual[0]);
            Assert.AreEqual(
                "phrase2",
                actual[1]);
        }

        /// <summary>
        /// Checks whether the GetAuthorisationPhrases method returns an empty array when no active phrases exist.
        /// </summary>
        [TestMethod]
        public async Task TestGetAuthorisationPhrasesEmpty()
        {
            (TokenService service, _) = CreateService("testphrase");

            string[] actual = await service.GetAuthorisationPhrases();

            Assert.AreEqual(
                0,
                actual.Length);
        }

        /// <summary>
        /// Checks whether the GetAuthorisationPhrases method excludes deleted phrases.
        /// </summary>
        [TestMethod]
        public async Task TestGetAuthorisationPhrasesExcludesDeleted()
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO Authorisation (Phrase, IsDeleted) VALUES ('active', 0), ('deleted', 1)",
                    conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            (TokenService service, _) = CreateService("testphrase");

            string[] actual = await service.GetAuthorisationPhrases();

            Assert.AreEqual(
                1,
                actual.Length);
            Assert.AreEqual(
                "active",
                actual[0]);
        }

    }
}

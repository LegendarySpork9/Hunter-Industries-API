// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Implementations;
using HunterIndustriesAPI.Models.Requests.Bodies.Configuration;
using HunterIndustriesAPI.Objects.Configuration;
using HunterIndustriesAPI.PersistenceTests.API.Helpers;
using HunterIndustriesAPI.Services;
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.PersistenceTests.API.Services
{
    [TestClass]
    [DoNotParallelize]
    public class ConfigurationServiceTest
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
        private ConfigurationService CreateService()
        {
            Mock<IDatabaseOptions> mockOptions = new();
            mockOptions.Setup(o => o.ConnectionString)
                .Returns(_ConnectionString);
            mockOptions.Setup(o => o.SQLFiles)
                .Returns(_SqlFilesPath);

            DatabaseWrapper database = new(mockOptions.Object);

            ConfigurationService service = new(
                _MockLogger.Object,
                _FileSystem,
                mockOptions.Object,
                database);

            return service;
        }

        /// <summary>
        /// Inserts a component record into the database.
        /// </summary>
        private void InsertComponent(
            string name,
            bool isDeleted = false)
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO Component ([Name], IsDeleted) VALUES (@name, @isDeleted)",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@name",
                        name);
                    cmd.Parameters.AddWithValue(
                        "@isDeleted",
                        isDeleted);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Inserts a machine record into the database.
        /// </summary>
        private void InsertMachine(
            string hostName,
            bool isDeleted = false)
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO Machine (HostName, IsDeleted) VALUES (@hostName, @isDeleted)",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@hostName",
                        hostName);
                    cmd.Parameters.AddWithValue(
                        "@isDeleted",
                        isDeleted);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Inserts an authorisation record and returns the generated ID.
        /// </summary>
        private int InsertAuthorisation(
            string phrase,
            bool isDeleted = false)
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO Authorisation (Phrase, IsDeleted) VALUES (@phrase, @isDeleted); SELECT SCOPE_IDENTITY();",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@phrase",
                        phrase);
                    cmd.Parameters.AddWithValue(
                        "@isDeleted",
                        isDeleted);

                    return (int)(decimal)cmd.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// Inserts an application record and returns the generated ID.
        /// </summary>
        private int InsertApplication(
            int phraseId,
            string name,
            bool isDeleted = false)
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO [Application] (PhraseId, [Name], IsDeleted) VALUES (@phraseId, @name, @isDeleted); SELECT SCOPE_IDENTITY();",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@phraseId",
                        phraseId);
                    cmd.Parameters.AddWithValue(
                        "@name",
                        name);
                    cmd.Parameters.AddWithValue(
                        "@isDeleted",
                        isDeleted);

                    return (int)(decimal)cmd.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// Inserts an application setting record into the database.
        /// </summary>
        private void InsertApplicationSetting(
            int applicationId,
            string name,
            string type,
            bool required,
            bool isDeleted = false)
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO ApplicationSetting (ApplicationId, [Name], [Type], [Required], IsDeleted) VALUES (@applicationId, @name, @type, @required, @isDeleted)",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@applicationId",
                        applicationId);
                    cmd.Parameters.AddWithValue(
                        "@name",
                        name);
                    cmd.Parameters.AddWithValue(
                        "@type",
                        type);
                    cmd.Parameters.AddWithValue(
                        "@required",
                        required);
                    cmd.Parameters.AddWithValue(
                        "@isDeleted",
                        isDeleted);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        /// <summary>
        /// Checks whether the GetRecords method returns the correct records and total count.
        /// </summary>
        [TestMethod]
        public async Task TestGetRecords()
        {
            InsertMachine("TestMachine");

            ConfigurationService service = CreateService();

            (List<object> actual, int totalRecords) = await service.GetRecords(
                "machine",
                null,
                true,
                10,
                1);

            Assert.AreEqual(
                1,
                actual.Count);
            Assert.AreEqual(
                1,
                totalRecords);
            Assert.AreEqual(
                "TestMachine",
                ((MachineRecord)actual[0]).HostName);
        }

        /// <summary>
        /// Checks whether the GetRecords method returns an empty list and zero count when the database returns no results.
        /// </summary>
        [TestMethod]
        public async Task TestGetRecordsEmpty()
        {
            ConfigurationService service = CreateService();

            (List<object> actual, int totalRecords) = await service.GetRecords(
                "machine",
                null,
                true,
                10,
                1);

            Assert.AreEqual(
                0,
                actual.Count);
            Assert.AreEqual(
                0,
                totalRecords);
        }

        /// <summary>
        /// Checks whether the GetRecords method groups application records with matching ids.
        /// </summary>
        [TestMethod]
        public async Task TestGetRecordsApplicationGrouping()
        {
            int phraseId = InsertAuthorisation("testphrase");
            int applicationId = InsertApplication(
                phraseId,
                "App1");
            InsertApplicationSetting(
                applicationId,
                "Setting1",
                "String",
                true);
            InsertApplicationSetting(
                applicationId,
                "Setting2",
                "Boolean",
                false);

            ConfigurationService service = CreateService();

            (List<object> actual, int totalRecords) = await service.GetRecords(
                "application",
                null,
                true,
                10,
                1);

            Assert.AreEqual(
                1,
                actual.Count);
            Assert.AreEqual(
                2,
                ((ApplicationRecord)actual[0]).Settings.Count);
        }

        /// <summary>
        /// Checks whether the GetRecords method filters out deleted application settings when grouping.
        /// </summary>
        [TestMethod]
        public async Task TestGetRecordsApplicationGroupingFiltersDeleted()
        {
            int phraseId = InsertAuthorisation("testphrase");
            int applicationId = InsertApplication(
                phraseId,
                "App1");
            InsertApplicationSetting(
                applicationId,
                "Setting1",
                "String",
                true);
            InsertApplicationSetting(
                applicationId,
                "Setting2",
                "Boolean",
                false,
                true);

            ConfigurationService service = CreateService();

            (List<object> actual, int totalRecords) = await service.GetRecords(
                "application",
                null,
                true,
                10,
                1);

            Assert.AreEqual(
                1,
                actual.Count);
            Assert.AreEqual(
                1,
                ((ApplicationRecord)actual[0]).Settings.Count);
            Assert.AreEqual(
                "Setting1",
                ((ApplicationRecord)actual[0]).Settings[0].Name);
        }

        /// <summary>
        /// Checks whether the GetRecords method filters by parent entity id when provided.
        /// </summary>
        [TestMethod]
        public async Task TestGetRecordsByParentEntityId()
        {
            int phraseId = InsertAuthorisation("testphrase");
            int applicationId = InsertApplication(
                phraseId,
                "App1");
            InsertApplicationSetting(
                applicationId,
                "Setting1",
                "String",
                true);

            ConfigurationService service = CreateService();

            (List<object> actual, int totalRecords) = await service.GetRecords(
                "applicationSetting",
                applicationId);

            Assert.AreEqual(
                1,
                actual.Count);
            Assert.AreEqual(
                1,
                totalRecords);
        }

        /// <summary>
        /// Checks whether the GetRecord method returns a single record when found.
        /// </summary>
        [TestMethod]
        public async Task TestGetRecord()
        {
            InsertComponent("TestComponent");

            int componentId;
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "SELECT TOP 1 ComponentId FROM Component WHERE [Name] = 'TestComponent'",
                    conn))
                {
                    componentId = (int)cmd.ExecuteScalar();
                }
            }

            ConfigurationService service = CreateService();

            object actual = await service.GetRecord(
                "component",
                componentId);

            Assert.IsNotNull(actual);
            Assert.AreEqual(
                "TestComponent",
                ((ComponentRecord)actual).Name);
        }

        /// <summary>
        /// Checks whether the GetRecord method returns null when no record is found.
        /// </summary>
        [TestMethod]
        public async Task TestGetRecordEmpty()
        {
            ConfigurationService service = CreateService();

            object actual = await service.GetRecord(
                "component",
                99999);

            Assert.IsNull(actual);
        }

        /// <summary>
        /// Checks whether the RecordExists method returns true when a matching record is found.
        /// </summary>
        [TestMethod]
        public async Task TestRecordExistsObject()
        {
            InsertComponent("TestComponent");

            ConfigurationService service = CreateService();

            bool actual = await service.RecordExists(
                "component",
                new ComponentModel { Name = "TestComponent" });

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the RecordExists method returns false when no matching record is found.
        /// </summary>
        [TestMethod]
        public async Task TestRecordExistsObjectNotFound()
        {
            ConfigurationService service = CreateService();

            bool actual = await service.RecordExists(
                "component",
                new ComponentModel { Name = "NonExistent" });

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the RecordExists method returns true when a matching record is found.
        /// </summary>
        [TestMethod]
        public async Task TestRecordExistsString()
        {
            InsertComponent("TestComponent");

            int componentId;
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "SELECT TOP 1 ComponentId FROM Component WHERE [Name] = 'TestComponent'",
                    conn))
                {
                    componentId = (int)cmd.ExecuteScalar();
                }
            }

            ConfigurationService service = CreateService();

            bool actual = await service.RecordExists(
                "component",
                componentId);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the RecordExists method returns false when no matching record is found.
        /// </summary>
        [TestMethod]
        public async Task TestRecordExistsStringNotFound()
        {
            ConfigurationService service = CreateService();

            bool actual = await service.RecordExists(
                "component",
                99999);

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the RecordCreated method returns true and the record id when the record is created.
        /// </summary>
        [TestMethod]
        public async Task TestRecordCreated()
        {
            ConfigurationService service = CreateService();

            (bool created, int recordId) = await service.RecordCreated(
                "component",
                new ComponentModel { Name = "NewComponent" });

            Assert.IsTrue(created);
            Assert.IsTrue(
                recordId > 0);
        }

        /// <summary>
        /// Checks whether the RecordCreated method returns false when the database returns null.
        /// </summary>
        [TestMethod]
        public async Task TestRecordCreatedNullResult()
        {
            ConfigurationService service = CreateService();

            (bool created, int recordId) = await service.RecordCreated(
                "application",
                new ApplicationModel { Name = "TestApp", Phrase = "NonExistentPhrase" });

            Assert.IsFalse(created);
            Assert.AreEqual(
                0,
                recordId);
        }

        /// <summary>
        /// Checks whether the RecordCreated method returns false when the database returns an error.
        /// </summary>
        [TestMethod]
        public async Task TestRecordCreatedWithError()
        {
            ConfigurationService service = CreateService();

            (bool created, int recordId) = await service.RecordCreated(
                "application",
                new ApplicationModel { Name = "ErrorApp", Phrase = null });

            Assert.IsFalse(created);
            Assert.AreEqual(
                0,
                recordId);
        }

        /// <summary>
        /// Checks whether the RecordUpdated method returns true when one row is affected.
        /// </summary>
        [TestMethod]
        public async Task TestRecordUpdated()
        {
            InsertComponent("OriginalComponent");

            int componentId;
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "SELECT TOP 1 ComponentId FROM Component WHERE [Name] = 'OriginalComponent'",
                    conn))
                {
                    componentId = (int)cmd.ExecuteScalar();
                }
            }

            ConfigurationService service = CreateService();

            bool actual = await service.RecordUpdated(
                "component",
                componentId,
                new ComponentModel { Name = "UpdatedComponent" });

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the RecordUpdated method returns false when zero rows are affected.
        /// </summary>
        [TestMethod]
        public async Task TestRecordUpdatedNoRowsAffected()
        {
            ConfigurationService service = CreateService();

            bool actual = await service.RecordUpdated(
                "component",
                99999,
                new ComponentModel { Name = "UpdatedComponent" });

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the RecordUpdated method returns false when the database returns an error.
        /// </summary>
        [TestMethod]
        public async Task TestRecordUpdatedWithError()
        {
            Mock<IDatabaseOptions> mockOptions = new();
            mockOptions.Setup(o => o.ConnectionString)
                .Returns("Server=(localdb)\\MSSQLLocalDB;Database=NonExistentDb_12345;Integrated Security=true;");
            mockOptions.Setup(o => o.SQLFiles)
                .Returns(_SqlFilesPath);

            DatabaseWrapper database = new(mockOptions.Object);

            ConfigurationService service = new(
                _MockLogger.Object,
                _FileSystem,
                mockOptions.Object,
                database);

            bool actual = await service.RecordUpdated(
                "component",
                1,
                new ComponentModel { Name = "UpdatedComponent" });

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the RecordDeleted method returns true when one row is affected.
        /// </summary>
        [TestMethod]
        public async Task TestRecordDeleted()
        {
            InsertComponent("DeleteMe");

            int componentId;
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "SELECT TOP 1 ComponentId FROM Component WHERE [Name] = 'DeleteMe'",
                    conn))
                {
                    componentId = (int)cmd.ExecuteScalar();
                }
            }

            ConfigurationService service = CreateService();

            bool actual = await service.RecordDeleted(
                "component",
                componentId);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the RecordDeleted method returns false when zero rows are affected.
        /// </summary>
        [TestMethod]
        public async Task TestRecordDeletedNoRowsAffected()
        {
            ConfigurationService service = CreateService();

            bool actual = await service.RecordDeleted(
                "component",
                99999);

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the RecordDeleted method returns false when the database returns an error.
        /// </summary>
        [TestMethod]
        public async Task TestRecordDeletedWithError()
        {
            Mock<IDatabaseOptions> mockOptions = new();
            mockOptions.Setup(o => o.ConnectionString)
                .Returns("Server=(localdb)\\MSSQLLocalDB;Database=NonExistentDb_12345;Integrated Security=true;");
            mockOptions.Setup(o => o.SQLFiles)
                .Returns(_SqlFilesPath);

            DatabaseWrapper database = new(mockOptions.Object);

            ConfigurationService service = new(
                _MockLogger.Object,
                _FileSystem,
                mockOptions.Object,
                database);

            bool actual = await service.RecordDeleted(
                "component",
                1);

            Assert.IsFalse(actual);
        }

    }
}

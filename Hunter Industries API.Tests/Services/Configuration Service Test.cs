// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Models.Requests.Bodies.Configuration;
using HunterIndustriesAPI.Objects.Configuration;
using HunterIndustriesAPI.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.Tests.Services
{
    [TestClass]
    public class ConfigurationServiceTest
    {
        private readonly Mock<ILoggerService> _mockLogger = new Mock<ILoggerService>();
        private readonly Mock<IFileSystem> _mockFileSystem = new Mock<IFileSystem>();
        private readonly Mock<IDatabaseOptions> _mockOptions = new Mock<IDatabaseOptions>();

        [TestInitialize]
        public void Setup()
        {
            _mockOptions.Setup(o => o.SQLFiles).Returns("C:\\SQL");
            _mockFileSystem.Setup(f => f.ReadAllText(It.IsAny<string>())).Returns("SELECT 1");
        }

        #region GetRecords

        /// <summary>
        /// Checks whether the GetRecords method returns the correct records and total count.
        /// </summary>
        [TestMethod]
        public async Task TestGetRecords()
        {
            List<object> records = new List<object>
            {
                new ComponentRecord
                {
                    Id = 1,
                    Name = "TestComponent",
                    IsDeleted = false
                }
            };

            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, object>>(), It.IsAny<SqlParameter[]>()).Result).Returns((records, null));
            _mockDatabase.Setup(d => d.QuerySingle(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, int>>(), It.IsAny<SqlParameter[]>()).Result).Returns((5, null));

            ConfigurationService service = new ConfigurationService(_mockLogger.Object, _mockFileSystem.Object, _mockOptions.Object, _mockDatabase.Object);

            (List<object> actual, int totalRecords) = await service.GetRecords("component", 0, null, 10, 1);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(5, totalRecords);
            Assert.AreEqual("TestComponent", ((ComponentRecord)actual[0]).Name);
        }

        /// <summary>
        /// Checks whether the GetRecords method returns an empty list and zero count when the database returns no results.
        /// </summary>
        [TestMethod]
        public async Task TestGetRecordsEmpty()
        {
            List<object> records = new List<object>();

            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, object>>(), It.IsAny<SqlParameter[]>()).Result).Returns((records, null));
            _mockDatabase.Setup(d => d.QuerySingle(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, int>>(), It.IsAny<SqlParameter[]>()).Result).Returns((0, null));

            ConfigurationService service = new ConfigurationService(_mockLogger.Object, _mockFileSystem.Object, _mockOptions.Object, _mockDatabase.Object);

            (List<object> actual, int totalRecords) = await service.GetRecords("component", 0, null, 10, 1);

            Assert.AreEqual(0, actual.Count);
            Assert.AreEqual(0, totalRecords);
        }

        /// <summary>
        /// Checks whether the GetRecords method returns a single record when an id is provided.
        /// </summary>
        [TestMethod]
        public async Task TestGetRecordsById()
        {
            List<object> records = new List<object>
            {
                new ComponentRecord
                {
                    Id = 1,
                    Name = "TestComponent",
                    IsDeleted = false
                }
            };

            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, object>>(), It.IsAny<SqlParameter[]>()).Result).Returns((records, null));

            ConfigurationService service = new ConfigurationService(_mockLogger.Object, _mockFileSystem.Object, _mockOptions.Object, _mockDatabase.Object);

            (List<object> actual, int totalRecords) = await service.GetRecords("component", 1);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(0, totalRecords);
        }

        /// <summary>
        /// Checks whether the GetRecords method groups application records with matching ids.
        /// </summary>
        [TestMethod]
        public async Task TestGetRecordsApplicationGrouping()
        {
            List<object> records = new List<object>
            {
                new ApplicationRecord
                {
                    Id = 1,
                    Name = "App1",
                    IsDeleted = false,
                    Settings = new List<ApplicationSettingRecord>
                    {
                        new ApplicationSettingRecord { Id = 1, Name = "Setting1", Type = "String", Required = true, IsDeleted = false }
                    }
                },
                new ApplicationRecord
                {
                    Id = 1,
                    Name = "App1",
                    IsDeleted = false,
                    Settings = new List<ApplicationSettingRecord>
                    {
                        new ApplicationSettingRecord { Id = 2, Name = "Setting2", Type = "Boolean", Required = false, IsDeleted = false }
                    }
                }
            };

            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, object>>(), It.IsAny<SqlParameter[]>()).Result).Returns((records, null));
            _mockDatabase.Setup(d => d.QuerySingle(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, int>>(), It.IsAny<SqlParameter[]>()).Result).Returns((1, null));

            ConfigurationService service = new ConfigurationService(_mockLogger.Object, _mockFileSystem.Object, _mockOptions.Object, _mockDatabase.Object);

            (List<object> actual, int totalRecords) = await service.GetRecords("application", 0, null, 10, 1);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(2, ((ApplicationRecord)actual[0]).Settings.Count);
        }

        /// <summary>
        /// Checks whether the GetRecords method filters out deleted application settings when grouping.
        /// </summary>
        [TestMethod]
        public async Task TestGetRecordsApplicationGroupingFiltersDeleted()
        {
            List<object> records = new List<object>
            {
                new ApplicationRecord
                {
                    Id = 1,
                    Name = "App1",
                    IsDeleted = false,
                    Settings = new List<ApplicationSettingRecord>
                    {
                        new ApplicationSettingRecord { Id = 1, Name = "Setting1", Type = "String", Required = true, IsDeleted = false }
                    }
                },
                new ApplicationRecord
                {
                    Id = 1,
                    Name = "App1",
                    IsDeleted = false,
                    Settings = new List<ApplicationSettingRecord>
                    {
                        new ApplicationSettingRecord { Id = 2, Name = "Setting2", Type = "Boolean", Required = false, IsDeleted = true }
                    }
                }
            };

            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, object>>(), It.IsAny<SqlParameter[]>()).Result).Returns((records, null));
            _mockDatabase.Setup(d => d.QuerySingle(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, int>>(), It.IsAny<SqlParameter[]>()).Result).Returns((1, null));

            ConfigurationService service = new ConfigurationService(_mockLogger.Object, _mockFileSystem.Object, _mockOptions.Object, _mockDatabase.Object);

            (List<object> actual, int totalRecords) = await service.GetRecords("application", 0, null, 10, 1);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(1, ((ApplicationRecord)actual[0]).Settings.Count);
            Assert.AreEqual("Setting1", ((ApplicationRecord)actual[0]).Settings[0].Name);
        }

        /// <summary>
        /// Checks whether the GetRecords method filters by parent entity id when provided.
        /// </summary>
        [TestMethod]
        public async Task TestGetRecordsByParentEntityId()
        {
            List<object> records = new List<object>
            {
                new ComponentRecord
                {
                    Id = 1,
                    Name = "TestComponent",
                    IsDeleted = false
                }
            };

            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, object>>(), It.IsAny<SqlParameter[]>()).Result).Returns((records, null));

            ConfigurationService service = new ConfigurationService(_mockLogger.Object, _mockFileSystem.Object, _mockOptions.Object, _mockDatabase.Object);

            (List<object> actual, int totalRecords) = await service.GetRecords("applicationSetting", 0, 1);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(0, totalRecords);
        }

        #endregion

        #region RecordExists (string, object, int)

        /// <summary>
        /// Checks whether the RecordExists method returns true when a matching record is found.
        /// </summary>
        [TestMethod]
        public async Task TestRecordExistsObject()
        {
            List<int> results = new List<int> { 1 };

            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, int>>(), It.IsAny<SqlParameter[]>()).Result).Returns((results, null));

            ConfigurationService service = new ConfigurationService(_mockLogger.Object, _mockFileSystem.Object, _mockOptions.Object, _mockDatabase.Object);

            bool actual = await service.RecordExists("component", new ComponentModel { Name = "TestComponent" });

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the RecordExists method returns false when no matching record is found.
        /// </summary>
        [TestMethod]
        public async Task TestRecordExistsObjectNotFound()
        {
            List<int> results = new List<int>();

            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, int>>(), It.IsAny<SqlParameter[]>()).Result).Returns((results, null));

            ConfigurationService service = new ConfigurationService(_mockLogger.Object, _mockFileSystem.Object, _mockOptions.Object, _mockDatabase.Object);

            bool actual = await service.RecordExists("component", new ComponentModel { Name = "TestComponent" });

            Assert.IsFalse(actual);
        }

        #endregion

        #region RecordExists (string, int)

        /// <summary>
        /// Checks whether the RecordExists method returns true when a matching record is found.
        /// </summary>
        [TestMethod]
        public async Task TestRecordExistsString()
        {
            List<int> results = new List<int> { 1 };

            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, int>>(), It.IsAny<SqlParameter[]>()).Result).Returns((results, null));

            ConfigurationService service = new ConfigurationService(_mockLogger.Object, _mockFileSystem.Object, _mockOptions.Object, _mockDatabase.Object);

            bool actual = await service.RecordExists("component", 1);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the RecordExists method returns false when no matching record is found.
        /// </summary>
        [TestMethod]
        public async Task TestRecordExistsStringNotFound()
        {
            List<int> results = new List<int>();

            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, int>>(), It.IsAny<SqlParameter[]>()).Result).Returns((results, null));

            ConfigurationService service = new ConfigurationService(_mockLogger.Object, _mockFileSystem.Object, _mockOptions.Object, _mockDatabase.Object);

            bool actual = await service.RecordExists("component", 1);

            Assert.IsFalse(actual);
        }

        #endregion

        #region RecordCreated

        /// <summary>
        /// Checks whether the RecordCreated method returns true and the record id when the record is created.
        /// </summary>
        [TestMethod]
        public async Task TestRecordCreated()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns(((object)1, null));

            ConfigurationService service = new ConfigurationService(_mockLogger.Object, _mockFileSystem.Object, _mockOptions.Object, _mockDatabase.Object);

            (bool created, int recordId) = await service.RecordCreated("component", new ComponentModel { Name = "TestComponent" });

            Assert.IsTrue(created);
            Assert.AreEqual(1, recordId);
        }

        /// <summary>
        /// Checks whether the RecordCreated method returns false when the database returns null.
        /// </summary>
        [TestMethod]
        public async Task TestRecordCreatedNullResult()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns(((object)null, null));

            ConfigurationService service = new ConfigurationService(_mockLogger.Object, _mockFileSystem.Object, _mockOptions.Object, _mockDatabase.Object);

            (bool created, int recordId) = await service.RecordCreated("component", new ComponentModel { Name = "TestComponent" });

            Assert.IsFalse(created);
            Assert.AreEqual(0, recordId);
        }

        /// <summary>
        /// Checks whether the RecordCreated method returns false when the database returns an error.
        /// </summary>
        [TestMethod]
        public async Task TestRecordCreatedWithError()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns(((object)null, new Exception("Database error")));

            ConfigurationService service = new ConfigurationService(_mockLogger.Object, _mockFileSystem.Object, _mockOptions.Object, _mockDatabase.Object);

            (bool created, int recordId) = await service.RecordCreated("component", new ComponentModel { Name = "TestComponent" });

            Assert.IsFalse(created);
            Assert.AreEqual(0, recordId);
        }

        #endregion

        #region RecordUpdated

        /// <summary>
        /// Checks whether the RecordUpdated method returns true when one row is affected.
        /// </summary>
        [TestMethod]
        public async Task TestRecordUpdated()
        {
            _mockFileSystem.Setup(f => f.ReadAllText(It.IsAny<string>())).Returns("update Table\nset Name = @name\nwhere Id = @id");

            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.Execute(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns((1, null));

            ConfigurationService service = new ConfigurationService(_mockLogger.Object, _mockFileSystem.Object, _mockOptions.Object, _mockDatabase.Object);

            bool actual = await service.RecordUpdated("component", 1, new ComponentModel { Name = "UpdatedComponent" });

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the RecordUpdated method returns false when zero rows are affected.
        /// </summary>
        [TestMethod]
        public async Task TestRecordUpdatedNoRowsAffected()
        {
            _mockFileSystem.Setup(f => f.ReadAllText(It.IsAny<string>())).Returns("update Table\nset Name = @name\nwhere Id = @id");

            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.Execute(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns((0, null));

            ConfigurationService service = new ConfigurationService(_mockLogger.Object, _mockFileSystem.Object, _mockOptions.Object, _mockDatabase.Object);

            bool actual = await service.RecordUpdated("component", 1, new ComponentModel { Name = "UpdatedComponent" });

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the RecordUpdated method returns false when the database returns an error.
        /// </summary>
        [TestMethod]
        public async Task TestRecordUpdatedWithError()
        {
            _mockFileSystem.Setup(f => f.ReadAllText(It.IsAny<string>())).Returns("update Table\nset Name = @name\nwhere Id = @id");

            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.Execute(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns((0, new Exception("Database error")));

            ConfigurationService service = new ConfigurationService(_mockLogger.Object, _mockFileSystem.Object, _mockOptions.Object, _mockDatabase.Object);

            bool actual = await service.RecordUpdated("component", 1, new ComponentModel { Name = "UpdatedComponent" });

            Assert.IsFalse(actual);
        }

        #endregion

        #region RecordDeleted

        /// <summary>
        /// Checks whether the RecordDeleted method returns true when one row is affected.
        /// </summary>
        [TestMethod]
        public async Task TestRecordDeleted()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.Execute(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns((1, null));

            ConfigurationService service = new ConfigurationService(_mockLogger.Object, _mockFileSystem.Object, _mockOptions.Object, _mockDatabase.Object);

            bool actual = await service.RecordDeleted("component", 1);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the RecordDeleted method returns false when zero rows are affected.
        /// </summary>
        [TestMethod]
        public async Task TestRecordDeletedNoRowsAffected()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.Execute(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns((0, null));

            ConfigurationService service = new ConfigurationService(_mockLogger.Object, _mockFileSystem.Object, _mockOptions.Object, _mockDatabase.Object);

            bool actual = await service.RecordDeleted("component", 1);

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the RecordDeleted method returns false when the database returns an error.
        /// </summary>
        [TestMethod]
        public async Task TestRecordDeletedWithError()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.Execute(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns((0, new Exception("Database error")));

            ConfigurationService service = new ConfigurationService(_mockLogger.Object, _mockFileSystem.Object, _mockOptions.Object, _mockDatabase.Object);

            bool actual = await service.RecordDeleted("component", 1);

            Assert.IsFalse(actual);
        }

        #endregion
    }
}

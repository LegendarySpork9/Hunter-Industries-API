// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Mappings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace HunterIndustriesAPI.Tests.Mappings
{
    [TestClass]
    public class ScopePermissionMappingTest
    {
        #region GetPermissions

        /// <summary>
        /// Tests whether the GetPermissions method returns an empty list when given an unknown scope.
        /// </summary>
        [TestMethod]
        public void TestGetPermissions()
        {
            List<string> actual = ScopePermissionMapping.GetPermissions(new List<string> { "Trombone" });

            Assert.AreEqual(0, actual.Count);
        }

        /// <summary>
        /// Tests whether the GetPermissions method returns the correct permissions when given "Control Panel API".
        /// </summary>
        [TestMethod]
        public void TestGetPermissionsControlPanelAPI()
        {
            List<string> actual = ScopePermissionMapping.GetPermissions(new List<string> { "Control Panel API" });

            Assert.AreEqual(13, actual.Count);
            Assert.IsTrue(actual.Contains("Assistant.Config"));
            Assert.IsTrue(actual.Contains("Assistant.Deletion"));
            Assert.IsTrue(actual.Contains("Assistant.Location"));
            Assert.IsTrue(actual.Contains("Assistant.Version"));
            Assert.IsTrue(actual.Contains("AuditHistory"));
            Assert.IsTrue(actual.Contains("Configuration"));
            Assert.IsTrue(actual.Contains("ErrorLog"));
            Assert.IsTrue(actual.Contains("Statistic"));
            Assert.IsTrue(actual.Contains("ServerStatus.Alert"));
            Assert.IsTrue(actual.Contains("ServerStatus.Event"));
            Assert.IsTrue(actual.Contains("ServerStatus.Information"));
            Assert.IsTrue(actual.Contains("User"));
            Assert.IsTrue(actual.Contains("UserSettings"));
        }

        /// <summary>
        /// Tests whether the GetPermissions method returns the correct permissions when given "Assistant API".
        /// </summary>
        [TestMethod]
        public void TestGetPermissionsAssistantAPI()
        {
            List<string> actual = ScopePermissionMapping.GetPermissions(new List<string> { "Assistant API" });

            Assert.AreEqual(4, actual.Count);
            Assert.IsTrue(actual.Contains("Assistant.Config"));
            Assert.IsTrue(actual.Contains("Assistant.Deletion"));
            Assert.IsTrue(actual.Contains("Assistant.Location"));
            Assert.IsTrue(actual.Contains("Assistant.Version"));
        }

        /// <summary>
        /// Tests whether the GetPermissions method returns the correct permissions when given "Server Status API".
        /// </summary>
        [TestMethod]
        public void TestGetPermissionsServerStatusAPI()
        {
            List<string> actual = ScopePermissionMapping.GetPermissions(new List<string> { "Server Status API" });

            Assert.AreEqual(7, actual.Count);
            Assert.IsTrue(actual.Contains("ServerStatus.Alert"));
            Assert.IsTrue(actual.Contains("ServerStatus.Event"));
            Assert.IsTrue(actual.Contains("ServerStatus.Information.Read"));
            Assert.IsTrue(actual.Contains("User.Read"));
            Assert.IsTrue(actual.Contains("User.Update"));
            Assert.IsTrue(actual.Contains("UserSettings.Read"));
            Assert.IsTrue(actual.Contains("UserSettings.Update"));
        }

        /// <summary>
        /// Tests whether the GetPermissions method returns distinct permissions when given multiple overlapping scopes.
        /// </summary>
        [TestMethod]
        public void TestGetPermissionsDistinct()
        {
            List<string> actual = ScopePermissionMapping.GetPermissions(new List<string> { "Control Panel API", "Assistant API" });

            Assert.AreEqual(13, actual.Count);
        }

        #endregion

        #region HasPermission

        /// <summary>
        /// Tests whether the HasPermission method returns false when given an unmatched permission.
        /// </summary>
        [TestMethod]
        public void TestHasPermission()
        {
            List<string> granted = new List<string> { "Assistant.Config" };
            bool actual = ScopePermissionMapping.HasPermission(granted, "User");

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Tests whether the HasPermission method returns true when given an exact match.
        /// </summary>
        [TestMethod]
        public void TestHasPermissionExact()
        {
            List<string> granted = new List<string> { "Assistant.Config" };
            bool actual = ScopePermissionMapping.HasPermission(granted, "Assistant.Config");

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Tests whether the HasPermission method returns true when the granted permission is a parent of the required permission.
        /// </summary>
        [TestMethod]
        public void TestHasPermissionParent()
        {
            List<string> granted = new List<string> { "User" };
            bool actual = ScopePermissionMapping.HasPermission(granted, "User.Read");

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Tests whether the HasPermission method returns true when the required permission is a parent of the granted permission.
        /// </summary>
        [TestMethod]
        public void TestHasPermissionChild()
        {
            List<string> granted = new List<string> { "User.Read" };
            bool actual = ScopePermissionMapping.HasPermission(granted, "User");

            Assert.IsTrue(actual);
        }

        #endregion
    }
}

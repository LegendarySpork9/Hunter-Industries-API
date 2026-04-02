// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HunterIndustriesAPI.Tests.Converters
{
    [TestClass]
    public class AuditHistoryConverterTest
    {
        #region GetEndpointID

        /// <summary>
        /// Tests whether the GetEndpointID method returns 0 when given any value.
        /// </summary>
        [TestMethod]
        public void TestGetEndpointID()
        {
            int expected = 0;
            int actual = AuditHistoryConverter.GetEndpointID("Trombone");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetEndpointID method returns 1 when given "token".
        /// </summary>
        [TestMethod]
        public void TestGetEndpointIDToken()
        {
            int expected = 1;
            int actual = AuditHistoryConverter.GetEndpointID("token");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetEndpointID method returns 2 when given "audithistory".
        /// </summary>
        [TestMethod]
        public void TestGetEndpointIDAuditHistory()
        {
            int expected = 2;
            int actual = AuditHistoryConverter.GetEndpointID("audithistory");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetEndpointID method returns 3 when given "assistant/config".
        /// </summary>
        [TestMethod]
        public void TestGetEndpointIDAssistantConfig()
        {
            int expected = 3;
            int actual = AuditHistoryConverter.GetEndpointID("assistant/config");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetEndpointID method returns 4 when given "assistant/version".
        /// </summary>
        [TestMethod]
        public void TestGetEndpointIDAssistantVersion()
        {
            int expected = 4;
            int actual = AuditHistoryConverter.GetEndpointID("assistant/version");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetEndpointID method returns 5 when given "assistant/deletion".
        /// </summary>
        [TestMethod]
        public void TestGetEndpointIDAssistantDeletion()
        {
            int expected = 5;
            int actual = AuditHistoryConverter.GetEndpointID("assistant/deletion");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetEndpointID method returns 6 when given "assistant/location".
        /// </summary>
        [TestMethod]
        public void TestGetEndpointIDAssistantLocation()
        {
            int expected = 6;
            int actual = AuditHistoryConverter.GetEndpointID("assistant/location");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetEndpointID method returns 7 when given "user".
        /// </summary>
        [TestMethod]
        public void TestGetEndpointIDUser()
        {
            int expected = 7;
            int actual = AuditHistoryConverter.GetEndpointID("user");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetEndpointID method returns 8 when given "usersettings".
        /// </summary>
        [TestMethod]
        public void TestGetEndpointIDUserSettings()
        {
            int expected = 8;
            int actual = AuditHistoryConverter.GetEndpointID("usersettings");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetEndpointID method returns 9 when given "serverstatus/serverinformation".
        /// </summary>
        [TestMethod]
        public void TestGetEndpointIDServerInformation()
        {
            int expected = 9;
            int actual = AuditHistoryConverter.GetEndpointID("serverstatus/serverinformation");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetEndpointID method returns 10 when given "serverstatus/serverevent".
        /// </summary>
        [TestMethod]
        public void TestGetEndpointIDServerEvent()
        {
            int expected = 10;
            int actual = AuditHistoryConverter.GetEndpointID("serverstatus/serverevent");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetEndpointID method returns 11 when given "serverstatus/serveralert".
        /// </summary>
        [TestMethod]
        public void TestGetEndpointIDServerAlert()
        {
            int expected = 11;
            int actual = AuditHistoryConverter.GetEndpointID("serverstatus/serveralert");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetEndpointID method returns 12 when given "errorlog".
        /// </summary>
        [TestMethod]
        public void TestGetEndpointIDErrorLog()
        {
            int expected = 12;
            int actual = AuditHistoryConverter.GetEndpointID("errorlog");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetEndpointID method returns 13 when given "configuration".
        /// </summary>
        [TestMethod]
        public void TestGetEndpointIDConfiguration()
        {
            int expected = 13;
            int actual = AuditHistoryConverter.GetEndpointID("configuration");

            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region GetEndpointVersionID

        /// <summary>
        /// Tests whether the GetEndpointVersionID method returns 1 when given any value.
        /// </summary>
        [TestMethod]
        public void TestGetEndpointVersionID()
        {
            int expected = 1;
            int actual = AuditHistoryConverter.GetEndpointVersionID("Trombone");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetEndpointVersionID method returns 1 when given "1.0".
        /// </summary>
        [TestMethod]
        public void TestGetEndpointVersionIDV10()
        {
            int expected = 1;
            int actual = AuditHistoryConverter.GetEndpointVersionID("1.0");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetEndpointVersionID method returns 2 when given "1.1".
        /// </summary>
        [TestMethod]
        public void TestGetEndpointVersionIDV11()
        {
            int expected = 2;
            int actual = AuditHistoryConverter.GetEndpointVersionID("1.1");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetEndpointVersionID method returns 3 when given "2.0".
        /// </summary>
        [TestMethod]
        public void TestGetEndpointVersionIDV20()
        {
            int expected = 3;
            int actual = AuditHistoryConverter.GetEndpointVersionID("2.0");

            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region GetMethodID

        /// <summary>
        /// Tests whether the GetMethodID method returns 0 when given any value.
        /// </summary>
        [TestMethod]
        public void TestGetMethodID()
        {
            int expected = 0;
            int actual = AuditHistoryConverter.GetMethodID("Trombone");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetMethodID method returns 1 when given "GET".
        /// </summary>
        [TestMethod]
        public void TestGetMethodIDGet()
        {
            int expected = 1;
            int actual = AuditHistoryConverter.GetMethodID("GET");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetMethodID method returns 2 when given "POST".
        /// </summary>
        [TestMethod]
        public void TestGetMethodIDPost()
        {
            int expected = 2;
            int actual = AuditHistoryConverter.GetMethodID("POST");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetMethodID method returns 3 when given "PATCH".
        /// </summary>
        [TestMethod]
        public void TestGetMethodIDPatch()
        {
            int expected = 3;
            int actual = AuditHistoryConverter.GetMethodID("PATCH");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetMethodID method returns 4 when given "DELETE".
        /// </summary>
        [TestMethod]
        public void TestGetMethodIDDelete()
        {
            int expected = 4;
            int actual = AuditHistoryConverter.GetMethodID("DELETE");

            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region GetStatusID

        /// <summary>
        /// Tests whether the GetStatusID method returns 0 when given any value.
        /// </summary>
        [TestMethod]
        public void TestGetStatusID()
        {
            int expected = 0;
            int actual = AuditHistoryConverter.GetStatusID("Trombone");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetStatusID method returns 1 when given "OK".
        /// </summary>
        [TestMethod]
        public void TestGetStatusIDOK()
        {
            int expected = 1;
            int actual = AuditHistoryConverter.GetStatusID("OK");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetStatusID method returns 2 when given "Created".
        /// </summary>
        [TestMethod]
        public void TestGetStatusIDCreated()
        {
            int expected = 2;
            int actual = AuditHistoryConverter.GetStatusID("Created");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetStatusID method returns 3 when given "BadRequest".
        /// </summary>
        [TestMethod]
        public void TestGetStatusIDBadRequest()
        {
            int expected = 3;
            int actual = AuditHistoryConverter.GetStatusID("BadRequest");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetStatusID method returns 4 when given "Unauthorized".
        /// </summary>
        [TestMethod]
        public void TestGetStatusIDUnauthorized()
        {
            int expected = 4;
            int actual = AuditHistoryConverter.GetStatusID("Unauthorized");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetStatusID method returns 5 when given "Forbidden".
        /// </summary>
        [TestMethod]
        public void TestGetStatusIDForbidden()
        {
            int expected = 5;
            int actual = AuditHistoryConverter.GetStatusID("Forbidden");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetStatusID method returns 6 when given "NotFound".
        /// </summary>
        [TestMethod]
        public void TestGetStatusIDNotFound()
        {
            int expected = 6;
            int actual = AuditHistoryConverter.GetStatusID("NotFound");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetStatusID method returns 7 when given "InternalServerError".
        /// </summary>
        [TestMethod]
        public void TestGetStatusIDInternalServerError()
        {
            int expected = 7;
            int actual = AuditHistoryConverter.GetStatusID("InternalServerError");

            Assert.AreEqual(expected, actual);
        }

        #endregion
    }
}

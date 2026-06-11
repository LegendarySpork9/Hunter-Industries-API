// Copyright © - 11/06/2026 - Toby Hunter
using HunterIndustriesAPI.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HunterIndustriesAPI.Tests.API.Converters
{
    [TestClass]
    public class AuditHistoryConverterTest
    {
        #region GetEndpointId

        /// <summary>
        /// Tests whether the GetEndpointId method returns 0 when given any value.
        /// </summary>
        [TestMethod]
        public void TestGetEndpointId()
        {
            int expected = 0;
            int actual = AuditHistoryConverter.GetEndpointId("Trombone");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetEndpointId method returns 1 when given "token".
        /// </summary>
        [TestMethod]
        public void TestGetEndpointIdToken()
        {
            int expected = 1;
            int actual = AuditHistoryConverter.GetEndpointId("token");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetEndpointId method returns 2 when given "audithistory".
        /// </summary>
        [TestMethod]
        public void TestGetEndpointIdAuditHistory()
        {
            int expected = 2;
            int actual = AuditHistoryConverter.GetEndpointId("audithistory");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetEndpointId method returns 3 when given "assistant/config".
        /// </summary>
        [TestMethod]
        public void TestGetEndpointIdAssistantConfig()
        {
            int expected = 3;
            int actual = AuditHistoryConverter.GetEndpointId("assistant/config");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetEndpointId method returns 4 when given "assistant/version".
        /// </summary>
        [TestMethod]
        public void TestGetEndpointIdAssistantVersion()
        {
            int expected = 4;
            int actual = AuditHistoryConverter.GetEndpointId("assistant/version");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetEndpointId method returns 5 when given "assistant/deletion".
        /// </summary>
        [TestMethod]
        public void TestGetEndpointIdAssistantDeletion()
        {
            int expected = 5;
            int actual = AuditHistoryConverter.GetEndpointId("assistant/deletion");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetEndpointId method returns 6 when given "assistant/location".
        /// </summary>
        [TestMethod]
        public void TestGetEndpointIdAssistantLocation()
        {
            int expected = 6;
            int actual = AuditHistoryConverter.GetEndpointId("assistant/location");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetEndpointId method returns 7 when given "user".
        /// </summary>
        [TestMethod]
        public void TestGetEndpointIdUser()
        {
            int expected = 7;
            int actual = AuditHistoryConverter.GetEndpointId("user");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetEndpointId method returns 8 when given "usersettings".
        /// </summary>
        [TestMethod]
        public void TestGetEndpointIdUserSettings()
        {
            int expected = 8;
            int actual = AuditHistoryConverter.GetEndpointId("usersettings");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetEndpointId method returns 9 when given "serverstatus/serverinformation".
        /// </summary>
        [TestMethod]
        public void TestGetEndpointIdServerInformation()
        {
            int expected = 9;
            int actual = AuditHistoryConverter.GetEndpointId("serverstatus/serverinformation");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetEndpointId method returns 10 when given "serverstatus/serverevent".
        /// </summary>
        [TestMethod]
        public void TestGetEndpointIdServerEvent()
        {
            int expected = 10;
            int actual = AuditHistoryConverter.GetEndpointId("serverstatus/serverevent");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetEndpointId method returns 11 when given "serverstatus/serveralert".
        /// </summary>
        [TestMethod]
        public void TestGetEndpointIdServerAlert()
        {
            int expected = 11;
            int actual = AuditHistoryConverter.GetEndpointId("serverstatus/serveralert");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetEndpointId method returns 12 when given "errorlog".
        /// </summary>
        [TestMethod]
        public void TestGetEndpointIdErrorLog()
        {
            int expected = 12;
            int actual = AuditHistoryConverter.GetEndpointId("errorlog");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetEndpointId method returns 13 when given "configuration".
        /// </summary>
        [TestMethod]
        public void TestGetEndpointIdConfiguration()
        {
            int expected = 13;
            int actual = AuditHistoryConverter.GetEndpointId("configuration");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetEndpointId method returns 14 when given "statistic".
        /// </summary>
        [TestMethod]
        public void TestGetEndpointIdStatistic()
        {
            int expected = 14;
            int actual = AuditHistoryConverter.GetEndpointId("statistic");

            Assert.AreEqual(
                expected,
                actual);
        }

        #endregion

        #region GetEndpointVersionId

        /// <summary>
        /// Tests whether the GetEndpointVersionId method returns 1 when given any value.
        /// </summary>
        [TestMethod]
        public void TestGetEndpointVersionId()
        {
            int expected = 1;
            int actual = AuditHistoryConverter.GetEndpointVersionId("Trombone");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetEndpointVersionId method returns 1 when given "1.0".
        /// </summary>
        [TestMethod]
        public void TestGetEndpointVersionIdV10()
        {
            int expected = 1;
            int actual = AuditHistoryConverter.GetEndpointVersionId("1.0");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetEndpointVersionId method returns 2 when given "1.1".
        /// </summary>
        [TestMethod]
        public void TestGetEndpointVersionIdV11()
        {
            int expected = 2;
            int actual = AuditHistoryConverter.GetEndpointVersionId("1.1");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetEndpointVersionId method returns 3 when given "2.0".
        /// </summary>
        [TestMethod]
        public void TestGetEndpointVersionIdV20()
        {
            int expected = 3;
            int actual = AuditHistoryConverter.GetEndpointVersionId("2.0");

            Assert.AreEqual(
                expected,
                actual);
        }

        #endregion

        #region GetMethodId

        /// <summary>
        /// Tests whether the GetMethodId method returns 0 when given any value.
        /// </summary>
        [TestMethod]
        public void TestGetMethodId()
        {
            int expected = 0;
            int actual = AuditHistoryConverter.GetMethodId("Trombone");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetMethodId method returns 1 when given "GET".
        /// </summary>
        [TestMethod]
        public void TestGetMethodIdGet()
        {
            int expected = 1;
            int actual = AuditHistoryConverter.GetMethodId("GET");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetMethodId method returns 2 when given "POST".
        /// </summary>
        [TestMethod]
        public void TestGetMethodIdPost()
        {
            int expected = 2;
            int actual = AuditHistoryConverter.GetMethodId("POST");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetMethodId method returns 3 when given "PATCH".
        /// </summary>
        [TestMethod]
        public void TestGetMethodIdPatch()
        {
            int expected = 3;
            int actual = AuditHistoryConverter.GetMethodId("PATCH");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetMethodId method returns 4 when given "DELETE".
        /// </summary>
        [TestMethod]
        public void TestGetMethodIdDelete()
        {
            int expected = 4;
            int actual = AuditHistoryConverter.GetMethodId("DELETE");

            Assert.AreEqual(
                expected,
                actual);
        }

        #endregion

        #region GetStatusId

        /// <summary>
        /// Tests whether the GetStatusId method returns 0 when given any value.
        /// </summary>
        [TestMethod]
        public void TestGetStatusId()
        {
            int expected = 0;
            int actual = AuditHistoryConverter.GetStatusId("Trombone");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetStatusId method returns 1 when given "OK".
        /// </summary>
        [TestMethod]
        public void TestGetStatusIdOK()
        {
            int expected = 1;
            int actual = AuditHistoryConverter.GetStatusId("OK");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetStatusId method returns 2 when given "Created".
        /// </summary>
        [TestMethod]
        public void TestGetStatusIdCreated()
        {
            int expected = 2;
            int actual = AuditHistoryConverter.GetStatusId("Created");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetStatusId method returns 3 when given "NoContent".
        /// </summary>
        [TestMethod]
        public void TestGetStatusIdNoContent()
        {
            int expected = 3;
            int actual = AuditHistoryConverter.GetStatusId("NoContent");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetStatusId method returns 4 when given "BadRequest".
        /// </summary>
        [TestMethod]
        public void TestGetStatusIdBadRequest()
        {
            int expected = 4;
            int actual = AuditHistoryConverter.GetStatusId("BadRequest");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetStatusId method returns 5 when given "Unauthorized".
        /// </summary>
        [TestMethod]
        public void TestGetStatusIdUnauthorized()
        {
            int expected = 5;
            int actual = AuditHistoryConverter.GetStatusId("Unauthorized");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetStatusId method returns 6 when given "Forbidden".
        /// </summary>
        [TestMethod]
        public void TestGetStatusIdForbidden()
        {
            int expected = 6;
            int actual = AuditHistoryConverter.GetStatusId("Forbidden");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetStatusId method returns 7 when given "NotFound".
        /// </summary>
        [TestMethod]
        public void TestGetStatusIdNotFound()
        {
            int expected = 7;
            int actual = AuditHistoryConverter.GetStatusId("NotFound");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetStatusId method returns 8 when given "InternalServerError".
        /// </summary>
        [TestMethod]
        public void TestGetStatusIdInternalServerError()
        {
            int expected = 8;
            int actual = AuditHistoryConverter.GetStatusId("InternalServerError");

            Assert.AreEqual(
                expected,
                actual);
        }

        #endregion
    }
}

using HunterIndustriesAPI.Functions;
using HunterIndustriesAPI.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;

namespace HunterIndustriesAPI.Tests.Functions.Tests
{
    [TestClass]
    public class TokenFunctionTest
    {
        private Mock<LoggerService> MockLogger;
        private Mock<TokenService> MockTokenService;

        [TestInitialize]
        public void Setup()
        {
            ConfigurationLoaderFunction.LoadConfig();

            MockLogger = new Mock<LoggerService>(null);

            JObject authJSON = JSONLoaderFunction.LoadJSON("Token/PostOK.json");

            MockTokenService = new Mock<TokenService>(authJSON["phrase"].ToString(), MockLogger.Object);
        }

        [TestMethod]
        public void TestExtractCredentialsFromBasicAuth()
        {
            Mock<TokenFunction> _mockTokenFunction = new Mock<TokenFunction>(MockTokenService.Object, MockLogger.Object);
            
            JObject authJSON = JSONLoaderFunction.LoadJSON("Token/PostOK.json");
            (string username, string password) = _mockTokenFunction.Object.ExtractCredentialsFromBasicAuth($"Basic {authJSON["basicCredentials"]}");

            Assert.IsTrue(username != string.Empty);
            Assert.IsTrue(password != string.Empty);
        }

        [TestMethod]
        public void TestExtractCredentialsFromBasicAuthFail()
        {
            Mock<TokenFunction> _mockTokenFunction = new Mock<TokenFunction>(MockTokenService.Object, MockLogger.Object);

            (string username, string password) = _mockTokenFunction.Object.ExtractCredentialsFromBasicAuth("Basic ");

            Assert.IsTrue(username == string.Empty);
            Assert.IsTrue(password == string.Empty);
        }

        [TestMethod]
        public void TestIsValidUser()
        {
            Mock<TokenFunction> _mockTokenFunction = new Mock<TokenFunction>(MockTokenService.Object, MockLogger.Object);

            JObject authJSON = JSONLoaderFunction.LoadJSON("Token/PostOK.json");
            (string username, string password) = _mockTokenFunction.Object.ExtractCredentialsFromBasicAuth($"Basic {authJSON["basicCredentials"]}");

            bool validUser = _mockTokenFunction.Object.IsValidUser(username, password, authJSON["phrase"].ToString());

            Assert.IsTrue(validUser);
        }

        [TestMethod]
        public void TestIsValidUserFalsePhrase()
        {
            Mock<TokenFunction> _mockTokenFunction = new Mock<TokenFunction>(MockTokenService.Object, MockLogger.Object);

            JObject authJSON = JSONLoaderFunction.LoadJSON("Token/PostOK.json");
            (string username, string password) = _mockTokenFunction.Object.ExtractCredentialsFromBasicAuth($"Basic {authJSON["basicCredentials"]}");

            bool validUser = _mockTokenFunction.Object.IsValidUser(username, password, "Luke, I am your father!");

            Assert.IsFalse(validUser);
        }

        [TestMethod]
        public void TestIsValidUserFalsePassword()
        {
            Mock<TokenFunction> _mockTokenFunction = new Mock<TokenFunction>(MockTokenService.Object, MockLogger.Object);

            JObject authJSON = JSONLoaderFunction.LoadJSON("Token/PostOK.json");
            (string username, string password) = _mockTokenFunction.Object.ExtractCredentialsFromBasicAuth($"Basic {authJSON["basicCredentials"]}");

            bool validUser = _mockTokenFunction.Object.IsValidUser(username, "Luke, I am your father!", authJSON["phrase"].ToString());

            Assert.IsFalse(validUser);
        }

        [TestMethod]
        public void TestIsValidUserFalseUsername()
        {
            Mock<TokenFunction> _mockTokenFunction = new Mock<TokenFunction>(MockTokenService.Object, MockLogger.Object);

            JObject authJSON = JSONLoaderFunction.LoadJSON("Token/PostOK.json");
            (string username, string password) = _mockTokenFunction.Object.ExtractCredentialsFromBasicAuth($"Basic {authJSON["basicCredentials"]}");

            bool validUser = _mockTokenFunction.Object.IsValidUser("Luke, I am your father!", password, authJSON["phrase"].ToString());

            Assert.IsFalse(validUser);
        }
    }
}

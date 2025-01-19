using HunterIndustriesAPI.Services;
using HunterIndustriesAPI.Tests.Functions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;

namespace HunterIndustriesAPI.Tests.Services
{
    [TestClass]
    public class TokenServiceTest
    {
        private Mock<TokenService> MockTokenService;

        [TestInitialize]
        public void SetUp()
        {
            ConfigurationLoaderFunction.LoadConfig();

            Mock<LoggerService> _mockLoggerService = new Mock<LoggerService>(null);
            JObject authJSON = JSONLoaderFunction.LoadJSON("Token/PostOK.json");
            MockTokenService = new Mock<TokenService>(authJSON["phrase"].ToString(), _mockLoggerService.Object);
        }

        [TestMethod]
        public void TestApplicationName()
        {
            string applicationName = MockTokenService.Object.ApplicationName();

            Assert.IsNotNull(applicationName);
            Assert.IsTrue(applicationName == "API Admin");
        }

        [TestMethod]
        public void TestGetUsers()
        {
            (string[] usernames, string[] passwords) = MockTokenService.Object.GetUsers();

            Assert.IsTrue(usernames.Length != 0);
            Assert.IsTrue(passwords.Length != 0);
        }

        [TestMethod]
        public void TestGetAuthorisationPhrases()
        {
            string[] phrases = MockTokenService.Object.GetAuthorisationPhrases();

            Assert.IsTrue(phrases.Length != 0);
        }
    }
}

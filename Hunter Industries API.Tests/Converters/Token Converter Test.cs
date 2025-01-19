using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Services;
using HunterIndustriesAPI.Tests.Functions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using System.Security.Claims;

namespace HunterIndustriesAPI.Tests.Converters
{
    [TestClass]
    public class TokenConverterTest
    {
        private Mock<TokenService> MockTokenService;

        [TestInitialize]
        public void Setup()
        {
            ConfigurationLoaderFunction.LoadConfig();

            Mock<LoggerService> _mockLoggerService = new Mock<LoggerService>(null);
            JObject authJSON = JSONLoaderFunction.LoadJSON("Token/PostOK.json");

            MockTokenService = new Mock<TokenService>(authJSON["phrase"].ToString(), _mockLoggerService.Object);
        }

        [TestMethod]
        public void TestGetClaims()
        {
            Mock<TokenConverter> _mockTokenConverter = new Mock<TokenConverter>(MockTokenService.Object);

            Claim[] claims = _mockTokenConverter.Object.GetClaims("UATTestUser");

            Assert.IsNotNull(claims);
            Assert.IsTrue(claims.Length == 4);
        }
    }
}

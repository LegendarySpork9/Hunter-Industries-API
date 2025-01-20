using HunterIndustriesAPI.Controllers;
using HunterIndustriesAPI.Models.Requests;
using HunterIndustriesAPI.Models.Responses;
using HunterIndustriesAPI.Tests.Functions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Results;

namespace HunterIndustriesAPI.Tests.Controllers
{
    [TestClass]
    public class TokenControllerTest
    {
        [TestInitialize]
        public void SetUp()
        {
            ConfigurationLoaderFunction.LoadConfig();
        }

        [TestMethod]
        public void PostOK()
        {
            JObject authJSON = JSONLoaderFunction.LoadJSON("Token/PostOK.json");

            AuthenticationModel authentication = new AuthenticationModel
            {
                Phrase = authJSON["phrase"].ToString()
            };

            TokenController controller = new TokenController();
            (HttpRequestMessage request, HttpControllerContext context) = MockAPICallFunction.SetUpCall("http://localhost/api/auth/token", HttpMethod.Post, "Basic", authJSON["basicCredentials"].ToString());

            controller.Request = request;
            controller.ControllerContext = context;

            IHttpActionResult result = controller.Post(authentication);
            var resultContent = result as NegotiatedContentResult<object>;

            Assert.IsTrue(resultContent.StatusCode == System.Net.HttpStatusCode.OK);
            Assert.IsNotNull(resultContent.Content);
            Assert.IsInstanceOfType(resultContent.Content, typeof(TokenResponseModel));
        }

        [TestMethod]
        public void PostBadRequestMissingAll()
        {
            TokenController controller = new TokenController();
            (HttpRequestMessage request, HttpControllerContext context) = MockAPICallFunction.SetUpCall("http://localhost/api/auth/token", HttpMethod.Post);

            controller.Request = request;
            controller.ControllerContext = context;

            IHttpActionResult result = controller.Post(new AuthenticationModel());
            var resultContent = result as NegotiatedContentResult<object>;
            object error = resultContent.Content;

            Assert.IsTrue(resultContent.StatusCode == System.Net.HttpStatusCode.BadRequest);
            Assert.IsNotNull(resultContent.Content);
            Assert.IsTrue(error.ToString() == "{ error = Invalid request, check the following. A body is provided with the 'Phrase' tag and the authorisation header is present. }");
        }

        [TestMethod]
        public void PostBadRequestMissingPhrase()
        {
            JObject authJSON = JSONLoaderFunction.LoadJSON("Token/PostBadRequestMissingPhrase.json");

            AuthenticationModel authentication = new AuthenticationModel();

            TokenController controller = new TokenController();
            (HttpRequestMessage request, HttpControllerContext context) = MockAPICallFunction.SetUpCall("http://localhost/api/auth/token", HttpMethod.Post, "Basic", authJSON["basicCredentials"].ToString());

            controller.Request = request;
            controller.ControllerContext = context;

            IHttpActionResult result = controller.Post(authentication);
            var resultContent = result as NegotiatedContentResult<object>;
            object error = resultContent.Content;

            Assert.IsTrue(resultContent.StatusCode == System.Net.HttpStatusCode.BadRequest);
            Assert.IsNotNull(resultContent.Content);
            Assert.IsTrue(error.ToString() == "{ error = Invalid request, check the following. A body is provided with the 'Phrase' tag and the authorisation header is present. }");
        }

        [TestMethod]
        public void PostBadRequestMissingAuth()
        {
            JObject authJSON = JSONLoaderFunction.LoadJSON("Token/PostBadRequestMissingAuth.json");

            AuthenticationModel authentication = new AuthenticationModel
            {
                Phrase = authJSON["phrase"].ToString()
            };

            TokenController controller = new TokenController();
            (HttpRequestMessage request, HttpControllerContext context) = MockAPICallFunction.SetUpCall("http://localhost/api/auth/token", HttpMethod.Post);

            controller.Request = request;
            controller.ControllerContext = context;

            IHttpActionResult result = controller.Post(authentication);
            var resultContent = result as NegotiatedContentResult<object>;
            object error = resultContent.Content;

            Assert.IsTrue(resultContent.StatusCode == System.Net.HttpStatusCode.BadRequest);
            Assert.IsNotNull(resultContent.Content);
            Assert.IsTrue(error.ToString() == "{ error = Invalid request, check the following. A body is provided with the 'Phrase' tag and the authorisation header is present. }");
        }

        [TestMethod]
        public void PostBadRequestMalformedHeader()
        {
            JObject authJSON = JSONLoaderFunction.LoadJSON("Token/PostBadRequestMissingAuth.json");

            AuthenticationModel authentication = new AuthenticationModel
            {
                Phrase = authJSON["phrase"].ToString()
            };

            TokenController controller = new TokenController();
            (HttpRequestMessage request, HttpControllerContext context) = MockAPICallFunction.SetUpCall("http://localhost/api/auth/token", HttpMethod.Post, "Basic");

            controller.Request = request;
            controller.ControllerContext = context;

            IHttpActionResult result = controller.Post(authentication);
            var resultContent = result as NegotiatedContentResult<object>;
            object error = resultContent.Content;

            Assert.IsTrue(resultContent.StatusCode == System.Net.HttpStatusCode.BadRequest);
            Assert.IsNotNull(resultContent.Content);
            Assert.IsTrue(error.ToString() == "{ error = Invalid or malformed basic authentication header. }");
        }

        [TestMethod]
        public void PostUnauthorisedCredentials()
        {
            JObject authJSON = JSONLoaderFunction.LoadJSON("Token/PostUnauthorisedCredentials.json");

            AuthenticationModel authentication = new AuthenticationModel
            {
                Phrase = authJSON["phrase"].ToString()
            };

            TokenController controller = new TokenController();
            (HttpRequestMessage request, HttpControllerContext context) = MockAPICallFunction.SetUpCall("http://localhost/api/auth/token", HttpMethod.Post, "Basic", authJSON["basicCredentials"].ToString());

            controller.Request = request;
            controller.ControllerContext = context;

            IHttpActionResult result = controller.Post(authentication);
            var resultContent = result as NegotiatedContentResult<object>;
            object error = resultContent.Content;

            Assert.IsTrue(resultContent.StatusCode == System.Net.HttpStatusCode.Unauthorized);
            Assert.IsNotNull(resultContent.Content);
            Assert.IsTrue(error.ToString() == "{ error = Invalid credentials or phrase provided. }");
        }

        [TestMethod]
        public void PostUnauthorisedPhrase()
        {
            JObject authJSON = JSONLoaderFunction.LoadJSON("Token/PostUnauthorisedPhrase.json");

            AuthenticationModel authentication = new AuthenticationModel
            {
                Phrase = authJSON["phrase"].ToString()
            };

            TokenController controller = new TokenController();
            (HttpRequestMessage request, HttpControllerContext context) = MockAPICallFunction.SetUpCall("http://localhost/api/auth/token", HttpMethod.Post, "Basic", authJSON["basicCredentials"].ToString());

            controller.Request = request;
            controller.ControllerContext = context;

            IHttpActionResult result = controller.Post(authentication);
            var resultContent = result as NegotiatedContentResult<object>;
            object error = resultContent.Content;

            Assert.IsTrue(resultContent.StatusCode == System.Net.HttpStatusCode.Unauthorized);
            Assert.IsNotNull(resultContent.Content);
            Assert.IsTrue(error.ToString() == "{ error = Invalid credentials or phrase provided. }");
        }
    }
}

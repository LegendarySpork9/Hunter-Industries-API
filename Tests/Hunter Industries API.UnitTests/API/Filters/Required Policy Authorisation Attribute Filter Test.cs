// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;

namespace HunterIndustriesAPI.UnitTests.API.Filters
{
    [TestClass]
    public class RequiredPolicyAuthorisationAttributeFilterTest
    {
        private HttpActionContext CreateActionContext(ClaimsPrincipal principal)
        {
            HttpConfiguration config = new HttpConfiguration();
            HttpRequestMessage request = new HttpRequestMessage();
            request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;

            HttpControllerDescriptor controllerDescriptor = new HttpControllerDescriptor(
                config,
                "Test",
                typeof(ApiController));

            HttpControllerContext controllerContext = new HttpControllerContext(
                config,
                new HttpRouteData(config.Routes.CreateRoute("test", null, null)),
                request)
            {
                ControllerDescriptor = controllerDescriptor
            };
            controllerContext.RequestContext.Principal = principal;

            Mock<HttpActionDescriptor> mockActionDescriptor = new Mock<HttpActionDescriptor>(controllerDescriptor);
            mockActionDescriptor.Setup(d => d.GetCustomAttributes<RequiredPolicyAuthorisationAttributeFilter>())
                .Returns(new System.Collections.ObjectModel.Collection<RequiredPolicyAuthorisationAttributeFilter>());

            return new HttpActionContext(controllerContext, mockActionDescriptor.Object);
        }
        /// <summary>
        /// Checks whether the OnAuthorization method returns 401 when the principal is null.
        /// </summary>
        [TestMethod]
        public void TestNullPrincipalReturnsUnauthorized()
        {
            RequiredPolicyAuthorisationAttributeFilter filter = new("User");
            HttpActionContext context = CreateActionContext(null);

            filter.OnAuthorization(context);

            Assert.IsNotNull(context.Response);
            Assert.AreEqual(
                HttpStatusCode.Unauthorized,
                context.Response.StatusCode);
        }

        /// <summary>
        /// Checks whether the OnAuthorization method returns 401 when the principal has no scope claims.
        /// </summary>
        [TestMethod]
        public void TestNoScopesReturnsUnauthorized()
        {
            ClaimsPrincipal principal = new ClaimsPrincipal(
                new ClaimsIdentity(new[]
                {
                    new Claim("username", "testuser")
                }));

            RequiredPolicyAuthorisationAttributeFilter filter = new("User");
            HttpActionContext context = CreateActionContext(principal);

            filter.OnAuthorization(context);

            Assert.IsNotNull(context.Response);
            Assert.AreEqual(
                HttpStatusCode.Unauthorized,
                context.Response.StatusCode);
        }

        /// <summary>
        /// Checks whether the OnAuthorization method allows access when the principal has a matching scope.
        /// </summary>
        [TestMethod]
        public void TestValidScopeGrantsAccess()
        {
            ClaimsPrincipal principal = new ClaimsPrincipal(
                new ClaimsIdentity(new[]
                {
                    new Claim("scope", "Control Panel API")
                }));

            RequiredPolicyAuthorisationAttributeFilter filter = new("User");
            HttpActionContext context = CreateActionContext(principal);

            filter.OnAuthorization(context);

            Assert.IsNull(context.Response);
        }

        /// <summary>
        /// Checks whether the OnAuthorization method returns 401 when the principal has a scope that does not match.
        /// </summary>
        [TestMethod]
        public void TestWrongScopeReturnsUnauthorized()
        {
            ClaimsPrincipal principal = new ClaimsPrincipal(
                new ClaimsIdentity(new[]
                {
                    new Claim("scope", "Assistant API")
                }));

            RequiredPolicyAuthorisationAttributeFilter filter = new("Media");
            HttpActionContext context = CreateActionContext(principal);

            filter.OnAuthorization(context);

            Assert.IsNotNull(context.Response);
            Assert.AreEqual(
                HttpStatusCode.Unauthorized,
                context.Response.StatusCode);
        }

        /// <summary>
        /// Checks whether the OnAuthorization method allows access with hierarchical permission matching.
        /// </summary>
        [TestMethod]
        public void TestHierarchicalPermissionGrantsAccess()
        {
            ClaimsPrincipal principal = new ClaimsPrincipal(
                new ClaimsIdentity(new[]
                {
                    new Claim("scope", "Server Status API")
                }));

            RequiredPolicyAuthorisationAttributeFilter filter = new("ServerStatus.Information.Read");
            HttpActionContext context = CreateActionContext(principal);

            filter.OnAuthorization(context);

            Assert.IsNull(context.Response);
        }

        /// <summary>
        /// Checks whether the OnAuthorization method allows access with multiple scopes where one matches.
        /// </summary>
        [TestMethod]
        public void TestMultipleScopesOneMatches()
        {
            ClaimsPrincipal principal = new ClaimsPrincipal(
                new ClaimsIdentity(new[]
                {
                    new Claim("scope", "Assistant API"),
                    new Claim("scope", "Media API")
                }));

            RequiredPolicyAuthorisationAttributeFilter filter = new("Media");
            HttpActionContext context = CreateActionContext(principal);

            filter.OnAuthorization(context);

            Assert.IsNull(context.Response);
        }

    }
}

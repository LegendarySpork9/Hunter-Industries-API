using System.IO;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;

namespace HunterIndustriesAPI.Tests.Functions
{
    internal static class MockAPICallFunction
    {
        // Sets up a mock HTTP request.
        public static (HttpRequestMessage, HttpControllerContext) SetUpCall(string url, HttpMethod method, string authSchema = null, string authParameter = null)
        {
            HttpRequest request = new HttpRequest("", url, "");
            HttpResponse response = new HttpResponse(new StringWriter());
            
            HttpContext.Current = new HttpContext(request, response);

            HttpConfiguration config = new HttpConfiguration();
            HttpRequestMessage requestMessage = new HttpRequestMessage(method, url);

            if (!string.IsNullOrWhiteSpace(authSchema))
            {
                requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(authSchema, authParameter);
            }

            requestMessage.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;

            HttpControllerContext controllerContext = new HttpControllerContext
            {
                Configuration = config,
                Request = requestMessage
            };

            return (requestMessage, controllerContext);
        }
    }
}

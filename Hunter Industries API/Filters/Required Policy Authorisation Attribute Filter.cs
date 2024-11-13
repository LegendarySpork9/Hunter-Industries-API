using System.Security.Claims;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace HunterIndustriesAPI.Filters
{
    /// <summary>
    /// </summary>
    public class RequiredPolicyAuthorisationAttributeFilter : AuthorizationFilterAttribute
    {
        private readonly string Policy;

        /// <summary>
        /// Sets the class's global variables.
        /// </summary>
        public RequiredPolicyAuthorisationAttributeFilter(string policy)
        {
            Policy = policy;
        }

        /// <summary>
        /// Checks if the token has the required claim to access the endpoint.
        /// </summary>
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            ClaimsPrincipal principal = actionContext.RequestContext.Principal as ClaimsPrincipal;

            if (principal == null || !ValidScope(Policy, principal))
            {
                actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            }
        }

        /// <summary>
        /// Checks if the token has a valid claim.
        /// </summary>
        private bool ValidScope(string policy, ClaimsPrincipal principal)
        {
            bool valid = false;

            switch (policy)
            {
                case "Assistant": valid = principal.HasClaim("scope", "Assistant API"); break;
                case "BookReader": valid = principal.HasClaim("scope", "Book Reader API"); break;
                case "APIControlPanel": valid = principal.HasClaim("scope", "Control Panel API"); break;
                case "AIAccess": valid = principal.HasClaim("scope", "Assistant API") || principal.HasClaim("scope", "Control Panel API"); break;
            }

            return valid;
        }
    }
}
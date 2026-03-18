// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Mappings;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace HunterIndustriesAPI.Filters
{
    /// <summary>
    /// Checks if the token has the required permission to access the endpoint.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Method, AllowMultiple = false)]
    public class RequiredPolicyAuthorisationAttributeFilter : AuthorizationFilterAttribute
    {
        private readonly string RequiredPermission;

        /// <summary>
        /// </summary>
        // Sets the class's global variables.
        public RequiredPolicyAuthorisationAttributeFilter(string requiredPermission)
        {
            RequiredPermission = requiredPermission;
        }

        /// <summary>
        /// Checks if the token has the required claim to access the endpoint.
        /// </summary>
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            ClaimsPrincipal principal = actionContext.RequestContext.Principal as ClaimsPrincipal;

            if (principal == null)
            {
                actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
                return;
            }

            string permissionToCheck = RequiredPermission;

            RequiredPolicyAuthorisationAttributeFilter actionAttribute = actionContext.ActionDescriptor
                .GetCustomAttributes<RequiredPolicyAuthorisationAttributeFilter>()
                .FirstOrDefault();

            RequiredPolicyAuthorisationAttributeFilter controllerAttribute = actionContext.ControllerContext.ControllerDescriptor
                .GetCustomAttributes<RequiredPolicyAuthorisationAttributeFilter>()
                .FirstOrDefault();

            if (actionAttribute != null && controllerAttribute != null && actionAttribute != controllerAttribute)
            {
                permissionToCheck = actionAttribute.RequiredPermission;
            }

            IEnumerable<string> scopes = principal.Claims
                .Where(c => c.Type == "scope")
                .Select(c => c.Value);

            List<string> grantedPermissions = ScopePermissionMapping.GetPermissions(scopes);

            if (!ScopePermissionMapping.HasPermission(grantedPermissions, permissionToCheck))
            {
                actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            }
        }
    }
}

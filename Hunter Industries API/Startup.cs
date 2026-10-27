// Copyright © - 11/06/2026 - Toby Hunter
using HunterIndustriesAPI.Filters;
using HunterIndustriesAPI.Models;
using HunterIndustriesAPICommon.Implementations;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;
using Owin;
using System;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

[assembly: OwinStartup(typeof(HunterIndustriesAPI.Startup))]
namespace HunterIndustriesAPI
{
    /// <summary>
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Sets up the app.
        /// </summary>
        public void Configuration(IAppBuilder app)
        {
            app.Use<CorsMiddleware>();

            HttpConfiguration config = new HttpConfiguration();
            config.MessageHandlers.Add(new RateLimitHandler(new SystemClockProvider()));

            app.UseWebApi(config);

            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = ValidationModel.Issuer,
                    ValidAudience = ValidationModel.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ValidationModel.SecretKey))
                }
            });
        }
    }

    /// <summary>
    /// Handles CORS headers for browser-based requests using allowed origins from configuration.
    /// </summary>
    public class CorsMiddleware : OwinMiddleware
    {
        private static readonly string[] _AllowedOrigins;

        static CorsMiddleware()
        {
            string originsSetting = ConfigurationManager.AppSettings["CorsAllowedOrigins"];
            _AllowedOrigins = string.IsNullOrWhiteSpace(originsSetting) ? Array.Empty<string>() : originsSetting.Split(',')
                .Select(o => o.Trim())
                .ToArray();
        }

        /// <summary>
        /// Sets the class's global variables.
        /// </summary>
        public CorsMiddleware(OwinMiddleware next) : base(next) { }

        /// <summary>
        /// Processes the request and adds CORS headers if the origin is allowed.
        /// </summary>
        public override async Task Invoke(IOwinContext context)
        {
            string origin = context.Request.Headers.Get("Origin");

            if (!string.IsNullOrWhiteSpace(origin) && _AllowedOrigins.Contains(
                    origin,
                    StringComparer.OrdinalIgnoreCase))
            {
                context.Response.Headers.Set(
                    "Access-Control-Allow-Origin",
                    origin);
                context.Response.Headers.Set(
                    "Access-Control-Allow-Methods",
                    "GET, POST, PUT, PATCH, DELETE, OPTIONS");
                context.Response.Headers.Set(
                    "Access-Control-Allow-Headers",
                    "Authorization, Content-Type, X-API-Version");
                context.Response.Headers.Set(
                    "Access-Control-Allow-Credentials",
                    "true");
            }

            if (context.Request.Method == "OPTIONS")
            {
                context.Response.StatusCode = 200;
                return;
            }

            await Next.Invoke(context);
        }
    }
}
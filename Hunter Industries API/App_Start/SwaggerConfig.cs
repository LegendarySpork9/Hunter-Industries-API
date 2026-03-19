// Copyright © - Unpublished - Toby Hunter
using System.Web.Http;
using WebActivatorEx;
using HunterIndustriesAPI;
using Swashbuckle.Application;
using System.IO;
using System;
using HunterIndustriesAPI.Filters;
using HunterIndustriesAPI.Filters.Document;
using HunterIndustriesAPI.Filters.Operation;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace HunterIndustriesAPI
{
    /// <summary>
    /// </summary>
    public class SwaggerConfig
    {
        /// <summary>
        /// Sets up the Swagger page.
        /// </summary>
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                    {
                        c.MultipleApiVersions(
                            (apiDesc, targetApiVersion) =>
                            {
                                var route = "/" + apiDesc.RelativePath.ToLower();
                                return route.StartsWith($"/api/{targetApiVersion}/");
                            },
                            vc =>
                            {
                                for (int i = VersionedRouteAttribute.ApiVersions.Length - 1; i >= 0; i--)
                                {
                                    string version = VersionedRouteAttribute.ApiVersions[i];
                                    vc.Version($"v{version}", $"Hunter Industries API V{version}").Description("This is the OpenAPI documentation for the Hunter Industries API.").Contact(contact =>
                                    {
                                        contact.Email("api@hunter-industries.co.uk");
                                    });
                                }
                            });

                        c.IncludeXmlComments(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{thisAssembly.GetName().Name}.xml"));

                        c.ApiKey("Bearer").Description("JWT authorization header using the Bearer scheme.").Name("Authorization").In("header");

                        c.OperationFilter<RequiredParameterOperationFilter>();
                        c.OperationFilter<RequiredHeaderFilter>();
                        c.OperationFilter<ParameterDetailOperationFilter>();
                        c.OperationFilter<ResponseExampleOperationFilter>();

                        c.DocumentFilter<BaseUrlDocumentFilter>();

                        c.PrettyPrint();
                    })
                .EnableSwaggerUi(c =>
                    {
                        c.DocumentTitle("Hunter Industries API");
                        c.InjectStylesheet(thisAssembly, "HunterIndustriesAPI.CSS.Swagger.css");
                        c.InjectJavaScript(thisAssembly, "HunterIndustriesAPI.JS.VersionSelector.js");
                        c.InjectJavaScript(thisAssembly, "HunterIndustriesAPI.JS.GroupHeaders.js");
                        c.EnableDiscoveryUrlSelector();
                    });
        }
    }
}

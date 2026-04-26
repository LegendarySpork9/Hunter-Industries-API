// Copyright © - Unpublished - Toby Hunter
using Swashbuckle.Swagger;
using System.Collections.Generic;
using System.Web.Http.Description;

namespace HunterIndustriesAPI.Filters.Document
{
    /// <summary>
    /// </summary>
    public class BaseUrlDocumentFilter : IDocumentFilter
    {
        /// <summary>
        /// Sets the base URL on the Swagger UI and strips the version prefix from endpoint paths.
        /// </summary>
        public void Apply(SwaggerDocument swaggerDoc,
            SchemaRegistry schemaRegistry,
            IApiExplorer apiExplorer)
        {
            string version = swaggerDoc.info.version;
            string versionPrefix = $"/{version}";

            swaggerDoc.basePath = "api.hunter-industries.co.uk";

            Dictionary<string, PathItem> updatedPaths = new Dictionary<string, PathItem>();

            foreach (KeyValuePair<string, PathItem> path in swaggerDoc.paths)
            {
                string newPath = path.Key.StartsWith(versionPrefix) ? path.Key.Substring(versionPrefix.Length) : path.Key;

                updatedPaths[newPath] = path.Value;
            }

            swaggerDoc.paths = updatedPaths;
        }
    }
}
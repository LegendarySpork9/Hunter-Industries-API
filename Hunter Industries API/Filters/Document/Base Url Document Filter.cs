using Swashbuckle.Swagger;
using System.Web.Http.Description;

namespace HunterIndustriesAPI.Filters.Document
{
    /// <summary>
    /// </summary>
    public class BaseUrlDocumentFilter : IDocumentFilter
    {
        /// <summary>
        /// Sets the base URL on the Swagger UI.
        /// </summary>
        public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
        {
            swaggerDoc.basePath = "hunter-industries.co.uk";
        }
    }
}
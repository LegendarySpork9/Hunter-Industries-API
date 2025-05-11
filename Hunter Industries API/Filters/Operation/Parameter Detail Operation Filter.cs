using HunterIndustriesAPI.Controllers;
using HunterIndustriesAPI.Controllers.Assistant;
using Swashbuckle.Swagger;
using System.Collections.Generic;
using System.Web.Http.Description;

namespace HunterIndustriesAPI.Filters.Operation
{
    /// <summary>
    /// </summary>
    public class ParameterDetailOperationFilter : IOperationFilter
    {
        /// <summary>
        /// Alters the details of the parameters on the Swagger UI depending on the controller.
        /// </summary>
        public void Apply(Swashbuckle.Swagger.Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (apiDescription.ActionDescriptor.ControllerDescriptor.ControllerType == typeof(TokenController))
            {
                foreach (Parameter param in operation.parameters)
                {
                    if (param.name == "request")
                    {
                        param.name = "phrase";
                        param.schema = new Schema
                        {
                            type = "object",
                            properties = new Dictionary<string, Schema>
                            {
                                {
                                    "phrase", new Schema
                                    {
                                        type = "string"
                                    }
                                }
                            }
                        };
                    }
                }
            }

            if (apiDescription.ActionDescriptor.ControllerDescriptor.ControllerType == typeof(AuditController))
            {
                foreach (Parameter param in operation.parameters)
                {
                    if (param.name == "filters.fromDate")
                    {
                        param.name = "fromDate";
                    }

                    if (param.name == "filters.ipAddress")
                    {
                        param.name = "ipAddress";
                    }

                    if (param.name == "filters.endpoint")
                    {
                        param.name = "endpoint";
                    }

                    if (param.name == "filters.pageSize")
                    {
                        param.name = "pageSize";
                    }

                    if (param.name == "filters.pageNumber")
                    {
                        param.name = "pageNumber";
                    }
                }
            }

            if (apiDescription.ActionDescriptor.ControllerDescriptor.ControllerType == typeof(ConfigController))
            {
                foreach (Parameter param in operation.parameters)
                {
                    if (param.name == "filters.assistantName")
                    {
                        param.name = "assistantName";
                    }

                    if (param.name == "filters.assistantId")
                    {
                        param.name = "assistantId";
                    }
                }
            }

            if (apiDescription.ActionDescriptor.ControllerDescriptor.ControllerType == typeof(ConfigController) && apiDescription.ActionDescriptor.ActionName == "Post")
            {
                foreach (Parameter param in operation.parameters)
                {
                    if (param.name == "request")
                    {
                        param.name = "assistant";
                    }
                }
            }

            if (apiDescription.ActionDescriptor.ControllerDescriptor.ControllerType == typeof(DeletionController))
            {
                foreach (Parameter param in operation.parameters)
                {
                    if (param.name == "request")
                    {
                        param.name = "deletion";
                    }

                    if (param.name == "filters.assistantName")
                    {
                        param.name = "assistantName";
                    }

                    if (param.name == "filters.assistantId")
                    {
                        param.name = "assistantId";
                    }
                }
            }

            if (apiDescription.ActionDescriptor.ControllerDescriptor.ControllerType == typeof(LocationController))
            {
                foreach (Parameter param in operation.parameters)
                {
                    if (param.name == "request")
                    {
                        param.name = "location";
                    }

                    if (param.name == "filters.assistantName")
                    {
                        param.name = "assistantName";
                    }

                    if (param.name == "filters.assistantId")
                    {
                        param.name = "assistantId";
                    }
                }
            }

            if (apiDescription.ActionDescriptor.ControllerDescriptor.ControllerType == typeof(VersionController))
            {
                foreach (Parameter param in operation.parameters)
                {
                    if (param.name == "request")
                    {
                        param.name = "version";
                    }

                    if (param.name == "filters.assistantName")
                    {
                        param.name = "assistantName";
                    }

                    if (param.name == "filters.assistantId")
                    {
                        param.name = "assistantId";
                    }
                }
            }

            if (apiDescription.ActionDescriptor.ControllerDescriptor.ControllerType == typeof(UserController))
            {
                foreach(Parameter param in operation.parameters)
                {
                    if (param.name == "filters.id")
                    {
                        param.name = "id";
                    }

                    if (param.name == "filters.username")
                    {
                        param.name = "username";
                    }
                }
            }

            if (apiDescription.ActionDescriptor.ControllerDescriptor.ControllerType == typeof(UserController) && (apiDescription.ActionDescriptor.ActionName == "Post" || apiDescription.ActionDescriptor.ActionName == "Patch"))
            {
                foreach (Parameter param in operation.parameters)
                {
                    if (param.name == "request")
                    {
                        param.name = "user";
                    }
                }
            }
        }
    }
}
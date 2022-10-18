using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS_BlogManagement.InfraStructure
{
    
    public class CustomHeaderSwaggerAttribute : IOperationFilter
    {

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "apiKey",
                In = ParameterLocation.Header,
                Required = true,
               // Description = "Type API Key",
                Schema = new OpenApiSchema
                {
                    Type = "string",
                    Default = new OpenApiString("/N6l7uJNaCy1LQuElcKtMQ==")

                }
            });
        }

    }
}

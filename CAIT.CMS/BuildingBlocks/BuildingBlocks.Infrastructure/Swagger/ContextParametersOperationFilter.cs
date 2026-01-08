using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BuildingBlocks.Infrastructure.Swagger
{
    public class ContextParametersOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();



            // =================================================================
            // 3. معالجة Resource ID (Header Only)
            // =================================================================
            // الهدف: إضافة X-ResourceId إذا لم يكن الرابط يحتوي على {resourceId}

            var hasRouteResourceId = operation.Parameters.Any(p =>
                p.In == ParameterLocation.Path &&
                (p.Name.Equals("resourceId", StringComparison.OrdinalIgnoreCase) ||
                 p.Name.Equals("id", StringComparison.OrdinalIgnoreCase)));

            if (!hasRouteResourceId)
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "X-ResourceId",
                    In = ParameterLocation.Header,
                    Description = "Target Resource ID (used if not provided in URL path).",
                    Required = false,
                    Schema = new OpenApiSchema { Type = "string", Format = "uuid" }
                });
            }

            // =================================================================
            // 1. معالجة Parent Resource ID (Query String)
            // =================================================================
            // الهدف: إضافة حقل ?parentResourceId=... في Swagger
            // الشرط: نضيفه فقط إذا لم يكن موجوداً مسبقاً (لتجنب التكرار)

            //var hasParentQuery = operation.Parameters.Any(p =>
            //    p.Name.Equals("parentResourceId", StringComparison.OrdinalIgnoreCase) &&
            //    p.In == ParameterLocation.Query);

            //if (!hasParentQuery)
            //{
            //    operation.Parameters.Add(new OpenApiParameter
            //    {
            //        Name = "parentResourceId",
            //        In = ParameterLocation.Query,
            //        Description = "Context Parent ID (e.g. CommitteeId) used by Middleware for permissions.",
            //        Required = false, // اختياري
            //        Schema = new OpenApiSchema { Type = "string", Format = "uuid" }
            //    });
            //}

            // =================================================================
            // 2. معالجة Parent Resource ID (Header) - اختياري ولكنه مفيد
            // =================================================================
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "X-ParentResourceId",
                In = ParameterLocation.Header,
                Description = "Alternative Header for Parent ID context.",
                Required = false,
                Schema = new OpenApiSchema { Type = "string", Format = "uuid" }
            });


        }
    }
}
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BuildingBlocks.Infrastructure.Swagger
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;
        private readonly SwaggerApiSettings _settings; // ✅ الإعدادات المحقونة

        // نقوم بحقن الإعدادات هنا
        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider, SwaggerApiSettings settings)
        {
            _provider = provider;
            _settings = settings;
        }

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
            }
        }

        private OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new OpenApiInfo
            {
                // ✅ نستخدم القيم القادمة من الإعدادات
                Title = _settings.Title,
                Version = description.ApiVersion.ToString(),
                Description = _settings.Description,
                Contact = new OpenApiContact
                {
                    Name = _settings.ContactName,
                    Email = _settings.ContactEmail
                }
            };

            if (description.IsDeprecated)
            {
                info.Description += " ⚠️ This API version has been deprecated.";
            }

            return info;
        }
    }
}
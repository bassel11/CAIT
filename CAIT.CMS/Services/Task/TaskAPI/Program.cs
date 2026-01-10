using Asp.Versioning.ApiExplorer;
using BuildingBlocks.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TaskAPI;
using TaskApplication;
using TaskInfrastructure;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCoreInfrastructure();
// Add services to the container.
builder.Services
    .AddApplicationServices(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration)
    .AddApiServices(builder.Configuration);

builder.Services.AddMemoryCache();

//  JWT Authentication

var jwtConfig = builder.Configuration.GetSection("Jwt");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtConfig["Issuer"],      // من Identity Service
        ValidAudience = jwtConfig["Audience"],  // من Identity Service
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtConfig["Key"]))
    };
});

//  Authorization

builder.Services.AddAuthorization();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddEndpointsApiExplorer();

builder.Services.AddEnterpriseVersioning(
    apiTitle: "Task API",
    apiDescription: "APIs for managing Tasks"
);

builder.Services.AddSwaggerGen(c =>
{
    //c.SwaggerDoc("v1", new OpenApiInfo { Title = "Task API", Version = "v1" });

    // تعريف الـ Bearer Token لـ Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your valid token in the text input below."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    c.OperationFilter<BuildingBlocks.Infrastructure.Swagger.ContextParametersOperationFilter>();
});

var app = builder.Build();

// تفعيل معالج الأخطاء الموحد (أول شيء ليمسك أي خطأ)
app.UseExceptionHandler();

app.UseApiServices();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    // ✅ 2. تحديث Swagger UI لدعم القائمة المنسدلة (v1, v2)
    app.UseSwaggerUI(options =>
    {
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

        // حلقة تكرارية لإنشاء Endpoint لكل إصدار
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant() // يظهر V1
            );
        }
    });

    // تهيئة قاعدة البيانات مع Seed إذا لزم
    //await app.Services.InitializeDatabaseAsync();
}

app.UseStaticFiles(); // Added for Locally Files on wwwroot

app.UseHttpsRedirection();

app.UseAuthentication();

app.UsePermissionMiddleware();

app.UseAuthorization();

app.MapControllers();

app.Run();

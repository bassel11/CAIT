using FluentValidation;
using FluentValidation.AspNetCore;
using Identity.API.Middlewares;
using Identity.API.Models;
using Identity.Application.Authorization;
using Identity.Application.Interfaces.Authorization;
using Identity.Application.Validators; // حيث يوجد PermissionQueryValidator
using Identity.Core.Entities;
using Identity.Infrastructure;
using Identity.Infrastructure.Authorization;
using Identity.Infrastructure.Configurations;
using Identity.Infrastructure.Data;
using Identity.Infrastructure.Grpc.Services;
using Identity.Infrastructure.Services.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;


var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.Configure(context.Configuration.GetSection("Kestrel"));
});


// Database 
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//{
//    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnectionString"));
//});

// Add services to the container.
builder.Services
    //.AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration);

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


// ---------------- JWT Config ----------------
var jwtConfig = builder.Configuration.GetSection("Jwt");
var azureConfig = builder.Configuration.GetSection("AzureAd");
var azureB2BConfig = builder.Configuration.GetSection("AzureB2B");


// ---------------- Authentication ----------------
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "BearerPolicy";
})
.AddJwtBearer("LocalJwt", options =>
{

    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtConfig["Issuer"],
        ValidAudience = jwtConfig["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["Key"])),

        RoleClaimType = ClaimTypes.Role, //  ضروري لقراءة الأدوار بشكل صحيح
        NameClaimType = ClaimTypes.Name //  لتحديد اسم المستخدم من التوكن

    };
    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            context.HandleResponse();

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var error = new ErrorResponse
            {
                StatusCode = 401,
                Error = "Unauthorized",
                Message = "Authentication Code is missing or invalid."
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(error));
        },

        OnForbidden = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";

            var error = new ErrorResponse
            {
                StatusCode = 403,
                Error = "Forbidden",
                Message = "You do not have access to this resource."
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(error));
        }
    };

})
// Azure AD internal
.AddJwtBearer("AzureAD", options =>
{
    var azureSection = builder.Configuration.GetSection("AzureAd");
    var authority = $"{azureSection["Instance"]}{azureSection["TenantId"]}/v2.0";

    options.Authority = authority;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidAudience = azureSection["Audience"],
        ValidIssuers = azureSection.GetSection("ValidIssuers").Get<string[]>(), // ← استخدم ValidIssuers
        NameClaimType = "preferred_username",
        RoleClaimType = "roles"
    };
    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            context.HandleResponse();

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var error = new ErrorResponse
            {
                StatusCode = 401,
                Error = "Unauthorized",
                Message = "Authentication code is missing or invalid."
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(error));
        },

        OnForbidden = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";

            var error = new ErrorResponse
            {
                StatusCode = 403,
                Error = "Forbidden",
                Message = "You do not have access to this resource."
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(error));
        }
    };
})
// Policy scheme to select the right bearer
.AddPolicyScheme("BearerPolicy", "BearerPolicy", options =>
{
    options.ForwardDefaultSelector = context =>
    {
        var authHeader = context.Request.Headers["Authorization"].ToString();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            return "LocalJwt";

        var token = authHeader.Substring("Bearer ".Length).Trim();
        try
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            if (jwt.Issuer?.Contains("login.microsoftonline.com") == true)
                return "AzureAD";
        }
        catch { }
        return "LocalJwt";
    };
});



//builder.Services.AddAuthorization();

// ---------------- FluentValidation Configuration ----------------
builder.Services
    .AddFluentValidationAutoValidation() // يفعّل التحقق التلقائي قبل Controller
    .AddFluentValidationClientsideAdapters()
    .AddValidatorsFromAssemblyContaining<PermissionFilterValidator>(); // يسجل كل الـ Validators


// Register permission checker & handler
builder.Services.AddScoped<IPermissionChecker, PermissionChecker>();

builder.Services.AddSingleton<IAuthorizationPolicyProvider, DynamicAuthorizationPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();

// Example policy registration
//using (var serviceProvider = builder.Services.BuildServiceProvider())
//{
//    using var scope = serviceProvider.CreateScope();

//    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

//    var permissions = dbContext.Permissions
//        .AsNoTracking()
//        .Select(p => p.Name) // <-- التعديل من Code إلى Name
//        .ToList();

//    builder.Services.AddAuthorization(options =>
//    {
//        foreach (var permission in permissions)
//        {
//            options.AddPolicy(permission, policy =>
//                policy.Requirements.Add(new PermissionRequirement(permission)));
//        }
//    });
//}



// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Identity.API", Version = "v1" });

    // إضافة دعم Authorization Bearer
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "أدخل 'Bearer {token}'"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


// ---------------- REGISTER gRPC SERVER (Important) ----------------
builder.Services.AddIdentityGrpc();

var app = builder.Build();


// Apply migrations and seed roles/admin
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // 1. جلب DbContext وتطبيق المايجريشن
    var dbContext = services.GetRequiredService<ApplicationDbContext>();

    try
    {
        Console.WriteLine("--> Updating Database...");
        dbContext.Database.Migrate();

        // 2. جلب الـ Interceptor لتعطيله
        var interceptor = services.GetRequiredService<Identity.Infrastructure.Interceptors.PermissionChangeInterceptor>();

        // 🔇 إيقاف النشر مؤقتاً
        interceptor.SuppressPublishing = true;

        Console.WriteLine("--> Seeding Data...");
        await PermissionSeeder.SeedPermissionsAsync(dbContext);
        await IdentitySeed.SeedRolesAndAdminAsync(services);

        // إعادة التفعيل (شكلياً)
        interceptor.SuppressPublishing = false;

        Console.WriteLine("--> Initialization Done.");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "❌ Error during database initialization.");
        // لا توقف التطبيق بالكامل إذا فشل الـ Seeding فقط، لكن إذا فشل المايجريشن سيتوقف لاحقاً
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity.API v1"));
}


app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseMiddleware<ResourceExtractionMiddleware>();
app.UseAuthorization();

app.MapControllers();

// ---------------- MAP gRPC SERVICE ----------------
app.MapGrpcService<UserGrpcService>();

app.Run();

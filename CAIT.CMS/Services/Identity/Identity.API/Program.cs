using Identity.Application.Interfaces;
using Identity.Application.Interfaces.Permissions;
using Identity.Application.Interfaces.RolePermissions;
using Identity.Application.Interfaces.Roles;
using Identity.Application.Interfaces.UserRoles;
using Identity.Application.Interfaces.Users;
using Identity.Core.Entities;
using Identity.Infrastructure.Data;
using Identity.Infrastructure.Services;
using Identity.Infrastructure.Services.Permissions;
using Identity.Infrastructure.Services.RolePermissions;
using Identity.Infrastructure.Services.Roles;
using Identity.Infrastructure.Services.UserRoles;
using Identity.Infrastructure.Services.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Database 
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnectionString"));
});

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

// Azure B2B
//.AddJwtBearer("AzureB2B", options =>
//{
//    var authority = $"{azureB2BConfig["Instance"]}{azureB2BConfig["TenantId"]}/v2.0";

//    options.Authority = authority;
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidAudience = azureB2BConfig["Audience"],
//        ValidIssuers = azureB2BConfig.GetSection("ValidIssuers").Get<string[]>(),
//        NameClaimType = "preferred_username",
//        RoleClaimType = "roles"
//    };
//})

// Policy scheme to select the right bearer
//.AddPolicyScheme("BearerPolicy", "BearerPolicy", options =>
//{
//    options.ForwardDefaultSelector = context =>
//    {
//        var authHeader = context.Request.Headers["Authorization"].ToString();
//        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
//            return "LocalJwt";

//        var token = authHeader.Substring("Bearer ".Length).Trim();
//        try
//        {
//            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

//            if (jwt.Issuer?.Contains("login.microsoftonline.com") == true)
//            {
//                var userType = jwt.Claims.FirstOrDefault(c => c.Type == "userType")?.Value;
//                if (userType == "Guest")
//                    return "AzureB2B"; // Guest external user
//                else
//                    return "AzureAD"; // Internal user
//            }
//        }
//        catch { }

//        return "LocalJwt";
//    };
//});

builder.Services.AddAuthorization();


// ---------------- Dependency Injection ----------------
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<ILdapAuthService, LdapAuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IMfaService, MfaService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAzureAuthService, AzureAuthService>();
builder.Services.AddScoped<IAzureB2BService, AzureB2BService>();
builder.Services.AddScoped<IGuestUserAsync, GuestUserAsync>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IRolePermissionService, RolePermissionService>();


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
var app = builder.Build();


// Apply migrations and seed roles/admin
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate(); // Apply migrations

    await IdentitySeed.SeedRolesAndAdminAsync(services); // Seed Basic roles & admin
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

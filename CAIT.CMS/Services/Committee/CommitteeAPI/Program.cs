using BuildingBlocks.Infrastructure;
using CommitteeAPI.Middlewares;
using CommitteeApplication;
using CommitteeInfrastructure;
using CommitteeInfrastructure.Configurations;
using CommitteeInfrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Database Context

//builder.Services.AddDbContext<CommitteeContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("CommitteeConnectionString"))
//);

// ==========================
// Register Services of All Layers
// ==========================
builder.Services.AddApplicationServices()
                .AddInfrastructureServices(builder.Configuration);

builder.Services.AddMemoryCache();
//builder.Services.AddHttpContextAccessor();

//builder.Services.AddTransient<JwtDelegatingHandler>();

//builder.Services.AddIdentityHttpClients(builder.Configuration);

//builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();
//builder.Services.AddSingleton<IAuthorizationPolicyProvider, DynamicAuthorizationPolicyProvider>();



// Identity gRPC Client Registration
// ==========================
var identityGrpcUrl = builder.Configuration.GetValue<string>("Services:GrpcUrl") ?? "http://localhost:9001";
builder.Services.AddIdentityGrpcClient(identityGrpcUrl);



// JWT Authentication
// ==========================
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
// ==========================
builder.Services.AddAuthorization();


//  Controllers & Swagger
// ==========================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "CommitteeAPI", Version = "v1" });

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



//  Localization Services
// ==========================
builder.Services.AddLocalization();

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en"),
        new CultureInfo("ar")
    };

    options.DefaultRequestCulture = new RequestCulture("en");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    options.ApplyCurrentCultureToResponseHeaders = true;
});


var app = builder.Build();


// Apply Migrations & Seed
// ==========================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<CommitteeContext>();
        var logger = services.GetRequiredService<ILogger<CommitteeContextSeed>>();

        // تطبيق أي Migrations أولاً
        await context.Database.MigrateAsync();

        // Seed البيانات (100 لجنة عشوائية)
        await CommitteeContextSeed.SeedAsync(context, logger);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
        throw; // يمكن إعادة رفع الاستثناء
    }
}


// ==========================
// HTTP Pipeline
// ==========================
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Localization Middleware (يجب أن يكون فوق UseHttpsRedirection)
var locOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
app.UseRequestLocalization(locOptions);


app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Authentication must come before Authorization
app.UseAuthentication();
//app.UseMiddleware<ResourceExtractionMiddleware>();
app.UsePermissionMiddleware();
app.UseAuthorization();

app.MapControllers();

app.Run();

using MeetingAPI.Middlewares;
using MeetingApplication;
using MeetingApplication.Authorization;
using MeetingInfrastructure;
using MeetingInfrastructure.Authorization;
using MeetingInfrastructure.Data;
using MeetingInfrastructure.Integrations.Committee;
using MeetingInfrastructure.Integrations.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ==========================
// Database MeetingDbContext
// ==========================
builder.Services.AddDbContext<MeetingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MeetingConnectionString"))
);

builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<JwtDelegatingHandler>();

builder.Services.AddIdentityServiceClient(builder.Configuration);
builder.Services.AddCommitteeServiceClient(builder.Configuration);

builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, DynamicAuthorizationPolicyProvider>();

// Register Services
builder.Services.AddApplicationServices()
                .AddInfrastructureServices();


// Add services to the container.


// ==========================
// 7️⃣ JWT Authentication
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

// ==========================
// 8️⃣ Authorization
// ==========================
builder.Services.AddAuthorization();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "MeetingAPI", Version = "v1" });

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


// ==========================
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


// ==========================
// Apply Migrations & Seed
// ==========================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<MeetingDbContext>();
        //var logger = services.GetRequiredService<ILogger<MeetingContextSeed>>();

        // تطبيق أي Migrations أولاً
        await context.Database.MigrateAsync();

        // Seed البيانات (100 لجنة عشوائية)
        //await MeetingContextSeed.SeedAsync(context, logger);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
        throw; // يمكن إعادة رفع الاستثناء
    }
}


// Configure the HTTP request pipeline.
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

app.UseAuthentication();
app.UseMiddleware<ResourceExtractionMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();

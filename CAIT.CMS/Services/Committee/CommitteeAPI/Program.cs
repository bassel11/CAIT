using CommitteeAPI.Extensions;
using CommitteeAPI.Middleware;
using CommitteeApplication.Authorization;
using CommitteeApplication.Behaviour;
using CommitteeApplication.Features.Committees.Commands.Handlers;
using CommitteeApplication.Mappers.CommitteeMembers;
using CommitteeApplication.Mappers.Committees;
using CommitteeCore.Repositories;
using CommitteeInfrastructure.Authorization;
using CommitteeInfrastructure.Data;
using CommitteeInfrastructure.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ==========================
// 1️⃣ Database Context
// ==========================
builder.Services.AddDbContext<CommitteeContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CommitteeConnectionString"))
);

builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();

//builder.Services.AddHttpClient<IPermissionService, PermissionServiceHttpClient>(client =>
//{
//    client.BaseAddress = new Uri(builder.Configuration["Services:IdentityBaseUrl"]);
//});
builder.Services.AddIdentityHttpClients(builder.Configuration);

builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, DynamicAuthorizationPolicyProvider>();

// ==========================
// 2️⃣ Repositories
// ==========================
builder.Services.AddScoped(typeof(IAsyncRepository<>), typeof(RepositoryBase<>));
builder.Services.AddScoped<ICommitteeRepository, CommitteeRepository>();
builder.Services.AddScoped<ICommitteeMemberRepository, CommitteeMemberRepository>();

// ==========================
// 3️⃣ AutoMapper
// ==========================
builder.Services.AddAutoMapper(typeof(CommitteeMappingProfile).Assembly);
builder.Services.AddAutoMapper(typeof(ComMemberMappingProfile).Assembly);

// ==========================
// 4️⃣ MediatR
// ==========================
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AddCommitteeCommandHandler).Assembly));

// ==========================
// 5️⃣ FluentValidation
// ==========================
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// ==========================
// 6️⃣ Pipeline Behaviours
// ==========================
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));

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

// ==========================
// 9️⃣ Controllers & Swagger
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
app.MigrateDatabase<CommitteeContext>((context, services) =>
{
    var logger = services.GetService<ILogger<CommitteeContextSeed>>();
    CommitteeContextSeed.SeedAsync(context, logger).Wait();
});

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


//  ضع Middleware الاستثناءات هنا
app.UseMiddleware<ExceptionHandlingMiddleware>();


// Authentication must come before Authorization
app.UseAuthentication();
app.UseMiddleware<ResourceExtractionMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();

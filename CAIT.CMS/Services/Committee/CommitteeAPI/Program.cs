using CommitteeAPI.Extensions;
using CommitteeApplication.Authorization;
using CommitteeApplication.Behaviour;
using CommitteeApplication.Handlers;
using CommitteeApplication.Interfaces.Authorization;
using CommitteeApplication.Mappers;
using CommitteeCore.Repositories;
using CommitteeInfrastructure.Authorization;
using CommitteeInfrastructure.Data;
using CommitteeInfrastructure.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

builder.Services.AddHttpClient<IPermissionService, PermissionServiceHttpClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:IdentityBaseUrl"]);
});

builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, DynamicAuthorizationPolicyProvider>();

// ==========================
// 2️⃣ Repositories
// ==========================
builder.Services.AddScoped(typeof(IAsyncRepository<>), typeof(RepositoryBase<>));
builder.Services.AddScoped<ICommitteeRepository, CommitteeRepository>();

// ==========================
// 3️⃣ AutoMapper
// ==========================
builder.Services.AddAutoMapper(typeof(CommitteeMappingProfile).Assembly);

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
builder.Services.AddSwaggerGen();

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

app.UseHttpsRedirection();

// ✅ Authentication must come before Authorization
app.UseAuthentication();
app.UseMiddleware<ResourceExtractionMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();

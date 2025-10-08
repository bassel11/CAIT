using CommitteeAPI.Extensions;
using CommitteeApplication.Behaviour;
using CommitteeApplication.Handlers;
using CommitteeApplication.Mappers;
using CommitteeCore.Repositories;
using CommitteeInfrastructure.Data;
using CommitteeInfrastructure.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
// 10️⃣ Apply Migrations & Seed
// ==========================
app.MigrateDatabase<CommitteeContext>((context, services) =>
{
    var logger = services.GetService<ILogger<CommitteeContextSeed>>();
    CommitteeContextSeed.SeedAsync(context, logger).Wait();
});

// ==========================
// 11️⃣ HTTP Pipeline
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
app.UseAuthorization();

app.MapControllers();

app.Run();

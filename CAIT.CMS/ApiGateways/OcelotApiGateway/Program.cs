using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// -------------------------------
// Ocelot Configuration
// -------------------------------
builder.Configuration.AddJsonFile(
    $"ocelot.{builder.Environment.EnvironmentName}.json",
    optional: true,
    reloadOnChange: true
);

//builder.Host.ConfigureAppConfiguration((env, config) =>
//{
//    config.AddJsonFile($"ocelot.{env.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
//});

// -------------------------------
// JWT Authentication Configuration
// -------------------------------
var jwtSettings = builder.Configuration.GetSection("Jwt");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer("Bearer", options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],      // "identity.api"
        ValidAudience = jwtSettings["Audience"],  // "committee.clients"
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["Key"])) // نفس Key في IdentityAPI
    };
});

// -------------------------------
// Services
// -------------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOcelot();

// -------------------------------
// Build App
// -------------------------------
var app = builder.Build();

// -------------------------------
// Middleware Pipeline
// -------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseHttpsRedirection();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapGet("/", async context =>
//    {
//        await context.Response.WriteAsync("Hello Ocelot Gateway");
//    });
//});

// لا تحتاج UseRouting أو UseEndpoints
app.MapGet("/", () => "Hello Ocelot Gateway");

// Ocelot Middleware
await app.UseOcelot();

await app.RunAsync();

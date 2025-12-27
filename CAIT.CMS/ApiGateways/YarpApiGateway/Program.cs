using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Add Health Checks
builder.Services.AddHealthChecks();


// Add CORS (Allow All for Development / Microservices interaction)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add Rate Limiting (Global & Policy-based)
//builder.Services.AddRateLimiter(options =>
//{
//    // A simple fixed window limiter for general protection
//    options.AddFixedWindowLimiter("fixed", limiterOptions =>
//    {
//        limiterOptions.PermitLimit = 100;
//        limiterOptions.Window = TimeSpan.FromSeconds(10);
//        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
//        limiterOptions.QueueLimit = 5;
//    });
//});

// Add Authentication (JWT)
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Set to true in Production
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
        //ClockSkew = TimeSpan.Zero
    };
});

// Authorization
builder.Services.AddAuthorization(options =>
{
    // Policy ????? Routes ???????
    options.AddPolicy("allowed", policy =>
        policy.RequireAuthenticatedUser());

    // LoginPolicy Route ?????
    options.AddPolicy("LoginPolicy", policy =>
        policy.RequireAssertion(_ => true)); // ???? ??? ??? ???? JWT

    // FallbackPolicy ???? ?? Route ???? AuthorizationPolicy
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});



//builder.Services.AddAuthorization(options =>
//{
//    // This policy can be applied to specific routes in appsettings.json via "AuthorizationPolicy": "Anonymous"
//    options.AddPolicy("allowed", policy => policy.RequireAssertion(c => true));

//    // Default fallback policy (optional, but YARP routes specify policies explicitly)
//    // options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
//});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// CORS
app.UseCors("AllowAll");

// Auth
app.UseAuthentication();
app.UseAuthorization();

// Rate Limiting
//app.UseRateLimiter();

// Health Check Endpoint
app.MapHealthChecks("/health");

app.MapControllers();

// YARP
app.MapReverseProxy();

app.Run();

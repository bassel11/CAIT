using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add YARP
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Add Health Checks
builder.Services.AddHealthChecks();

// Add CORS (Restricted in Production, Open in Dev)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add Rate Limiting (Critical Security Feature)
// هذا الجزء ضروري جداً لحماية بوابة الدخول من الهجمات
builder.Services.AddRateLimiter(options =>
{
    // سياسة صارمة لعمليات المصادقة (Login, Register)
    // تسمح بـ 10 محاولات فقط في الدقيقة لكل IP
    options.AddFixedWindowLimiter("AuthLimiter", limiterOptions =>
    {
        limiterOptions.PermitLimit = 10;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 2;
    });

    // سياسة عامة لباقي الخدمات (أكثر مرونة)
    options.AddFixedWindowLimiter("GeneralLimiter", limiterOptions =>
    {
        limiterOptions.PermitLimit = 100;
        limiterOptions.Window = TimeSpan.FromSeconds(10);
        limiterOptions.QueueLimit = 10;
    });
});

// 5. Add Authentication (JWT)
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
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero // لإلغاء فترة السماح الافتراضية (5 دقائق) عند انتهاء التوكن
    };
});

// 6. Authorization Policies (The Core Logic)
builder.Services.AddAuthorization(options =>
{
    // A. سياسة للمستخدمين المسجلين (تتطلب توكن)
    options.AddPolicy("AuthenticatedPolicy", policy =>
        policy.RequireAuthenticatedUser());

    // B. سياسة عامة (للدخول، التسجيل، تحديث التوكن)
    // هذه السياسة تسمح بالمرور دون فحص التوكن
    options.AddPolicy("PublicPolicy", policy =>
        policy.RequireAssertion(_ => true));

    // C. Zero Trust Fallback: أي راوت لم يحدد سياسة، سيتم رفضه تلقائياً
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddControllers();
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

// 1. CORS
app.UseCors("AllowAll");

// 2. Authentication & Authorization Middlewares
app.UseAuthentication();
app.UseAuthorization();

// 3. Rate Limiter Middleware (Must be after Auth)
app.UseRateLimiter();

app.MapHealthChecks("/health");
app.MapControllers();

// 4. Map YARP
app.MapReverseProxy();

app.Run();
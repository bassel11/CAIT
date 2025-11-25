using MeetingApplication;
using MeetingInfrastructure;
using MeetingInfrastructure.Data;
using MeetingInfrastructure.Integrations.Committee;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// ==========================
// Database MeetingDbContext
// ==========================
builder.Services.AddDbContext<MeetingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MeetingConnectionString"))
);

builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();


builder.Services.AddCommitteeServiceClient(builder.Configuration);



// Register Services
builder.Services.AddApplicationServices()
                .AddInfrastructureServices();


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// Localization Middleware (يجب أن يكون فوق UseHttpsRedirection)
var locOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
app.UseRequestLocalization(locOptions);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

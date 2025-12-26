using NotificationService;
using NotificationService.Hubs;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddNotificationInfrastructure(builder.Configuration);
// Add services to the container.
// Add configuration


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
app.UseStaticFiles(); // Added for Locally Files on wwwroot

app.UseHttpsRedirection();

// ? 2. ????? CORS (??? ?? ???? ??? MapHub ? UseAuthorization)
app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

// 3. ????? ???? ??? Hub
app.MapHub<NotificationHub>("/hubs/notifications");

app.Run();

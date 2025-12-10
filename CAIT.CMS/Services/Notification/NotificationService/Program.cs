using MassTransit;
using NotificationService.Consumers.SendEmail;
using NotificationService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Add configuration
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));

// Add Email service
builder.Services.AddScoped<IEmailService, EmailService>();

// Add MassTransit
builder.Services.AddMassTransit(x =>
{
    x.AddConsumers(typeof(MoMApprovedNotificationConsumer).Assembly);

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"] ?? "localhost", "/", h =>
        {
            h.Username(builder.Configuration["RabbitMQ:User"] ?? "guest");
            h.Password(builder.Configuration["RabbitMQ:Pass"] ?? "guest");
        });

        cfg.ConfigureEndpoints(context);
    });
});



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

app.UseAuthorization();

app.MapControllers();

app.Run();

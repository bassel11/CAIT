using CommitteeAPI.Extensions;
using CommitteeApplication.Behaviour;
using CommitteeApplication.Handlers;
using CommitteeCore.Repositories;
using CommitteeInfrastructure.Data;
using CommitteeInfrastructure.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// تسجيل DbContext مع Connection String من appsettings.json
builder.Services.AddDbContext<CommitteeContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CommitteeConnectionString"))
);

// 2️⃣ تسجيل Repositories للـ Dependency Injection
builder.Services.AddScoped(typeof(IAsyncRepository<>), typeof(RepositoryBase<>));
builder.Services.AddScoped<ICommitteeRepository, CommitteeRepository>();

// 3️⃣ تسجيل AutoMapper
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

// 4️⃣ تسجيل MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AddCommitteeCommandHandler).Assembly));

// 5️⃣ تسجيل FluentValidation
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// 6️⃣ تسجيل Pipeline Behaviours
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));


// 2️⃣ تسجيل الـ Repositories للـ Dependency Injection
builder.Services.AddScoped(typeof(IAsyncRepository<>), typeof(RepositoryBase<>));
builder.Services.AddScoped<ICommitteeRepository, CommitteeRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


//Apply db migration
app.MigrateDatabase<CommitteeContext>((context, services) =>
{
    var logger = services.GetService<ILogger<CommitteeContextSeed>>();
    CommitteeContextSeed.SeedAsync(context, logger).Wait();
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

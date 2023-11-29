using advert.api.DbContext;
using advert.api.Models;
using advert.api.Services.abstractt;
using advert.api.Services.Concrete;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using static Dapper.SqlMapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var settings = builder.Configuration.GetSection("ConnectionStrings").Get<ConnectionSetingsModel>();
//builder.Services.AddSingleton(new DbConnectionFactory(settings.DefaultConnection));
//builder.Services.AddScoped<IRabbitMqService, RabbitMqService>();

builder.Services.AddSingleton<IRabbitMqService>(provider =>
{
    var hostName = "localhost"; // RabbitMQ sunucu adresi
    var queueName = "myQueue";  // RabbitMQ kuyruk adý
    var userName = "userName";  // RabbitMQ kuyruk adý
    var password = "password";  // RabbitMQ kuyruk adý
    return new RabbitMqService(hostName, queueName, userName, password);
});

// IDbConnectionFactory servisini ekleyin
//builder.Services.AddSingleton<IDbConnectionFactory>(new DbConnectionFactory(settings.DefaultConnection));

builder.Services.AddTransient<IDbConnectionFactory>(_ =>
{
    var settings = builder.Configuration.GetSection("ConnectionStrings").Get<ConnectionSetingsModel>();
    return new DbConnectionFactory(settings.DefaultConnection);
});
builder.Services.AddSingleton<IAdvertRepository, AdvertRepository>();
builder.Services.AddSingleton<IRabbitMqConsumer, RabbitMqConsumer>();


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

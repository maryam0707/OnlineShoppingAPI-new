using OnlineShopping.Models;
using OnlineShopping.Services;
using NLog;
using NLog.Web;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using NLog.Extensions.Logging;


var builder = WebApplication.CreateBuilder(args);
var logger = NLog.LogManager.GetCurrentClassLogger();
//var logger  = LogManager.EnableLogging().
// Add services to the container.
//builder.Logging.ClearProviders();
//builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
//nlog logging
//builder.WebHost.UseNLog();
////AspNetExtensions.UseNLog(IWebHostBuilder);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDBSettings"));
builder.Services.AddSingleton<MongoDBService>();
builder.Logging.ClearProviders();
builder.WebHost.UseNLog();



var app = builder.Build();

//kafka
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//app.UseWelcomePage();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

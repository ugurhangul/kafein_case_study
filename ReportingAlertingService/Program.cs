using Microsoft.AspNetCore.Mvc;
using Nest;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddSingleton<IElasticClient>(new ElasticClient(new ConnectionSettings(new Uri("http://elasticsearch:9200"))));
builder.Services.AddHttpClient();
builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();
app.Run();
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using RuleConfigService.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddDbContext<RuleConfigDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));
builder.Services.AddControllers();
builder.WebHost.UseUrls("http://*:5001");

var app = builder.Build();
app.MapControllers();
app.Run();
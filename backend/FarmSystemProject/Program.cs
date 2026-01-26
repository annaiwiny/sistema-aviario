using Microsoft.EntityFrameworkCore;
using FarmSystemProject.Data;
using FarmSystemProject.Interfaces.IFarm;
using FarmSystemProject.Interfaces.IProductiveMonitoring;
using FarmSystemProject.Interfaces.IHealthMonitoring;
using FarmSystemProject.Services.HelthMonitoringService;
using FarmSystemProject.Services.ProductiveMonitoringService;
using FarmSystemProject.Services.FarmService;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IRaceService, RaceService>();
builder.Services.AddScoped<IEggService, EggService>();
builder.Services.AddScoped<IMortalityService, MortalityService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

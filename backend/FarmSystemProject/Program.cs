using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
using FarmSystemProject.Data;
using FarmSystemProject.Interfaces.IProductiveMonitoring;
using FarmSystemProject.Interfaces.IHealthMonitoring;
using FarmSystemProject.Services.HelthMonitoringService;
using FarmSystemProject.Services.ProductiveMonitoringService;
using FarmSystemProject.Interfaces;
using FarmSystemProject.Services.ReportService;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IEggService, EggService>();
builder.Services.AddScoped<IMortalityService, MortalityService>();
builder.Services.AddScoped<IVaccinationService, VaccinationService>();
builder.Services.AddScoped<IMortalityReportService, MortalityReportService>();

QuestPDF.Settings.License = LicenseType.Community;

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

using FarmSystemProject.Data;
using FarmSystemProject.Interfaces;
using FarmSystemProject.Interfaces.IHealthMonitoring;
using FarmSystemProject.Interfaces.ILots;
using FarmSystemProject.Interfaces.INutritionalControl;
using FarmSystemProject.Interfaces.IProductiveMonitoring;
using FarmSystemProject.Interfaces.IReportInterface;
using FarmSystemProject.Interfaces.IReportService;
using FarmSystemProject.Interfaces.ISales;
using FarmSystemProject.Interfaces.ISensors;
using FarmSystemProject.Middlewares;
using FarmSystemProject.Services;
using FarmSystemProject.Services.FarmService;
using FarmSystemProject.Services.HealthMonitoringService;
using FarmSystemProject.Services.HelthMonitoringService;
using FarmSystemProject.Services.Interfaces.IFarm;
using FarmSystemProject.Services.LotsService;
using FarmSystemProject.Services.NutritionalControl;
using FarmSystemProject.Services.ProductiveMonitoringService;
using FarmSystemProject.Services.ReportService;
using FarmSystemProject.Services.Sales;
using FarmSystemProject.Services.Sensors;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Infrastructure;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "FarmSystem API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Informe o token JWT no formato: Bearer {seu_token}"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddScoped<ISensorService, SensorService>();

builder.Services.AddScoped<IEggProductionService, EggProductionService>();
builder.Services.AddScoped<IMortalityService, MortalityService>();
builder.Services.AddScoped<IVaccinationService, VaccinationService>();
builder.Services.AddScoped<ILotService, LotService>();
builder.Services.AddScoped<IVaccinationReportService, VaccinationReportService>();
builder.Services.AddScoped<IMortalityReportService, MortalityReportService>();
builder.Services.AddScoped<IEggProductionReportService, EggProductionReportService>();
builder.Services.AddScoped<ISaleService, SaleService>();
builder.Services.AddScoped<ISaleReportService, SaleReportService>();
builder.Services.AddScoped<IFeedingService, FeedingService>();
builder.Services.AddScoped<IFeedingReportService, FeedingReportService>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IFarmService, FarmService>();

QuestPDF.Settings.License = LicenseType.Community;

var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]!)
            ),
            ClockSkew = TimeSpan.Zero
        };
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate(); 
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro ao migrar o banco de dados.");
    }
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseCors("AllowAll");

// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
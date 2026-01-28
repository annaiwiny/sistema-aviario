<<<<<<< Updated upstream
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
using FarmSystemProject.Data;
using FarmSystemProject.Interfaces.IProductiveMonitoring;
using FarmSystemProject.Interfaces.IHealthMonitoring;
using FarmSystemProject.Services.HelthMonitoringService;
using FarmSystemProject.Services.ProductiveMonitoringService;
using FarmSystemProject.Interfaces;
using FarmSystemProject.Services.ReportService;
using FarmSystemProject.Interfaces.IFarm;
using FarmSystemProject.Services.FarmService;
=======
using FarmSystemProject.Data;
using FarmSystemProject.Interfaces;
using FarmSystemProject.Interfaces.IFarm;
using FarmSystemProject.Interfaces.IHealthMonitoring;
using FarmSystemProject.Interfaces.IProductiveMonitoring;
using FarmSystemProject.Middlewares;
using FarmSystemProject.Services;
using FarmSystemProject.Services.FarmService;
using FarmSystemProject.Services.HelthMonitoringService;
using FarmSystemProject.Services.Interfaces.IFarm;
using FarmSystemProject.Services.ProductiveMonitoringService;
using FarmSystemProject.Services.ReportService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Infrastructure;
using System.Text;
>>>>>>> Stashed changes

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
<<<<<<< Updated upstream
builder.Services.AddSwaggerGen();
=======

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

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IFarmService, FarmService>();
builder.Services.AddScoped<ILotService, LotService>();
>>>>>>> Stashed changes
builder.Services.AddScoped<IEggService, EggService>();
builder.Services.AddScoped<IMortalityService, MortalityService>();
builder.Services.AddScoped<IVaccinationService, VaccinationService>();
builder.Services.AddScoped<IMortalityReportService, MortalityReportService>();
<<<<<<< Updated upstream
builder.Services.AddScoped<ILotService, LotService>();

QuestPDF.Settings.License = LicenseType.Community;
=======
builder.Services.AddScoped<IEggReportService, EggReportService>();
builder.Services.AddScoped<IVaccinationReportService, VaccinationReportService>();

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
>>>>>>> Stashed changes

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

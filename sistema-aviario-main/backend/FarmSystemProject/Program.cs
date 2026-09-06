using FarmSystemProject.Configuration;
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
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Infrastructure;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

const string CorsPolicyName = "FarmSystemCors";

var isDevelopment = builder.Environment.IsDevelopment();

// ---------------------------------------------------------------------------
// Segredos e configuracao
//
// Nada de credencial em appsettings.json. Tudo abaixo vem de variavel de
// ambiente (Docker/.env, App Service, etc.) ou de user-secrets no dev local.
// A validacao roda no startup: a API se recusa a subir mal configurada em vez
// de rodar com um valor default inseguro.
// ---------------------------------------------------------------------------

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "ConnectionStrings__DefaultConnection nao configurada. " +
        "Defina a variavel de ambiente antes de iniciar a API (veja .env.example).");
}

builder.Services.AddOptions<JwtOptions>()
    .Bind(builder.Configuration.GetSection(JwtOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<Esp32Options>()
    .Bind(builder.Configuration.GetSection(Esp32Options.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

var frontendOptions = builder.Services.AddOptions<FrontendOptions>()
    .Bind(builder.Configuration.GetSection(FrontendOptions.SectionName));

var emailOptions = builder.Services.AddOptions<EmailOptions>()
    .Bind(builder.Configuration.GetSection(EmailOptions.SectionName));

// Em producao esses dois sao obrigatorios (link de reset de senha + SMTP).
// Em desenvolvimento seguem opcionais para nao travar quem so quer rodar a API.
if (!isDevelopment)
{
    frontendOptions.ValidateDataAnnotations().ValidateOnStart();
    emailOptions.ValidateDataAnnotations().ValidateOnStart();
}

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddControllers();

// ---------------------------------------------------------------------------
// CORS restrito por lista de origens (Cors__AllowedOrigins__0, __1, ...).
// Lista vazia = nenhuma origem externa liberada. No deploy padrao isso e o
// esperado: o nginx do frontend faz proxy de /api, entao tudo e mesma origem.
// ---------------------------------------------------------------------------

var allowedOrigins = (builder.Configuration
        .GetSection(CorsOptions.SectionName)
        .Get<CorsOptions>() ?? new CorsOptions())
    .AllowedOrigins
    .Where(origin => !string.IsNullOrWhiteSpace(origin))
    .Select(origin => origin.Trim().TrimEnd('/'))
    .Distinct(StringComparer.OrdinalIgnoreCase)
    .ToArray();

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS")
              .WithHeaders(HeaderNames.Authorization, HeaderNames.ContentType, HeaderNames.Accept, "X-Secret-Key")
              .WithExposedHeaders(HeaderNames.ContentDisposition)
              .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
    });
});

// A API roda atras do nginx: sem isso o ASP.NET Core enxerga o IP do proxy
// e o esquema http, e nao o do cliente real.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;

    // O proxy fica na rede interna do Docker, cujo range varia a cada ambiente.
    // A API nao e publicada para fora, entao confiar no proxy da rede e aceitavel.
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
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
builder.Services.AddScoped<IFeedService, FeedService>();
builder.Services.AddScoped<IFeedReportService, FeedReportService>();
builder.Services.AddScoped<ISensorReportService, SensorReportService>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IFarmService, FarmService>();

QuestPDF.Settings.License = LicenseType.Community;

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Em producao o token trafega por TLS terminado no proxy da borda.
        options.RequireHttpsMetadata = !isDevelopment;
    });

// Liga a validacao do token as mesmas JwtOptions validadas no startup,
// em vez de reler a configuracao crua aqui.
builder.Services
    .AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<IOptions<JwtOptions>>((bearer, jwt) =>
    {
        var jwtOptions = jwt.Value;

        bearer.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtOptions.Key)
            ),
            ClockSkew = TimeSpan.Zero
        };
    });

var app = builder.Build();

var startupLogger = app.Services.GetRequiredService<ILogger<Program>>();

// Swagger: automatico em Development; em producao so com Swagger__Enabled=true.
var swaggerEnabled = app.Configuration
    .GetSection(SwaggerOptions.SectionName)
    .Get<SwaggerOptions>()?.Enabled ?? app.Environment.IsDevelopment();

if (swaggerEnabled)
{
    if (!app.Environment.IsDevelopment())
    {
        startupLogger.LogWarning(
            "Swagger habilitado fora de Development. Desligue (Swagger__Enabled=false) assim que terminar.");
    }

    app.UseSwagger();
    app.UseSwaggerUI();
}

if (allowedOrigins.Length == 0)
{
    startupLogger.LogInformation("CORS: nenhuma origem externa liberada (somente mesma origem).");
}
else
{
    startupLogger.LogInformation("CORS: origens liberadas -> {Origins}", string.Join(", ", allowedOrigins));
}

MigrateDatabase(app, startupLogger);

app.UseForwardedHeaders();

app.UseMiddleware<ExceptionMiddleware>();

app.UseCors(CorsPolicyName);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Aplica as migrations com retentativa: o SQL Server pode aceitar conexao e
// ainda estar concluindo o recovery quando o container do backend sobe.
static void MigrateDatabase(WebApplication app, ILogger logger)
{
    const int maxAttempts = 5;

    for (var attempt = 1; attempt < maxAttempts; attempt++)
    {
        try
        {
            using var scope = app.Services.CreateScope();
            scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
            return;
        }
        catch (Exception ex)
        {
            var delay = TimeSpan.FromSeconds(attempt * 3);
            logger.LogWarning(ex, "Falha ao migrar o banco (tentativa {Attempt}/{Max}). Nova tentativa em {Delay}s.",
                attempt, maxAttempts, delay.TotalSeconds);
            Thread.Sleep(delay);
        }
    }

    // Ultima tentativa sem catch: se falhar aqui, o processo aborta.
    // Melhor nao subir do que atender requisicoes contra um schema desatualizado.
    using var finalScope = app.Services.CreateScope();
    finalScope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
}
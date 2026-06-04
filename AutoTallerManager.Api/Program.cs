using System.Text;
using AutoTallerManager.Application.Interfaces;
using AutoTallerManager.Application.Services;
using AutoTallerManager.Application.Configuration;
using AutoTallerManager.Domain.Entities;
using AutoTallerManager.Infrastructure.Persistence;
using AutoTallerManager.Infrastructure.Repositories;
using AutoTallerManager.Infrastructure.Configuration;
using AutoTallerManager.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using AspNetCoreRateLimit;

// Cargar variables de entorno del archivo .env antes de inicializar la configuración de la app
EnvironmentConstants.LoadEnvFile();

// Desactivar el mapeo de claims heredado por compatibilidad estandarizada de nombres de claims JWT
System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<AutoTallerManager.Api.Filters.ValidationFilter>();
});
builder.Services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
builder.Services.AddEndpointsApiExplorer();

// CONFIGURACIÓN DE SWAGGER CON SEGURIDAD JWT BEARER
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "AutoTallerManager API", Version = "v1" });

    // Agregar el candado de seguridad Bearer JWT
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese 'Bearer' seguido de un espacio y su token JWT.\n\nEjemplo: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...'"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// CONFIGURACIÓN DEL CONTENEDOR DE DEPENDENCIAS PARA POSTGRESQL (EF CORE)
var connectionString = EnvironmentConstants.ConnectionString;
builder.Services.AddDbContext<AutoTallerDbContext>(options =>
    options.UseNpgsql(connectionString, b => 
        b.MigrationsAssembly("AutoTallerManager.Infrastructure"))); // Define dónde se guardarán físicamente las migraciones

// REGISTRO DE PATRONES DE PERSISTENCIA (CONSOLIDACIÓN Y UNIFICACIÓN DE CICLO DE VIDA SCOPED - PUNTO 5 AUDITORÍA)
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// REGISTRO DE SERVICIOS DE APLICACIÓN Y MAPSTER
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IOrdenServicioService, OrdenServicioService>();
builder.Services.AddScoped<IServicioTallerService, ServicioTallerService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<ICitaService, CitaService>();
builder.Services.AddScoped<IFacturaService, FacturaService>();
builder.Services.AddScoped<IGarantiaService, GarantiaService>();
builder.Services.AddSingleton<IPasswordHasher, BcIdentityPasswordHasher>();
builder.Services.AddSingleton<ITokenBlocklistService, TokenBlocklistService>(); // Registro del servicio de bloqueo de tokens
builder.Services.AddApplicationServices(); // Registrar validadores de FluentValidation
MapsterConfig.RegisterMappings();

// CONFIGURACIÓN DE HSTS (HTTP Strict Transport Security) PARA PRODUCCIÓN
builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365); // 1 año de transporte estrictamente seguro
});

// CONFIGURACIÓN TIPADA DE JWT SETTINGS PARA LA CAPA DE APLICACIÓN
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

// Post-configuración de seguridad: inyectar el secreto simétrico desde variables de entorno
builder.Services.PostConfigure<JwtSettings>(options =>
{
    options.Secret = EnvironmentConstants.JwtSecretKey;
});

// CONFIGURACIÓN DE AUTENTICACIÓN JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = EnvironmentConstants.JwtSecretKey;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Requerir HTTPS metadata estrictamente si el entorno actual es de producción
    options.RequireHttpsMetadata = builder.Environment.IsProduction();
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"] ?? "AutoTallerManagerApi",
        ValidAudience = jwtSettings["Audience"] ?? "AutoTallerManagerClients",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero,
        RoleClaimType = System.Security.Claims.ClaimTypes.Role,
        NameClaimType = System.Security.Claims.ClaimTypes.Name
    };
    options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            if (!context.HttpContext.Request.Headers.TryGetValue("X-Correlation-ID", out var correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
            }
            logger.LogWarning(context.Exception, "Fallo en la autenticación JWT. [CorrelationId: {CorrelationId}] [Path: {Path}] [Method: {Method}]", 
                correlationId.ToString(), context.HttpContext.Request.Path, context.HttpContext.Request.Method);
            return Task.CompletedTask;
        }
    };
});

// CONFIGURACIÓN DE AUTORIZACIÓN POR ROLES/POLÍTICAS
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireMecanicoRole", policy => policy.RequireRole("Mecanico"));
    options.AddPolicy("RequireRecepcionistaRole", policy => policy.RequireRole("Recepcionista"));
    options.AddPolicy("RequireClienteRole", policy => policy.RequireRole("Cliente"));
});

// CONFIGURACIÓN DE RATE LIMITING (LIMITACIÓN DE TASA DE PETICIONES)
builder.Services.AddOptions();
builder.Services.AddMemoryCache();
builder.Services.AddDistributedMemoryCache();

builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.EnableEndpointRateLimiting = true;
    options.StackBlockedRequests = false;
    options.HttpStatusCode = 429;
    options.RealIpHeader = "X-Real-IP";
    options.ClientIdHeader = "X-ClientId";
    options.GeneralRules = new List<RateLimitRule>
    {
        new RateLimitRule
        {
            Endpoint = "post:/api/usuarios/login",
            Period = "1m",
            Limit = 5
        },
        new RateLimitRule
        {
            Endpoint = "post:/api/usuarios/registrar",
            Period = "1m",
            Limit = 5
        },
        new RateLimitRule
        {
            Endpoint = "*:/api/ordenes/*",
            Period = "1m",
            Limit = 60
        }
    };
    options.QuotaExceededResponse = new QuotaExceededResponse
    {
        Content = "{\"status\": 429, \"mensaje\": \"Has superado el límite de peticiones permitido. Por favor, inténtelo de nuevo más tarde.\"}",
        ContentType = "application/json",
        StatusCode = 429
    };
});

builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

var app = builder.Build();

// Middleware Global de Excepciones para blindar el backend
app.UseMiddleware<AutoTallerManager.Api.Middleware.GlobalExceptionMiddleware>();

// Habilitar limitación de peticiones por IP (Rate Limiting)
app.UseIpRateLimiting();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts(); // Habilitar Strict Transport Security (HSTS) en entornos no locales
}

app.UseHttpsRedirection();

// EL ORDEN ES CRUCIAL: UseAuthentication() DEBE IR ANTES DE UseAuthorization()
app.UseAuthentication();
app.UseMiddleware<AutoTallerManager.Api.Middleware.JwtBlocklistMiddleware>(); // Validar revocación de tokens
app.UseAuthorization();
app.UseMiddleware<AutoTallerManager.Api.Middleware.AuditoriaMiddleware>();

app.MapControllers();

// INICIALIZACIÓN DE BASE DE DATOS (MIGRACIONES Y SEEDING SEGURO)
await DatabaseInitializer.InitializeDatabaseAsync(app.Services);

app.Run();


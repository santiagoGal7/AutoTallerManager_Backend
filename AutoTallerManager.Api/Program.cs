using System.Text;
using AutoTallerManager.Application.Interfaces;
using AutoTallerManager.Application.Services;
using AutoTallerManager.Application.Configuration;
using AutoTallerManager.Domain.Entities;
using AutoTallerManager.Infrastructure.Persistence;
using AutoTallerManager.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
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
var connectionString = builder.Configuration.GetConnectionString("PostgresConnection");
builder.Services.AddDbContext<AutoTallerDbContext>(options =>
    options.UseNpgsql(connectionString, b => 
        b.MigrationsAssembly("AutoTallerManager.Infrastructure"))); // Define dónde se guardarán físicamente las migraciones

// REGISTRO DE PATRONES DE PERSISTENCIA (REPOSITORIO GENÉRICO Y UNIT OF WORK)
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// REGISTRO DE SERVICIOS DE APLICACIÓN Y MAPSTER
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IOrdenServicioService, OrdenServicioService>();
builder.Services.AddScoped<IServicioTallerService, ServicioTallerService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
MapsterConfig.RegisterMappings();

// CONFIGURACIÓN DE AUTENTICACIÓN JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Secret Key is not configured.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Habilitar en producción
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
});

// CONFIGURACIÓN DE AUTORIZACIÓN POR ROLES/POLÍTICAS
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireMecanicoRole", policy => policy.RequireRole("Mecanico"));
    options.AddPolicy("RequireRecepcionistaRole", policy => policy.RequireRole("Recepcionista"));
    options.AddPolicy("RequireClienteRole", policy => policy.RequireRole("Cliente"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// EL ORDEN ES CRUCIAL: UseAuthentication() DEBE IR ANTES DE UseAuthorization()
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// SEMBRADO INICIAL DE USUARIO ADMINISTRADOR
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AutoTallerDbContext>();
        context.Database.EnsureCreated();
        
        if (!context.Usuarios.Any())
        {
            var adminUser = new Usuario
            {
                Nombre = "Administrador del Sistema",
                Correo = "admin@autotaller.com",
                Rol = "Admin",
                Activo = true
            };
            var passwordHasher = new PasswordHasher<Usuario>();
            adminUser.ContrasenaHash = passwordHasher.HashPassword(adminUser, "Admin123*");
            
            context.Usuarios.Add(adminUser);
            context.SaveChanges();
            
            Console.WriteLine("--> Usuario administrador por defecto sembrado exitosamente: admin@autotaller.com / Admin123*");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"--> Error durante la siembra de base de datos: {ex.Message}");
    }
}

app.Run();


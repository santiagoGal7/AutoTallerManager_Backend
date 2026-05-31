using AutoTallerManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CONFIGURACIÓN DEL CONTENEDOR DE DEPENDENCIAS PARA POSTGRESQL (EF CORE)
var connectionString = builder.Configuration.GetConnectionString("PostgresConnection");
builder.Services.AddDbContext<AutoTallerDbContext>(options =>
    options.UseNpgsql(connectionString, b => 
        b.MigrationsAssembly("AutoTallerManager.Infrastructure"))); // Define dónde se guardarán físicamente las migraciones

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();

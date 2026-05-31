using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AutoTallerManager.Infrastructure.Persistence;

public class AutoTallerDbContext : DbContext
{
    public AutoTallerDbContext(DbContextOptions<AutoTallerDbContext> options) : base(options)
    {
    }

    // Registro explícito de los principales DbSet operativos expuestos al backend
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Vehiculo> Vehiculos => Set<Vehiculo>();
    public DbSet<ServicioTaller> ServiciosTaller => Set<ServicioTaller>();
    public DbSet<OrdenServicio> OrdenesServicio => Set<OrdenServicio>();
    public DbSet<Repuesto> Repuestos => Set<Repuesto>();
    public DbSet<Factura> Facturas => Set<Factura>();
    public DbSet<Aseguradora> Aseguradoras => Set<Aseguradora>();
    public DbSet<BahiaServicio> BahiasServicio => Set<BahiaServicio>();
    public DbSet<Herramienta> Herramientas => Set<Herramienta>();
    public DbSet<AuditoriaTransaccion> AuditoriaTransacciones => Set<AuditoriaTransaccion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ESCANEO AUTOMÁTICO DE CONFIGURACIONES FLUENT API MEDIANTE REFLEXIÓN
        // Lee dinámicamente cada interfaz IEntityTypeConfiguration implementada en este proyecto.
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}

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
    public DbSet<OrdenServicio> OrdenesServicio { get; set; } = null!;
    public DbSet<Repuesto> Repuestos => Set<Repuesto>();
    public DbSet<Factura> Facturas => Set<Factura>();
    public DbSet<Aseguradora> Aseguradoras => Set<Aseguradora>();
    public DbSet<BahiaServicio> BahiasServicio => Set<BahiaServicio>();
    public DbSet<Herramienta> Herramientas => Set<Herramienta>();
    public DbSet<AuditoriaTransaccion> AuditoriaTransacciones => Set<AuditoriaTransaccion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // CONFIGURACIÓN FLUENT API PARA ORDEN DE SERVICIO
        modelBuilder.Entity<OrdenServicio>(entity =>
        {
            entity.ToTable("OrdenesServicio"); // Nombre exacto con mayúsculas de PostgreSQL

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("Id");
            entity.Property(e => e.VehiculoId).HasColumnName("VehiculoId"); // Crucial: Mapea la propiedad VehiculoId a la columna física 'VehiculoId'

            entity.Property(os => os.DescripcionProblema)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(os => os.DiagnosticoMecanico)
                .HasMaxLength(1000);

            entity.Property(os => os.Estado)
                .IsRequired()
                .HasMaxLength(30);

            entity.Property(os => os.FechaIngreso)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(os => os.FechaEntrega)
                .IsRequired(false);

            entity.Property(os => os.CostoEstimado)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            entity.Property(os => os.CostoTotal)
                .IsRequired(false)
                .HasColumnType("decimal(18,2)");

            // Configurar relación inversa: Un Vehículo tiene muchas Órdenes de forma unificada
            entity.HasOne(d => d.Vehiculo)
                  .WithMany(v => v.OrdenesServicio) 
                  .HasForeignKey(d => d.VehiculoId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // CONFIGURACIÓN DE RELACIÓN VEHICULO-CLIENTE (FUERZA COLUMNA IdCliente)
        modelBuilder.Entity<Vehiculo>()
            .HasOne(v => v.Cliente)
            .WithMany(c => c.Vehiculos)
            .HasForeignKey(v => v.IdCliente);

        // CONFIGURACIÓN DE TABLAS INTERMEDIAS DE DETALLES
        modelBuilder.Entity<DetalleOrdenServicio>(entity =>
        {
            entity.ToTable("DetallesOrdenServicio");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.IdOrdenServicio).HasColumnName("IdOrdenServicio");
            entity.Property(e => e.IdServicioTaller).HasColumnName("IdServicioTaller");
        });

        modelBuilder.Entity<DetalleOrdenRepuesto>(entity =>
        {
            entity.ToTable("DetallesOrdenRepuesto");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrdenServicioId).HasColumnName("OrdenServicioId");
            entity.Property(e => e.RepuestoId).HasColumnName("RepuestoId");
        });

        // CONFIGURACIÓN DE SERVICIO TALLER (PÓBLADO DE CATÁLOGO)
        modelBuilder.Entity<ServicioTaller>(entity =>
        {
            entity.ToTable("ServiciosTaller"); // Nombre exacto con mayúsculas en Supabase
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("Id");
            entity.Property(e => e.Nombre).HasColumnName("Nombre").IsRequired();
            entity.Property(e => e.Descripcion).HasColumnName("Descripcion");
            entity.Property(e => e.TarifaBaseManoObra).HasColumnName("TarifaBaseManoObra").HasColumnType("numeric");
            entity.Property(e => e.Activo).HasColumnName("Activo").HasDefaultValue(true);
        });

        // ESCANEO AUTOMÁTICO DE CONFIGURACIONES FLUENT API MEDIANTE REFLEXIÓN
        // Lee dinámicamente cada interfaz IEntityTypeConfiguration implementada en este proyecto.
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}

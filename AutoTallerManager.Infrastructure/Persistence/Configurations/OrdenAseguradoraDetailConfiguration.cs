using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Persistence.Configurations;

public class OrdenAseguradoraDetailConfiguration : IEntityTypeConfiguration<OrdenAseguradoraDetail>
{
    public void Configure(EntityTypeBuilder<OrdenAseguradoraDetail> builder)
    {
        builder.ToTable("OrdenAseguradoraDetails");

        builder.HasKey(oad => oad.Id);

        builder.Property(oad => oad.NumeroSiniestro)
            .IsRequired()
            .HasMaxLength(50); // Código o volante de autorización del seguro

        builder.Property(oad => oad.MontoMaximoAprobado)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(oad => oad.FechaAutorizacion)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Relaciones con Restricciones de Integridad Referencial Cruzada
        builder.HasOne(oad => oad.OrdenServicio)
            .WithMany() // Enlaza de manera virtual sin errores de tipo o sintaxis
            .HasForeignKey(oad => oad.OrdenServicioId)
            .OnDelete(DeleteBehavior.Cascade); // Si se anula la orden desde borrador, cae su detalle de póliza

        builder.HasOne(oad => oad.Aseguradora)
            .WithMany(a => a.OrdenesAmparadas)
            .HasForeignKey(oad => oad.AseguradoraId)
            .OnDelete(DeleteBehavior.Restrict); // Impide borrar la compañía si tiene siniestros en curso
    }
}

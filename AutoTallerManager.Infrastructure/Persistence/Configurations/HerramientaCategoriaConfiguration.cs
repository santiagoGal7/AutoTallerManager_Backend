using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Persistence.Configurations;

public class HerramientaCategoriaConfiguration : IEntityTypeConfiguration<HerramientaCategoria>
{
    public void Configure(EntityTypeBuilder<HerramientaCategoria> builder)
    {
        builder.ToTable("HerramientaCategorias");

        builder.HasKey(hc => hc.Id);

        builder.Property(hc => hc.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(hc => hc.Descripcion)
            .HasMaxLength(255)
            .HasDefaultValue(string.Empty);

        // Relación Uno a Muchos: Una categoría agrupa muchas herramientas
        builder.HasMany(hc => hc.Herramientas)
            .WithOne() // Mapeo implícito si no existe propiedad inversa tipada en Herramienta
            .HasForeignKey("CategoriaId") // Llave foránea en la tabla física de Herramientas
            .OnDelete(DeleteBehavior.Restrict); // Impide borrar una categoría con herramientas activas
    }
}

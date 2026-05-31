using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Persistence.Configurations;

public class UsuarioHorarioConfiguration : IEntityTypeConfiguration<UsuarioHorario>
{
    public void Configure(EntityTypeBuilder<UsuarioHorario> builder)
    {
        builder.ToTable("UsuarioHorarios");

        builder.HasKey(uh => uh.Id);

        builder.Property(uh => uh.DiaSemana)
            .IsRequired()
            .HasConversion<int>(); // Guarda el enum DayOfWeek como entero en PostgreSQL

        builder.Property(uh => uh.HoraInicio)
            .IsRequired();

        builder.Property(uh => uh.HoraFin)
            .IsRequired();

        // Relación Muchos a Uno con Usuario
        builder.HasOne(uh => uh.Usuario)
            .WithMany() // No requiere propiedad de colección inversa obligatoria en Usuario
            .HasForeignKey(uh => uh.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

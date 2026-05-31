using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Persistence.Configurations;

public class MecanicoEspecialidadConfiguration : IEntityTypeConfiguration<MecanicoEspecialidad>
{
    public void Configure(EntityTypeBuilder<MecanicoEspecialidad> builder)
    {
        builder.ToTable("MecanicoEspecialidades");

        builder.HasKey(me => me.Id);

        builder.Property(me => me.Especialidad)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(me => me.NivelExperiencia)
            .IsRequired()
            .HasMaxLength(50);

        // Relación Muchos a Uno con Usuario
        builder.HasOne(me => me.Usuario)
            .WithMany(u => u.Especialidades)
            .HasForeignKey(me => me.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

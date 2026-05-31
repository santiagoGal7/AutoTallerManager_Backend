using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Persistence.Configurations;

public class AuditoriaTransaccionConfiguration : IEntityTypeConfiguration<AuditoriaTransaccion>
{
    public void Configure(EntityTypeBuilder<AuditoriaTransaccion> builder)
    {
        builder.ToTable("AuditoriaTransacciones");

        builder.HasKey(at => at.Id);

        builder.Property(at => at.EntidadAfectada)
            .IsRequired()
            .HasMaxLength(100); // Ej: 'Vehiculo', 'Repuesto'

        builder.Property(at => at.TipoAccion)
            .IsRequired()
            .HasMaxLength(20); // 'INSERT', 'UPDATE', 'DELETE'

        // 🐘 MAPEO JSONB DE ALTO RENDIMIENTO PARA HISTÓRICOS DE AUDITORÍA
        builder.Property(at => at.DetalleDatos)
            .HasColumnType("jsonb")
            .IsRequired()
            .HasDefaultValue("{}");

        builder.Property(at => at.FechaHora)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Relación Muchos a Uno opcional (Nulable en Dominio si la acción fue del sistema/automatizada)
        builder.HasOne(at => at.Usuario)
            .WithMany()
            .HasForeignKey(at => at.UsuarioId)
            .OnDelete(DeleteBehavior.SetNull); // Si el usuario es purgado, conservamos el registro histórico de la transacción
    }
}

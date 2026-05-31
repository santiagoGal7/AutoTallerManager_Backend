using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Persistence.Configurations;

public class CierreContableDiarioConfiguration : IEntityTypeConfiguration<CierreContableDiario>
{
    public void Configure(EntityTypeBuilder<CierreContableDiario> builder)
    {
        builder.ToTable("CierresContablesDiarios");

        builder.HasKey(ccd => ccd.Id);

        builder.Property(ccd => ccd.FechaCierre)
            .IsRequired();

        builder.Property(ccd => ccd.TotalEsperadoSistema)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(ccd => ccd.TotalRealFisico)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(ccd => ccd.Diferencia)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(ccd => ccd.Observaciones)
            .HasMaxLength(500)
            .HasDefaultValue(string.Empty);

        // Relación Muchos a Uno con el Usuario de Rol Administrativo que ejecutó el arqueo
        builder.HasOne(ccd => ccd.UsuarioAdministrador)
            .WithMany()
            .HasForeignKey(ccd => ccd.UsuarioAdministradorId)
            .OnDelete(DeleteBehavior.Restrict); // Impide borrar al empleado si firmó cierres de caja
    }
}

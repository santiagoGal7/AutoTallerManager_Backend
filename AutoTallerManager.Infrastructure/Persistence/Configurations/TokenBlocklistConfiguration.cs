using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Persistence.Configurations;

public class TokenBlocklistConfiguration : IEntityTypeConfiguration<TokenBlocklist>
{
    public void Configure(EntityTypeBuilder<TokenBlocklist> builder)
    {
        builder.ToTable("TokenBlocklist");

        builder.HasKey(tb => tb.Id);

        builder.Property(tb => tb.TokenHash)
            .IsRequired()
            .HasMaxLength(255);

        // Índice para búsquedas ultrarrápidas de tokens invalidados en cada petición REST
        builder.HasIndex(tb => tb.TokenHash)
            .IsUnique();

        builder.Property(tb => tb.FechaRevocacion)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(tb => tb.FechaExpiracionOriginal)
            .IsRequired();
    }
}

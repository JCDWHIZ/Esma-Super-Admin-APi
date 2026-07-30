using Domain.Schools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Schools;

internal sealed class SisModuleConfiguration : IEntityTypeConfiguration<SisModule>
{
    public void Configure(EntityTypeBuilder<SisModule> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Key)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500)
            .IsRequired();

        builder.HasIndex(x => x.Key)
            .IsUnique();
    }
}

using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.SchoolAdmin;

internal sealed class SchoolAdminsConfiguration : IEntityTypeConfiguration<SchoolAdmins>
{
    public void Configure(EntityTypeBuilder<SchoolAdmins> builder)
    {
        builder.HasKey(sa => sa.Id);

        builder.Property(sa => sa.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(sa => sa.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(sa => sa.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(sa => sa.PasswordHash)
            .HasMaxLength(255);

        builder.Property(sa => sa.Role)
            .HasConversion<string>();

        builder.Property(sa => sa.Username)
            .HasMaxLength(100);

        builder.Property(sa => sa.PhoneNumber)
            .HasMaxLength(20);

        builder.HasIndex(sa => sa.Email)
            .IsUnique();

        builder.HasIndex(sa => sa.Username)
            .IsUnique();
    }
}
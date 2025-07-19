using Domain.Schools;
using Domain.Subscriptions;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Enums;

namespace Infrastructure.Configurations.Schools;

internal sealed class SchoolsConfiguration : IEntityTypeConfiguration<Domain.Schools.Schools>
{
    public void Configure(EntityTypeBuilder<Domain.Schools.Schools> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.SchoolName)
            .HasMaxLength(255);

        builder.Property(s => s.LogoUrl)
            .HasMaxLength(500);

        builder.OwnsOne(s => s.Address, addressBuilder =>
        {
            addressBuilder.Property(a => a.State)
                .HasMaxLength(100)
                .HasColumnName("AddressState");

            addressBuilder.Property(a => a.Country)
                .HasMaxLength(100)
                .HasColumnName("AddressCountry");

            addressBuilder.Property(a => a.LGA)
                .HasMaxLength(100)
                .HasColumnName("AddressLGA");

            addressBuilder.Property(a => a.StreetAddress)
                .HasMaxLength(255)
                .HasColumnName("AddressStreetAddress");
        });

        builder.Property(s => s.EmailAddress)
            .HasMaxLength(255);

        builder.Property(s => s.OrganizationId)
            .HasMaxLength(100);

        builder.Property(s => s.Status)
            .HasConversion<string>();

        builder.Property(s => s.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(s => s.DocumentUrl)
            .HasConversion(
                v => string.Join(';', v),
                v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList())
            .HasColumnType("text");

        builder.Property(s => s.Modules)
            .HasConversion(
                v => string.Join(';', v.Select(m => m.ToString())),
                v => v.Split(';', StringSplitOptions.RemoveEmptyEntries)
                      .Select(m => Enum.Parse<Modules>(m))
                      .ToList())
            .HasColumnType("text");

        builder.HasOne(s => s.Subscriptions)
            .WithOne(sub => sub.Schools)
            .HasForeignKey<Subscriptions>(sub => sub.SchoolId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
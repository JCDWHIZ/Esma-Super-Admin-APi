using Domain.Subscriptions;
using Domain.Schools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Subscription;

internal sealed class SubscriptionsConfiguration : IEntityTypeConfiguration<Subscriptions>
{
    public void Configure(EntityTypeBuilder<Subscriptions> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.SubscriptionType)
            .HasConversion<string>();

        builder.Property(s => s.StartDate)
            .HasConversion(d => d != null ? DateTime.SpecifyKind(d.Value, DateTimeKind.Utc) : d, v => v);

        builder.Property(s => s.EndDate)
            .HasConversion(d => d != null ? DateTime.SpecifyKind(d.Value, DateTimeKind.Utc) : d, v => v);

        builder.Property(s => s.Amount)
            .HasColumnType("decimal(18,2)");

        builder.HasOne(s => s.Schools)
            .WithOne(sch => sch.Subscriptions)
            .HasForeignKey<Subscriptions>(s => s.SchoolId);
    }
}
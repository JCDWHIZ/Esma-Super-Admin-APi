using Domain.HelpRequests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.HelpRequest;

internal sealed class HelpRequestsConfiguration : IEntityTypeConfiguration<Domain.HelpRequests.HelpRequests>
{
    public void Configure(EntityTypeBuilder<Domain.HelpRequests.HelpRequests> builder)
    {
        builder.HasKey(hr => hr.Id);

        builder.Property(hr => hr.TicketId)
            .HasMaxLength(50);

        builder.Property(hr => hr.Status)
            .HasConversion<string>();

        builder.Property(hr => hr.Category)
            .HasConversion<string>();

        builder.HasMany(hr => hr.Messages)
            .WithOne(m => m.HelpRequest)
            .HasForeignKey(m => m.HelpRequestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
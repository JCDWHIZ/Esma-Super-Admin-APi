using Domain.HelpRequests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.HelpRequest;

internal sealed class HelpRequestMessagesConfiguration : IEntityTypeConfiguration<HelpRequestMessages>
{
    public void Configure(EntityTypeBuilder<HelpRequestMessages> builder)
    {
        builder.HasKey(hrm => hrm.Id);

        builder.Property(hrm => hrm.Title)
            .HasMaxLength(255);

        builder.Property(hrm => hrm.Attachments)
            .HasConversion(
                v => string.Join(';', v),
                v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList())
            .HasColumnType("text");

        builder.HasOne(hrm => hrm.HelpRequest)
            .WithMany(hr => hr.Messages)
            .HasForeignKey(hrm => hrm.HelpRequestId);
    }
}
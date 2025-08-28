using Domain.Blogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Blogs;

internal sealed class BlogConfiguration : IEntityTypeConfiguration<Blog>
{
    public void Configure(EntityTypeBuilder<Blog> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Title)
            .HasMaxLength(255);

        builder.Property(b => b.Content)
            .HasColumnType("text");

        builder.Property(b => b.BackdropUrl)
            .HasMaxLength(500);

        builder.Property(b => b.RejectReason);

        builder.Property(b => b.Status)
            .HasConversion<string>();

        builder.Property(b => b.PublishDate)
            .HasConversion(d => d != null ? DateTime.SpecifyKind(d.Value, DateTimeKind.Utc) : d, v => v);
    }
}

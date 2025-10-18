using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Configurations.Template;
public sealed class TemplateConfiguration : IEntityTypeConfiguration<Domain.Templates.Template>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Domain.Templates.Template> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.TemplateName)
            .HasMaxLength(100)
            .IsRequired(true);
        builder.Property(u => u.TemplateBody)
            .IsRequired(true);

        builder.Property(u => u.ExpectedVariables)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null)
            )
            .IsRequired(false);

        builder.HasIndex(u => u.TemplateName).IsUnique();
        builder.HasIndex(u => u.TemplateTrigger).IsUnique();
    }
}

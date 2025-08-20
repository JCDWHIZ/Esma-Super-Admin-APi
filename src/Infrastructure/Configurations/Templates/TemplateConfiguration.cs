using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        builder.HasIndex(u => u.TemplateName).IsUnique();
        builder.HasIndex(u => u.TemplateTrigger).IsUnique();
    }
}

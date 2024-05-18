using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagement.Core.Models;

namespace ProjectManagement.Infrastructure.Persistence.Configurations;

public class TasksConfiguration : IEntityTypeConfiguration<ProjectTask>
{
    public void Configure(EntityTypeBuilder<ProjectTask> builder)
    {
        builder.HasKey(t => t.Id);

        builder
            .HasOne(t => t.Project)
            .WithMany(p => p.Tasks);
    }
}

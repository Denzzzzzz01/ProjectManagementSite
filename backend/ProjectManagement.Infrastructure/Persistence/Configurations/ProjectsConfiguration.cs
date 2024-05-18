using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagement.Core.Models;

namespace ProjectManagement.Infrastructure.Persistence.Configurations;
    
public class ProjectsConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.HasKey(p => p.Id);

        //builder
        //    .HasMany(p => p.Tasks)
        //    .WithOne(t => t.Project)
        //    .HasForeignKey(t => t.Id);

        //builder
        //    .HasMany(p => p.Users)
        //    .WithMany(u => u.Projects);

        //builder
        //    .HasOne(p => p.Owner)
        //    .WithMany(u => u.Projects);
    }
}

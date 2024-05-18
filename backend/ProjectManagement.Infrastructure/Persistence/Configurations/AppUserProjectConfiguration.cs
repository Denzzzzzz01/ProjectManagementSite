using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagement.Core.Models;

namespace ProjectManagement.Infrastructure.Persistence.Configurations;

public class AppUserProjectConfiguration : IEntityTypeConfiguration<AppUserProject>
{
    public void Configure(EntityTypeBuilder<AppUserProject> builder)
    {
        builder.HasKey(ap => new { ap.AppUserId, ap.ProjectId });

        builder
            .HasOne(ap => ap.User)
            .WithMany(a => a.AppUserProjects)
            .HasForeignKey(ap => ap.AppUserId);

        builder
            .HasOne(ap => ap.Project)
            .WithMany(p => p.AppUserProjects)
            .HasForeignKey(ap => ap.ProjectId);
    }
}

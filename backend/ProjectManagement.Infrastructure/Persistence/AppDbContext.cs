using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Core.Models;
using ProjectManagement.Infrastructure.Persistence.Configurations;

namespace ProjectManagement.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new AppUserProjectConfiguration());
        builder.ApplyConfiguration(new AppUsersConfiguration());
        builder.ApplyConfiguration(new ProjectsConfiguration());
        builder.ApplyConfiguration(new TasksConfiguration());
        base.OnModelCreating(builder);
        

        var roles = new List<IdentityRole<Guid>>
        {
            new IdentityRole<Guid>
            {
                Id = Guid.NewGuid(),
                Name= "Admin",
                NormalizedName = "ADMIN"
            },
            new IdentityRole<Guid>
            {
                Id = Guid.NewGuid(),
                Name= "User",
                NormalizedName = "USER"
            }
        };
        builder.Entity<IdentityRole<Guid>>().HasData(roles);
    }

    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectTask> Tasks { get; set; }

    public DbSet<AppUserProject> AppUserProjects { get; set; }
}

using Microsoft.EntityFrameworkCore;
using ProjectManagement.Core.Models;

namespace ProjectManagement.Application.Interfaces;

public interface IAppDbContext
{
    DbSet<Project> Projects { get; set; }
    DbSet<ProjectTask> Tasks { get; set; }
    DbSet<AppUserProject> AppUserProjects { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
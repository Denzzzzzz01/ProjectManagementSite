using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Exceptions;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Core.Models;

namespace ProjectManagement.Application.Services;

public class ProjectService
{
    private readonly IAppDbContext _dbContext;

    public ProjectService(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Project>> GetUserProjects(CancellationToken ct)
    {
        var projects = await _dbContext.Projects
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .ToListAsync(ct);

        return projects;
    }

    public async Task<Project> GetProjectById(Guid id, CancellationToken ct)
    {
        var project = await _dbContext.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project is null)
            throw new NotFoundException(nameof(Project), id);

        return project;
    }


    public async Task<Guid> CreateProject(Project project, AppUser appUser, CancellationToken ct)
    {
        var appUserProject = new AppUserProject
        {
            Project = project,
            AppUserId = appUser.Id
        };
        await _dbContext.AppUserProjects.AddAsync(appUserProject, ct);
        project.AppUserProjects.Add(appUserProject);

        await _dbContext.Projects.AddAsync(project, ct);
        await _dbContext.SaveChangesAsync(ct);
        return project.Id;
    }

    public async Task<Guid> UpdateProject(Project project, CancellationToken ct)
    {
        await _dbContext.Projects
            .Where(p => p.Id == project.Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.Name, project.Name),
                ct);

        return project.Id;
    }

    public async Task DeleteProject(Guid id, CancellationToken ct)
    {
        await _dbContext.Projects
            .Where(p => p.Id == id)
            .ExecuteDeleteAsync(ct);
    }
}

using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Contracts.Project;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Core.Enums;
using ProjectManagement.Core.Models;

namespace ProjectManagement.Application.Services;

public class ProjectService
{
    private readonly IAppDbContext _dbContext;

    public ProjectService(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Project>> GetUserProjects(AppUser appUser, CancellationToken ct = default)
    {
        var projects = await _dbContext.Projects
            .AsNoTracking()
            .Where(p => p.AppUserProjects.Any(aup => aup.AppUserId == appUser.Id))
            .OrderBy(p => p.Name)
            .ToListAsync(ct);

        return projects;
    }

    //public async Task<Project> GetProjectById(Guid id, AppUser appUser, CancellationToken ct = default)
    //{
    //    var project = await _dbContext.Projects
    //        .AsNoTracking()
    //        .Where(p => p.AppUserProjects.Any(aup => aup.AppUserId == appUser.Id))
    //        .FirstOrDefaultAsync(p => p.Id == id);

    //    if (project is null)
    //        throw new NotFoundException(nameof(Project), id);

    //    return project;
    //}


    public async Task<Guid> CreateProject(CreateProjectDto projectDto, AppUser appUser, CancellationToken ct = default)
    {
        var project = new Project
        {
            Id = Guid.NewGuid(),
            CreatedTime = DateTime.UtcNow, 
            AppUserProjects = new List<AppUserProject>(),
            Tasks = new List<ProjectTask>(),
            Status = Status.InProgress,

            OwnerId = appUser.Id,
            Name = projectDto.Name,
        };

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

    public async Task<Guid> UpdateProject(UpdateProjectDto projectDto, AppUser appUser, CancellationToken ct = default)
    {
        await _dbContext.Projects
            .Where(p => p.AppUserProjects.Any(aup => aup.AppUserId == appUser.Id))
            .Where(p => p.Id == projectDto.Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.Name, projectDto.Name),
                //.SetProperty(p => p.Status, project.Status),
                ct);

        return projectDto.Id;
    }

    public async Task<Guid> UpdateProjectStatus(UpdateProjectStatusDto projectDto, AppUser appUser, CancellationToken ct = default)
    {
        await _dbContext.Projects
            .Where(p => p.AppUserProjects.Any(aup => aup.AppUserId == appUser.Id))
            .Where(p => p.Id == projectDto.Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.Status, projectDto.Status),
                ct);

        return projectDto.Id;
    }

    public async Task DeleteProject(Guid id, AppUser appUser, CancellationToken ct = default)
    {
        await _dbContext.Projects
            .Where(p => p.AppUserProjects.Any(aup => aup.AppUserId == appUser.Id))
            .Where(p => p.Id == id)
            .ExecuteDeleteAsync(ct);
    }
}

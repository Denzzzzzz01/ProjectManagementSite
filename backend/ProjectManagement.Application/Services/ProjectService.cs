using Mapster;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Contracts.Project;
using ProjectManagement.Application.Common.Exceptions;
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

    public async Task<List<ProjectVm>> GetUserProjects(AppUser appUser, CancellationToken ct = default)
    {
        var projects = await _dbContext.Projects
            .AsNoTracking()
            .Where(p => p.AppUserProjects.Any(aup => aup.AppUserId == appUser.Id))
            .OrderBy(p => p.Name)
            .ToListAsync(ct);

        var projectsVm = projects.Adapt<List<ProjectVm>>();
        return projectsVm;
    }

    public async Task<ProjectDetailedVm> GetProjectById(Guid id, AppUser appUser, CancellationToken ct = default)
    {
        var project = await _dbContext.Projects
        .AsNoTracking()
        .Where(p => p.AppUserProjects.Any(aup => aup.AppUserId == appUser.Id))
        .Include(p => p.Tasks)
        .FirstOrDefaultAsync(p => p.Id == id, ct);

        if (project is null)
            throw new NotFoundException(nameof(Project), id);

        var projectVm = project.Adapt<ProjectDetailedVm>();
        return projectVm;
    }


    public async Task<ProjectDetailedVm> CreateProject(CreateProjectDto projectDto, AppUser appUser, CancellationToken ct = default)
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
        var projectVm = project.Adapt<ProjectDetailedVm>();
        return projectVm;
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

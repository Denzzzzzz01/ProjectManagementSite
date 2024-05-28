using Mapster;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Contracts.Project;
using ProjectManagement.Application.Common.Exceptions;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Core.Enums;
using ProjectManagement.Core.Models;
using Microsoft.Extensions.Logging;

namespace ProjectManagement.Application.Services;

public class ProjectService
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<ProjectService> _logger;

    public ProjectService(IAppDbContext dbContext, ILogger<ProjectService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<List<ProjectVm>> GetUserProjects(AppUser appUser, CancellationToken ct = default)
    {
        _logger.LogInformation("Fetching projects for user {UserId}", appUser.Id);

        var projects = await _dbContext.Projects
            .AsNoTracking()
            .Where(p => p.AppUserProjects.Any(aup => aup.AppUserId == appUser.Id))
            .OrderBy(p => p.Name)
            .ToListAsync(ct);

        var projectsVm = projects.Adapt<List<ProjectVm>>();
        _logger.LogInformation("Fetched {ProjectCount} projects for user {UserId}", projectsVm.Count, appUser.Id);
        return projectsVm;
    }

    public async Task<ProjectDetailedVm> GetProjectById(Guid id, AppUser appUser, CancellationToken ct = default)
    {
        _logger.LogInformation("Fetching project {ProjectId} for user {UserId}", id, appUser.Id);

        var project = await _dbContext.Projects
            .AsNoTracking()
            .Where(p => p.AppUserProjects.Any(aup => aup.AppUserId == appUser.Id))
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        if (project is null)
        {
            _logger.LogWarning("Project {ProjectId} not found for user {UserId}", id, appUser.Id);
            throw new NotFoundException(nameof(Project), id);
        }

        var projectVm = project.Adapt<ProjectDetailedVm>();
        _logger.LogInformation("Fetched project {ProjectId} for user {UserId}", id, appUser.Id);
        return projectVm;
    }

    public async Task<ProjectDetailedVm> CreateProject(CreateProjectDto projectDto, AppUser appUser, CancellationToken ct = default)
    {
        _logger.LogInformation("Creating project for user {UserId}", appUser.Id);

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
        _logger.LogInformation("Created project {ProjectId} for user {UserId}", projectVm.Id, appUser.Id);
        return projectVm;
    }

    public async Task<Guid> UpdateProject(UpdateProjectDto projectDto, AppUser appUser, CancellationToken ct = default)
    {
        _logger.LogInformation("Updating project {ProjectId} for user {UserId}", projectDto.Id, appUser.Id);

        var rowsAffected = await _dbContext.Projects
            .Where(p => p.AppUserProjects.Any(aup => aup.AppUserId == appUser.Id))
            .Where(p => p.Id == projectDto.Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.Name, projectDto.Name), ct);

        if (rowsAffected == 0)
        {
            _logger.LogWarning("Project {ProjectId} not found or not updated for user {UserId}", projectDto.Id, appUser.Id);
            throw new NotFoundException(nameof(Project), projectDto.Id);
        }

        _logger.LogInformation("Updated project {ProjectId} for user {UserId}", projectDto.Id, appUser.Id);
        return projectDto.Id;
    }

    public async Task<Guid> UpdateProjectStatus(UpdateProjectStatusDto projectDto, AppUser appUser, CancellationToken ct = default)
    {
        _logger.LogInformation("Updating status of project {ProjectId} for user {UserId}", projectDto.Id, appUser.Id);

        var rowsAffected = await _dbContext.Projects
            .Where(p => p.AppUserProjects.Any(aup => aup.AppUserId == appUser.Id))
            .Where(p => p.Id == projectDto.Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.Status, projectDto.Status), ct);

        if (rowsAffected == 0)
        {
            _logger.LogWarning("Project {ProjectId} not found or status not updated for user {UserId}", projectDto.Id, appUser.Id);
            throw new NotFoundException(nameof(Project), projectDto.Id);
        }

        _logger.LogInformation("Updated status of project {ProjectId} for user {UserId}", projectDto.Id, appUser.Id);
        return projectDto.Id;
    }

    public async Task DeleteProject(Guid id, AppUser appUser, CancellationToken ct = default)
    {
        _logger.LogInformation("Deleting project {ProjectId} for user {UserId}", id, appUser.Id);

        var rowsAffected = await _dbContext.Projects
            .Where(p => p.AppUserProjects.Any(aup => aup.AppUserId == appUser.Id))
            .Where(p => p.Id == id)
            .ExecuteDeleteAsync(ct);

        if (rowsAffected == 0)
        {
            _logger.LogWarning("Project {ProjectId} not found or not deleted for user {UserId}", id, appUser.Id);
            throw new NotFoundException(nameof(Project), id);
        }

        _logger.LogInformation("Deleted project {ProjectId} for user {UserId}", id, appUser.Id);
    }
}

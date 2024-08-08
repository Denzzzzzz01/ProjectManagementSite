using Mapster;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Contracts.Project;
using ProjectManagement.Application.Common.Exceptions;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Core.Enums;
using ProjectManagement.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Distributed;

namespace ProjectManagement.Application.Services;

public class ProjectService
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<ProjectService> _logger;
    private readonly ICacheService _cache;

    public ProjectService(IAppDbContext dbContext, ILogger<ProjectService> logger, ICacheService cache)
    {
        _dbContext = dbContext;
        _logger = logger;
        _cache = cache;
    }

    public async Task<List<ProjectVm>> GetUserProjects(AppUser appUser, CancellationToken ct = default)
    {
        var cacheKey = $"UserProjects_{appUser.Id}";
        var cachedProjects = await _cache.GetAsync<List<ProjectVm>>(cacheKey, ct);

        if (cachedProjects != null)
        {
            _logger.LogInformation("Fetching projects for user {UserId} from cache", appUser.Id);
            return cachedProjects;
        }

        _logger.LogInformation("Fetching projects for user {UserId} from database", appUser.Id);
        var projects = await _dbContext.Projects
            .AsNoTracking()
            .Where(p => p.AppUserProjects.Any(aup => aup.AppUserId == appUser.Id))
            .OrderBy(p => p.Name)
            .ToListAsync(ct);

        var projectsVm = projects.Adapt<List<ProjectVm>>();
        _logger.LogInformation("Fetched {ProjectCount} projects for user {UserId}", projectsVm.Count, appUser.Id);

        await _cache.SetAsync(cacheKey, projectsVm, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
        }, ct);

        return projectsVm;
    }

    public async Task<ProjectDetailedVm> GetProjectById(Guid id, AppUser appUser, CancellationToken ct = default)
    {
        var cacheKey = $"Project_{id}_{appUser.Id}";
        var cachedProject = await _cache.GetAsync<ProjectDetailedVm>(cacheKey, ct);

        if (cachedProject != null)
        {
            _logger.LogInformation("Fetching project {ProjectId} for user {UserId} from cache", id, appUser.Id);
            return cachedProject;
        }

        _logger.LogInformation("Fetching project {ProjectId} for user {UserId} from database", id, appUser.Id);
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

        await _cache.SetAsync(cacheKey, projectVm, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
        }, ct);

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
            Description = projectDto.Description
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

        var cacheKey = $"UserProjects_{appUser.Id}";
        await _cache.RemoveAsync(cacheKey, ct);

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

        var cacheKey = $"UserProjects_{appUser.Id}";
        await _cache.RemoveAsync(cacheKey, ct);

        var projectCacheKey = $"Project_{projectDto.Id}_{appUser.Id}";
        await _cache.RemoveAsync(projectCacheKey, ct);

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

        var cacheKey = $"UserProjects_{appUser.Id}";
        await _cache.RemoveAsync(cacheKey, ct);

        var projectCacheKey = $"Project_{projectDto.Id}_{appUser.Id}";
        await _cache.RemoveAsync(projectCacheKey, ct);

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

        var cacheKey = $"UserProjects_{appUser.Id}";
        await _cache.RemoveAsync(cacheKey, ct);

        var projectCacheKey = $"Project_{id}_{appUser.Id}";
        await _cache.RemoveAsync(projectCacheKey, ct);
    }
}

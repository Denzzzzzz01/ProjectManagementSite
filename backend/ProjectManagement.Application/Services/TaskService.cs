using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using ProjectManagement.Application.Common.Exceptions;
using ProjectManagement.Application.Contracts.Task;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Core.Models;

public class TaskService
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<TaskService> _logger;
    private readonly ICacheService _cache;

    public TaskService(IAppDbContext dbContext, ILogger<TaskService> logger, ICacheService cache)
    {
        _dbContext = dbContext;
        _logger = logger;
        _cache = cache;
    }

    public async Task<List<TaskVm>> GetProjectTasks(Guid projectId, AppUser appUser, CancellationToken ct = default)
    {
        var cacheKey = $"ProjectTasks_{projectId}_{appUser.Id}";
        var cachedTasks = await _cache.GetAsync<List<TaskVm>>(cacheKey, ct);

        if (cachedTasks != null)
        {
            _logger.LogInformation("Fetching tasks for project {ProjectId} and user {UserId} from cache", projectId, appUser.Id);
            return cachedTasks;
        }

        _logger.LogInformation("Fetching tasks for project {ProjectId} and user {UserId} from database", projectId, appUser.Id);
        var project = await _dbContext.Projects
            .AsNoTracking()
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == projectId && p.AppUserProjects.Any(aup => aup.AppUserId == appUser.Id), ct);

        if (project is null)
        {
            _logger.LogWarning("Project {ProjectId} not found for user {UserId}", projectId, appUser.Id);
            throw new NotFoundException(nameof(Project), projectId);
        }

        var tasks = project.Tasks.OrderBy(t => t.AddedTime).ToList();
        var tasksVm = tasks.Adapt<List<TaskVm>>();
        _logger.LogInformation("Fetched {TaskCount} tasks for project {ProjectId} and user {UserId}", tasksVm.Count, projectId, appUser.Id);

        await _cache.SetAsync(cacheKey, tasksVm, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
        }, ct);

        return tasksVm;
    }

    public async Task<TaskDetailedVm> AddTask(AddTaskDto taskDto, AppUser appUser, CancellationToken ct = default)
    {
        _logger.LogInformation("Adding task to project {ProjectId} for user {UserId}", taskDto.ProjectId, appUser.Id);

        var projectExists = await _dbContext.Projects
            .AsNoTracking()
            .AnyAsync(p => p.Id == taskDto.ProjectId && p.OwnerId == appUser.Id, ct);  // Проверка владельца проекта

        if (!projectExists)
        {
            _logger.LogWarning("Project {ProjectId} not found for user {UserId}", taskDto.ProjectId, appUser.Id);
            throw new NotFoundException(nameof(Project), taskDto.ProjectId);
        }

        var task = new ProjectTask
        {
            Id = Guid.NewGuid(),
            ProjectId = taskDto.ProjectId,

            AddedTime = DateTime.UtcNow,
            DoneTime = null,
            DueDate = null,
            IsDone = false,

            Title = taskDto.Title,
            Description = taskDto.Description,
            Priority = taskDto.Priority
        };

        await _dbContext.Tasks.AddAsync(task, ct);
        await _dbContext.SaveChangesAsync(ct);

        var taskVm = task.Adapt<TaskDetailedVm>();
        _logger.LogInformation("Added task {TaskId} to project {ProjectId} for user {UserId}", taskVm.Id, taskDto.ProjectId, appUser.Id);

        var cacheKey = $"ProjectTasks_{taskDto.ProjectId}_{appUser.Id}";
        await _cache.RemoveAsync(cacheKey, ct);

        await _cache.InvalidateProjectCache(taskDto.ProjectId, appUser.Id, ct);

        return taskVm;
    }

    public async Task<Guid> UpdateTask(UpdateTaskDto taskDto, AppUser appUser, CancellationToken ct = default)
    {
        _logger.LogInformation("Updating task {TaskId} in project {ProjectId} for user {UserId}", taskDto.TaskId, taskDto.ProjectId, appUser.Id);

        var rowsAffected = await _dbContext.Tasks
            .Where(t => t.Id == taskDto.TaskId && t.ProjectId == taskDto.ProjectId)
            .Join(_dbContext.Projects,
                  t => t.ProjectId,
                  p => p.Id,
                  (t, p) => new { Task = t, Project = p })
            .Where(tp => tp.Project.OwnerId == appUser.Id)  // Проверка владельца проекта
            .Select(tp => tp.Task)
            .ExecuteUpdateAsync(s => s
                .SetProperty(t => t.Title, taskDto.Title)
                .SetProperty(t => t.Description, taskDto.Description)
                .SetProperty(t => t.Priority, taskDto.Priority), ct);

        if (rowsAffected == 0)
        {
            _logger.LogWarning("Task {TaskId} or project {ProjectId} not found for user {UserId}", taskDto.TaskId, taskDto.ProjectId, appUser.Id);
            throw new NotFoundException(nameof(Project), taskDto.ProjectId);
        }

        _logger.LogInformation("Updated task {TaskId} in project {ProjectId} for user {UserId}", taskDto.TaskId, taskDto.ProjectId, appUser.Id);

        var cacheKey = $"ProjectTasks_{taskDto.ProjectId}_{appUser.Id}";
        await _cache.RemoveAsync(cacheKey, ct);

        await _cache.InvalidateProjectCache(taskDto.ProjectId, appUser.Id, ct);

        return taskDto.TaskId;
    }

    public async Task RemoveTask(RemoveTaskDto taskDto, AppUser appUser, CancellationToken ct = default)
    {
        _logger.LogInformation("Removing task {TaskId} from project {ProjectId} for user {UserId}", taskDto.TaskId, taskDto.ProjectId, appUser.Id);

        var rowsAffected = await _dbContext.Tasks
            .Where(t => t.Id == taskDto.TaskId && t.ProjectId == taskDto.ProjectId)
            .Join(_dbContext.Projects,
                  t => t.ProjectId,
                  p => p.Id,
                  (t, p) => new { Task = t, Project = p })
            .Where(tp => tp.Project.OwnerId == appUser.Id)  // Проверка владельца проекта
            .Select(tp => tp.Task)
            .ExecuteDeleteAsync(ct);

        if (rowsAffected == 0)
        {
            _logger.LogWarning("Task {TaskId} or project {ProjectId} not found for user {UserId}", taskDto.TaskId, taskDto.ProjectId, appUser.Id);
            throw new NotFoundException(nameof(Project), taskDto.ProjectId);
        }

        _logger.LogInformation("Removed task {TaskId} from project {ProjectId} for user {UserId}", taskDto.TaskId, taskDto.ProjectId, appUser.Id);

        var cacheKey = $"ProjectTasks_{taskDto.ProjectId}_{appUser.Id}";
        await _cache.RemoveAsync(cacheKey, ct);

        await _cache.InvalidateProjectCache(taskDto.ProjectId, appUser.Id, ct);
    }


    public async Task<Guid> DoTask(DoTaskDto taskDto, AppUser appUser, CancellationToken ct = default)
    {
        _logger.LogInformation("Marking task {TaskId} as done in project {ProjectId} for user {UserId}", taskDto.TaskId, taskDto.ProjectId, appUser.Id);

        var rowsAffected = await _dbContext.Tasks
            .Where(t => t.Id == taskDto.TaskId && t.ProjectId == taskDto.ProjectId)
            .Join(_dbContext.Projects,
                  t => t.ProjectId,
                  p => p.Id,
                  (t, p) => new { Task = t, Project = p })
            .Where(tp => tp.Project.AppUserProjects.Any(aup => aup.AppUserId == appUser.Id))
            .Select(tp => tp.Task)
            .ExecuteUpdateAsync(s => s
                .SetProperty(t => t.IsDone, taskDto.IsDone), ct);

        if (rowsAffected == 0)
        {
            _logger.LogWarning("Task {TaskId} or project {ProjectId} not found for user {UserId}", taskDto.TaskId, taskDto.ProjectId, appUser.Id);
            throw new NotFoundException(nameof(Project), taskDto.ProjectId);
        }

        _logger.LogInformation("Marked task {TaskId} as done in project {ProjectId} for user {UserId}", taskDto.TaskId, taskDto.ProjectId, appUser.Id);

        var cacheKey = $"ProjectTasks_{taskDto.ProjectId}_{appUser.Id}";
        await _cache.RemoveAsync(cacheKey, ct);

        await _cache.InvalidateProjectCache(taskDto.ProjectId, appUser.Id, ct);

        return taskDto.TaskId;
    }

}

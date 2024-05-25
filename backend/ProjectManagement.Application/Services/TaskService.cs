using Mapster;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Contracts.Task;
using ProjectManagement.Application.Common.Exceptions;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Core.Models;

namespace ProjectManagement.Application.Services;

public class TaskService
{
    private readonly IAppDbContext _dbContext;

    public TaskService(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<TaskVm>> GetProjectTasks(Guid projectId, AppUser appUser, CancellationToken ct = default)
    {
        var project = await _dbContext.Projects
            .AsNoTracking()
            .Include(p => p.Tasks) 
            .FirstOrDefaultAsync(p => p.Id == projectId && p.AppUserProjects.Any(aup => aup.AppUserId == appUser.Id), ct);

        if (project is null)
            throw new NotFoundException(nameof(Project), projectId);

        var tasks = project.Tasks.OrderBy(t => t.AddedTime).ToList();
        var tasksVm = tasks.Adapt<List<TaskVm>>();
        return tasksVm;
    }

    public async Task<TaskDetailedVm> AddTask(AddTaskDto taskDto, AppUser appUser, CancellationToken ct = default)
    {
        var projectExists = await _dbContext.Projects
            .AsNoTracking()
            .AnyAsync(p => p.Id == taskDto.ProjectId && p.AppUserProjects.Any(aup => aup.AppUserId == appUser.Id), ct);

        if (!projectExists)
            throw new NotFoundException(nameof(Project), taskDto.ProjectId);

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
        return taskVm;
    }


    public async Task<Guid> UpdateTask(UpdateTaskDto taskDto, AppUser appUser, CancellationToken ct = default)
    {
        var rowsAffected = await _dbContext.Tasks
            .Where(t => t.Id == taskDto.TaskId && t.ProjectId == taskDto.ProjectId)
            .Join(_dbContext.Projects,
                  t => t.ProjectId,
                  p => p.Id,
                  (t, p) => new { Task = t, Project = p })
            .Where(tp => tp.Project.AppUserProjects.Any(aup => aup.AppUserId == appUser.Id))
            .Select(tp => tp.Task)
            .ExecuteUpdateAsync(s => s
                .SetProperty(t => t.Title, taskDto.Title)
                .SetProperty(t => t.Description, taskDto.Description)
                .SetProperty(t => t.Priority, taskDto.Priority), ct);

        if (rowsAffected == 0)
            throw new NotFoundException(nameof(Project), taskDto.ProjectId);

        return taskDto.TaskId;
    }

    public async Task<Guid> DoTask(DoTaskDto taskDto, AppUser appUser, CancellationToken ct = default)
    {
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
            throw new NotFoundException(nameof(Project), taskDto.ProjectId);

        return taskDto.TaskId;
    }

    public async Task RemoveTask(RemoveTaskDto taskDto, AppUser appUser, CancellationToken ct = default)
    {
        var rowsAffected = await _dbContext.Tasks
            .Where(t => t.Id == taskDto.TaskId && t.ProjectId == taskDto.ProjectId)
            .Join(_dbContext.Projects,
                  t => t.ProjectId,
                  p => p.Id,
                  (t, p) => new { Task = t, Project = p })
            .Where(tp => tp.Project.AppUserProjects.Any(aup => aup.AppUserId == appUser.Id))
            .Select(tp => tp.Task)
            .ExecuteDeleteAsync(ct);

        if (rowsAffected == 0)
            throw new NotFoundException(nameof(Project), taskDto.ProjectId);
    }
}
﻿using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Contracts.Task;
using ProjectManagement.Application.Exceptions;
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

    public async Task<List<ProjectTask>> GetProjectTasks(Guid projectId, AppUser appUser, CancellationToken ct = default)
    {
        var tasks = await _dbContext.Projects
            .Where(p => p.Id == projectId && p.AppUserProjects.Any(aup => aup.AppUserId == appUser.Id))
            .SelectMany(p => p.Tasks)
            .AsNoTracking()
            .OrderBy(t => t.AddedTime)
            .ToListAsync(ct);

        if (!tasks.Any())
            throw new NotFoundException(nameof(Project), projectId);

        return tasks;
    }

    public async Task<Guid> AddTask(AddTaskDto taskDto, AppUser appUser, CancellationToken ct = default)
    {
        var projectExists = await _dbContext.Projects
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
        return task.Id;
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

    public async Task DeleteTask(DeleteTaskDto taskDto, AppUser appUser, CancellationToken ct = default)
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

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.Contracts.Task;
using ProjectManagement.Core.Models;

namespace ProjectManagement.Api.Controllers;

public class TaskController : BaseController
{
    private readonly TaskService _taskService;

    public TaskController(TaskService taskService, UserManager<AppUser> userManager) : base(userManager) 
    {
        _taskService = taskService;
    }


    //[HttpGet(nameof(GetProjectTasks))]
    //public async Task<ActionResult<List<TaskVm>>> GetProjectTasks(Guid projectId, CancellationToken ct)
    //{
    //    var appUser = await GetCurrentUserAsync();
    //    var tasks = await _taskService.GetProjectTasks(projectId, appUser, ct);
    //    return Ok(tasks);
    //}

    [HttpPost(nameof(AddTask))]
    public async Task<ActionResult<TaskDetailedVm>> AddTask([FromBody] AddTaskDto taskDto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var appUser = await GetCurrentUserAsync();
        var task = await _taskService.AddTask(taskDto, appUser, ct);
        return CreatedAtAction(nameof(AddTask), task);
    }

    [HttpPut(nameof(UpdateTask))]
    public async Task<ActionResult<Guid>> UpdateTask([FromBody] UpdateTaskDto taskDto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var appUser = await GetCurrentUserAsync();
        var taskId = await _taskService.UpdateTask(taskDto, appUser, ct);
        return Ok(taskId);
    }

    [HttpPut(nameof(DoTask))]
    public async Task<ActionResult<Guid>> DoTask([FromBody] DoTaskDto taskDto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var appUser = await GetCurrentUserAsync();
        var taskId = await _taskService.DoTask(taskDto, appUser, ct);
        return Ok(taskId);
    }

    [HttpDelete(nameof(RemoveTask))]
    public async Task<IActionResult> RemoveTask([FromBody] RemoveTaskDto taskDto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var appUser = await GetCurrentUserAsync();
        await _taskService.RemoveTask(taskDto, appUser, ct);
        return Ok();
    }
}


using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.Contracts.Project;
using ProjectManagement.Application.Services;
using ProjectManagement.Core.Models;

namespace ProjectManagement.Api.Controllers;

public class ProjectController : BaseController
{
    private readonly ProjectService _projectService;

    public ProjectController(ProjectService projectService, UserManager<AppUser> userManager) : base(userManager)
    {
        _projectService = projectService;
    }

    [HttpGet(nameof(GetUserProjects))]
    public async Task<ActionResult<List<ProjectVm>>> GetUserProjects(CancellationToken ct)
    {
        var appUser = await GetCurrentUserAsync();
        var projects = await _projectService.GetUserProjects(appUser, ct);
        return Ok(projects);
    }

    
    [HttpGet(nameof(GetProjectById))]
    public async Task<ActionResult<ProjectDetailedVm>> GetProjectById(Guid projectId, CancellationToken ct)
    {
        var appUser = await GetCurrentUserAsync();
        var project = await _projectService.GetProjectById(projectId, appUser, ct);
        return project;
    }

    [HttpPost(nameof(CreateProject))]
    public async Task<ActionResult<ProjectDetailedVm>> CreateProject([FromBody] CreateProjectDto projectDto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var appUser = await GetCurrentUserAsync();
        var project = await _projectService.CreateProject(projectDto, appUser, ct);
        return CreatedAtAction(nameof(CreateProject), project);
    }

    [HttpPut(nameof(UpdateProject))]
    public async Task<ActionResult<Guid>> UpdateProject([FromBody]  UpdateProjectDto projectDto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var appUser = await GetCurrentUserAsync();
        var projectId = await _projectService.UpdateProject(projectDto, appUser, ct);
        return Ok(projectId);   
    }

    [HttpPut(nameof(UpdateProjectStaus))]
    public async Task<ActionResult<Guid>> UpdateProjectStaus([FromBody] UpdateProjectStatusDto projectDto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var appUser = await GetCurrentUserAsync();
        var projectId = await _projectService.UpdateProjectStatus(projectDto, appUser, ct);
        return Ok(projectId);
    }

    [HttpDelete(nameof(DeleteProject))]
    public async Task<IActionResult> DeleteProject(Guid ProjectId, CancellationToken ct)
    {
        var appUser = await GetCurrentUserAsync();
        await _projectService.DeleteProject(ProjectId, appUser, ct);
        return Ok();
    }
}

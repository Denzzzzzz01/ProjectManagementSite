using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.Contracts.Project;
using ProjectManagement.Application.Services;
using ProjectManagement.Core.Enums;
using ProjectManagement.Core.Models;
using System.Security.Claims;

namespace ProjectManagement.Api.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")] 
public class ProjectController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ProjectService _projectService;

    public ProjectController(ProjectService projectService, UserManager<AppUser> userManager)
    {
        _projectService = projectService;
        _userManager = userManager;
    }

    [HttpGet(nameof(GetUserProjects))]
    public async Task<ActionResult<List<Project>>> GetUserProjects(CancellationToken ct)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        var appUser = await _userManager.FindByEmailAsync(userEmail!);
        if (appUser is null)
            return Unauthorized();

        var projects = await _projectService.GetUserProjects(appUser, ct);
        return Ok(projects);
    }

    [HttpPost(nameof(CreateProject))]
    public async Task<ActionResult<Guid>> CreateProject([FromBody] CreateProjectDto projectDto, CancellationToken ct)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        var appUser = await _userManager.FindByEmailAsync(userEmail!);
        if (appUser is null)
            return Unauthorized();

        var id = await _projectService.CreateProject(projectDto, appUser, ct);

        return Ok(id);
    }
}

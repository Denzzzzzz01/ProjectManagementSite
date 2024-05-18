using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Api.Contracts.Project;
using ProjectManagement.Api.Extensions;
using ProjectManagement.Application.Services;
using ProjectManagement.Core.Enums;
using ProjectManagement.Core.Models;
using System.Security.Claims;

namespace ProjectManagement.Api.Controllers;

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
        var username = User.GetUsername();
        if (username is null)
            return Unauthorized();
        var appUser = await _userManager.FindByNameAsync(username);
        if (appUser is null)
            return Unauthorized();

        var projects = await _projectService.GetUserProjects(ct);
        return Ok(projects);
    }

    [HttpPost(nameof(CreateProject))]
    public async Task<ActionResult<Guid>> CreateProject([FromBody] ProjectDto projectDto, CancellationToken ct)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        if (userEmail is null)
            return Unauthorized();
        var appUser = await _userManager.FindByEmailAsync(userEmail);
        if (appUser is null)
            return Unauthorized();
        

        var project = new Project
        {
            Id = Guid.NewGuid(),
            //Users = new List<AppUser>() { appUser },
            CreatedTime = DateTime.UtcNow,
            Tasks = new List<ProjectTask>(),
            Status = Status.InProgress,

            //Owner = appUser,
            Name = projectDto.Name,

        };


        var id = await _projectService.CreateProject(project, appUser, ct);

        return Ok(id);
    }
}

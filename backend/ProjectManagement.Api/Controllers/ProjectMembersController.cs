using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.Contracts.Project;
using ProjectManagement.Application.Services;
using ProjectManagement.Core.Models;

namespace ProjectManagement.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProjectMembersController : BaseController
{
    private readonly ProjectMembersService _projectMembersService;

    public ProjectMembersController(ProjectMembersService projectMembersService, UserManager<AppUser> userManager) : base(userManager)
    {
        _projectMembersService = projectMembersService;
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<AppUserDto>>> SearchUsers(string term, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(term) || term.Length < 3)
            return BadRequest("Search term must be at least 3 characters long.");

        var users = await _projectMembersService.SearchUsers(term, ct);
        return Ok(users);
    }

    [HttpPost("{projectId}/add")]
    public async Task<ActionResult> AddUserToProject(Guid projectId, Guid userId, CancellationToken ct)
    {
        var currentUser = await GetCurrentUserAsync();

        await _projectMembersService.AddUserToProject(projectId, userId, currentUser, ct);
        return NoContent();
    }

    [HttpGet("{projectId}/members")]
    public async Task<ActionResult<List<AppUserDto>>> GetProjectMembers(Guid projectId, CancellationToken ct)
    {
        var members = await _projectMembersService.GetProjectMembers(projectId, ct);
        return Ok(members);
    }

    [HttpDelete("{projectId}/members/{userId}")]
    public async Task<IActionResult> RemoveUserFromProject(Guid projectId, Guid userId, CancellationToken ct)
    {
        try
        {
            await _projectMembersService.RemoveUserFromProject(projectId, userId, ct);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}

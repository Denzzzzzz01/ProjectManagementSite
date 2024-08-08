using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Common.Exceptions;
using ProjectManagement.Application.Contracts.Project;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Core.Models;

namespace ProjectManagement.Application.Services;

public class ProjectMembersService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IAppDbContext _dbContext;

    public ProjectMembersService(UserManager<AppUser> userManager, IAppDbContext dbContext)
    {
        _userManager = userManager;
        _dbContext = dbContext;
    }

    public async Task<List<AppUserDto>> SearchUsers(string searchTerm, CancellationToken ct = default)
    {
        if (searchTerm.Length < 3)
            throw new ArgumentException("Search term must be at least 3 characters long.");

        var users = await _userManager.Users
            .Where(u => u.UserName.StartsWith(searchTerm))
            .Select(u => new AppUserDto
            {
                Id = u.Id,
                UserName = u.UserName
            })
            .ToListAsync(ct);

        return users;
    }

    public async Task AddUserToProject(Guid projectId, Guid userId, AppUser owner, CancellationToken ct = default)
    {
        var project = await _dbContext.Projects
            .Include(p => p.AppUserProjects)
            .FirstOrDefaultAsync(p => p.Id == projectId, ct);

        if (project == null)
            throw new NotFoundException(nameof(Project), projectId);

        if (project.OwnerId != owner.Id)
            throw new UnauthorizedAccessException("Only the project owner can add members.");

        if (project.AppUserProjects.Any(aup => aup.AppUserId == userId))
            throw new InvalidOperationException("User is already a member of the project.");

        var userProject = new AppUserProject
        {
            ProjectId = projectId,
            AppUserId = userId
        };

        project.AppUserProjects.Add(userProject);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task<List<AppUserDto>> GetProjectMembers(Guid projectId, CancellationToken ct = default)
    {
        var project = await _dbContext.Projects
            .Include(p => p.AppUserProjects)
            .ThenInclude(aup => aup.User)
            .FirstOrDefaultAsync(p => p.Id == projectId, ct);

        if (project is null)
        {
            throw new NotFoundException(nameof(Project), projectId);
        }

        var members = project.AppUserProjects.Select(aup => new AppUserDto
        {
            Id = aup.AppUserId,
            UserName = aup.User.UserName
        }).ToList();

        return members;
    }

    public async Task RemoveUserFromProject(Guid projectId, Guid userId, CancellationToken ct = default)
    {
        var appUserProject = await _dbContext.AppUserProjects
            .FirstOrDefaultAsync(aup => aup.ProjectId == projectId && aup.AppUserId == userId, ct);

        if (appUserProject == null)
        {
            throw new KeyNotFoundException("User not found in project");
        }

        _dbContext.AppUserProjects.Remove(appUserProject);
        await _dbContext.SaveChangesAsync(ct);
    }
}

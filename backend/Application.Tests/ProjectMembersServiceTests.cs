using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ProjectManagement.Application.Common.Exceptions;
using ProjectManagement.Application.Contracts.Project;
using ProjectManagement.Application.Services;
using ProjectManagement.Core.Models;
using ProjectManagement.Infrastructure.Persistence;

namespace Application.Tests;

public class ProjectMembersServiceTests
{
    private readonly Mock<UserManager<AppUser>> _userManagerMock;
    private readonly Mock<ICacheService> _cacheMock;
    private readonly Mock<ILogger<ProjectMembersService>> _loggerMock;
    private readonly AppDbContext _context;

    public ProjectMembersServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();

        _userManagerMock = new Mock<UserManager<AppUser>>(
            new Mock<IUserStore<AppUser>>().Object,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null
        );

        _cacheMock = new Mock<ICacheService>();
        _loggerMock = new Mock<ILogger<ProjectMembersService>>();
    }

    
    [Fact]
    public async Task AddUserToProject_ShouldAddUser_WhenProjectExists()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var owner = new AppUser { Id = Guid.NewGuid() };

        var project = new Project
        {
            Id = projectId,
            OwnerId = owner.Id,
            AppUserProjects = new List<AppUserProject>()
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        _userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new AppUser { Id = userId });

        var service = new ProjectMembersService(
            _userManagerMock.Object,
            _context,
            _cacheMock.Object,
            _loggerMock.Object
        );

        // Act
        await service.AddUserToProject(projectId, userId, owner);

        // Assert
        var userProject = await _context.AppUserProjects
            .FirstOrDefaultAsync(up => up.ProjectId == projectId && up.AppUserId == userId);
        Assert.NotNull(userProject);
    }

    [Fact]
    public async Task AddUserToProject_ShouldThrowNotFoundException_WhenProjectDoesNotExist()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var owner = new AppUser { Id = Guid.NewGuid() };

        var service = new ProjectMembersService(
            _userManagerMock.Object,
            _context,
            _cacheMock.Object,
            _loggerMock.Object
        );

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => service.AddUserToProject(projectId, userId, owner));
    }

    [Fact]
    public async Task AddUserToProject_ShouldThrowUnauthorizedAccessException_WhenUserIsNotOwner()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var owner = new AppUser { Id = Guid.NewGuid() };

        var project = new Project
        {
            Id = projectId,
            OwnerId = Guid.NewGuid(),
            AppUserProjects = new List<AppUserProject>()
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        _userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new AppUser { Id = userId });

        var service = new ProjectMembersService(
            _userManagerMock.Object,
            _context,
            _cacheMock.Object,
            _loggerMock.Object
        );

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.AddUserToProject(projectId, userId, owner));
    }

    [Fact]
    public async Task GetProjectMembers_ShouldReturnMembers_WhenCacheIsMiss()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var user = new AppUser { Id = Guid.NewGuid(), UserName = "user" };

        var project = new Project
        {
            Id = projectId,
            AppUserProjects = new List<AppUserProject>
            {
                new AppUserProject { AppUserId = user.Id, User = user }
            }
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        _cacheMock.Setup(c => c.GetAsync<List<AppUserDto>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((List<AppUserDto>)null);

        var service = new ProjectMembersService(
            _userManagerMock.Object,
            _context,
            _cacheMock.Object,
            _loggerMock.Object
        );

        // Act
        var result = await service.GetProjectMembers(projectId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(user.Id, result.First().Id);
        Assert.Equal(user.UserName, result.First().UserName);
    }

    [Fact]
    public async Task RemoveUserFromProject_ShouldRemoveUser_WhenUserExistsInProject()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var appUserProject = new AppUserProject
        {
            ProjectId = projectId,
            AppUserId = userId
        };

        _context.AppUserProjects.Add(appUserProject);
        await _context.SaveChangesAsync();

        var service = new ProjectMembersService(
            _userManagerMock.Object,
            _context,
            _cacheMock.Object,
            _loggerMock.Object
        );

        // Act
        await service.RemoveUserFromProject(projectId, userId);

        // Assert
        var removedUserProject = await _context.AppUserProjects
            .FirstOrDefaultAsync(up => up.ProjectId == projectId && up.AppUserId == userId);
        Assert.Null(removedUserProject);
    }
}

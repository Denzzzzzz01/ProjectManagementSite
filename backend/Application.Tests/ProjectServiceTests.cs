using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ProjectManagement.Application.Contracts.Project;
using ProjectManagement.Application.Services;
using ProjectManagement.Core.Models;
using ProjectManagement.Infrastructure.Persistence;
using Microsoft.Extensions.Caching.Distributed;

namespace Application.Tests;

public class ProjectServiceTests
{
    [Fact]
    public async Task GetUserProjects_ShouldReturnUserProjects()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "UserProjectsTest")
            .Options;

        var appUser = new AppUser { Id = Guid.NewGuid() };

        using (var context = new AppDbContext(options))
        {
            var firstProject = new Project { Id = Guid.NewGuid(), Name = "Project 1", OwnerId = appUser.Id };
            var secondProject = new Project { Id = Guid.NewGuid(), Name = "Project 2", OwnerId = appUser.Id };

            context.Projects.AddRange(
                firstProject, secondProject,
                new Project { Id = Guid.NewGuid(), Name = "Project 3" }
            );

            context.AppUserProjects.AddRange(
                new AppUserProject { AppUserId = appUser.Id, ProjectId = firstProject.Id },
                new AppUserProject { AppUserId = appUser.Id, ProjectId = secondProject.Id }
            );
            context.SaveChanges();
        }

        using (var context = new AppDbContext(options))
        {
            var mockLogger = new Mock<ILogger<ProjectService>>();
            var mockCache = new Mock<ICacheService>();
            var service = new ProjectService(context, mockLogger.Object, mockCache.Object);

            mockCache
                .Setup(x => x.GetAsync<List<ProjectVm>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((List<ProjectVm>)null);

            mockCache
                .Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<List<ProjectVm>>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var projects = await service.GetUserProjects(appUser);

            // Assert
            Assert.Equal(2, projects.Count);
            mockCache.Verify(x => x.GetAsync<List<ProjectVm>>(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            mockCache.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<List<ProjectVm>>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }

    [Fact]
    public async Task GetProjectById_ShouldReturnProject()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "ProjectByIdTest")
            .Options;

        var projectId = Guid.NewGuid();
        var appUser = new AppUser { Id = Guid.NewGuid() };

        using (var context = new AppDbContext(options))
        {
            var project = new Project { Id = projectId, Name = "Project 1", OwnerId = appUser.Id };

            context.Projects.Add(project);
            context.AppUserProjects.Add(new AppUserProject { AppUserId = appUser.Id, ProjectId = project.Id });

            context.SaveChanges();
        }

        using (var context = new AppDbContext(options))
        {
            var mockLogger = new Mock<ILogger<ProjectService>>();
            var mockCache = new Mock<ICacheService>();
            var service = new ProjectService(context, mockLogger.Object, mockCache.Object);

            mockCache
                .Setup(x => x.GetAsync<ProjectDetailedVm>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ProjectDetailedVm)null);

            mockCache
                .Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<ProjectDetailedVm>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var project = await service.GetProjectById(projectId, appUser);

            // Assert
            Assert.NotNull(project);
            Assert.Equal(projectId, project.Id);
            mockCache.Verify(x => x.GetAsync<ProjectDetailedVm>(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            mockCache.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<ProjectDetailedVm>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }

    [Fact]
    public async Task CreateProject_ShouldCreateAndReturnProject()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        var appUser = new AppUser { Id = Guid.NewGuid() };
        var createProjectDto = new CreateProjectDto { Name = "New Project", Description = "Description" };

        var loggerMock = new Mock<ILogger<ProjectService>>();
        var mockCache = new Mock<ICacheService>();
        using (var context = new AppDbContext(options))
        {
            context.Users.Add(appUser);
            context.SaveChanges();
        }

        using (var context = new AppDbContext(options))
        {
            var service = new ProjectService(context, loggerMock.Object, mockCache.Object);

            mockCache
                .Setup(x => x.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await service.CreateProject(createProjectDto, appUser);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createProjectDto.Name, result.Name);
            mockCache.Verify(x => x.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

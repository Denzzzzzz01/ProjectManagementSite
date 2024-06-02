using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ProjectManagement.Application.Contracts.Project;
using ProjectManagement.Application.Services;
using ProjectManagement.Core.Models;
using ProjectManagement.Infrastructure.Persistence;

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
            var service = new ProjectService(context, mockLogger.Object);

             
            // Act
            var projects = await service.GetUserProjects(appUser);

            // Assert
            Assert.Equal(2, projects.Count);
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
            var service = new ProjectService(context, mockLogger.Object);

            // Act
            var project = await service.GetProjectById(projectId, appUser);

            // Assert
            Assert.NotNull(project);
            Assert.Equal(projectId, project.Id);
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
        var createProjectDto = new CreateProjectDto { Name = "New Project" };

        var loggerMock = new Mock<ILogger<ProjectService>>();
        using (var context = new AppDbContext(options))
        {
            context.Users.Add(appUser);
            context.SaveChanges();
        }

        using (var context = new AppDbContext(options))
        {
            var service = new ProjectService(context, loggerMock.Object);

            // Act
            var result = await service.CreateProject(createProjectDto, appUser);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createProjectDto.Name, result.Name);
        }
    }
}
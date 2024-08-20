using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ProjectManagement.Application.Common.Exceptions;
using ProjectManagement.Application.Contracts.Task;
using ProjectManagement.Core.Models;
using ProjectManagement.Infrastructure.Persistence;

namespace Application.Tests;

public class TaskServiceTests
{
    private readonly Mock<ICacheService> _cacheMock;

    public TaskServiceTests()
    {
        _cacheMock = new Mock<ICacheService>();
    }

    [Fact]
    public async Task GetProjectTasks_ShouldReturnTasks()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        var appUser = new AppUser { Id = Guid.NewGuid() };
        var projectId = Guid.NewGuid();
        var project = new Project
        {
            Id = projectId,
            Name = "Project",
            OwnerId = appUser.Id,
            Tasks = new List<ProjectTask>
            {
                new ProjectTask { Id = Guid.NewGuid(), ProjectId = projectId, Title = "Task 1" },
                new ProjectTask { Id = Guid.NewGuid(), ProjectId = projectId, Title = "Task 2" }
            }
        };

        using (var context = new AppDbContext(options))
        {
            context.Users.Add(appUser);
            context.Projects.Add(project);
            context.AppUserProjects.Add(new AppUserProject { AppUserId = appUser.Id, ProjectId = projectId });
            context.SaveChanges();
        }

        var loggerMock = new Mock<ILogger<TaskService>>();
        using (var context = new AppDbContext(options))
        {
            var service = new TaskService(context, loggerMock.Object, _cacheMock.Object);

            // Act
            var result = await service.GetProjectTasks(projectId, appUser);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Task 1", result[0].Title);
            Assert.Equal("Task 2", result[1].Title);
        }
    }

    [Fact]
    public async Task AddTask_ShouldAddTask_WhenProjectExists()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        var appUser = new AppUser { Id = Guid.NewGuid() };
        var projectId = Guid.NewGuid();
        var projectName = "Project";
        var addTaskDto = new AddTaskDto { ProjectId = projectId, Title = "New Task", Description = "Description" };

        using (var context = new AppDbContext(options))
        {
            context.Users.Add(appUser);
            context.Projects.Add(new Project { Id = projectId, OwnerId = appUser.Id, Name = projectName });
            context.AppUserProjects.Add(new AppUserProject { AppUserId = appUser.Id, ProjectId = projectId });
            context.SaveChanges();
        }

        var loggerMock = new Mock<ILogger<TaskService>>();
        using (var context = new AppDbContext(options))
        {
            var service = new TaskService(context, loggerMock.Object, _cacheMock.Object);

            // Act
            var result = await service.AddTask(addTaskDto, appUser);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(addTaskDto.Title, result.Title);

            var addedTask = await context.Tasks.FindAsync(result.Id);
            Assert.NotNull(addedTask);
            Assert.Equal(addTaskDto.Title, addedTask.Title);
        }
    }

    [Fact]
    public async Task AddTask_ShouldThrowNotFoundException_WhenProjectDoesNotExist()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        var appUserId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var addTaskDto = new AddTaskDto { ProjectId = projectId, Title = "New Task" };

        var loggerMock = new Mock<ILogger<TaskService>>();
        using (var context = new AppDbContext(options))
        {
            // Add user to database
            context.Users.Add(new AppUser { Id = appUserId });

            context.SaveChanges();

            var service = new TaskService(context, loggerMock.Object, _cacheMock.Object);

            // Act and Assert
            await Assert.ThrowsAsync<NotFoundException>(() => service.AddTask(addTaskDto, new AppUser { Id = appUserId }));
        }
    }

    [Fact]
    public async Task AddTask_ShouldThrowNotFoundException_WhenUserIsNotMemberOfProject()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        var appUser = new AppUser { Id = Guid.NewGuid() };
        var projectId = Guid.NewGuid();
        var projectName = "Project";
        var addTaskDto = new AddTaskDto { ProjectId = projectId, Title = "New Task" };

        using (var context = new AppDbContext(options))
        {
            context.Users.Add(appUser);
            context.Projects.Add(new Project { Id = projectId, Name = projectName });
            context.SaveChanges();
        }

        var loggerMock = new Mock<ILogger<TaskService>>();
        using (var context = new AppDbContext(options))
        {
            var service = new TaskService(context, loggerMock.Object, _cacheMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => service.AddTask(addTaskDto, appUser));
        }
    }
}

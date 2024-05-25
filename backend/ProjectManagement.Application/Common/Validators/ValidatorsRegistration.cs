using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ProjectManagement.Application.Contracts.Account;
using ProjectManagement.Application.Contracts.Project;
using ProjectManagement.Application.Contracts.Task;

namespace ProjectManagement.Application.Common.Validators;

public static class ValidatorsRegistration
{
    public static void AddValidators(this IServiceCollection services)
    {
        services.AddTransient<IValidator<AddTaskDto>, AddTaskDtoValidator>();
        services.AddTransient<IValidator<UpdateTaskDto>, UpdateTaskDtoValidator>();
        services.AddTransient<IValidator<RemoveTaskDto>, RemoveTaskDtoValidator>();
        services.AddTransient<IValidator<DoTaskDto>, DoTaskDtoValidator>();
        services.AddTransient<IValidator<UpdateProjectStatusDto>, UpdateProjectStatusDtoValidator>();
        services.AddTransient<IValidator<UpdateProjectDto>, UpdateProjectDtoValidator>();
        services.AddTransient<IValidator<CreateProjectDto>, CreateProjectDtoValidator>();
        services.AddTransient<IValidator<RegisterDto>, RegisterDtoValidator>();
        services.AddTransient<IValidator<LoginDto>, LoginDtoValidator>();
    }
}

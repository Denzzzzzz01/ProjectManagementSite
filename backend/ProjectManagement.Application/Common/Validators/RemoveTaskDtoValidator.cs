using FluentValidation;
using ProjectManagement.Application.Contracts.Task;

public class RemoveTaskDtoValidator : AbstractValidator<RemoveTaskDto>
{
    public RemoveTaskDtoValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("Project ID is required.");

        RuleFor(x => x.TaskId)
            .NotEmpty().WithMessage("Task ID is required.");
    }
}

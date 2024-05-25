using FluentValidation;
using ProjectManagement.Application.Contracts.Task;

public class DoTaskDtoValidator : AbstractValidator<DoTaskDto>
{
    public DoTaskDtoValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("Project ID is required.");

        RuleFor(x => x.TaskId)
            .NotEmpty().WithMessage("Task ID is required.");

        RuleFor(x => x.IsDone)
            .NotNull().WithMessage("IsDone status is required.");
    }
}

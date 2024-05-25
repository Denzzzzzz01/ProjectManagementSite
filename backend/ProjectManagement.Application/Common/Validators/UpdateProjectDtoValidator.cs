using FluentValidation;
using ProjectManagement.Application.Contracts.Project;

public class UpdateProjectDtoValidator : AbstractValidator<UpdateProjectDto>
{
    public UpdateProjectDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Project ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(3).WithMessage("Name must be at least 3 character long.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
    }
}

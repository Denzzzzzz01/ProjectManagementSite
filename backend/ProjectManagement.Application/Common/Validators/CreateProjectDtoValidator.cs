using FluentValidation;
using ProjectManagement.Application.Contracts.Project;

public class CreateProjectDtoValidator : AbstractValidator<CreateProjectDto>
{
    public CreateProjectDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(3).WithMessage("Name must be at least 3 character long.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
    }
}

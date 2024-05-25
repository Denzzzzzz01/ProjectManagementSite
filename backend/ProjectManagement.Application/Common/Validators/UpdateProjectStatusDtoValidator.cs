using FluentValidation;
using ProjectManagement.Application.Contracts.Project;

public class UpdateProjectStatusDtoValidator : AbstractValidator<UpdateProjectStatusDto>
{
    public UpdateProjectStatusDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Project ID is required.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid status value.");
    }
}

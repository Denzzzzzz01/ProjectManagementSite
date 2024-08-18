using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Application.Contracts.Project;

public class CreateProjectDto
{
    [Required(ErrorMessage = "Project name is required.")]
    [StringLength(36, MinimumLength = 3, ErrorMessage = "Project name must be between 3 and 36 characters.")]
    public string Name { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string Description { get; set; }
}

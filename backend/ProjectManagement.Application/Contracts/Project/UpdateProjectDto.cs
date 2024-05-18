using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Application.Contracts.Project;

public class UpdateProjectDto
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public string Name { get; set; }
}

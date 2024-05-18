using ProjectManagement.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Application.Contracts.Project;

public class UpdateProjectStatusDto
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public Status Status { get; set; }
}

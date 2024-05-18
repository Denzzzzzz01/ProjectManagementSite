using ProjectManagement.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Application.Contracts.Project;

public class CreateProjectDto
{
    [Required]
    public string Name { get; set; }
}

using ProjectManagement.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Application.Contracts.Task;

public class AddTaskDto
{
    [Required]
    public Guid ProjectId { get; set; }

    [Required]
    public Guid TaskId { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public Priority Priority { get; set; }
}

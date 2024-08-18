using ProjectManagement.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Application.Contracts.Task;

public class UpdateTaskDto
{
    [Required(ErrorMessage = "Project ID is required.")]
    public Guid ProjectId { get; set; }

    [Required(ErrorMessage = "Task ID is required.")]
    public Guid TaskId { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 100 characters.")]
    public string Title { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Priority is required.")]
    public Priority Priority { get; set; }
}

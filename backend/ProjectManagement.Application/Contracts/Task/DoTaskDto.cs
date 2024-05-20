using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Application.Contracts.Task;

public class DoTaskDto
{
    [Required]
    public Guid ProjectId { get; set; }
    [Required]
    public Guid TaskId { get; set; }
    [Required]
    public bool IsDone { get; set; }
}

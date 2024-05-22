using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Application.Contracts.Task;

public class RemoveTaskDto
{
    [Required]
    public Guid ProjectId { get; set; }
    [Required]
    public Guid TaskId { get; set; }    
}

using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Application.Contracts.Task;

public class DeleteTaskDto
{
    [Required]
    public Guid ProjectId { get; set; }
    [Required]
    public Guid TaskId { get; set; }    
}

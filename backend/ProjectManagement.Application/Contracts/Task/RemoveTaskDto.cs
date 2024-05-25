
namespace ProjectManagement.Application.Contracts.Task;

public class RemoveTaskDto
{
    public Guid ProjectId { get; set; }
    public Guid TaskId { get; set; }    
}

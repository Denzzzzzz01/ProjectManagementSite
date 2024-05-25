using ProjectManagement.Core.Enums;

namespace ProjectManagement.Application.Contracts.Task;

public class UpdateTaskDto
{
    public Guid ProjectId { get; set; }
    public Guid TaskId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public Priority Priority { get; set; }
}

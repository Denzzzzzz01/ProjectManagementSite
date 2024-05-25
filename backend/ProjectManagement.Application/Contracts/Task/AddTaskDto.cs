using ProjectManagement.Core.Enums;

namespace ProjectManagement.Application.Contracts.Task;

public class AddTaskDto
{
    public Guid ProjectId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public Priority Priority { get; set; }
}


using ProjectManagement.Core.Enums;

namespace ProjectManagement.Application.Contracts.Task;

public class TaskVm
{
    public Guid Id { get; set; }
    public DateTime AddedTime { get; set; }
    public bool IsDone { get; set; }
    public Priority Priority { get; set; }
    public string Title { get; set; }
}

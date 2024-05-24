using ProjectManagement.Core.Enums;

namespace ProjectManagement.Application.Contracts.Task;

public class TaskDetailedVm
{
    public Guid Id { get; set; }
    //public Guid AssignedTo { get; set; }
    public DateTime AddedTime { get; set; }
    //public DateTime? DoneTime { get; set; }
    //public DateTime? DueDate { get; set; }
    public bool IsDone { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public Priority Priority { get; set; }
}

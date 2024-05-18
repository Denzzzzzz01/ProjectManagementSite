using ProjectManagement.Core.Enums;

namespace ProjectManagement.Core.Models;

public class ProjectTask
{
    public Guid Id { get; set; } = Guid.NewGuid();
    //public Guid ProjectId { get; set; }
    public Project Project { get; set; }
    public Guid AssignedTo { get; set; }
    public DateTime AddedTime {  get; set; } = DateTime.UtcNow;
    public DateTime? DoneTime { get; set; } = null;
    public DateTime? DueDate {  get; set; } = null;
        
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsDone { get; set; } = false;
    public Priority priority { get; set; } = Priority.Medium;
}

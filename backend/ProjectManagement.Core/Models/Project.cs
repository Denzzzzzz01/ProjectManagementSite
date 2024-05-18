using ProjectManagement.Core.Enums;

namespace ProjectManagement.Core.Models;

public class Project
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OwnerId { get; set; }
    public List<AppUserProject> AppUserProjects { get; set; } = new List<AppUserProject>();

    public DateTime CreatedTime {  get; set; } = DateTime.UtcNow;  
    
    public string Name { get; set; } = string.Empty;
    public List<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
    public Status Status { get; set; } = Status.InProgress;
}

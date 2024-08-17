using ProjectManagement.Core.Enums;

namespace ProjectManagement.Application.Contracts.Project;

public class ProjectVm
{
    public Guid Id { get; set; }
    public DateTime CreatedTime { get; set; }
    public string Name { get; set; }
    public Status Status { get; set; }
    public string Description { get; set; }
}

using ProjectManagement.Core.Enums;

namespace ProjectManagement.Application.Contracts.Project;

public class UpdateProjectStatusDto
{
    public Guid Id { get; set; }
    public Status Status { get; set; }
}

namespace ProjectManagement.Application.Contracts.Project;

public class UpdateProjectDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string LogoUrl { get; set; }
}

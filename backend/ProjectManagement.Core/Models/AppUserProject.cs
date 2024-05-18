namespace ProjectManagement.Core.Models;

public class AppUserProject
{
    public Guid AppUserId { get; set; }
    public Guid ProjectId { get; set; }

    public AppUser User { get; set; } = null!;
    public Project Project { get; set; } = null!;
}

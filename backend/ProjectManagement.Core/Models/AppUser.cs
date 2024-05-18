using Microsoft.AspNetCore.Identity;

namespace ProjectManagement.Core.Models;

public class AppUser : IdentityUser<Guid>
{
    //public List<Project> Projects { get; set; } = new List<Project>();
    public List<AppUserProject> AppUserProjects { get; set; } = new List<AppUserProject>();
}

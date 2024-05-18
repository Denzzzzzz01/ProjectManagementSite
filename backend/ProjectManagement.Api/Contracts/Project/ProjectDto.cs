using ProjectManagement.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Api.Contracts.Project
{
    public class ProjectDto
    {
        [Required]
        public string Name { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Api.Contracts.Account;

public class LoginDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}

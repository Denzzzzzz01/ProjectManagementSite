using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Application.Contracts.Account;

public class LoginDto
{
    [EmailAddress]
    public string Email { get; set; }
    public string Password { get; set; }
}

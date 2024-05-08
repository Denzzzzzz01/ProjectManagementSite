using ProjectManagement.Core.Models;

namespace ProjectManagement.Application.Interfaces;

public interface ITokenService 
{

    string CreateToken(AppUser appUser);
}

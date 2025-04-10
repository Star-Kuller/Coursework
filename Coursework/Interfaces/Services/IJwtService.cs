using System.Security.Claims;

namespace Coursework.Interfaces.Services;

public interface IJwtService
{
    string GenerateToken(long userId, string userName, string roleName);
    ClaimsPrincipal? ValidateToken(string token);
}
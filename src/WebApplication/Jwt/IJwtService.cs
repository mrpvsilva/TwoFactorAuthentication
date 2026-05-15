using WebApplication.Entities;

namespace WebApplication.Jwt
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}

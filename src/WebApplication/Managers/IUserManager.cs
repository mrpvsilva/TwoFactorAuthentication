using System.Threading.Tasks;
using WebApplication.Entities;

namespace WebApplication.Managers
{
    public interface IUserManager
    {
        Task<User> AddUserAsync(User user);
    }
}

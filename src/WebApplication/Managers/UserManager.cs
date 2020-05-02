using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Entities;

namespace WebApplication.Managers
{
    public class UserManager : IUserManager
    {
        private readonly TFAContext _ctx;

        public UserManager(TFAContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<User> AddUserAsync(User user)
        {


            user.HashPasword();
            await _ctx.AddAsync(user);
            await _ctx.SaveChangesAsync();

            return user;
        }



    }
}

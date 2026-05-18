using System.Threading.Tasks;

namespace WebApplication.Services
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string body);
    }
}

using System.Threading.Tasks;
using Gallery.Data.Models;

namespace Gallery.Infrastructure.Services
{
    public interface IUserService : IService
    {
        Task<User> GetUserAsync(int userId);
        Task<User> FindUserAsync(string username, string password);
        Task LoginAsync(string username, string password);
        Task<User> RegisterAsync(string username, string email, string password, int[] roles);
        Task UpdateUserLastActivityAsync(int userId);        
    }
}

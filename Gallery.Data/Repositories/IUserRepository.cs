using System.Collections.Generic;
using System.Threading.Tasks;
using Gallery.Data.Models;

namespace Gallery.Data.Repositories
{
    public interface IUserRepository : IEntityBaseRepository<User>
    {
        User GetSingleByUsername(string username);
        Task<User> GetSingleByUsernameAsync(string username);
        Task<IEnumerable<Role>> GetUserRolesAsync(string username);
    }
}

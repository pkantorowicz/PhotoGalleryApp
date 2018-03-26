using System.Collections.Generic;
using System.Threading.Tasks;
using Gallery.Data.Models;

namespace Gallery.Infrastructure.Services
{
    public interface IRoleService : IService
    {
        Task<List<Role>> FindUserRolesAsync(int userId);
        Task<IEnumerable<User>> FindUsersInRoleAsync(string roleName);
        Task<bool> IsUserInRole(int userId, string roleName);
    }
}

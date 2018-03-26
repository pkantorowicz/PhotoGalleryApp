using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gallery.Data.Models;
using Gallery.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gallery.Infrastructure.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public RoleService(IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        public async Task<List<Role>> FindUserRolesAsync(int userId)
        {
            var query = from role in _roleRepository.QueryAll()
                        from userRoles in role.UserRoles
                        where userRoles.UserId == userId
                        select role;

            return await query.OrderBy(x => x.Name).ToListAsync();
        }

        public async Task<IEnumerable<User>> FindUsersInRoleAsync(string roleName)
        {
            var query = from role in _roleRepository.QueryAll()
                        where role.Name == roleName
                        from user in role.UserRoles
                        select user.UserId;

            var userRole = await _userRepository.QueryAll().Where(user => query.Contains(user.Id)).ToListAsync();
            return userRole;
        }

        public async Task<bool> IsUserInRole(int userId, string roleName)
        {
            var query = from role in _roleRepository.QueryAll()
                        where role.Name == roleName
                        from user in role.UserRoles
                        where user.UserId == userId
                        select role;

            var userRole = await  query.FirstOrDefaultAsync();
            return userRole != null;
        }
    }
}

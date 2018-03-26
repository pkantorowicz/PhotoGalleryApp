using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gallery.Data.Models;
using Gallery.Data.Repositories;
using Gallery.Infrastructure.EF;

namespace Gallery.Infrastructure.Repositories
{
    public class UserRepository : EntityBaseRepository<User>, IUserRepository, ISqlRepository
    {
        private readonly IRoleRepository _roleReposistory;

        public UserRepository(GalleryContext context, IRoleRepository roleReposistory)
            : base(context)
        {
            _roleReposistory = roleReposistory;
        }

        public User GetSingleByUsername(string username)
            => GetSingle(x => x.Username == username);

        public async Task<User> GetSingleByUsernameAsync(string username)       
             => await GetSingleAsync(x => x.Username == username);
        
        public async Task<IEnumerable<Role>> GetUserRolesAsync(string username)
        {
            var roles = new List<Role>();

            var user = await GetSingleAsync(u => u.Username == username, u => u.UserRoles);
            if(user != null)
            {
                roles = user.UserRoles.Select(userRole => 
                    _roleReposistory.GetSingle(userRole.RoleId)).ToList();
            }

            return roles;
        }
    }
}

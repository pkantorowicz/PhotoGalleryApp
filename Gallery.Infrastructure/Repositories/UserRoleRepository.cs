using Gallery.Data.Models;
using Gallery.Data.Repositories;
using Gallery.Infrastructure.EF;

namespace Gallery.Infrastructure.Repositories
{
    public class UserRoleRepository : EntityBaseRepository<UserRole>, IUserRoleRepository, ISqlRepository
    {
        public UserRoleRepository(GalleryContext context)
            : base(context)
        { }
    }
}

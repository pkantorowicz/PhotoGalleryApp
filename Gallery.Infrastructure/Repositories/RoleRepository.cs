using Gallery.Data.Models;
using Gallery.Data.Repositories;
using Gallery.Infrastructure.EF;

namespace Gallery.Infrastructure.Repositories
{
    public class RoleRepository : EntityBaseRepository<Role>, IRoleRepository, ISqlRepository
    {
        public RoleRepository(GalleryContext context)
            : base(context)
        { }
    }
}

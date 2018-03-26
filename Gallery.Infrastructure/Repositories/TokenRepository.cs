using Gallery.Data.Models;
using Gallery.Data.Repositories;
using Gallery.Infrastructure.EF;

namespace Gallery.Infrastructure.Repositories
{
    public class TokenRepository : EntityBaseRepository<UserToken>, ITokenRepository, ISqlRepository
    {
        public TokenRepository(GalleryContext context)
            : base(context)
        { }
    }
}

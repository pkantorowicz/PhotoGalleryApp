using Gallery.Data.Models;
using Gallery.Data.Repositories;
using Gallery.Infrastructure.EF;

namespace Gallery.Infrastructure.Repositories
{
    public class AlbumRepository : EntityBaseRepository<Album>, IAlbumRepository, ISqlRepository
    {
        public AlbumRepository(GalleryContext context)
            : base(context)
        { }
    }
}

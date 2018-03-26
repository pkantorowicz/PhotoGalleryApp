using Gallery.Data.Models;
using Gallery.Data.Repositories;
using Gallery.Infrastructure.EF;

namespace Gallery.Infrastructure.Repositories
{
    public class PhotoRepository : EntityBaseRepository<Photo>, IPhotoRepository, ISqlRepository
    {
        public PhotoRepository(GalleryContext context)
            : base(context)
        { }
    }
}

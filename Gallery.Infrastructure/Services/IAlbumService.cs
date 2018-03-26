using System.Collections.Generic;
using System.Threading.Tasks;
using Gallery.Infrastructure.DTO;

namespace Gallery.Infrastructure.Services
{
    public interface IAlbumService : IService
    {
        Task<IEnumerable<AlbumDto>> GetAlbumAsync();
        Task<IEnumerable<PhotoDto>> GetPhotosFromAlbumAsync(int id);
    }
}

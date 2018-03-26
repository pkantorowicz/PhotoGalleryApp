using System.Collections.Generic;
using System.Threading.Tasks;
using Gallery.Infrastructure.DTO;

namespace Gallery.Infrastructure.Services
{
    public interface IPhotosService : IService
    {
        Task<IEnumerable<PhotoDto>> QueryAllAsync();
        Task<IEnumerable<PhotoDto>> QueryIncluding();
        Task Delete(int id);
    }
}

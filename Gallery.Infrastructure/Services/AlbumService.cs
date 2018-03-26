using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Gallery.Data.Models;
using Gallery.Data.Repositories;
using Gallery.Infrastructure.DTO;

namespace Gallery.Infrastructure.Services
{
    public class AlbumService : IAlbumService
    {
        private readonly IAlbumRepository _albumRepository;
        private readonly IMapper _mapper;

        public AlbumService(IAlbumRepository albumRepository, IMapper mapper)
        {
            _albumRepository = albumRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AlbumDto>> GetAlbumAsync()
        {
            var albums = await _albumRepository.AllIncludingAsync(a => a.Photos);

            var albumsMap = _mapper.Map<IEnumerable<Album>,IEnumerable<AlbumDto>>(albums);

            return albumsMap;
        }

        public async Task<IEnumerable<PhotoDto>> GetPhotosFromAlbumAsync(int id)
        {
            var album = await _albumRepository.GetSingleAsync(a => a.Id == id, a => a.Photos);
            var photos = album.Photos.OrderBy(p => p.Id).ToList();

            var photoMap = _mapper.Map<IEnumerable<Photo>, IEnumerable<PhotoDto>>(photos);

            return photoMap;
        }
    }
}

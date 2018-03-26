using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Gallery.Data.Models;
using Gallery.Data.Repositories;
using Gallery.Infrastructure.DTO;

namespace Gallery.Infrastructure.Services
{
    public class PhotosService : IPhotosService
    {
        private readonly IPhotoRepository _photoRepository;
        private readonly IMapper _mapper;

        public PhotosService(IPhotoRepository photoRepository, IMapper mapper)
        {
            _photoRepository = photoRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PhotoDto>> QueryAllAsync()
        {
            var photos = await _photoRepository.GetAllAsync();

            return _mapper.Map<IEnumerable<Photo>, IEnumerable<PhotoDto>>(photos);     
        }

        public async Task<IEnumerable<PhotoDto>> QueryIncluding()
        {
            var photos = await _photoRepository.AllIncludingAsync(p => p.Album);

            return _mapper.Map<IEnumerable<Photo>, IEnumerable<PhotoDto>>(photos);
        }

        public async Task Add(Photo photo)
        {
            var nPhoto = await _photoRepository.GetSingleAsync(x => x.Id == photo.Id);

            if (nPhoto != null)
            {
                throw new Exception($"Photo with this title: {photo.Title} already exists.");
            }

            nPhoto = new Photo
            {
                Id = photo.Id,
                Uri = photo.Uri,
                AlbumId = photo.AlbumId,
                Title = photo.Title
            };

            await _photoRepository.AddAsync(nPhoto);
            await _photoRepository.CommitAsync();
        }

        public async Task Delete(int id)
        {
            var photoToRemove = await _photoRepository.GetSingleAsync(id);

            _photoRepository.Delete(photoToRemove);
            await _photoRepository.CommitAsync();
        }
    }
}

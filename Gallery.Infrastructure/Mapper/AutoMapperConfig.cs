using AutoMapper;
using Gallery.Data.Models;
using Gallery.Infrastructure.DTO;
using System.Linq;

namespace Gallery.Infrastructure.Mapper
{
    public static class AutoMapperConfig
    {
        public static IMapper Initialize()
            => new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Photo, PhotoDto>()
                        .ForMember(vm => vm.Uri, map => map.MapFrom(p => "/images/" + p.Uri));

                    cfg.CreateMap<Album, AlbumDto>()
                        .ForMember(vm => vm.TotalPhotos, map => map.MapFrom(a => a.Photos.Count))
                        .ForMember(vm => vm.Thumbnail, map =>
                            map.MapFrom(a => (a.Photos != null && a.Photos.Count > 0) ?
                                "/images/" + a.Photos.First().Uri :
                                "/images/thumbnail-default.png"));
                })
                .CreateMapper();
    }
}
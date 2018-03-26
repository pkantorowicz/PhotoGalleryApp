using System.Threading.Tasks;
using Gallery.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gallery.App.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class AlbumsController : ApiControllerBase
    {
        private readonly IAlbumService _albumService;

        public AlbumsController(IAlbumService albumService)
        {
            _albumService = albumService;
        }

        [HttpGet]
        [Route("GetAllPageable")]
        public async Task<IActionResult> Get(int page = 1, int pageSize = 10)
        {
            var album = await _albumService.GetAlbumAsync();

            var pagedPhotos = CreatePagedResults(album, page - 1, pageSize);

            return Json(pagedPhotos);
        }

        [HttpGet]
        [Route("GetPhotosInAlbum/{id}")]
        public async Task<IActionResult> Get(int id, int page = 1, int pageSize = 10)
        {
            var photomFromAlbum = await _albumService.GetPhotosFromAlbumAsync(id);

            var pagedPhotos = CreatePagedResults(photomFromAlbum, page - 1, pageSize);

            return Json(pagedPhotos);
        }
    }
}



using System.Threading.Tasks;
using Gallery.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gallery.App.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class PhotosController : ApiControllerBase
    {
        private readonly IPhotosService _photosService;

        public PhotosController(IPhotosService photosService)
        {
            _photosService = photosService;
        }

        [HttpGet]
        [Route("GetAllPageable")]
        public async Task<IActionResult> Get(int page = 1, int pageSize = 10)
        {
            var photos = await _photosService.QueryIncluding();

            var pagedPhotos = CreatePagedResults(photos, page - 1, pageSize);

            return Json(pagedPhotos);
        }

        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _photosService.Delete(id);

            return NoContent();
        }
    }
}

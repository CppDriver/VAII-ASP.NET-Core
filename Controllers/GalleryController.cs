using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MultimediaLibrary.Models.TransferModels;
using MultimediaLibrary.Services.GalleryService;

namespace MultimediaLibrary.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GalleryController : ControllerBase
    {
        private readonly IGalleryService _galleryService;

        public GalleryController(IGalleryService galleryService)
        {
            _galleryService = galleryService;
        }

        [HttpGet("getThumbnails/{galleryId}/{noThumbnail}")]
        public async Task<IActionResult> GetThumbnails(ulong galleryId, int noThumbnail)
        {
            var images = await _galleryService.GetThumbnails(galleryId, noThumbnail);
            if (images == null)
            {
                return NotFound();
            }
            return File(images, "image/jpeg");
        }

        [HttpGet("getGallery/{galleryId}")]
        public async Task<IActionResult> GetGallery(ulong galleryId)
        {
            var images = await _galleryService.GetGallery(galleryId);
            if (images == null)
            {
                return NotFound();
            }
            return Ok(images);
        }

        [HttpGet("getGalleryDataFull/{galleryId}")]
        public async Task<IActionResult> GetGalleryDataFull(ulong galleryId)
        {
            var images = await _galleryService.GetGalleryDataFull(galleryId);
            if (images == null)
            {
                return NotFound();
            }
            return Ok(images);
        }

        [HttpPost("createGallery")]
        public async Task<IActionResult> CreateGallery([FromBody] CreateGalleryData data)
        {
            var result = await _galleryService.CreateGallery(data);
            if (!result.Equals("Success"))
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok(result);
        }

        [HttpGet("getGalleries/{username}")]
        public async Task<IActionResult> GetUserGalleries(string username)
        {
            var result = await _galleryService.GetUserGalleries(username);
            if (result == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok(result);
        }

        [HttpPut("updateGallery")]
        public async Task<IActionResult> UpdateGallerry([FromBody] GalleryUpdateData media)
        {
            var result = await _galleryService.UpdateGallery(media);
            if (result.Equals("Media updated"))
                return Ok();
            return NotFound();
        }

        [HttpDelete("deleteGallery")]
        public async Task<IActionResult> DeleteGallery([FromQuery] ulong galleryId)
        {
            var result = await _galleryService.DeleteGallery(galleryId);
            if (result.Equals("Gallery deleted"))
                return Ok();
            return NotFound();
        }
    }
}

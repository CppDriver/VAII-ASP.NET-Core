using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using MultimediaLibrary.Models;
using MultimediaLibrary.Models.TransferModels;
using MultimediaLibrary.Services.ImageService;

namespace MultimediaLibrary.Controllers;

[ApiController]
[Route("[controller]")]
public class MediaController : ControllerBase
{
    private readonly IMediaService _mediaService;

    public MediaController(IMediaService mediaService)
    {
        _mediaService = mediaService;
    }

    [HttpGet("GetImages")]
    public async Task<List<MediaDataShort>> GetImagesList()
    {
        var images = await _mediaService.GetImages();
        return images;
    }

    [HttpGet("GetUserImages/{username}")]
    public async Task<List<MediaDataShort>> GetUserImages(string username)
    {
        var images = await _mediaService.GetUserImages(username);
        return images;
    }

    [HttpGet("GetGalleryImages/{galleryId}")]
    public async Task<List<MediaDataShort>> GetGalleryImages(string galleryId)
    {
        var images = await _mediaService.GetGalleryImages(galleryId);
        return images;
    }

    [HttpGet("GetMediaInfo/{mediaUuid}")]
    public async Task<MediaDataFull> GetMediaInfo(string mediaUuid)
    {
        var media = await _mediaService.GetMediaInfo(mediaUuid);
        return media;
    }

    [HttpGet("GetImage/{imageUuid}/{size}")]
    public async Task<IActionResult> GetImage(string imageUuid, string size)
    {
        var result = await _mediaService.GetImage(imageUuid, size);
        if (result == null)
        {
            return NotFound();
        }
        return File(result, "image/jpeg");
    }

    [HttpDelete("deleteMedia")]
    public async Task<IActionResult> DeleteMedia([FromQuery] string uuid)
    {
        var result = await _mediaService.DeleteMedia(uuid);
        if (result.Equals("Media deleted"))
            return Ok();
        return NotFound();
    }

    [HttpPut("updateMedia")]
    public async Task<IActionResult> UpdateMedia([FromBody] MediaDataFull media)
    {
        var result = await _mediaService.UpdateMedia(media);
        if (result.Equals("Media updated"))
            return Ok();
        return NotFound();
    }

    [HttpPut("addToGallery")]
    public async Task<IActionResult> AddToGallery([FromQuery] string galleryId, [FromQuery] string mediaUuid)
    {
        var result = await _mediaService.AddToGallery(galleryId, mediaUuid);
        if (result.Equals("Success"))
            return Ok();
        return StatusCode(StatusCodes.Status400BadRequest);
    }
}

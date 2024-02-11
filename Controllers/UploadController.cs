using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MultimediaLibrary.Models.TransferModels;
using MultimediaLibrary.Services.UploadService;

namespace MultimediaLibrary.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly IUploadService _uploadService;

        public UploadController(IUploadService uploadService)
        {
            _uploadService = uploadService;
        }

        [HttpPost("uploadFile")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            await _uploadService.UploadFile(file);
            return Ok();
        }

        [HttpPost("cancelUpload")]
        public async Task<IActionResult> CancelUpload([FromBody] CancelUploadData data)
        {
            await _uploadService.CancelUpload(data.Uuid);
            return Ok();
        }

        [HttpPost("saveUpload")]
        public async Task<IActionResult> SaveUpload([FromBody] SaveUploadData data)
        {
            var time = DateTime.Now;
            await _uploadService.SaveUpload(data);
            Console.WriteLine($"SaveUpload: {DateTime.Now - time}");
            return Ok();
        }
    }
}

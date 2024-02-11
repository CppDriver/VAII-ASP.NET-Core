using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MultimediaLibrary.Models.TransferModels;
using MultimediaLibrary.Services.CommentService;

namespace MultimediaLibrary.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost]
        [Route("postComment")]
        public async Task<IActionResult> PostComment([FromBody] CommentData comment)
        {
            var res = await _commentService.postComment(comment);
            if (res == null)
                return StatusCode(StatusCodes.Status500InternalServerError);
            return Ok(res);
        }

        [HttpGet]
        [Route("getComments")]
        public IActionResult GetComments([FromQuery] string? mediaUuid, [FromQuery] ulong? galleryId)
        {
            var comments = _commentService.getComments(mediaUuid, galleryId);
            if (comments == null)
                return StatusCode(StatusCodes.Status500InternalServerError);
            return Ok(comments);
        }
    }
}

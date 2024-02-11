using Microsoft.EntityFrameworkCore;
using MultimediaLibrary.Data;
using MultimediaLibrary.Models;
using MultimediaLibrary.Models.TransferModels;
using MultimediaLibrary.Services.AuthService;

namespace MultimediaLibrary.Services.CommentService
{
    public class CommentService : ICommentService
    {
        private readonly DatabaseContext _db;
        private readonly IAuthService _authService;

        public CommentService(DatabaseContext db, IAuthService authService)
        {
            _db = db;
            _authService = authService;
        }

        public async Task<CommentData?> postComment(CommentData comment)
        {
            var userid = _authService.GetCurrentUserId();
            if (userid == null)
                return null;

            var newComment = new Comment
            {
                UserId = (ulong)userid,
                MediaUuid = comment.MediaUuid,
                GalleryId = comment.GalleryId,
                Text = comment.Text,
                DateCreated = DateTime.Now,
            };
            try
            {
                _db.Comments.Add(newComment);
                _db.SaveChanges();

            }
            catch (Exception e)
            {
                return null;
            }

            return new CommentData
            {
                CommentId = newComment.CommentId,
                UserId = newComment.UserId,
                Username = _db.Users.Find(newComment.UserId).Username,
                MediaUuid = newComment.MediaUuid,
                GalleryId = newComment.GalleryId,
                Text = newComment.Text,
                DateCreated = newComment.DateCreated
            };
        }

        public async Task<List<CommentData>> getComments(string? mediaUuid, ulong? galleryId)
        {
            List<Comment> comments;
            if (mediaUuid != null && mediaUuid.Length > 0)
                comments = _db.Comments.Include(c => c.User).Where(c => c.MediaUuid == mediaUuid).ToList();
            else if (galleryId != null)
                comments = _db.Comments.Include(c => c.User).Where(c => c.GalleryId == galleryId).ToList();
            else
                comments = new List<Comment>();
            return comments.Select(c => new CommentData
            {
                CommentId = c.CommentId,
                UserId = c.UserId,
                Username = c.User.Username,
                MediaUuid = c.MediaUuid,
                GalleryId = c.GalleryId,
                Text = c.Text,
                DateCreated = c.DateCreated
            }).ToList();
        }
    }
}

using MultimediaLibrary.Models.TransferModels;
using System.Runtime.CompilerServices;

namespace MultimediaLibrary.Services.CommentService
{
    public interface ICommentService
    {
        Task<CommentData> postComment(CommentData comment);
        Task<List<CommentData>> getComments(string? mediaUuid, ulong? galleryId);
    }
}

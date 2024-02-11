
using MultimediaLibrary.Models.TransferModels;

namespace MultimediaLibrary.Services.UploadService
{
    public interface IUploadService
    {
        Task<string> UploadFile(IFormFile file);
        Task<string> CancelUpload(string uuid);
        Task<string> SaveUpload(SaveUploadData data);
    }
}

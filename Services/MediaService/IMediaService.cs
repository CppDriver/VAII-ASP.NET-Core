using Microsoft.AspNetCore.Mvc;
using MultimediaLibrary.Models;
using MultimediaLibrary.Models.TransferModels;

namespace MultimediaLibrary.Services.ImageService
{
    public interface IMediaService
    {
        Task<List<MediaDataShort>> GetImages();
        Task<List<MediaDataShort>> GetUserImages(string username);
        Task<List<MediaDataShort>> GetGalleryImages(string galleryId);
        Task<byte[]?> GetImage(string imageUuid, string size);
        Task<MediaDataFull> GetMediaInfo(string mediaUuid);
        Task<string> DeleteMedia(string uuid);
        Task<string> UpdateMedia(MediaDataFull mediaData);
        Task<string> AddToGallery(string galleryId, string mediaUuid);
    }
}

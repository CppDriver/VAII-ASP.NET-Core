using Microsoft.AspNetCore.Mvc;
using MultimediaLibrary.Models.TransferModels;

namespace MultimediaLibrary.Services.GalleryService
{
    public interface IGalleryService
    {
        Task<byte[]?> GetThumbnails(ulong galleryId, int noThumbnail); 
        Task<List<MediaDataShort>> GetGallery(ulong galleryId);
        Task<string> CreateGallery(CreateGalleryData data);
        Task<List<GalleryDataShort>> GetUserGalleries(string username);
        Task<GalleryDataFull?> GetGalleryDataFull(ulong galleryId);
        Task<bool> UpdateGallery(GalleryUpdateData data);
        Task<bool> DeleteGallery(ulong galleryId);

    }
}

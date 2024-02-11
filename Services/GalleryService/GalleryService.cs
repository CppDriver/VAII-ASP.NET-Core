
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MultimediaLibrary.Data;
using MultimediaLibrary.Models;
using MultimediaLibrary.Models.TransferModels;
using MultimediaLibrary.Services.AuthService;
using MultimediaLibrary.Services.ImageService;
using System.Runtime;

namespace MultimediaLibrary.Services.GalleryService
{
    public class GalleryService : IGalleryService
    {
        private readonly DatabaseContext _db;
        private readonly MultimediaLibrarySettings _settings;
        private readonly IMediaService _mediaService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthService _authService;

        public GalleryService(DatabaseContext db, IOptions<MultimediaLibrarySettings> settings, IMediaService mediaService, IHttpContextAccessor httpContextAccessor, IAuthService authService)
        {
            _db = db;
            _settings = settings.Value;
            _mediaService = mediaService;
            _httpContextAccessor = httpContextAccessor;
            _authService = authService;
        }

        public async Task<byte[]?> GetThumbnails(ulong galleryId, int noThumbnail)
        {
            var gallery = await _db.Galleries
                .Include(g => g.Media)
                .FirstOrDefaultAsync(g => g.GalleryId == galleryId);

            if (gallery == null)
                return null;

            byte[]? thumb = null;
            string? thumbUuids = gallery.Media.Select(m => m.MediaUuid).Skip(noThumbnail - 1).FirstOrDefault();
            if (thumbUuids != null)
                thumb = await _mediaService.GetImage(thumbUuids, "thumbnail");
            return thumb;
        }

        public async Task<List<MediaDataShort>> GetGallery(ulong galleryId)
        {
            var gallery = await _db.Galleries
                .Include(g => g.Media)
                .FirstOrDefaultAsync(g => g.GalleryId == galleryId);

            if (gallery == null)
                return null;

            var images = gallery.Media.Select(media => new MediaDataShort
            {
                Uuid = media.MediaUuid,
                Title = media.Title,
                Author = media.User.Username,
                Width = media.Width,
                Height = media.Height,
                Blurhash = media.Blurhash
            }).ToList();

            return images;
        }

        public async Task<string> CreateGallery(CreateGalleryData data)
        {
            var userid = _authService.GetCurrentUserId();
            if (userid == null)
                return "User not found";

            var newGallery = new Gallery
            {
                UserId = (ulong)userid,
                Title = data.Title,
                Description = data.Description,
                DateCreated = DateTime.Now,
                Access = 1
            };
            try
            {
                _db.Galleries.Add(newGallery);
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                return "Gallery not created";
            }

            return "Success";
        }

        public async Task<List<GalleryDataShort>> GetUserGalleries(string username)
        {
            var dbUser = _db.Users.FirstOrDefault(u => u.Username == username);
            if (dbUser == null)
                return null;

            return await _db.Galleries.Where(g => g.UserId == dbUser.UserId).Select(g => new GalleryDataShort() { UserId = g.UserId, GalleryId = g.GalleryId, Title = g.Title, MediaCount = g.Media.Count }).ToListAsync();
        }

        public async Task<GalleryDataFull?> GetGalleryDataFull(ulong galleryId)
        {
            var gallery = await _db.Galleries
                .Include(g => g.Media)
                .Include(g => g.User)
                .FirstOrDefaultAsync(g => g.GalleryId == galleryId);

            if (gallery == null)
                return null;

            return new GalleryDataFull
            {
                GalleryId = gallery.GalleryId,
                UserId = gallery.UserId,
                Username = gallery.User.Username,
                Title = gallery.Title,
                Description = gallery.Description ?? "",
                MediaCount = gallery.Media.Count,
            };
        }

        public async Task<bool> UpdateGallery(GalleryUpdateData data)
        {
            var userid = _authService.GetCurrentUserId();
            if (userid == null)
                return false;

            var gallery = await _db.Galleries.FirstOrDefaultAsync(g => g.GalleryId == data.Id);
            if (gallery == null || gallery.UserId != userid)
                return false;

            gallery.Title = data.Title;
            gallery.Description = data.Description;
            _db.SaveChanges();
            return true;
        }

        public async Task<bool> DeleteGallery(ulong galleryId)
        {
            var userid = _authService.GetCurrentUserId();
            if (userid == null)
                return false;

            var gallery = await _db.Galleries.FirstOrDefaultAsync(g => g.GalleryId == galleryId);
            if (gallery == null || gallery.UserId != userid)
                return false;

            _db.Galleries.Remove(gallery);
            _db.SaveChanges();
            return true;
        }
    }
}

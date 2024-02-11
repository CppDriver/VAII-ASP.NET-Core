using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MultimediaLibrary.Data;
using MultimediaLibrary.Models;
using MultimediaLibrary.Models.TransferModels;
using MultimediaLibrary.Services.AuthService;
using System.Diagnostics.Eventing.Reader;

namespace MultimediaLibrary.Services.ImageService
{
    public class MediaService : IMediaService
    {
        private readonly DatabaseContext _db;
        private readonly MultimediaLibrarySettings _settings;
        private readonly IAuthService _authService;

        public MediaService(DatabaseContext db, IOptions<MultimediaLibrarySettings> mls, IAuthService authService)
        {
            _db = db;
            _settings = mls.Value;
            _authService = authService;
        }

        public async Task<List<MediaDataShort>> GetImages()
        {
            var requestFromUser = _authService.GetCurrentUserId();
            var images = await _db.Media.Where(m => m.UserId == requestFromUser || m.Access == 1).OrderByDescending(m => m.DateCreated).Take(100).Select(media => new MediaDataShort
            {
                Uuid = media.MediaUuid,
                Title = media.Title,
                Author = media.User.Username,
                Width = media.Width,
                Height = media.Height,
                Blurhash = media.Blurhash
            }).ToListAsync();

            return images;
        }

        public async Task<List<MediaDataShort>> GetUserImages(string username)
        {
            var requestFromUser = _authService.GetCurrentUserId();
            if (!username.IsNullOrEmpty())
            {
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
                if (user == null)
                    return null;
                var owner = requestFromUser == user.UserId;
                var images = await _db.Media
                    .Where(m => m.UserId == user.UserId && owner ? true : m.Access == 1).OrderByDescending(m => m.DateCreated)
                    .Select(media => new MediaDataShort
                    {
                        Uuid = media.MediaUuid,
                        Title = media.Title,
                        Author = media.User.Username,
                        Width = media.Width,
                        Height = media.Height,
                        Blurhash = media.Blurhash
                    }).ToListAsync();
                return images;
            }
            return null;
        }

        public async Task<List<MediaDataShort>> GetGalleryImages(string galleryId)
        {
            var requestFromUser = _authService.GetCurrentUserId();
            var gallery = await _db.Galleries.Include(g => g.Media).FirstOrDefaultAsync(g => g.GalleryId == ulong.Parse(galleryId));
            if (gallery == null)
                return null;
            var images = _db.Media.Include(m => m.User).Include(m => m.Galleries).Where(m => (m.UserId == requestFromUser || m.Access == 1) && m.Galleries.Contains(gallery)).OrderByDescending(m => m.DateCreated)
                .Select(media => new MediaDataShort
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

        public async Task<MediaDataFull> GetMediaInfo(string mediaUuid)
         {
            var media = await _db.Media
                .Include(m => m.User)
                .Include(m => m.Galleries)
                    .ThenInclude(g => g.User)
                .Include(m => m.Tags)
                .FirstOrDefaultAsync(m => m.MediaUuid == mediaUuid);
            if (media == null)
            {
                return null;
            }

            var mediaData = new MediaDataFull
            {
                Uuid = media.MediaUuid,
                Title = media.Title,
                UserId = media.UserId,
                Username = media.User.Username,
                Width = media.Width,
                Height = media.Height,
                Size = media.Size,
                Blurhash = media.Blurhash,
                DateCreated = media.DateCreated,
                Description = media.Description,
                Galleries = media.Galleries.Select(g => new GalleryDataShort() { GalleryId = g.GalleryId, Title = g.Title, UserId = g.User.UserId, Username = g.User.Username, MediaCount = g.Media.Count }).ToArray(),
                Tags = media.Tags.Select(t => t.Name).ToArray()
            };

            return mediaData;
        }

        public async Task<byte[]?> GetImage(string imageUuid, string size)
        {
            var dir = _settings.DirectoryPaths.Original;
            switch (size)
            {
                case "uhd": dir = _settings.DirectoryPaths.Uhd; break;
                case "fhd": dir = _settings.DirectoryPaths.Fhd; break;
                case "hd": dir = _settings.DirectoryPaths.Hd; break;
                case "sd": dir = _settings.DirectoryPaths.Sd; break;
                case "thumbnail": dir = _settings.DirectoryPaths.Thumbnail; break;
            }
            var imagePath = Path.Combine(dir, imageUuid);

            if (!System.IO.File.Exists(imagePath))
            {
                imagePath = Path.Combine(_settings.DirectoryPaths.Original, imageUuid);
                if (!System.IO.File.Exists(imagePath))
                    return null;
            }

            var imageBytes = await System.IO.File.ReadAllBytesAsync(imagePath);
            return imageBytes;
        }

        public async Task<string?> DeleteMedia(string uuid)
        {
            if (!_authService.IsAuthenticated())
                return await Task.FromResult("User is not authenticated");

            var media = _db.Media.Include(m => m.Galleries).Include(m => m.Tags).Include(m => m.Comments).FirstOrDefault(m => m.MediaUuid == uuid);
            if (media == null)
                return await Task.FromResult("Media does not exist");

            if (_authService.GetCurrentUserId() != media.UserId)
                return await Task.FromResult("User is not owner");

            _db.Media.Remove(media);
            if (await _db.SaveChangesAsync() > 0)
            {
                deleteFiles(uuid);
                return await Task.FromResult("Media deleted");
            }
            return await Task.FromResult("Media not found");
        }
        public async Task<string> UpdateMedia(MediaDataFull mediaData)
        {
            if (!_authService.IsAuthenticated())
                return await Task.FromResult("User is not authenticated");
            var media = _db.Media.Include(m => m.Galleries).Include(m => m.Tags).Include(m => m.Comments).FirstOrDefault(m => m.MediaUuid == mediaData.Uuid);
            if (media == null)
                return await Task.FromResult("Media does not exist");
            if (_authService.GetCurrentUserId() != media.UserId)
                return await Task.FromResult("User is not owner");

            updateTags(mediaData.Tags);

            media.Title = mediaData.Title;
            media.Description = mediaData.Description;
            media.Tags = _db.Tags.Where(t => mediaData.Tags.Contains(t.Name)).ToList();
            media.Access = mediaData.Access;
            media.DateUpdated = DateTime.Now;
            media.Galleries = _db.Galleries.Where(g => mediaData.Galleries.Select(g => g.GalleryId).Contains(g.GalleryId)).ToList();
            _db.Media.Update(media);
            if (await _db.SaveChangesAsync() > 0)
            {
                return await Task.FromResult("Media updated");
            }
            return await Task.FromResult("Media not found");
        }

        public async Task<string> AddToGallery(string galleryId, string mediaUuid)
        {
            var requestFromUser = _authService.GetCurrentUserId();
            var gallery = await _db.Galleries.Include(g => g.Media).FirstOrDefaultAsync(g => g.GalleryId == ulong.Parse(galleryId));
            var media = await _db.Media.FirstOrDefaultAsync(m => m.MediaUuid == mediaUuid);
            if (gallery == null || media == null)
                return "Gallery or media not found";
            if (gallery.UserId != requestFromUser)
                return "User is not owner of the gallery";
            gallery.Media.Add(media);
            if (await _db.SaveChangesAsync() > 0)
                return "Success";
            return "Error";
        }

        private void updateTags(string[] tags)
        {
            foreach (var tag in tags)
            {
                var tagInDb = _db.Tags.FirstOrDefault(t => t.Name == tag);
                if (tagInDb == null)
                {
                    tagInDb = new Tag { Name = tag };
                    _db.Tags.Add(tagInDb);
                }
            }
            _db.SaveChanges();
        }

        private void deleteFiles(string uuid)
        {
            var originalPath = Path.Combine(_settings.DirectoryPaths.Original, uuid);
            var uhdPath = Path.Combine(_settings.DirectoryPaths.Uhd, uuid);
            var fhdPath = Path.Combine(_settings.DirectoryPaths.Fhd, uuid);
            var hdPath = Path.Combine(_settings.DirectoryPaths.Hd, uuid);
            var sdPath = Path.Combine(_settings.DirectoryPaths.Sd, uuid);
            var thumbnailPath = Path.Combine(_settings.DirectoryPaths.Thumbnail, uuid);

            if (System.IO.File.Exists(originalPath))
                System.IO.File.Delete(originalPath);
            if (System.IO.File.Exists(uhdPath))
                System.IO.File.Delete(uhdPath);
            if (System.IO.File.Exists(fhdPath))
                System.IO.File.Delete(fhdPath);
            if (System.IO.File.Exists(hdPath))
                System.IO.File.Delete(hdPath);
            if (System.IO.File.Exists(sdPath))
                System.IO.File.Delete(sdPath);
            if (System.IO.File.Exists(thumbnailPath))
                System.IO.File.Delete(thumbnailPath);
        }
    }
}

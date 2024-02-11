using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Extensions.Options;
using MultimediaLibrary.Data;
using MultimediaLibrary.Models;
using MultimediaLibrary.Models.TransferModels;
using MultimediaLibrary.Services.AuthService;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Runtime;

namespace MultimediaLibrary.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly DatabaseContext _db;
        private readonly IAuthService _authService;
        private readonly MultimediaLibrarySettings _settings;

        public UserService(DatabaseContext db, IAuthService authService, IOptions<MultimediaLibrarySettings> settings)
        {
            _db = db;
            _authService = authService;
            _settings = settings.Value;
        }

        public async Task<UserData?> GetUser(string username)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
            return new UserData
            {
                Id = user.UserId.ToString(),
                Name = user.Name,
                Surname = user.Surname,
                Username = user.Username,
                Email = user.Email,
                Location = user.Location,
                Bio = user.Bio,
            };
        }

        public async Task<UserDataFull?> GetUserDataFull(string username)
        {
            var user = await _db.Users.Include(u => u.Media).Include(u => u.Galleries).Include(u => u.Followers).Include(u => u.Following).FirstOrDefaultAsync(u => u.Username == username);
            return new UserDataFull
            {
                Id = user.UserId.ToString(),
                Name = user.Name,
                Surname = user.Surname,
                Username = user.Username,
                Email = user.Email,
                Location = user.Location,
                Bio = user.Bio,
                MediaCount = user.Media.Count,
                GalleryCount = user.Galleries.Count,
                FollowersCount = user.Followers.Count,
                FollowingCount = user.Following.Count,
            };
        }

        public async Task<UserData?> GetUserById(ulong userId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            return new UserData
            {
                Id = user.UserId.ToString(),
                Name = user.Name,
                Surname = user.Surname,
                Username = user.Username,
                Email = user.Email,
                Location = user.Location,
                Bio = user.Bio,
            };
        }

        public async Task<bool> UpdateUserData(UserData user)
        {
            var userId = _authService.GetCurrentUserId();
            if (userId != null)
            {
                var dbUser = await _db.Users.FirstOrDefaultAsync(u => u.UserId == userId);
                if (dbUser != null)
                {
                    dbUser.Name = user.Name;
                    dbUser.Surname = user.Surname;
                    dbUser.Username = user.Username;
                    dbUser.Email = user.Email;
                    dbUser.Location = user.Location;
                    dbUser.Bio = user.Bio;
                    _db.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public async Task<byte[]> GetUserProfileImg(ulong userId)
        {
            var imagePath = _settings.DirectoryPaths.UserProfile + '/' + userId;
            if (!File.Exists(imagePath))
                imagePath = _settings.DirectoryPaths.UserProfile + '/' + "default";
            return await System.IO.File.ReadAllBytesAsync(imagePath); 
        }

        public async Task<string> UpdateProfileImg(IFormFile file)
        {
            var userId = _authService.GetCurrentUserId();
            if (userId != null)
            {
                if (file == null || file.Length == 0)
                {
                    return "No file was uploaded";
                }

                string filePath = Path.Combine(_settings.DirectoryPaths.UserProfile, userId.Value.ToString());

                if (!Directory.Exists(_settings.DirectoryPaths.UserProfile))
                {
                    Directory.CreateDirectory(_settings.DirectoryPaths.UserProfile);
                }

                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;
                    using (var image = Image.Load(memoryStream))
                    {
                        image.Mutate(i => i.Resize(new ResizeOptions { Size = new Size(200, 200), Mode = ResizeMode.Crop} ));
                        image.SaveAsJpeg(filePath, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder() { Quality = 70 });
                    }
                }

                return "OK";
            }
            return "User not logged in";
        }

        public async Task<List<GalleryDataShort>> GetUserGalleries(string username)
        {
            var owner = username.Equals(this._authService.GetCurrentUserUsername());
            var user = await _db.Users.Include(u => u.Galleries).FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
                return new List<GalleryDataShort>();
            return user.Galleries.Where(g => owner ? true : g.Access == 1).Select(g => new GalleryDataShort { GalleryId = g.GalleryId, Title = g.Title, UserId = g.UserId, Username = g.User.Username, MediaCount = g.Media.Count }).ToList();
        }
    }
}

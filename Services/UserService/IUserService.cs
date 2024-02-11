using MultimediaLibrary.Models;
using MultimediaLibrary.Models.TransferModels;

namespace MultimediaLibrary.Services.UserService
{
    public interface IUserService
    {
        Task<UserData?> GetUser(string username);
        Task<UserDataFull?> GetUserDataFull(string username);
        Task<UserData?> GetUserById(ulong userId);
        Task<bool> UpdateUserData(UserData user);
        Task<byte[]> GetUserProfileImg(ulong userId);
        Task<string> UpdateProfileImg(IFormFile file);
        Task<List<GalleryDataShort>> GetUserGalleries(string username);
    }
}

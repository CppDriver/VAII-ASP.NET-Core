using Microsoft.AspNetCore.Identity;

namespace MultimediaLibrary.Services.AuthService
{
    public interface IAuthService
    {
        Task<IdentityResult> Login(string username, string password);
        Task<IdentityResult> Register(string email, string username, string password);
        Task<IdentityResult> ChangePassword(string username, string oldPassword, string newPassword);
        Task<IdentityResult> DeleteAccount(string username, string password);
        string GenerateToken(string username);
        bool IsAuthenticated();
        ulong? GetCurrentUserId();
        string GetCurrentUserUsername();
    }
}

using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MultimediaLibrary.Data;
using MultimediaLibrary.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace MultimediaLibrary.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly DatabaseContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(DatabaseContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IdentityResult> Login(string username, string password)
        {
            // validate user info
            if (username.IsNullOrEmpty() || password.IsNullOrEmpty() || !(checkUsername(username) && checkPassword(password)))
            {
                return IdentityResult.Failed(new IdentityError { Description = "Invalid user info"});
            }

            // check if user exists, if not return error
            var user = _db.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User does not exist" });
            }
            if (user.Password == Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(password + user.Salt))))
            {
                return IdentityResult.Success;
            }
            else
            {
                return IdentityResult.Failed(new IdentityError { Description = "Incorrect password" });
            }   
        }

        public async Task<IdentityResult> Register(string email, string username, string password)
        {
            // validate user info
            if (email.IsNullOrEmpty() || username.IsNullOrEmpty() || password.IsNullOrEmpty() || !(checkEmail(email) && checkUsername(username) && checkPassword(password)))
            {
                return IdentityResult.Failed(new IdentityError { Description = "Invalid user info"});
            }

            // check if user exists, if so return error
            if (_db.Users.Any(u => u.Email == email))
            {
                return IdentityResult.Failed(new IdentityError { Description = "Email already exists" });
            }
            if (_db.Users.Any(u => u.Username == username))
            {
                return IdentityResult.Failed(new IdentityError { Description = "Username already exists" });
            }

            //if user does not exist, create user
            var saltBytes = RandomNumberGenerator.GetBytes(64);
            var salt = Convert.ToBase64String(saltBytes);
            var passwordBytes = Encoding.UTF8.GetBytes(password + salt);
            var hashBytes = SHA256.HashData(passwordBytes);
            var hash = Convert.ToBase64String(hashBytes);
            var user = new User
            {
                Email = email,
                Username = username,
                Password = hash,
                Salt = salt
            };
            _db.Users.Add(user);
            _db.SaveChanges();

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> ChangePassword(string username, string oldPassword, string newPassword)
        {
            // validate user info
            if (username.IsNullOrEmpty() || oldPassword.IsNullOrEmpty() || newPassword.IsNullOrEmpty() || !(checkUsername(username) && checkPassword(oldPassword) && checkPassword(newPassword)))
            {
                return IdentityResult.Failed(new IdentityError { Description = "Invalid user info"});
            }

            // check if user exists, if not return error
            var user = _db.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User does not exist" });
            }
            if (user.Password == Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(oldPassword + user.Salt))))
            {
                var saltBytes = RandomNumberGenerator.GetBytes(64);
                var salt = Convert.ToBase64String(saltBytes);
                var passwordBytes = Encoding.UTF8.GetBytes(newPassword + salt);
                var hashBytes = SHA256.HashData(passwordBytes);
                var hash = Convert.ToBase64String(hashBytes);
                user.Password = hash;
                user.Salt = salt;
                _db.SaveChanges();
                return IdentityResult.Success;
            }
            else
            {
                return IdentityResult.Failed(new IdentityError { Description = "Incorrect password" });
            }   
        }

        public async Task<IdentityResult> DeleteAccount(string username, string password)
        {
            // validate user info
            if (username.IsNullOrEmpty() || password.IsNullOrEmpty() || !(checkUsername(username) && checkPassword(password)))
            {
                return IdentityResult.Failed(new IdentityError { Description = "Invalid user info"});
            }

            if (!username.Equals(GetCurrentUserUsername()))
                return IdentityResult.Failed(new IdentityError { Description = "Not authenticated" });

            // check if user exists, if not return error
            var user = _db.Users.Include(u => u.Media).Include(u => u.Galleries).Include(u => u.Comments).FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User does not exist" });
            }
            if (user.Password == Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(password + user.Salt))))
            {
                _db.Users.Remove(user);
                _db.SaveChanges();
                return IdentityResult.Success;
            }
            else
            {
                return IdentityResult.Failed(new IdentityError { Description = "Incorrect password" });
            }   
        }

        private bool checkEmail(string email)
        {
            if (email.IsNullOrEmpty())
                return false;
            var recipient = email.Substring(0, email.IndexOf('@'));
            var domain = email.Substring(email.IndexOf('@') + 1, email.LastIndexOf('.') - email.IndexOf('@') - 1);
            var tld = email.Substring(email.LastIndexOf('.') + 1);
            var recipientRegex = new Regex(@"^[a-zA-Z0-9!#$%&'*+\/=?^_`{|}~.-]+$");
            var domainRegex = new Regex(@"^[a-zA-Z0-9.-]+$");
            var tldRegex = new Regex(@"^[a-zA-Z]+$");
            if (recipientRegex.IsMatch(recipient) && domainRegex.IsMatch(domain) && tldRegex.IsMatch(tld) && recipient.Length <= 64 && domain.Length <= 255 && tld.Length <= 63)
                return true;
            else
                return false;
        }

        private bool checkUsername(string username)
        {
            if (username.IsNullOrEmpty())
                return false;
            var usernameRegex = new Regex(@"^[a-zA-Z0-9!#$%&'*+\/=?^_`{|}~.-]+$");
            if (usernameRegex.IsMatch(username) && username.Length <= 64)
                return true;
            else
                return false;
        }

        private bool checkPassword(string password)
        {
            if (password.IsNullOrEmpty())
                return false;
            var passwordRegex = new Regex(@"^[a-zA-Z0-9!#$%&'*+\/=?^_`{|}~.-]+$");
            if (passwordRegex.IsMatch(password) && password.Length <= 64)
                return true;
            else
                return false;
        }

        public string GenerateToken(string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("temporary solution, needs 256 bits, it had only 144, but why it was working till now");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.NameIdentifier, _db.Users.FirstOrDefault(u => u.Username == username).UserId.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(4),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = "MultimediaLibrary",
                Audience = "MultimediaLibrary"
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public bool IsAuthenticated()
        {
            return _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
        }
        
        public ulong? GetCurrentUserId()
        {
            ulong ret;
            if (ulong.TryParse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), out ret))
                return ret;
            else
                return null;
        }

        public string? GetCurrentUserUsername()
        {
            return _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
        }
    }
}

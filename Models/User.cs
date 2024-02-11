using System.ComponentModel.DataAnnotations.Schema;

namespace MultimediaLibrary.Models
{
    public class User
    {
        public ulong UserId { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Location { get; set; }
        public string? Bio { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Salt { get; set; } = null!;

        public List<User> Following { get; } = null!;
        public List<User> Followers { get; } = null!;
        public List<Media> Media { get; } = null!;
        public List<Gallery> Galleries { get; } = null!;
        public List<Comment> Comments { get; } = null!;
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultimediaLibrary.Models
{
    public class Media
    {
        [Key]
        [Column(TypeName = "VARCHAR")]
        [StringLength(36)]
        public string MediaUuid { get; set; } = null!;
        public ulong UserId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string Blurhash { get; set; } = null!;
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public int Size { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Access { get; set; }
        public string? Exif { get; set; }

        public User User { get; set; } = null!;
        public List<Gallery> Galleries { get; set; } = null!;
        public List<Tag> Tags { get; set; } = null!;
        public List<Comment> Comments { get; set; } = null!;
    }
}

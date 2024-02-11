using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultimediaLibrary.Models
{
    public class Comment
    {
        public ulong CommentId { get; set; }
        public ulong UserId { get; set; }
        [ForeignKey("Media")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(36)]
        public string? MediaUuid { get; set; }
        public ulong? GalleryId { get; set; }
        public string Text { get; set; } = null!;
        public DateTime DateCreated { get; set; }

        public User User { get; set; } = null!;
        public Media? Media { get; set; }
        public Gallery? Gallery { get; set; }
    }
}

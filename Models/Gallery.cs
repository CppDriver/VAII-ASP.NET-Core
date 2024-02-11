
namespace MultimediaLibrary.Models
{
    public class Gallery
    {
        public ulong GalleryId { get; set; }
        public ulong UserId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? DateCreated { get; set; }
        public int Access { get; set; }

        public User User { get; set; } = null!;
        public List<Media> Media { get; set; } = null!;
    }
}

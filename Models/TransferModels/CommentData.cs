namespace MultimediaLibrary.Models.TransferModels
{
    public class CommentData
    {
        public ulong? CommentId { get; set; }
        public ulong? UserId { get; set; }
        public string Username { get; set; } = null!;
        public string? MediaUuid { get; set; }
        public ulong? GalleryId { get; set; }
        public string Text { get; set; } = null!;
        public DateTime DateCreated { get; set; }
    }
}

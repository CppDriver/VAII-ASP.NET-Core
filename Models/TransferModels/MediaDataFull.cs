namespace MultimediaLibrary.Models.TransferModels
{
    public class MediaDataFull
    {
        public string Uuid { get; set; }
        public string Title { get; set; }
        public ulong? UserId { get; set; }
        public string? Username { get; set; }
        public string Description { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public int? Size { get; set; }
        public string? Blurhash { get; set; }
        public GalleryDataShort[] Galleries { get; set; }
        public string[] Tags { get; set; }
        public int Access { get; set; }
    }
}

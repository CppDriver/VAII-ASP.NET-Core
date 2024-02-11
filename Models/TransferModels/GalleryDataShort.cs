namespace MultimediaLibrary.Models.TransferModels
{
    public class GalleryDataShort
    {
        public ulong GalleryId { get; set; }
        public string Title { get; set; }
        public ulong UserId { get; set; }
        public string Username { get; set; }
        public int MediaCount { get; set; }
    }
}

namespace MultimediaLibrary.Models.TransferModels
{
    public class MediaDataShort
    {
        public string Uuid { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Blurhash { get; set; }
    }
}

namespace MultimediaLibrary.Models
{
    public class Tag
    {
        public ulong TagId { get; set; }
        public string Name { get; set; } = null!;

        public List<Media> Media { get; set; } = null!;
    }
}

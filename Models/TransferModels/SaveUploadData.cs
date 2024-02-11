namespace MultimediaLibrary.Models.TransferModels
{
    public class SaveUploadData
    {
        public string? Uuid { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public int? Size { get; set; }
        public string? Access { get; set; }
    }
}

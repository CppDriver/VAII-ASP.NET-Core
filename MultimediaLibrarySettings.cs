namespace MultimediaLibrary
{
    public class MultimediaLibrarySettings
    {
        public DirectoryPaths DirectoryPaths { get; set; }
    }

    public class DirectoryPaths
    {
        public string Original { get; set; }
        public string Uhd { get; set; }
        public string Fhd { get; set; }
        public string Hd { get; set; }
        public string Sd { get; set; }
        public string Thumbnail { get; set; }
        public string UserProfile { get; set; }
    }
}

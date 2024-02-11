namespace MultimediaLibrary.Models.TransferModels
{
    public class UserDataFull
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Location { get; set; }
        public string Bio { get; set; }
        public int MediaCount { get; set; }
        public int GalleryCount { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
    }
}

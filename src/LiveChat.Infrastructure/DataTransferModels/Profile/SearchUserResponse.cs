namespace LiveChat.Infrastructure.DataTransferModels.Profile
{
    public class SearchUserResponse
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ProfilePicturePath { get; set; }
    }
}

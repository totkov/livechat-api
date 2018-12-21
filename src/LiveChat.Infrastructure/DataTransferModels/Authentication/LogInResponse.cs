namespace LiveChat.Infrastructure.DataTransferModels
{
    public class LogInResponse
    {
        public int UserId { get; set; }

        public string Email { get; set; }

        public string Token { get; set; }

        public string Message { get; set; }
    }
}

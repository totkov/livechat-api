using System;
using System.Collections.Generic;

namespace LiveChat.Infrastructure.DataTransferModels
{
    public class ChatDetailsResponse
    {
        public int Id { get; set; }

        public IEnumerable<UserInChatModel> Users { get; set; }

        public IEnumerable<MessageModel> Messages { get; set; }
    }

    public class MessageModel
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public DateTime DateTime { get; set; }

        public int AuthorId { get; set; }

        public string AuthorFirstName { get; set; }

        public string AuthoeLastName { get; set; }
    }

    public class UserInChatModel
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}

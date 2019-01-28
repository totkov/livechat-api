using System;
using System.Collections.Generic;

namespace LiveChat.Infrastructure.DataTransferModels
{
    public class ChatResponse
    {
        public int Id { get; set; }

        public IEnumerable<string> Users { get; set; }

        public string LastMessageText { get; set; }

        public string LastMessageAuthor { get; set; }

        public DateTime LastMessageDate { get; set; }
    }
}

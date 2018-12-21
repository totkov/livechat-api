using System.Collections.Generic;

namespace LiveChat.Infrastructure.DataTransferModels
{
    public class CreateChatRequest
    {
        public IEnumerable<string> UserEmails { get; set; }
    }
}

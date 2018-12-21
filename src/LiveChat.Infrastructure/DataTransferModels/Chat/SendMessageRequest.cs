using System;

namespace LiveChat.Infrastructure.DataTransferModels
{
    public class SendMessageRequest
    {
        public string Text { get; set; }

        public DateTime UtcDate { get; set; }
    }
}

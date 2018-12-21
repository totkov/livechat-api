using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LiveChat.Data.Models
{
    public class Chat
    {
        [Key]
        public int Id { get; set; }

        public List<UserChat> UserChats { get; set; }

        public List<Message> Messages { get; set; }
    }
}

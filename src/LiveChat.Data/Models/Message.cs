using System;
using System.ComponentModel.DataAnnotations;

namespace LiveChat.Data.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        public int ChatId { get; set; }
        public Chat Chat { get; set; }

        public int? AuthorId { get; set; }
        public User Author { get; set; }

        public DateTime Date { get; set; }

        public string Text { get; set; }
    }
}

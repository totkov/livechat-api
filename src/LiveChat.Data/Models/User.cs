using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LiveChat.Data.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(6)]
        [MaxLength(60)]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        [MaxLength(60)]
        public string Password { get; set; }

        [Required]
        [MaxLength(128)]
        public byte[] Salt { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(30)]
        public string FirstName { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(30)]
        public string LastName { get; set; }

        public List<UserChat> UserChats { get; set; }

        public List<Message> Messages { get; set; }
    }
}

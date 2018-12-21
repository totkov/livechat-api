using System.ComponentModel.DataAnnotations;

namespace LiveChat.Infrastructure.DataTransferModels
{
    public class RegistrationRequest
    {
        [Required]
        [MinLength(6)]
        [MaxLength(60)]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        [MaxLength(60)]
        public string Password { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(30)]
        public string FirstName { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(30)]
        public string LastName { get; set; }
    }
}

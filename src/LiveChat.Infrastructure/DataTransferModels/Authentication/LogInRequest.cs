using System.ComponentModel.DataAnnotations;

namespace LiveChat.Infrastructure.DataTransferModels
{
    public class LogInRequest
    {
        [Required]
        [MinLength(5)]
        [MaxLength(30)]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        [MaxLength(30)]
        public string Password { get; set; }
    }
}

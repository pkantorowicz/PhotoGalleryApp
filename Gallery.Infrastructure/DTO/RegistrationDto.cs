using System.ComponentModel.DataAnnotations;

namespace Gallery.Infrastructure.DTO
{
    public class RegistrationDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}

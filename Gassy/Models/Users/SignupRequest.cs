using System.ComponentModel.DataAnnotations;

namespace Gassy.Models.Users
{
    public class SignupRequest
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string UserPassword { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set;}

        [Required]
        public string Email { get; set; }

        [Required]
        public string PhoneNumber { get; set ; }
    }
}
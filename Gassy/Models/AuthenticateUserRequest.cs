using System.ComponentModel.DataAnnotations;

namespace Gassy.Models
{
    public class AuthenticateUserRequest
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string UserPassword { get; set; }
    }
}
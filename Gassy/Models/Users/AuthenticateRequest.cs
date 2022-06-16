using System.ComponentModel.DataAnnotations;

namespace Gassy.Models.Users
{
    public class AuthenticateRequest
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string UserPassword { get; set; }
    }
}
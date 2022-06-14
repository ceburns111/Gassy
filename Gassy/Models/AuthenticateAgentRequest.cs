using System.ComponentModel.DataAnnotations;

namespace Gassy.Models
{
    public class AuthenticateAgentRequest
    {
        [Required]
        public string AgentName { get; set; }

        [Required]
        public string AgentPassword { get; set; }
    }
}
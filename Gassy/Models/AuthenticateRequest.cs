using System.ComponentModel.DataAnnotations;
using GassyFunctionHelpers.Models;

namespace Gassy.Models
{
    public class AuthenticateRequest
    {
        [Required]
        public string AgentName { get; set; }

        [Required]
        public string AgentPassword { get; set; }
    }
}
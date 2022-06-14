
namespace Gassy.Models
{
    public class AuthenticateAgentResponse
    {
        public int Id { get; set; }
        public string AgentName { get; set; }
        public string Token { get; set; }

        public AuthenticateAgentResponse(Agent agent, string token)
        {
            Id = agent.Id; 
            AgentName = agent.AgentName;
            Token = token;
        }
    }
}
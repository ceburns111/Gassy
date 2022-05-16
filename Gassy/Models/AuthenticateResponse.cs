namespace Gassy.Models
{
    public class AuthenticateResponse
    {
        public int Id { get; set; }
        public string AgentName { get; set; }
        public string Token { get; set; }

        public AuthenticateResponse(Agent agent, string token)
        {
            Id = agent.Id; 
            AgentName = agent.AgentName;
            Token = token;
        }
    }
}
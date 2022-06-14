using Gassy.Models;

namespace Gassy.Services
{
     public interface IAgentService
    {
        AuthenticateAgentResponse Authenticate(AuthenticateAgentRequest model);
        Task<Agent> GetById(int id);
    }
}
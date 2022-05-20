using Gassy.Models;


namespace Gassy.Services
{
     public interface IAgentService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model);
        Task<Agent> GetById(int id);
    }
}
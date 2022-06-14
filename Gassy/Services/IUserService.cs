using Gassy.Models;

namespace Gassy.Services
{
     public interface IUserService
    {
        AuthenticateUserResponse Authenticate(AuthenticateUserRequest model);
        Task<User> GetById(int id);
    }
}
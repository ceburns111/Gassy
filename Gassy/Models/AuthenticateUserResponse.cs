namespace Gassy.Models
{
    public class AuthenticateUserResponse
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }

        public AuthenticateUserResponse(User user, string token)
        {
            Id = user.Id; 
            UserName = user.UserName;
            Token = token;
        }
    }
}
using Gassy.Entities;

namespace Gassy.Models.Users
{
    public class UserDTO
    {
        public int Id { get; set;}
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set;}
        public string Email { get; set; }
        public string PhoneNumber { get; set ; }
    }
}
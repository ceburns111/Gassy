using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace Gassy.Entities; 

public class User
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string UserPassword { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set;}
    public string Email { get; set; }
    [JsonIgnore]
    public string PasswordHash { get; set; }
    public RoleId RoleId { get; set; }

   
    [JsonIgnore]
    public List<RefreshToken> RefreshTokens { get; set; }
}
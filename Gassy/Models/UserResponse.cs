using System.Text.Json.Serialization;
namespace Gassy.Models;

public class UserResponse
{
    [JsonPropertyName("id")]
    public int Id { get;set; }

    [JsonPropertyName("userName")]
    public string UserName { get;set; }

    [JsonPropertyName("token")]
    public string Token { get; set; } 
}

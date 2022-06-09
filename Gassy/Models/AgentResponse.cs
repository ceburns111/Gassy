using System.Text.Json.Serialization;
namespace Gassy.Models;

public class AgentResponse
{
    [JsonPropertyName("id")]
    public int id { get;set; }

    [JsonPropertyName("agentName")]
    public string agentName { get;set; }

    [JsonPropertyName("token")]
    public string token { get; set; } 
}

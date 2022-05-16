using System.Text.Json.Serialization;


namespace Gassy.Models
{
    public class Agent
    {
        public int Id { get; set; }
        public string AgentName { get; set; }

        [JsonIgnore]
        public string AgentPassword { get; set; }
    }
}
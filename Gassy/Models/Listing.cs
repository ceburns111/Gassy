using System.Text.Json.Serialization;

namespace Gassy.Models
{
    public class Listing
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("siteid")]
        public string SiteId { get; set; }

        [JsonPropertyName("make")]
        public string? Make { get; set; }

        [JsonPropertyName("model")]
        public string? Model { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        // [JsonPropertyName("condition")]
        // public string Condition { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("offers_enabled")]
        public bool OffersEnabled { get; set; }

        [JsonPropertyName("link")]
        public string Link { get; set; }

        [JsonPropertyName("condition")]
        public string Condition { get; set; }
    }
}
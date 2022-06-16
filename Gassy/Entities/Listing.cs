
namespace Gassy.Models;

public class Listing
{
        public int Id { get; set; }
        public int SiteId { get; set;}
        public string Make { get; set;}
        public string Model { get; set;}
        public decimal Price { get; set;}
        public decimal Shipping {get; set;}
        public string ItemDescription { get; set;}
        public string ItemCondition { get; set;}
        public bool OffersEnabled { get; set;}
        public string Link { get; set;}
        public DateTime CreatedAt { get; set;}
        public DateTime UpdatedAt { get; set;}
}

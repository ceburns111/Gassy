using System;

namespace Gassy.Models
{
    public class ReverbListingDto
    {
        public int ReverbId { get; set;}
        public string Make { get; set;}
        public string Model { get; set;}
        public decimal Price { get; set;}
        public string ItemDescription { get; set;}
        public string ItemCondition { get; set;}
        public bool OffersEnabled { get; set;}
        public string Link { get; set;}
        public DateTime ListingCreatedAt { get; set;}
        public DateTime ListingPublishedAt { get; set;}
        public DateTime UpdatedAt { get; set;}
    }
}
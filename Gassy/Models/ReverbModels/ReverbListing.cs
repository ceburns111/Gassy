namespace Gassy.Models.ReverbModels
{
    public class ReverbListing
    {
        public int id { get; set; }
        public string make { get; set; }
        public string model { get; set; }
        public string finish { get; set; }
        public string year { get; set; }
        public string title { get; set; }
        public DateTime created_at { get; set; }
        public string shop_name { get; set; }
        public string description { get; set; }
        //public string condition { get; set; }
        public string condition_uuid { get; set; }
        public string condition_slug { get; set; }
        public Price price { get; set; }
        public int inventory { get; set; }
        public bool has_inventory { get; set; }
        public bool offers_enabled { get; set; }
        public bool auction { get; set; }
        public List<string> category_uuids { get; set; }
        public string listing_currency { get; set; }
        public DateTime published_at { get; set; }
        public BuyerPrice buyer_price { get; set; }
        public string sku { get; set; }
        public State state { get; set; }
        public Shipping shipping { get; set; }
        public string slug { get; set; }
        public List<Photo> photos { get; set; }
        public Links _links { get; set; }
    }
}
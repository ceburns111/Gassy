namespace Gassy.Entities
{
    public class WishlistItem
    {
        public string Make { get; set; } 
        public string Model { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }        
    }
}
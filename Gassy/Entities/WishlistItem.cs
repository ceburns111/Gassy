namespace Gassy.Entities
{
    public class WishlistItem
    {
        public int Id { get;set; }
        public int OwnerId { get;set; }
        public string Make { get; set; } 
        public string Model { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }        
    }
}
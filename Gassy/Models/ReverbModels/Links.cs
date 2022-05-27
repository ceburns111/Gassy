namespace Gassy.Models.ReverbModels
{
    public class Links
    {
        public LargeCrop large_crop { get; set; }
        public SmallCrop small_crop { get; set; }
        public Full full { get; set; }
        public Thumbnail thumbnail { get; set; }
        public Photo photo { get; set; }
        public Self self { get; set; }
        public Update update { get; set; }
        public End end { get; set; }
        public Want want { get; set; }
        public Unwant unwant { get; set; }
        public Edit edit { get; set; }
        public Web web { get; set; }
        public AddToWishlist add_to_wishlist { get; set; }
        public RemoveFromWishlist remove_from_wishlist { get; set; }
        public Cart cart { get; set; }
        public Next next { get; set; }
        public Suggestion suggestion { get; set; }
        public Follow follow { get; set; }
    }
}
namespace Gassy.Models.ReverbModels
{
    public class Root
    {
        public int total { get; set; }
        public int current_page { get; set; }
        public int total_pages { get; set; }
        public List<ReverbListing> listings { get; set; }
        public string ships_to { get; set; }
        public string humanized_params { get; set; }
        public Links _links { get; set; }
    }
}
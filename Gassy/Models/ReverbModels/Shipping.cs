namespace Gassy.Models.ReverbModels
{
    public class Shipping
    {
        public bool local { get; set; }
        public bool us { get; set; }
        public UsRate? us_rate { get; set; }
    }
}
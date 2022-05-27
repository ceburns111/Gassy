namespace Gassy.Models.ReverbModels
{
    public class BuyerPrice
    {
        public string amount { get; set; }
        public int amount_cents { get; set; }
        public string currency { get; set; }
        public string symbol { get; set; }
        public string display { get; set; }
        public string tax_included_hint { get; set; }
        public bool tax_included { get; set; }
        public int tax_included_rate { get; set; }
    }
}

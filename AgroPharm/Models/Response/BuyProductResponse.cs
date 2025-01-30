namespace AgroPharm.Models.Response
{
    public class BuyProductResponse : EntityBase
    {
        public int ProductNameID { get; set; }
        public string? ProductName { get; set; }
        public int OrganizationNameID {  get; set; } 
        public string? OrganizationName { get; set; }
        public decimal? BuyProductPrice { get; set; }
        public decimal? BuyProductPriceUSD { get; set; }
        public double? BuyProductObem { get; set; }
        public decimal? BuyProductSumPrice { get; set; }
        public decimal? BuyProductSumPriceUSD { get; set; }
        public DateTime? BuyProductDate { get; set; }
        public string? BuyComment { get; set; }
    }
}

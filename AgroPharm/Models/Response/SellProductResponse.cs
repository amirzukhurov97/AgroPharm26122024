namespace AgroPharm.Models.Response
{
    public class SellProductResponse : EntityBase
    {
        public int ProductNameID { get; set; }
        public string? ProductName { get; set; }
        public int CustomerNameID { get; set; }
        public string? CustomerName { get; set; }
        public decimal? SellProductPrice { get; set; }
        public decimal? SellProductPriceUSD { get; set; }
        public double? SellProductObem { get; set; }
        public decimal? SellProductSumPrice { get; set; }
        public decimal? SellProductSumPriceUSD { get; set; }
        public DateTime? SellProductDate { get; set; }
        public string? SellComment { get; set; }
    }
}

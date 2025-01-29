namespace AgroPharm.Models.ReturnIn
{
    public class ReturnInResponse : EntityBase
    {
        public int ProductNameID { get; set; }
        public string? ProductName { get; set; }
        public int CustomerNameID { get; set; }
        public string? CustomerName { get; set; }
        public decimal? ReturnInProductPrice { get; set; }
        public decimal? ReturnInProductPriceUSD { get; set; }
        public double? ReturnInProductObem { get; set; }
        public decimal? ReturnInSumProductPrice { get; set; }
        public decimal? ReturnInSumProductPriceUSD { get; set; }
        public DateTime? ReturnInProductDate { get; set; }
        public string? ReturnInComment { get; set; }
    }
}

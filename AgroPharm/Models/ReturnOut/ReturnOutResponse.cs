namespace AgroPharm.Models.ReturnOut
{
    public class ReturnOutResponse : EntityBase
    {
        public int ProductNameID { get; set; }
        public string? ProductName { get; set; }
        public int OrganizationNameID { get; set; }
        public string? OrganizationName { get; set; }
        public decimal? ReturnOutProductPrice { get; set; }
        public decimal? ReturnOutProductPriceUSD { get; set; }
        public double? ReturnOutProductObem { get; set; }
        public decimal? ReturnOutSumProductPrice { get; set; }
        public decimal? ReturnOutSumProductPriceUSD { get; set; }
        public DateTime? ReturnOutProductDate { get; set; }
        public string? ReturnOutComment { get; set; }

    }
}

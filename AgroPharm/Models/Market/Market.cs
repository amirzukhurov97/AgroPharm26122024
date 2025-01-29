namespace AgroPharm.Models.Market
{
    public class Market : EntityBase
    {
        public int ProductNameID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public double ObemProducts { get; set; } = 0;
    }
}

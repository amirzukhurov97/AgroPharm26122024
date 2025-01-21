namespace AgroPharm.Models
{
    public class Currency : EntityBase
    {
        public decimal USDtoTJS { get; set; }
        public DateTime DateChangeCurrency { get; set; } = DateTime.Now;
    }
}

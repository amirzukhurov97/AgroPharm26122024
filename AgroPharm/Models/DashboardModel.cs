using AgroPharm.Models.Buy;
using AgroPharm.Models.ReturnIn;
using AgroPharm.Models.ReturnOut;
using AgroPharm.Models.Sell;

namespace AgroPharm.Models
{
    public class DashboardModel
    {
        public BuyProductPrices? BuyProductPrices { get; set; }
        public SellProductPrices? SellProductPrices { get; set; }
        public ReturnInProductPrices? ReturnInProductPrices { get; set; }
        public ReturnOutProductPrices? ReturnOutProductPrices { get; set; }
    }
}

using AgroPharm.Models.Buy;
using AgroPharm.Models.ReturnIn;
using AgroPharm.Models.ReturnOut;
using AgroPharm.Models.Sell;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AgroPharm.Models
{
    public class ViewModel
    {
        public double? tempQuantity {  get; set; }
        public List<SelectListItem>? Products { get; set; }
        public List<SelectListItem>? Organizations { get; set; }
        public List<SelectListItem>? Customers { get; set; }
        public BuyProductRequest? BuyProductRequest { get; set; }
        public BuyProductResponse? BuyProductResponse { get; set; }
        public SellProductRequest? SellRequest { get; set; }
        public SellProductResponse? SellResponse { get; set; } 
        public ReturnOutRequest? ReturnOutRequest { get; set; }
        public ReturnOutResponse? ReturnOutResponse { get; set; } 
        public ReturnInRequest? ReturnInRequest { get; set; }
        public ReturnInResponse? ReturnInResponse { get; set; }
        public IEnumerable<Currency>? Currency { get; set; }
        public decimal? CurrencyNow { get; set; }

        public int? ProductNameID { get; set; }
        public int? OrganizationNameID { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

    }

}

using AgroPharm.Models.Request;
using AgroPharm.Models.Response;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AgroPharm.Models
{
    public class ViewModel
    {
        public double? tempQuantity {  get; set; }
        public List<SelectListItem>? Products { get; set; }
        public List<SelectListItem>? Organizations { get; set; }
        public List<SelectListItem>? Customers { get; set; }
        public BuyProductRequest? Request { get; set; }
        public BuyProductResponse? Response { get; set; }
        public SellProductRequest? SellRequest { get; set; }
        public SellProductResponse? SellResponse { get; set; } 
        public ReturnOutRequest? ReturnOutRequest { get; set; }
        public ReturnOutResponse? ReturnOutResponse { get; set; } 
        public ReturnInRequest? ReturnInRequest { get; set; }
        public ReturnInResponse? ReturnInResponse { get; set; }

    }

}

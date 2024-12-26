using AgroPharm.Models.Request;
using AgroPharm.Models.Response;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AgroPharm.Models
{
    public class BuyProductViewModel
    {
        public List<SelectListItem> Products { get; set; }
        public List<SelectListItem> Organizations { get; set; }
        public BuyProductRequest Request { get; set; }
        public BuyProductResponse Response { get; set; }
    }

}

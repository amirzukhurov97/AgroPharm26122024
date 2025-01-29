using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AgroPharm.Models.Buy
{
    public class BuyProductRequest : EntityBase
    {
        public int ProductNameID { get; set; }
        public int OrganizationNameID { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Введите корректную цену.")]
        public decimal? BuyProductPrice { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Введите корректную цену.")]
        public decimal? BuyProductPriceUSD { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Введите корректное количество.")]
        public double? BuyProductObem { get; set; }
        public decimal? BuyProductSumPrice { get; set; }
        public decimal? BuyProductSumPriceUSD { get; set; }
        public DateTime? BuyProductDate { get; set; } = DateTime.Now;
        public string? BuyComment { get; set; } = string.Empty;
    }
}

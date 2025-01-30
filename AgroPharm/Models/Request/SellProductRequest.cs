using System.ComponentModel.DataAnnotations;

namespace AgroPharm.Models.Request
{
    public class SellProductRequest : EntityBase
    {
        public int ProductNameID { get; set; }
        public int CustomerNameID { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Введите корректную цену.")]
        public decimal? SellProductPrice { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Введите корректную цену.")]
        public decimal? SellProductPriceUSD { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Введите корректное количество.")]
        public double? SellProductObem { get; set; }
        public decimal? SellProductSumPrice { get; set; }
        public decimal? SellProductSumPriceUSD { get; set; }
        public DateTime SellProductDate { get; set; } = DateTime.Now;
        public string? SellComment { get; set; } = string.Empty;
    }
}

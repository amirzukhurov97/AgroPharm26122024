using System.ComponentModel.DataAnnotations;

namespace AgroPharm.Models.ReturnIn
{
    public class ReturnInRequest : EntityBase
    {
        public int ProductNameID { get; set; }
        public int CustomerNameID { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Введите корректную цену.")]
        public decimal? ReturnInProductPrice { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Введите корректную цену.")]
        public decimal? ReturnInProductPriceUSD { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Введите корректное количество.")]
        public double? ReturnInProductObem { get; set; }
        public decimal? ReturnInProductSumPrice { get; set; }
        public decimal? ReturnInProductSumPriceUSD { get; set; }
        public DateTime ReturnInProductDate { get; set; } = DateTime.Now;
        public string? ReturnInComment { get; set; } = string.Empty;
    }
}

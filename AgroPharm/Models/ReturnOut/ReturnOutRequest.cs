using System.ComponentModel.DataAnnotations;

namespace AgroPharm.Models.ReturnOut
{
    public class ReturnOutRequest : EntityBase
    {
        public int ProductNameID { get; set; }
        public int OrganizationNameID { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Введите корректную цену.")]
        public decimal? ReturnOutProductPrice { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Введите корректную цену.")]
        public decimal? ReturnOutProductPriceUSD { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Введите корректное количество.")]
        public double? ReturnOutProductObem { get; set; }
        public decimal? ReturnOutProductSumPrice { get; set; }
        public decimal? ReturnOutProductSumPriceUSD { get; set; }
        public DateTime ReturnOutProductDate { get; set; } = DateTime.Now;
        public string? ReturnOutComment { get; set; } = string.Empty;
    }
}

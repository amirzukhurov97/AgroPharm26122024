using System.ComponentModel.DataAnnotations;

namespace AgroPharm.Models
{
    public class Product : EntityBase
    {
        [Required(ErrorMessage = "Название продукта обязательно")]
        [StringLength(100, ErrorMessage = "Название не может быть длиннее 100 символов")]
        public string ProductName { get; set; } = string.Empty;
    }
}

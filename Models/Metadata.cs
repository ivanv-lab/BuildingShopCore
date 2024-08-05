using System.ComponentModel.DataAnnotations;

namespace BuildingShopCore.Models
{
    public class OrderMetadata
    {
        [Display(Name = "Клиент")]
        public int ClientId { get; set; }

        [Display(Name = "Сотрудник")]
        public int EmployeeId { get; set; }

        public bool Ready { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
    }

    public class ProductCategoryMetadata
    {
        [RegularExpression(@"^[^\d]+$", ErrorMessage = "Поле не должно содержать цифры.")]
        [Required]
        public string Name { get; set; }
    }

    public class ClientMetadata
    {
        [Required]
        [Display(Name = "Номер телефона")]
        [StringLength(16, MinimumLength = 16, ErrorMessage = "Длина номера телефона должна быть 16 символов")]
        public string Phone { get; set; }

        [Required]
        [RegularExpression(@"^[^\d]+$", ErrorMessage = "Поле не должно содержать цифры.")]
        public string FIO { get; set; }
    }

    public class EmployeeMetadata
    {
        [Required]
        [RegularExpression(@"^[^\d]+$", ErrorMessage = "Поле не должно содержать цифры.")]
        public string FIO { get; set; }

        [Required]
        [Display(Name = "Номер телефона")]
        [StringLength(16, MinimumLength = 16, ErrorMessage = "Длина номера телефона должна быть 16 символов")]
        public string Phone { get; set; }
    }

    public class OrderProductMetadata
    {
        [Required]
        [Display(Name = "Товар")]
        public int ProductId { get; set; }
        [Display(Name = "Заказ")]
        public int OrderId { get; set; }

        [Required]
        [Display(Name = "Количество")]
        [Range(1, 100)]
        [RegularExpression(@"^\d+$", ErrorMessage = "Поле должно содержать только цифры")]
        public int Count { get; set; }
    }

    public class ProductMetadata
    {
        [Display(Name = "Категория")]
        public int CategoryId { get; set; }
        [Required]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Страна-производитель")]
        [RegularExpression(@"^[^\d]+$", ErrorMessage = "Поле не должно содержать цифры.")]
        public string CountryProd { get; set; }

        [Required]
        [Display(Name = "Производитель")]
        public string Prod { get; set; }

        [Required]
        [Display(Name = "Цена")]
        [Range(0, 1_000_000)]
        [RegularExpression("^[0-9!@#$%^&*().]+$", ErrorMessage = "Поле должно содержать только цифры.")]
        public decimal Price { get; set; }

        [Display(Name = "Категория товара")]
        public virtual ProductCategory Category { get; set; }

        [Required]
        [Display(Name = "Количество")]
        [Range(0, 100)]
        [RegularExpression(@"^\d+$", ErrorMessage = "Поле должно содержать только цифры")]
        public int Count { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuildingShopCore.Models
{
    public class Product
    {
        public Product()
        {
            this.ProductList=new HashSet<OrderProduct>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [ForeignKey("ProductCategories")]
        public Nullable<int> CategoryId {  get; set; }
        [Required]
        [Range(0,1_000_000)]
        [RegularExpression("^[0-9!@#$%^&*().]+$", ErrorMessage = "Поле должно содержать только цифры.")]
        public decimal Price { get; set; }
        public string CountryProd { get; set; }
        public string Prod {  get; set; }
        public int Count {  get; set; }
        public bool IsDeleted { get; set; }=false;
        //public ProductCategory? Category { get; set; }
        public virtual ProductCategory? Category { get; set; }
        public ICollection<OrderProduct> ProductList { get; set; }
    }
}

namespace BuildingShopCore.Models
{
    public partial class ProductCategory
    {
        public ProductCategory()
        {
            this.Products=new HashSet<Product>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; } = false;
        public virtual ICollection<Product> Products { get; set; }
    }
}

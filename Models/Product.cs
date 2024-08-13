namespace BuildingShopCore.Models
{
    public partial class Product
    {
        public Product()
        {
            this.ProductList=new HashSet<OrderProduct>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId {  get; set; }
        public decimal Price { get; set; }
        public string CountryProd { get; set; }
        public string Prod {  get; set; }
        public int Count {  get; set; }
        public bool IsDeleted { get; set; }=false;
        public virtual ProductCategory Category { get; set; }
        public ICollection<OrderProduct> ProductList { get; set; }
    }
}

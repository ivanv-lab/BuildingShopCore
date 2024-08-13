namespace BuildingShopCore.Models
{
    public partial class OrderProduct
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int OrderId {  get; set; }
        public int Count {  get; set; }
        public decimal Price {  get; set; }
        public bool IsDeleted { get; set; }=false;
        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}

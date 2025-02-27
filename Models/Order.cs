﻿namespace BuildingShopCore.Models
{
    public partial class Order
    {
        public Order()
        {
            this.ProductList=new HashSet<OrderProduct>();
        }
        public int Id { get; set; }
        public int ClientId {  get; set; }
        public int EmployeeId {  get; set; }
        public bool Ready {  get; set; }
        public System.DateTime Date { get; set; }
        public decimal Sum {  get; set; }
        public bool IsDeleted { get; set; }
        public virtual Client Client { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual ICollection<OrderProduct> ProductList { get; set; }
    }
}

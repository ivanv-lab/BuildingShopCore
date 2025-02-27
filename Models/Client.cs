﻿namespace BuildingShopCore.Models
{
    public partial class Client
    {
        public Client()
        {
            this.Orders=new HashSet<Order>();
        }
        public int Id { get; set; }
        public string FIO { get; set; }
        public string Address {  get; set; }
        public string Phone {  get; set; }
        public bool IsDeleted {  get; set; }=false;
        public virtual ICollection<Order> Orders { get; set; }
    }
}

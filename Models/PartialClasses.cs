using System.ComponentModel.DataAnnotations;

namespace BuildingShopCore.Models
{
    [MetadataType(typeof(ClientMetadata))]
    public partial class Client { }
    [MetadataType(typeof(EmployeeMetadata))]
    public partial class Employee { }
    [MetadataType(typeof(OrderMetadata))]
    public partial class Order { }
    [MetadataType(typeof(ProductMetadata))]
    public partial class Product { }
    [MetadataType(typeof(OrderProductMetadata))]
    public partial class OrderProduct { }
    [MetadataType(typeof(ProductCategoryMetadata))]
    public partial class ProductCategory { }
}

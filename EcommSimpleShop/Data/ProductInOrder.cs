using System.ComponentModel.DataAnnotations.Schema;

namespace EcommSimpleShop.Data
{
    [Table("OrderProducts")]
    public class ProductInOrder
    {
        public Order Order { get; set; } = null!;
        public int OrderId { get; set; }
        public Product Product { get; set; } = null!;
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
    }
}
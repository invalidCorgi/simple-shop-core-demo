using EcommSimpleShop.Data;

namespace EcommSimpleShop.Models
{
    public class ItemInCartModel
    {
        public Product Product { get; set; } = null!;
        public int Quantity { get; set; }
    }
}
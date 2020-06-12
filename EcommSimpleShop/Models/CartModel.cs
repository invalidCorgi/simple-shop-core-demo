using System.Collections.Generic;

namespace EcommSimpleShop.Models
{
    public class CartModel
    {
        public IList<CartItemModel> Items { get; set; } = new List<CartItemModel>();
    }
}
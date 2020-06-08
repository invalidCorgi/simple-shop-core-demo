using System.ComponentModel.DataAnnotations.Schema;

namespace EcommSimpleShop.Data
{
    [Table("Products")]
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Price { get; set; }
        public string Description { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public int Quantity { get; set; }
    }
}
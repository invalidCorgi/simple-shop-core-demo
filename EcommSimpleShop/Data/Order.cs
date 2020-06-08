using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace EcommSimpleShop.Data
{
    [Table("Orders")]
    public class Order
    {
        public int Id { get; set; }
        public User User { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public ICollection<ProductInOrder> Products { get; set; } = null!;
    }
}
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace EcommSimpleShop.Data
{
    public class User : IdentityUser
    {
        public ICollection<Order> Order { get; set; } = null!;
    }
}
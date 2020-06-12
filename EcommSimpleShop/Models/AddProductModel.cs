using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace EcommSimpleShop.Models
{
    public class AddProductModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public double Price { get; set; }
        public string? Description { get; set; }
        public IFormFile? Image { get; set; }
        public string? ImageUrl { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
}
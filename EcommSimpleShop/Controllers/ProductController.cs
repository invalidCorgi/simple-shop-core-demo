using System.Linq;
using EcommSimpleShop.Data;
using Microsoft.AspNetCore.Mvc;

namespace EcommSimpleShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        
        public ProductController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Index(int page = 0)
        {
            const int productsPerPage = 10;
            
            var products = _dbContext
                .Products
                .Skip(page * productsPerPage)
                .Take(productsPerPage)
                .ToList();
            
            return View(products);
        }
    }
}
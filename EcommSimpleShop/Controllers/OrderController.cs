using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EcommSimpleShop.Data;
using EcommSimpleShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommSimpleShop.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        
        // GET
        public OrderController(ApplicationDbContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var orders = await _dbContext
                .Orders
                .Where(x => x.UserId == _userManager.GetUserId(User))
                .Include(x => x.Products)
                .ThenInclude(x => x.Product)
                .ToListAsync();
            
            return View(orders);
        }

        [HttpGet]
        public IActionResult Payment(int id)
        {
            var amount = _dbContext
                .OrderProducts
                .Where(x => x.OrderId == id)
                .Sum(x => x.Quantity * x.Price) / 100.0;
            ViewBag.Amount = amount.ToString("N2");
            ViewBag.OrderId = id;
            
            return View();
        }
    }
}
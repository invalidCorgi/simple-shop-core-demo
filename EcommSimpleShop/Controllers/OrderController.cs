using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
            ViewBag.Description = $"Payment for order {id}";
            
            using (var hasher = SHA256.Create())
            {
                var inputString = "dTiQD9vwBr3sAC2HsYgEY5Vjh3yNswaP" +
                                  "754872" +
                                  ViewBag.Amount +
                                  "PLN" +
                                  ViewBag.Description +
                                  "https://localhost:5001/Order/Confirmation" +
                                  "0";

                var inputBytes = Encoding.UTF8.GetBytes(inputString);

                var hash = hasher.ComputeHash(inputBytes);
                
                var builder = new StringBuilder();

                foreach (var hashByte in hash)
                {
                    builder.Append(hashByte.ToString("X2"));
                }

                ViewBag.Chk = builder.ToString().ToLower();
            }
            
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        [HttpPost]
        public IActionResult Confirmation(string status)
        {
            ViewBag.Ok = status == "OK";
            
            return View();
        }
    }
}
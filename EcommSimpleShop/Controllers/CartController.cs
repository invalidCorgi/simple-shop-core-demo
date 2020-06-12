using System.Collections.Generic;
using System.Linq;
using Core.Flash;
using EcommSimpleShop.Data;
using EcommSimpleShop.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommSimpleShop.Controllers
{
    public class CartController : Controller
    {
        private readonly IFlasher _flasher;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _dbContext;
        private ISession Session => _httpContextAccessor.HttpContext.Session;
        
        public CartController(IFlasher flasher, IHttpContextAccessor httpContextAccessor, ApplicationDbContext dbContext)
        {
            _flasher = flasher;
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var cart = Session.GetObjectFromJson<CartModel>("Cart");

            if (cart == default)
            {
                cart = new CartModel();
                Session.SetObjectAsJson("Cart", cart);
            }
            
            var mapperCart = cart
                .Items
                .Select(cartItem => new ItemInCartModel
                {
                    Product = _dbContext.Products.FirstOrDefault(x => x.Id == cartItem.Id),
                    Quantity = cartItem.Quantity
                })
                .ToList();

            mapperCart = mapperCart.Where(x => x.Product != default).ToList();

            return View(mapperCart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(int id, int quantity, string returnUrl)
        {
            var cart = Session.GetObjectFromJson<CartModel>("Cart") ?? new CartModel();

            var product = cart.Items.FirstOrDefault(x => x.Id == id);
            if (product == default)
            {
                product = new CartItemModel
                {
                    Id = id,
                    Quantity = 0
                };
                cart.Items.Add(product);
            }

            product.Quantity += quantity;
            Session.SetObjectAsJson("Cart", cart);
            _flasher.Flash(Types.Success, "Successfully added item to cart");

            return Redirect(returnUrl);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int id)
        {
            var cart = Session.GetObjectFromJson<CartModel>("Cart") ?? new CartModel();
            
            var product = cart.Items.FirstOrDefault(x => x.Id == id);
            if (product != default)
            {
                cart.Items.Remove(product);
            }
            
            Session.SetObjectAsJson("Cart", cart);
            _flasher.Flash(Types.Success, "Successfully removed item from cart");
            
            return RedirectToAction("Index");
        }
    }
}
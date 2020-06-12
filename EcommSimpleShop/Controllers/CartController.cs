using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Flash;
using EcommSimpleShop.Data;
using EcommSimpleShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EcommSimpleShop.Controllers
{
    public class CartController : Controller
    {
        private readonly IFlasher _flasher;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private ISession Session => _httpContextAccessor.HttpContext.Session;

        public CartController(IFlasher flasher, IHttpContextAccessor httpContextAccessor,
            ApplicationDbContext dbContext, UserManager<User> userManager)
        {
            _flasher = flasher;
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
            _userManager = userManager;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            var cart = Session.GetObjectFromJson<CartModel>("Cart") ?? new CartModel();

            if (!cart.Items.Any())
            {
                _flasher.Flash(Types.Danger, "You don't have any products in cart");
                return RedirectToAction("Index");
            }

            var order = new Order
            {
                UserId = _userManager.GetUserId(User),
                Products = new List<ProductInOrder>()
            };

            foreach (var cartItem in cart.Items)
            {
                var product = await _dbContext.Products.FindAsync(cartItem.Id);
                if (product.Quantity < cartItem.Quantity)
                {
                    _flasher.Flash(Types.Danger, $"There is less {product.Name} in stock than you want to buy");
                    return RedirectToAction("Index");
                }

                product.Quantity -= cartItem.Quantity;

                order.Products.Add(new ProductInOrder
                {
                    Price = product.Price,
                    Quantity = cartItem.Quantity,
                    Product = product
                });
            }

            await _dbContext.Orders.AddAsync(order);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _flasher.Flash(Types.Danger, "Internal error occured during processing of your order, try again");
                return RedirectToAction("Index");
            }
            
            cart = new CartModel();
            Session.SetObjectAsJson("Cart", cart);

            return RedirectToAction("Payment", "Order", new {id = order.Id});
        }
    }
}
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EcommSimpleShop.Data;
using EcommSimpleShop.Models;
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

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddProductModel model)
        {
            if (!ModelState.IsValid) 
                return View(model);
            
            if (model.Image != null)
            {
                if (!Directory.Exists("Images"))
                {
                    Directory.CreateDirectory("Images");
                }

                var imgPath = $"/Images/{model.Image.FileName}";
                await using (var stream = new FileStream($"wwwroot{imgPath}", FileMode.Create))
                {
                    await model.Image.CopyToAsync(stream);
                }

                model.ImageUrl = imgPath;
            }

            await _dbContext.Products.AddAsync(new Product
            {
                Name = model.Name,
                Description = model.Description ?? "",
                Price = (int) Math.Round(model.Price * 100),
                Quantity = model.Quantity,
                ImageUrl = model.ImageUrl
            });

            await _dbContext.SaveChangesAsync();
            
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var model = _dbContext.Products.Find(id);
            return View(model);
        }
        
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var rawModel = _dbContext.Products.Find(id);
            
            var model = new AddProductModel
            {
                Id = rawModel.Id,
                Description = rawModel.Description,
                Name = rawModel.Name,
                Price = rawModel.Price / 100.0,
                Quantity = rawModel.Quantity,
                ImageUrl = rawModel.ImageUrl
            };
            
            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AddProductModel model)
        {
            if (!ModelState.IsValid) 
                return View(model);
            
            if (model.Image != null)
            {
                if (!Directory.Exists("Images"))
                {
                    Directory.CreateDirectory("Images");
                }

                var imgPath = $"/Images/{model.Image.FileName}";
                await using (var stream = new FileStream($"wwwroot{imgPath}", FileMode.Create))
                {
                    await model.Image.CopyToAsync(stream);
                }

                model.ImageUrl = imgPath;
            }

            var dbModel = await _dbContext.Products.FindAsync(model.Id);

            dbModel.Description = model.Description ?? "";
            dbModel.Name = model.Name;
            dbModel.Price = (int) Math.Round(model.Price * 100);
            dbModel.Quantity = model.Quantity;
            dbModel.ImageUrl = model.ImageUrl;

            await _dbContext.SaveChangesAsync();
            
            return RedirectToAction("Index");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _dbContext.Products.FindAsync(id);
            _dbContext.Products.Remove(model);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}

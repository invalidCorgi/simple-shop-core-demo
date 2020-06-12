using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Flash;
using EcommSimpleShop.Data;
using EcommSimpleShop.Models;
using Microsoft.AspNetCore.Mvc;

namespace EcommSimpleShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IFlasher _flasher;

        public ProductController(ApplicationDbContext dbContext, IFlasher flasher)
        {
            _dbContext = dbContext;
            _flasher = flasher;
        }

        [HttpGet]
        public IActionResult Index(int page = 1)
        {
            const int productsPerPage = 4;

            var products = _dbContext
                .Products
                .Skip((page - 1) * productsPerPage)
                .Take(productsPerPage)
                .ToList();

            ViewBag.Page = page;
            ViewBag.MaxPages = _dbContext.Products.Count() / productsPerPage + 1;

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

            _flasher.Flash(Types.Success, "Product successfully added");

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

            _flasher.Flash(Types.Success, "Product successfully updated");

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _dbContext.Products.FindAsync(id);
            _dbContext.Products.Remove(model);
            await _dbContext.SaveChangesAsync();

            _flasher.Flash(Types.Success, "Product successfully deleted");

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Populate()
        {
            var random = new Random();
            const string description =
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";

            _dbContext.Products.Add(new Product
            {
                Name = "Bubble woods",
                Price = random.Next(100, 10000),
                Quantity = random.Next(10, 100),
                Description = description,
                ImageUrl = "https://img.cdn.famobi.com/portal/html5games/images/tmp/256/BubbleWoodsTeaser.jpg"
            });

            _dbContext.Products.Add(new Product
            {
                Name = "Clash of orcs",
                Price = random.Next(100, 10000),
                Quantity = random.Next(10, 100),
                Description = description,
                ImageUrl = "https://www.yupi.io/files/image/1574190396.jpg"
            });

            _dbContext.Products.Add(new Product
            {
                Name = "Link's Awakening",
                Price = random.Next(100, 10000),
                Quantity = random.Next(10, 100),
                Description = description,
                ImageUrl = "https://yuzu-emu.org/images/game/boxart/the-legend-of-zelda-links-awakening.png"
            });

            _dbContext.Products.Add(new Product
            {
                Name = "Jumanji",
                Price = random.Next(100, 10000),
                Quantity = random.Next(10, 100),
                Description = description,
                ImageUrl =
                    "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTnnLGEIL1VcBWvWdzqw8GPcqTZZ8UzQgHHU3Bw2IF3GGX8bZNX&s"
            });

            _dbContext.Products.Add(new Product
            {
                Name = "Pokemon trade",
                Price = random.Next(100, 10000),
                Quantity = random.Next(10, 100),
                Description = description,
                ImageUrl = "https://pokeserwis.pl/wp-content/uploads/2015/05/TCG_EN_boxart.png"
            });

            _dbContext.Products.Add(new Product
            {
                Name = "War brokers",
                Price = random.Next(100, 10000),
                Quantity = random.Next(10, 100),
                Description = description,
                ImageUrl = "https://warbrokers.io/img/regular_wb_button.png"
            });

            _dbContext.Products.Add(new Product
            {
                Name = "Gwent",
                Price = random.Next(100, 10000),
                Quantity = random.Next(10, 100),
                Description = description,
                ImageUrl =
                    "https://dl1.cbsistatic.com/i/2017/06/20/79be9e48-48e2-443d-96db-16c014500eba/48c8e2f05843ee6f0005c764f9d61c64/imgingest-3094869985329463841.png"
            });

            _dbContext.Products.Add(new Product
            {
                Name = "Metro 2033 Redux",
                Price = random.Next(100, 10000),
                Quantity = random.Next(10, 100),
                Description = description,
                ImageUrl = "https://lowcygier.pl/wp-content/uploads/2019/09/Metro-2033-Redux.jpg"
            });

            _dbContext.Products.Add(new Product
            {
                Name = "Animal Crossing: New Horizons",
                Price = random.Next(100, 10000),
                Quantity = random.Next(10, 100),
                Description = description,
                ImageUrl = "https://yuzu-emu.org/images/game/boxart/animal-crossing-new-horizons.png"
            });

            _dbContext.Products.Add(new Product
            {
                Name = "Risk of rain 2",
                Price = random.Next(100, 10000),
                Quantity = random.Next(10, 100),
                Description = description,
                ImageUrl =
                    "https://gamepedia.cursecdn.com/riskofrain2_gamepedia_en/thumb/7/7f/Early_Access_OST_Cover.jpg/256px-Early_Access_OST_Cover.jpg?version=89c58e87b75aa3565ffcbc88912de302"
            });

            _dbContext.Products.Add(new Product
            {
                Name = "Ori and the will of the wisps",
                Price = random.Next(100, 10000),
                Quantity = random.Next(10, 100),
                Description = description,
                ImageUrl = "https://a.wattpad.com/useravatar/oriwipspc.256.143841.jpg"
            });

            _dbContext.Products.Add(new Product
            {
                Name = "The game of life",
                Price = random.Next(100, 10000),
                Quantity = random.Next(10, 100),
                Description = description,
                ImageUrl =
                    "https://vignette.wikia.nocookie.net/boardgamemanuals/images/5/5a/39cf3c7e83feedbed4f3101b52d7a120.jpg/revision/latest/scale-to-width-down/340?cb=20191102125348"
            });

            _dbContext.Products.Add(new Product
            {
                Name = "Warioland 4",
                Price = random.Next(100, 10000),
                Quantity = random.Next(10, 100),
                Description = description,
                ImageUrl = "https://4.allegroimg.com/s1024/0c4942/5d69bb31411d89933c89c4f17264"
            });

            _dbContext.Products.Add(new Product
            {
                Name = "Bayonetta 2",
                Price = random.Next(100, 10000),
                Quantity = random.Next(10, 100),
                Description = description,
                ImageUrl =
                    "https://dt2t1o4a01q3k.cloudfront.net/assets/games/eshop_icons/switch/bayonetta-2-5126af6af346ae0f946e8682124a5c92834cbe7f90cb6a81be5b9bd68e6746d1.png"
            });

            _dbContext.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
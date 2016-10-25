using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jenpbiz.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Jenpbiz.Controllers
{
    public class ProductController : Controller
    {
        private readonly JenpbizContext _context = new JenpbizContext();

        // GET: Product
        public async  Task<ActionResult> Index()
        {
            ViewBag.Title = "Product";
            var count = _context.Products.Count();

            Product p = new Product() 
            {
                ProductTitle = "Banana",
                PruductDescription = "Yellow banana",
                ProductPrice = 23,
                ProductLink = "www.banana.com",
                ProductImageLink = "www.banana/img/banana01.png"
                
            };

            _context.Products.Add(p);
            _context.SaveChanges();

            randomProducts();

            return View(_context.Products.ToList());
        }

        public void randomProducts()
        {

            Random rnd = new Random();
            string[] randomTitles = { "Apple", "Banana", "Iphone 7", "NIKE Sneakers", "Trumpet" };
            int productTitleIndex = 0;


            for (int i = 0; i < 3; i++)
            {
                productTitleIndex = rnd.Next(0, randomTitles.Length - 1);
                _context.Products.Add(new Product()
                {
                    ProductTitle = randomTitles[productTitleIndex],
                    ProductPrice = (uint)rnd.Next(1, 1000),
                    ProductLink = "https://www.example.com/" + rnd.Next(50000, 200000) + "/" + randomTitles[productTitleIndex] + "/",
                    ProductImageLink = "https://www.example.com/" + rnd.Next(50000, 200000) + "/" + randomTitles[productTitleIndex] + ".jpg",
                    ProductDescription = "I am a " + randomTitles[productTitleIndex]
                });
                
            }

            _context.SaveChanges();
            return;
        }
    }
}
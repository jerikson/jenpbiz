using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Jenpbiz.Models;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.ShoppingContent.v2;
using System.Net.Http;

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

            // Add some placeholder products if there are none
            if (!_context.Products.Any())
            { 
                RandomProducts();
            }
            return View(_context.Products.ToList());
        }

        public ActionResult GetProduct(int id)
        {
            Product p = _context.Products.Find(id);
            if (p == null) {
                return RedirectToAction("/Index", "Product");
            }
            return View(p);
        }

        public ActionResult DeleteProduct(int id)
        {
            Product p = _context.Products.Find(id);
            if (p != null) {
                _context.Products.Remove(p);
                _context.SaveChanges();
            }
            return RedirectToAction("/Index", "Product");
        }

        public void RandomProducts()
        {
            Random rnd = new Random();
            string[] randomTitles = { "Apple", "Banana", "Iphone 7", "NIKE Sneakers", "Trumpet" };
            string[] randomCurrencies = { "SEK", "USD", "GBP", "AUD" };
            int productTitleIndex = 0;
            int productCurrencyIndex = 0;


            for (int i = 0; i < 3; i++)
            {
                productTitleIndex = rnd.Next(0, randomTitles.Length - 1);
                productCurrencyIndex = rnd.Next(0, randomCurrencies.Length - 1);

                Product newProduct = new Product()
                {
                    ProductTitle = randomTitles[productTitleIndex],
                    ProductLink = "https://www.example.com/" + rnd.Next(50000, 200000) + "/" + randomTitles[productTitleIndex] + "/",
                    ProductImageLink = "https://www.example.com/" + rnd.Next(50000, 200000) + "/" + randomTitles[productTitleIndex] + ".jpg",
                    ProductDescription = "I am a " + randomTitles[productTitleIndex],

                };

                Price randomPrice = new Price() { PriceCurrency = randomCurrencies[productCurrencyIndex], PriceValue = rnd.Next(100, 1000), Product = newProduct };
                newProduct.Price = randomPrice;

                _context.Products.Add(newProduct);
            }
                

            _context.SaveChanges();
            return;
        }

        
        // 113298073
        // AIzaSyBwAM56fn0HOMYZehTLcNCTGVPzYauEEs8
        public ActionResult GetMerchantProduct()
        {
            Debug.WriteLine("asd");
            
            WebRequest request = WebRequest.Create("https://www.googleapis.com/content/v2/113298073/products?includeInvalidInsertedItems=true&key={AIzaSyBwAM56fn0HOMYZehTLcNCTGVPzYauEEs8}");
            
            //Stream dataStream = request.GetResponse().GetResponseStream();
            //StreamReader reader = new StreamReader(dataStream);
            WebResponse response = request.GetResponse();
            //string response = reader.ReadToEnd();

            return Json(response);
        

            //using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            //{
            //    client.BaseAddress = new Uri("");
            //    HttpResponseMessage response = client.GetAsync("").Result;
            //    response.EnsureSuccessStatusCode();
            //    string result = response.Content.ReadAsStringAsync().Result;
            //    Console.WriteLine("Result: " + result);
            //}

        }



      
    }
}
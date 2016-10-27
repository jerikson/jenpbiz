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

        public ActionResult GetProduct(int? id)
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
            string[] randomTitles = { "Apple", "Banana", "Iphone 7", "Nike Sneakers", "Trumpet" };
            string[] randomCurrencies = { "SEK", "USD", "GBP", "AUD" };
            string[] randomBrands = { "Microsoft", "Apple", "Nike", "Adidas", "Eldorado" };
            int productTitleIndex = 0;
            int productCurrencyIndex = 0;
            int productBrandIndex = 0;


            for (int i = 0; i < 5; i++)
            {
                productTitleIndex = rnd.Next(0, randomTitles.Length);
                productCurrencyIndex = rnd.Next(0, randomCurrencies.Length);
                productBrandIndex = rnd.Next(0, randomBrands.Length);

                Product newProduct = new Product()
                {
                    ProductTitle = randomTitles[productTitleIndex],
                    ProductLink = "https://www.example.com/" + rnd.Next(50000, 200001) + "/" + randomTitles[productTitleIndex] + "/",
                    ProductImageLink = "https://www.example.com/" + rnd.Next(50000, 200001) + "/" + randomTitles[productTitleIndex] + ".jpg",
                    ProductDescription = "I am a " + randomTitles[productTitleIndex],
                    ProductAvailability = (Product.ProductAvailabilityEnum)rnd.Next(0, 3),
                    ProductCategory = (Product.ProductCategoryEnum)rnd.Next(0, 6),
                    ProductCondition = (Product.ProductConditionEnum)rnd.Next(0, 3),
                    ProductGtin = GtinGenerator("Europe", rnd)

                };

                Price randomPrice = new Price() { PriceCurrency = randomCurrencies[productCurrencyIndex], PriceValue = rnd.Next(100, 1000), Product = newProduct };
                newProduct.Price = randomPrice;

                if(newProduct.ProductCategory != Product.ProductCategoryEnum.Books)
                {
                    newProduct.ProductBrand = randomBrands[productBrandIndex];

                }

                if (newProduct.ProductAvailability == Product.ProductAvailabilityEnum.Pre_order)
                {
                    newProduct.ProductAvailabilityDate = DateTime.Now.AddDays(3);

                }

                _context.Products.Add(newProduct);
            }
                

            _context.SaveChanges();
            return;
        }

        public string GtinGenerator(string region, Random rnd)
        {
            //Generates a gtin for a continent
            string gtin = "";

            if (region == "Europe")
            {
                gtin += rnd.Next(100, 1000);
                gtin += rnd.Next(100, 1000);
                gtin += rnd.Next(100, 1000);
                gtin += rnd.Next(100, 1000);
                gtin += rnd.Next(0, 10);

            }
            else if (region == "North America")
            {
                gtin += rnd.Next(100, 1000);
                gtin += rnd.Next(100, 1000);
                gtin += rnd.Next(100, 1000);
                gtin += rnd.Next(100, 1000);

            }

            return gtin;
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
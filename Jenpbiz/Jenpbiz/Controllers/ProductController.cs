using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jenpbiz.Models;

namespace Jenpbiz.Controllers
{
    public class ProductController : Controller
    {
        private readonly JenpbizContext _context = new JenpbizContext();

        // GET: Product
        public ActionResult Index()
        {
            ViewBag.Title = "Product";
            var count = _context.Products.Count();
            /*
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
      */


            return View(_context.Products.ToList());
        }
    }
}
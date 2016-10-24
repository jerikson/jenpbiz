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
            var count = _context.Products.Count();

            ViewBag.Title = "Product";
            return View();
        }
    }
}
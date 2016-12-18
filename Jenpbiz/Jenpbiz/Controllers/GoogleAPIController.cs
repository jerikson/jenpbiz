using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.ShoppingContent.v2;
using System;
using System.Diagnostics;
using Jenpbiz.Models;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Jenpbiz.Controllers
{
    public class GoogleApiController : Controller
    {
        private static string CLIENT_ID = "896777409399-ghva93bgs7qpv293tqj1vp4eefi7n82c.apps.googleusercontent.com";
        private static string CLIENT_SECRET = "Or7cg3mMtWmMsxIhBjecHcRq";
        private static ulong MERCHANT_ID = 113298073;
        private int unique_id_increment = 0;
        internal string Url2 = "URL HERE, BRO";

        internal GoogleApi googleObject = new GoogleApi(113298073);

        // GET: GoogleAPI
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetProduct(int? maxResults, int? page)
        {
            ViewBag.Title = "Products";

            if (maxResults == null)
                maxResults = 5;
            if (page == null)
                page = 1;

            List<Google.Apis.ShoppingContent.v2.Data.Product> productsResponseList = googleObject.ProductsReturn(maxResults, page);
            List<Google.Apis.ShoppingContent.v2.Data.ProductStatus> productStatusesResponseList = googleObject.ProductStatusesReturn(maxResults, page);

            if (productsResponseList != null && productStatusesResponseList != null)
            {
                Jenpbiz.Models.GoogleLists fullProductInfo = new Models.GoogleLists()
                {
                    Products = productsResponseList,
                    ProductsStatuses = productStatusesResponseList
                };

                return View(fullProductInfo);
            }

            return View();
        }

        public ActionResult InsertProduct()
        {
            googleObject.ProductInsert(Url2);

            return RedirectToAction("/GetProduct", "GoogleApi");
        }

        public ActionResult DeleteProduct(string productId)
        {
            googleObject.ProductDelete(Url2);

            return RedirectToAction("/GetProduct", "GoogleApi");
        }

        public ActionResult EditProduct()
        {
            googleObject.ProductInsert(Url2);

            return RedirectToAction("/GetProduct", "GoogleApi");
        }

        public ActionResult NextPageExists(int? maxResults, int? page)
        {
            bool pageExists = googleObject.NextPageExists(maxResults, page);

            return Json(new { exists = pageExists}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PreviousPageExists(int? maxResults, int? page)
        {
            bool pageExists = googleObject.PreviousPageExists(maxResults, page);

            return Json(new { exists = pageExists }, JsonRequestBehavior.AllowGet);
        }

        public void JSONStuff()
        {

            WebClient client = new WebClient();

            Stream data = client.OpenRead("http://one.dev.parttrap.com/catalog/getrelatedchildproducts/?stockCode=GOOGLE&relationId=4");
            StreamReader reader = new StreamReader(data);
            string result = reader.ReadToEnd();
            data.Close();
            reader.Close();

            string[] products = System.Text.RegularExpressions.Regex.Split(result, "{\"StockCode\":");

            foreach (string p in products)
            {
                Debug.WriteLine(p + "\n\n\n");
            }

            Debug.WriteLine(products.Count());

            object test = JsonConvert.DeserializeObject(result);

            Debug.WriteLine("obj test: " + test);
            Debug.WriteLine("obj test tostring: " + test.ToString());

            //Debug.WriteLine("RAW: " + result);
            //Debug.WriteLine("\n\n\n\n\n");
            //Debug.WriteLine("---------------END OF THE LINE---------------");
                
        }

        public ActionResult GetProductInfo(string productId)
        {
            UserCredential credential = Authenticate();
            ShoppingContentService service = CreateService(credential);

            ProductsResource.GetRequest accountRequest = service.Products.Get(MERCHANT_ID, productId);
            Google.Apis.ShoppingContent.v2.Data.Product selectedProduct = accountRequest.Execute();


            return Json(new { clickedProduct = selectedProduct },
                JsonRequestBehavior.AllowGet);
        }

        public UserCredential Authenticate()
        {
            string[] scopes = new string[] { ShoppingContentService.Scope.Content };

            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                          new ClientSecrets
                          {
                              ClientId = CLIENT_ID,
                              ClientSecret = CLIENT_SECRET
                          },
                          scopes,
                          "user",
                          CancellationToken.None).Result;

            return credential;
        }

        public ShoppingContentService CreateService(UserCredential credential)
        {
            var service = new ShoppingContentService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "jenpbiz",
            });

            return service;
        }

        internal String GetUniqueId()
        {
            unique_id_increment += 1;
            String unixTimestamp =
                ((Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString();
            return unixTimestamp + unique_id_increment.ToString();
        }

    }


}


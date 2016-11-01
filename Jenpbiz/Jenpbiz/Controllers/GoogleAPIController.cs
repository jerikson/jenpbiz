using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.ShoppingContent.v2;
using Google.Apis.ShoppingContent.v2.Data;
using System;
using System.Diagnostics;

namespace Jenpbiz.Controllers
{
    public class GoogleApiController : Controller
    {
        private static string CLIENT_ID = "896777409399-ghva93bgs7qpv293tqj1vp4eefi7n82c.apps.googleusercontent.com";
        private static string CLIENT_SECRET = "vq_UOyNpiO2q6uTb-QKcCykt";
        private static ulong MERCHANT_ID = 113298073;
        private static int unique_id_increment = 0;
        //private static ulong MCA_MERCHANT_ID = 0;
        //private static readonly int MaxListPageSize = 50;

        // GET: GoogleAPI
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetProduct()
        {
            ViewBag.Title = "Products";


            UserCredential credential = Authenticate();
            ShoppingContentService service = CreateService(credential);

            string pageToken = null;
            long maxResults = 10;
            ProductsListResponse productsResponse = null;
            do
            {
                ProductsResource.ListRequest accountRequest = service.Products.List(MERCHANT_ID);
                accountRequest.MaxResults = maxResults;
                accountRequest.PageToken = pageToken;
                accountRequest.IncludeInvalidInsertedItems = true;
                productsResponse = accountRequest.Execute();

                if (productsResponse.Resources != null && productsResponse.Resources.Count != 0)
                {

                }
                pageToken = productsResponse.NextPageToken;
            }
            while (pageToken != null);

            return View(productsResponse.Resources.ToList());
        }

        public ActionResult InsertProduct()
        {
            UserCredential credential = Authenticate();
            ShoppingContentService service = CreateService(credential);

            Debug.WriteLine("Title: " + Request["inputProductTitle"]);
            Debug.WriteLine("Description: " + Request["inputProductDescription"]);
            Debug.WriteLine("Link: " + Request["inputProductLink"]);
            Debug.WriteLine("Image link: " + Request["InputProductImageLink"]);
            Debug.WriteLine("Target Country: " + Request["selectProductTargetCountry"]);
            Debug.WriteLine("Availability: " + Request["selectProductAvailability"]);
            Debug.WriteLine("Condition: " + Request["selectProductCondition"]);
            Debug.WriteLine("Category: " + Request["selectProductCategory"]);
            Debug.WriteLine("Gtin: " + Request["inputProductGtin"]);

            string targetCountry = Request["selectProductTargetCountry"].ToUpper();

            Product newProduct = new Product()
            {
                OfferId = GetUniqueId(),
                Title = Request["inputProductTitle"],
                Description = Request["inputProductDescription"],
                Link = Request["inputProductLink"],
                ImageLink = Request["InputProductImageLink"],
                //ContentLanguage = Request["selectProductTargetCountry"].ToUpper(),
                TargetCountry = targetCountry,
                Channel = "online",
                Availability = Request["selectProductAvailability"],
                Condition = Request["selectProductCondition"],
                GoogleProductCategory = Request["selectProductCategory"],
                Gtin = Request["inputProductGtin"]
            };

            if (targetCountry == "SE")
            {
                newProduct.ContentLanguage = "sv";
            }
            else
            {
                newProduct.ContentLanguage = "en";
            }

            newProduct.Price = new Price()
            {
                Currency = "SEK",
                Value = Request["inputProductPrice"]
            };

            try
            {
                ProductsResource.InsertRequest accountRequest = service.Products.Insert(newProduct, MERCHANT_ID);
                //accountRequest.DryRun = true;
                accountRequest.Execute();
                Debug.WriteLine(newProduct.Title + newProduct.Description);
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Debug.WriteLine("EXCEPTION THROWN @114");
                System.Diagnostics.Debug.WriteLine("Message: " + Ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack Trace: " + Ex.StackTrace);
                System.Diagnostics.Debug.WriteLine("Target Site: " + Ex.TargetSite);
            }
            

            return RedirectToAction("/GetProduct", "GoogleApi");
        }

        public ActionResult DeleteProduct(string productId)
        {
            UserCredential credential = Authenticate();
            ShoppingContentService service = CreateService(credential);

            if (productId.Contains("_"))
            {
                productId = productId.Replace('_', ':');
            }


            try
            {
                ProductsResource.DeleteRequest accountRequest = service.Products.Delete(MERCHANT_ID, productId);
                //accountRequest.DryRun = true;
                accountRequest.Execute();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Debug.WriteLine("EXCEPTION THROWN @DeleteProduct()");
                System.Diagnostics.Debug.WriteLine("Message: " + Ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack Trace: " + Ex.StackTrace);
                System.Diagnostics.Debug.WriteLine("Target Site: " + Ex.TargetSite);
            }

            return RedirectToAction("/GetProduct", "GoogleApi");
            //return Json(new { successfullyDeleted = successfullyDeleted }, "json", JsonRequestBehavior.DenyGet);
        }

        public ActionResult getProductInfo(string productId)
        {
            UserCredential credential = Authenticate();
            ShoppingContentService service = CreateService(credential);

            ProductsResource.GetRequest accountRequest = service.Products.Get(MERCHANT_ID, productId);
            Product selectedProduct = accountRequest.Execute();


            return Json(new { clickedProduct = selectedProduct },
                JsonRequestBehavior.DenyGet);
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
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
        internal string Url = "http://one.dev.parttrap.com/catalog/getrelatedchildproducts/?stockCode=";
        internal string StockId = "GOOGLE&relationId=4";


        // GET: GoogleAPI
        public ActionResult Index()
        {
            List<Model.Product> productList = GetProduct2(Url, StockId);
            ViewBag.Stockcode = StockId;
            return View(productList);
        }


        public List<Model.Product> GetProduct2(string url, string stockId)
        {
            List<Model.Product> productList = new List<Model.Product>();
            WebRequest request = WebRequest.Create(url + stockId);
            Stream dataStream = request.GetResponse().GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string response = reader.ReadToEnd();
            JArray jsonObj = (JArray)JsonConvert.DeserializeObject(response);

            for (var i = 0; i < jsonObj.Count; i++)
            {
                Model.Product a = (Model.Product)JsonConvert.DeserializeObject(jsonObj[i].ToString(), typeof(Model.Product));
                productList.Add(a);
            }
            return productList;
        }



        public ActionResult GetProduct()
        {
            ViewBag.Title = "Products";

            UserCredential credential = Authenticate();
            ShoppingContentService service = CreateService(credential);

            bool firstRun = true;
            string pageToken = null;

            const long maxResults = 250;

            ProductsListResponse productsResponse = null;
            ProductstatusesListResponse productStatusesResponseList = null;

            while (pageToken != null || firstRun == true)
            {
                ProductsResource.ListRequest accountRequest = service.Products.List(MERCHANT_ID);
                accountRequest.MaxResults = maxResults;
                accountRequest.PageToken = pageToken;
                accountRequest.IncludeInvalidInsertedItems = true;
                productsResponse = accountRequest.Execute();

                if (productsResponse.Resources != null && productsResponse.Resources.Count != 0)
                {

                }
                firstRun = false;
                pageToken = productsResponse.NextPageToken;
            }

            if (productsResponse.Resources != null)
            {
                if (productsResponse.Resources != null && productStatusesResponseList.Resources != null)
                {
                    GoogleLists fullProductInfo = new GoogleLists()
                    {
                        Products = productsResponse.Resources.ToList(),
                        ProductsStatuses = productStatusesResponseList.Resources.ToList()
                    };
                    return View(fullProductInfo);
                }
            }
            return View();
        }

        public ActionResult InsertProduct()
        {
            UserCredential credential = Authenticate();
            ShoppingContentService service = CreateService(credential);

            string targetCountry = Request["selectProductTargetCountry"].ToUpper();
            string availabilityDateStr = Request["selectProductAvailabilityDate"];
            string expirationDateStr = Request["selectProductAvailabilityExpiryDate"];


            DateTime availabilityDate = DateTime.Now;
            DateTime expirationDate = DateTime.Now.AddDays(7);
            DateTime.TryParse(availabilityDateStr, out availabilityDate);
            DateTime.TryParse(expirationDateStr, out expirationDate);

            if (!string.IsNullOrWhiteSpace(availabilityDateStr))
            {
                availabilityDateStr = availabilityDate.ToString("s", System.Globalization.CultureInfo.InvariantCulture);
            }
            else
            {
                availabilityDateStr = null;
            }

            if (!string.IsNullOrWhiteSpace(expirationDateStr))
            {
                expirationDateStr = expirationDate.ToString("s", System.Globalization.CultureInfo.InvariantCulture);
            }
            else
            {
                expirationDateStr = null;
            }

            availabilityDateStr = availabilityDate.ToString("s", System.Globalization.CultureInfo.InvariantCulture);
            expirationDateStr = expirationDate.ToString("s", System.Globalization.CultureInfo.InvariantCulture);

            Google.Apis.ShoppingContent.v2.Data.Product newProduct = new Google.Apis.ShoppingContent.v2.Data.Product()
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
                Gtin = Request["inputProductGtin"],
                IdentifierExists = false,
                
                AvailabilityDate = availabilityDateStr,
                ExpirationDate = expirationDateStr
            };

            Google.Apis.ShoppingContent.v2.Data.Price priceInfo = new Google.Apis.ShoppingContent.v2.Data.Price()
            {
                Value = Request["inputProductPrice"]
            };

            switch (targetCountry)
            {
                case "SE":
                    newProduct.ContentLanguage = "sv";
                    priceInfo.Currency = "SEK";
                    break;

                case "GB":
                    newProduct.ContentLanguage = "en";
                    priceInfo.Currency = "GBP";
                    break;

                case "AU":
                    newProduct.ContentLanguage = "en";
                    priceInfo.Currency = "AUD";
                    break;

                case "USA":
                    newProduct.ContentLanguage = "en";
                    priceInfo.Currency = "USD";
                    break;

                default:
                    newProduct.ContentLanguage = "en";
                    priceInfo.Currency = "USD";
                    break;
            }

            newProduct.Price = priceInfo;

            try
            {
                ProductsResource.InsertRequest accountRequest = service.Products.Insert(newProduct, MERCHANT_ID);
                //accountRequest.DryRun = true;
                accountRequest.Execute();
                Debug.WriteLine(newProduct.Title + newProduct.Description);
            }
            catch (Exception e)
            {
                Debug.WriteLine("EXCEPTION THROWN @InsertProduct()");
                Debug.WriteLine("Message: " + e.Message);
                Debug.WriteLine("Stack Trace: " + e.StackTrace);
                Debug.WriteLine("Target Site: " + e.TargetSite);
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
            catch (Exception e)
            {
                Debug.WriteLine("EXCEPTION THROWN @DeleteProduct()");
                Debug.WriteLine("Message: " + e.Message);
                Debug.WriteLine("Stack Trace: " + e.StackTrace);
                Debug.WriteLine("Target Site: " + e.TargetSite);
            }

            return RedirectToAction("/GetProduct", "GoogleApi");
            //return Json(new { successfullyDeleted = successfullyDeleted }, "json", JsonRequestBehavior.DenyGet);
        }

        public ActionResult EditProduct()
        {

            UserCredential credential = Authenticate();
            ShoppingContentService service = CreateService(credential);

            string category = Request["editSelectProductCategory"];
            string availability = Request["editSelectProductAvailability"];
            string condition = Request["editSelectProductCondition"];
            string targetCountry = Request["editSelectProductTargetCountry"];
            string availabilityDateStr = Request["editSelectProductAvailabilityDate"];
            string expirationDateStr = Request["editSelectProductAvailabilityExpiryDate"];

            string id = Request["editProductId"];
            string title = Request["editProductTitle"];
            string description = Request["editProductDescription"];
            string link = Request["editProductLink"];
            string imageLink = Request["editProductImageLink"];
            string price = Request["editProductPrice"];
            string gtin = Request["EditProductGtin"];


            DateTime availabilityDate = DateTime.Now;
            DateTime expirationDate = DateTime.Now.AddDays(7);
            DateTime.TryParse(availabilityDateStr, out availabilityDate);
            DateTime.TryParse(expirationDateStr, out expirationDate);

            if (!string.IsNullOrWhiteSpace(availabilityDateStr))
            {
                availabilityDateStr = availabilityDate.ToString("s", System.Globalization.CultureInfo.InvariantCulture);
            }
            else
            {
                availabilityDateStr = null;
            }

            if (!string.IsNullOrWhiteSpace(expirationDateStr))
            {
                expirationDateStr = expirationDate.ToString("s", System.Globalization.CultureInfo.InvariantCulture);
            }
            else
            {
                expirationDateStr = null;
            }

            

            Google.Apis.ShoppingContent.v2.Data.Product updateProduct = new Google.Apis.ShoppingContent.v2.Data.Product();

            try
            {
                ProductsResource.GetRequest updateProductRequest = service.Products.Get(MERCHANT_ID, id);
                updateProduct = updateProductRequest.Execute();
            }
            catch (Exception e)
            {
                Debug.WriteLine("EXCEPTION THROWN @EditProduct() 1");
                Debug.WriteLine("Message: " + e.Message);
                Debug.WriteLine("Stack Trace: " + e.StackTrace);
                Debug.WriteLine("Target Site: " + e.TargetSite);
            }

                updateProduct.ETag = null;
                updateProduct.GoogleProductCategory = category;
                updateProduct.Availability = availability;
                updateProduct.Condition = condition;
                updateProduct.TargetCountry = targetCountry;
                updateProduct.AvailabilityDate = availabilityDateStr;
                updateProduct.ExpirationDate = expirationDateStr;

                updateProduct.Title = title;
                updateProduct.Description = description;
                updateProduct.Link = link;
                updateProduct.ImageLink = imageLink;
                updateProduct.Price.Value = price;
                updateProduct.Gtin = gtin;


            switch (targetCountry)
            {
                case "SE":
                    updateProduct.ContentLanguage = "sv";
                    updateProduct.Price.Currency = "SEK";
                    break;

                case "GB":
                    updateProduct.ContentLanguage = "en";
                    updateProduct.Price.Currency = "GBP";
                    break;

                case "AU":
                    updateProduct.ContentLanguage = "en";
                    updateProduct.Price.Currency = "AUD";
                    break;

                case "USA":
                    updateProduct.ContentLanguage = "en";
                    updateProduct.Price.Currency = "USD";
                    break;

                default:
                    updateProduct.ContentLanguage = "en";
                    updateProduct.Price.Currency = "USD";
                    break;
            }

            List<ProductShipping> shippingList = new List<ProductShipping>()
            {
                new ProductShipping { Country = "SE", Price = new Google.Apis.ShoppingContent.v2.Data.Price { Value = "50", Currency = "SEK"  }}
            };

            updateProduct.Shipping = shippingList;

            try
            {
                ProductsResource.InsertRequest accountRequest = service.Products.Insert(updateProduct, MERCHANT_ID);
                //accountRequest.DryRun = true;
                accountRequest.Execute();
            }
            catch (Exception e)
            {
                Debug.WriteLine("EXCEPTION THROWN @EditProduct() 2");
                Debug.WriteLine("Message: " + e.Message);
                Debug.WriteLine("Stack Trace: " + e.StackTrace);
                Debug.WriteLine("Target Site: " + e.TargetSite);
            }


            return RedirectToAction("/GetProduct", "GoogleApi");
        }

        public ActionResult CreatePartTrapProduct()
        {
            UserCredential credential = Authenticate();
            ShoppingContentService service = CreateService(credential);

            Google.Apis.ShoppingContent.v2.Data.Product newProduct = new Google.Apis.ShoppingContent.v2.Data.Product()
            {
                OfferId = GetUniqueId(),
                Title = "PartTrap One",
                Description = "En företagstjänst för B2B eller B2C företag. Innehåller CMS, PIM, ERP, eCommerce. ",
                Link = "https://www.parttrap.com/sv/ExplodedDiagramBooks/Index/8f0fef24-6bfb-4229-a64e-6b2fb0e09991",
                ImageLink = "https://media.licdn.com/mpr/mpr/shrink_200_200/AAEAAQAAAAAAAAdRAAAAJGEwNWFlNjk4LTI3NDQtNDdmOS05YzZjLTM5MjQ2Mzk5MzQzMA.png",
                ContentLanguage = "sv",
                TargetCountry = "SE",
                Channel = "online",
                Availability = "in stock",
                Condition = "new",
                GoogleProductCategory = "5300",
                IdentifierExists = false,
                Brand = "PartTrap",
                OnlineOnly = true,
                Price = new Google.Apis.ShoppingContent.v2.Data.Price
                {
                    Value = "500000",
                    Currency = "SEK"
                },
                ProductType = "Software > Computer Software > Business & Productivity Software",

            };


            try
            {
                ProductsResource.InsertRequest accountRequest = service.Products.Insert(newProduct, MERCHANT_ID);
                //accountRequest.DryRun = true;
                accountRequest.Execute();
                Debug.WriteLine(newProduct.Title + newProduct.Description);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("EXCEPTION THROWN @CreatePartTrapProduct()");
                System.Diagnostics.Debug.WriteLine("Message: " + e.Message);
                System.Diagnostics.Debug.WriteLine("Stack Trace: " + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("Target Site: " + e.TargetSite);
            }


            return RedirectToAction("/GetProduct", "GoogleApi");

        }

        public ActionResult DeletePartTrapProduct(string productId)
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
                System.Diagnostics.Debug.WriteLine("EXCEPTION THROWN @DeletePartTrapProduct()");
                System.Diagnostics.Debug.WriteLine("Message: " + Ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack Trace: " + Ex.StackTrace);
                System.Diagnostics.Debug.WriteLine("Target Site: " + Ex.TargetSite);
            }

            return RedirectToAction("/GetProduct", "GoogleApi");
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


        // ----------------------------------------------------------------------------------


        public JArray GetProductsFromJSON(string url)
        {
            List<JToken> productList = new List<JToken>();
            WebRequest request = WebRequest.Create(url);
            Stream dataStream = request.GetResponse().GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);

            string response = reader.ReadToEnd();
            JArray jsonObj = (JArray)JsonConvert.DeserializeObject(response);

            return jsonObj;
        }


        public void InsertProductFromJSON(string url)
        {
            UserCredential credential = Authenticate();
            ShoppingContentService service = CreateService(credential);

            JArray productsToInsert = GetProductsFromJSON(url);
            List<Google.Apis.ShoppingContent.v2.Data.Product> googleProducts = new List<Google.Apis.ShoppingContent.v2.Data.Product>();

            //OfferId = GetUniqueId()
            //Title = "PartTrap One"
            //Description = "En företagstjänst för B2B eller B2C företag. Innehåller CMS, PIM, ERP, eCommerce. "
            //Link = "https://www.parttrap.com/sv/ExplodedDiagramBooks/Index/8f0fef24-6bfb-4229-a64e-6b2fb0e09991"
            //ImageLink = "https://media.licdn.com/mpr/mpr/shrink_200_200/AAEAAQAAAAAAAAdRAAAAJGEwNWFlNjk4LTI3NDQtNDdmOS05YzZjLTM5MjQ2Mzk5MzQzMA.png"
            //ContentLanguage = "sv"
            //TargetCountry = "SE"
            //Channel = "online"
            //Availability = "in stock"
            //Condition = "new"
            //GoogleProductCategory = "5300"
            //IdentifierExists = false
            //Brand = "PartTrap"
            //OnlineOnly = true
            //Price = new Price
            //{
            //    Value = "500000"
            //    Currency = "SEK"
            //}
            //ProductType = "Software > Computer Software > Business & Productivity Software"

            foreach (var product in productsToInsert)
            {
                Google.Apis.ShoppingContent.v2.Data.Product newProduct = new Google.Apis.ShoppingContent.v2.Data.Product()
                {
                    OfferId = product["ProductID"].ToString(),
                    Title = product["SOMETHING HERE"].ToString()
                };

                googleProducts.Add(newProduct);
            }

            try
            {
                ProductsResource.InsertRequest accountRequest = service.Products.Insert(newProduct, MERCHANT_ID);
                //accountRequest.DryRun = true;
                accountRequest.Execute();
                Debug.WriteLine(newProduct.Title + newProduct.Description);
            }
            catch (Exception Ex)
            {
                Debug.WriteLine("EXCEPTION THROWN @InsertProduct()");
                Debug.WriteLine("Message: " + Ex.Message);
                Debug.WriteLine("Stack Trace: " + Ex.StackTrace);
                Debug.WriteLine("Target Site: " + Ex.TargetSite);
            }

        }

    }
}

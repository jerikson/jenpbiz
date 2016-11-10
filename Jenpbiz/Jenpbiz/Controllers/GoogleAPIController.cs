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
        private static string CLIENT_ID = "134786682471-b2nh5tpgcq06r9nqpanmnpi027t3aunh.apps.googleusercontent.com";
        //private static string CLIENT_SECRET_OLD = "vq_UOyNpiO2q6uTb-QKcCykt";
        private static string CLIENT_SECRET = "nZiPHZYfF_LDpxqijfToD_oe";
        private static ulong MERCHANT_ID = 113731084;

        private static int unique_id_increment = 0;
        //private static ulong MCA_MERCHANT_ID = 0;

        // GET: GoogleAPI
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetProduct()
        {
            ViewBag.Title = "Products";

            //DeleteProduct("online:sv:SE:14781753601");

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

                ProductstatusesResource.ListRequest statusesRequest = service.Productstatuses.List(MERCHANT_ID);
                statusesRequest.MaxResults = maxResults;
                statusesRequest.PageToken = pageToken;
                statusesRequest.IncludeInvalidInsertedItems = true;
                productStatusesResponseList = statusesRequest.Execute();
                

                if (productsResponse.Resources != null && productsResponse.Resources.Count != 0)
                {

                }
                firstRun = false;
                pageToken = productsResponse.NextPageToken;
            }

            if (productsResponse.Resources != null && productStatusesResponseList.Resources != null)
            {
                Jenpbiz.Models.GoogleLists fullProductInfo = new Models.GoogleLists()
                {
                    Products = productsResponse.Resources.ToList(),
                    ProductsStatuses = productStatusesResponseList.Resources.ToList()
                };
                return View(fullProductInfo);
            }


            //foreach (var status in fullProductInfo.ProductsStatuses.AsEnumerable())
            //{
            //    Debug.WriteLine("Status ProductId: " + status.ProductId);
            //    Debug.WriteLine("Status Product Title: " + status.Title);
            //    Debug.WriteLine("Status Product Link: " + status.Link);
            //    if (status.DataQualityIssues != null)
            //    {
            //        for (int i = 0; i < status.DataQualityIssues.Count; i++)
            //        {
            //            Debug.WriteLine("Issue Timestamp: " + status.DataQualityIssues[i].Timestamp);
            //            Debug.WriteLine(status.DataQualityIssues[i].Severity + " - Issue " + i + ": " + status.DataQualityIssues[i].Detail);
            //        }
            //    }

            //}



            return View();
            // OLD: return View(productsResponse.Resources.ToList());
        }

        public ActionResult InsertProduct()
        {
            UserCredential credential = Authenticate();
            ShoppingContentService service = CreateService(credential);

            //Debug.WriteLine("Title: " + Request["inputProductTitle"]);
            //Debug.WriteLine("Description: " + Request["inputProductDescription"]);
            //Debug.WriteLine("Link: " + Request["inputProductLink"]);
            //Debug.WriteLine("Image link: " + Request["InputProductImageLink"]);
            //Debug.WriteLine("Target Country: " + Request["selectProductTargetCountry"]);
            //Debug.WriteLine("Availability: " + Request["selectProductAvailability"]);
            //Debug.WriteLine("Condition: " + Request["selectProductCondition"]);
            //Debug.WriteLine("Category: " + Request["selectProductCategory"]);
            //Debug.WriteLine("Gtin: " + Request["inputProductGtin"]);
            //Debug.WriteLine("Availability Date: " + Request["selectProductAvailabilityDate"]);
            //Debug.WriteLine("Expiration Date: " + Request["selectProductAvailabilityExpiryDate"]);

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


            Debug.WriteLine("Availability Date: " + availabilityDate.ToString());
            Debug.WriteLine("Expiration Date: " + availabilityDate.ToString());
            Debug.WriteLine("Availability Date ToString(s): " + availabilityDate.ToString("s", System.Globalization.CultureInfo.InvariantCulture));
            Debug.WriteLine("Expiration Date ToString(s): " + availabilityDate.ToString("s", System.Globalization.CultureInfo.InvariantCulture));
            Debug.WriteLine("ISO Availability Date: " + availabilityDateStr);
            Debug.WriteLine("ISO Expiration Date: " + expirationDateStr);

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
                Gtin = Request["inputProductGtin"],
                IdentifierExists = false,
                
                AvailabilityDate = availabilityDateStr,
                ExpirationDate = expirationDateStr
            };

            Price priceInfo = new Price()
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
            catch (Exception Ex)
            {
                System.Diagnostics.Debug.WriteLine("EXCEPTION THROWN @InsertProduct()");
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

            

            Product updateProduct = new Product();

            try
            {
                ProductsResource.GetRequest updateProductRequest = service.Products.Get(MERCHANT_ID, id);
                updateProduct = updateProductRequest.Execute();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Debug.WriteLine("EXCEPTION THROWN @EditProduct() 1");
                System.Diagnostics.Debug.WriteLine("Message: " + Ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack Trace: " + Ex.StackTrace);
                System.Diagnostics.Debug.WriteLine("Target Site: " + Ex.TargetSite);
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
                new ProductShipping { Country = "SE", Price = new Price { Value = "50", Currency = "SEK"  }}
            };

            updateProduct.Shipping = shippingList;

            try
            {
                ProductsResource.InsertRequest accountRequest = service.Products.Insert(updateProduct, MERCHANT_ID);
                //accountRequest.DryRun = true;
                accountRequest.Execute();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Debug.WriteLine("EXCEPTION THROWN @EditProduct() 2");
                System.Diagnostics.Debug.WriteLine("Message: " + Ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack Trace: " + Ex.StackTrace);
                System.Diagnostics.Debug.WriteLine("Target Site: " + Ex.TargetSite);
            }


            return RedirectToAction("/GetProduct", "GoogleApi");
        }

        public ActionResult CreatePartTrapProduct()
        {
            UserCredential credential = Authenticate();
            ShoppingContentService service = CreateService(credential);

            Product newProduct = new Product()
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
                Price = new Price
                {
                    Value = "500000",
                    Currency = "SEK"
                },
                ProductType = "Software > Computer Software > Business & Productivity Software"

            };

            //Product newProduct = new Product()
            //{
            //    OfferId = GetUniqueId(),
            //    Title = "PartTrap skjorta",
            //    Description = "En fin, vit t-shirt med PartTrap skriven på den.",
            //    Link = "http://imgur.com/a/ZRGaP",
            //    ImageLink = "http://i.imgur.com/79uA7kQ.jpg",
            //    ContentLanguage = "sv",
            //    TargetCountry = "SE",
            //    Channel = "online",
            //    Availability = "in stock",
            //    Condition = "new",
            //    GoogleProductCategory = "1604",
            //    IdentifierExists = false,
            //    Brand = "PartTrap",
            //    OnlineOnly = true,
            //    Price = new Price
            //    {
            //        Value = "5000",
            //        Currency = "SEK"
            //    },

            //};


            try
            {
                ProductsResource.InsertRequest accountRequest = service.Products.Insert(newProduct, MERCHANT_ID);
                //accountRequest.DryRun = true;
                accountRequest.Execute();
                Debug.WriteLine(newProduct.Title + newProduct.Description);
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Debug.WriteLine("EXCEPTION THROWN @CreatePartTrapProduct()");
                System.Diagnostics.Debug.WriteLine("Message: " + Ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack Trace: " + Ex.StackTrace);
                System.Diagnostics.Debug.WriteLine("Target Site: " + Ex.TargetSite);
            }


            return RedirectToAction("/GetProduct", "GoogleApi");

        }

        public ActionResult GetProductInfo(string productId)
        {
            UserCredential credential = Authenticate();
            ShoppingContentService service = CreateService(credential);

            ProductsResource.GetRequest accountRequest = service.Products.Get(MERCHANT_ID, productId);
            Product selectedProduct = accountRequest.Execute();


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


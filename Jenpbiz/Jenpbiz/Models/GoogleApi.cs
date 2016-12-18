using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.ShoppingContent.v2;
using Google.Apis.ShoppingContent.v2.Data;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;
using System;
using System.Linq;


namespace Jenpbiz.Models
{
    public class GoogleApi
    {
        private UserCredential _credential;
        private ShoppingContentService _service;
        private ulong _merchantID;
        private string _clientId = "134786682471-b2nh5tpgcq06r9nqpanmnpi027t3aunh.apps.googleusercontent.com";
        private string _clientSecret = "nZiPHZYfF_LDpxqijfToD_oe";


        public GoogleApi(ulong merchantID)
        {
            _credential = Authenticate();
            _service = CreateService(_credential);
            _merchantID = merchantID;
        }

        private UserCredential Authenticate()
        {
            string[] scopes = new string[] { ShoppingContentService.Scope.Content };

            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                          new ClientSecrets
                          {
                              ClientId = _clientId,
                              ClientSecret = _clientSecret
                          },
                          scopes,
                          "user",
                          System.Threading.CancellationToken.None).Result;

            return credential;
        }

        private ShoppingContentService CreateService(UserCredential credential)
        {
            var service = new ShoppingContentService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "jenpbiz",
            });
            return service;
        }

        private JArray ProductGet(string productUrl)
        {
            //WebClient version
            JArray jsonObj = new JArray();
            using (WebClient client = new WebClient())
            {
                try
                {
                    string json = client.DownloadString(productUrl);
                    jsonObj = JArray.Parse(json);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Message: " + e.Message + "\nStacktrace: " + e.StackTrace + "\nTarget: " + e.TargetSite);
                    System.Diagnostics.Debug.WriteLine("\n\n*DINGDING* Something is wrong in the Private ProductGet() method. The url probably isn't returning valid JSON.");
                    return null;
                }
            }

            return jsonObj;
        }


        ///<summary>
        ///Inserts and/or updates products to a Google Shopping account using custombatch method to send multiple requests as one.
        ///<para>productUrl is a URI to something that returns a JSON object.</para>
        /// </summary>
        public void ProductInsert(string productUrl)
        {
            JArray productsToPush = ProductGet(productUrl);
            ProductsCustomBatchRequest batchRequest = new ProductsCustomBatchRequest();
            batchRequest.Entries = new List<ProductsCustomBatchRequestEntry>();

            string domainUrl = productUrl.Substring(0, _customIndexOf(productUrl, '/', 3));


            foreach (var product in productsToPush)
            {

                Google.Apis.ShoppingContent.v2.Data.Product newProduct = new Google.Apis.ShoppingContent.v2.Data.Product()
                {
                    OfferId = (string)product["ProductID"],
                    Title = (string)product["GoogleTitle"],
                    Description = (string)product["GoogleDscription"],
                    Link = domainUrl + (string)product["AdditionalValues"]["DetailLink"],
                    ImageLink = domainUrl + (string)product["AdditionalValues"]["ImageUrl"],
                    ContentLanguage = (string)product["GoogleContentLanguage"],
                    TargetCountry = (string)product["GoogleTargetCountry"],
                    Channel = "online",
                    Availability = (string)product["GoogleAvailability"],
                    Condition = (string)product["GoogleCondition"],
                    GoogleProductCategory = (string)product["GoogleProductCategory"],
                    IdentifierExists = false
                };

                newProduct.Price = new Google.Apis.ShoppingContent.v2.Data.Price
                {
                    Currency = (string)product["NetPriceInclVAT"]["Currency"]["Code"],
                    Value = (string)product["NetPriceInclVAT"]["Amount"]
                };

                ProductsCustomBatchRequestEntry newEntry = new ProductsCustomBatchRequestEntry()
                {
                    Method = "insert",
                    BatchId = long.Parse(newProduct.OfferId),
                    MerchantId = _merchantID,
                    Product = newProduct
                };

                batchRequest.Entries.Add(newEntry);

            }
        

            try
            {
                ProductsResource.CustombatchRequest reeeq = _service.Products.Custombatch(batchRequest);
                reeeq.Execute();
            }
            catch (Exception e)
            {
               System.Diagnostics.Debug.WriteLine("EXCEPTION THROWN @ProductInsert()");
               System.Diagnostics.Debug.WriteLine("Message: " + e.Message);
               System.Diagnostics.Debug.WriteLine("Stack Trace: " + e.StackTrace);
               System.Diagnostics.Debug.WriteLine("Target Site: " + e.TargetSite);
            }

            return;
        }


        ///<summary>
        ///Deletes products from a Google Shopping account using custombatch method to send multiple requests as one.
        ///<para>productUrl is a URI to something that returns a JSON object.</para>
        ///</summary>
        public void ProductDelete(string productUrl)
        {
            JArray productsToDelete = ProductGet(productUrl);

            ProductsCustomBatchRequest batchRequest = new ProductsCustomBatchRequest();
            batchRequest.Entries = new List<ProductsCustomBatchRequestEntry>();
            int batchId = 1;

            //channel:language:TARGET_COUNTRY:OfferId
            string idConstruct = "";

            foreach (var product in productsToDelete)
            {
                idConstruct = "online:" + (string)product["GoogleContentLanguage"] + ":" + (string)product["GoogleTargetCountry"] + ":" + (string)product["ProductID"];
                ProductsCustomBatchRequestEntry newEntry = new ProductsCustomBatchRequestEntry()
                {
                    BatchId = batchId,
                    MerchantId = _merchantID,
                    Method = "delete",
                    ProductId = idConstruct
                };
                batchRequest.Entries.Add(newEntry);
                batchId++;
            }


            try
            {
                ProductsResource.CustombatchRequest reeeq = _service.Products.Custombatch(batchRequest);
                reeeq.Execute();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("EXCEPTION THROWN @ProductDelete()");
                System.Diagnostics.Debug.WriteLine("Message: " + e.Message);
                System.Diagnostics.Debug.WriteLine("Stack Trace: " + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("Target Site: " + e.TargetSite);
            }

            return;
        }


        /// <summary>
        /// Returns a list of the Merchant's Google Shopping Products
        /// <para>Min 1 per page, Max 250 per page</para>
        /// <para>Returns null if page doesn't exist.</para>
        /// </summary>
        /// <param name="maxResults">Max result of the page you're getting. Defaults to 20</param>
        /// <param name="page">Which page you're getting. Defaults to 1</param>
        /// <returns>Returns a list of Google Shopping Products</returns>
        public List<Google.Apis.ShoppingContent.v2.Data.Product> ProductsReturn(int? maxResults, int? page)
        {
            
            if (maxResults == null)
            {
                maxResults = 20;
            }
                
            if (maxResults < 1 || maxResults > 250)
            {
                maxResults = 20;
            }
            string pageToken = null;

            List<Google.Apis.ShoppingContent.v2.Data.Product> allProducts = new List<Google.Apis.ShoppingContent.v2.Data.Product>();

            ProductsListResponse productsResponse = null;
            do
            {
                ProductsResource.ListRequest productsRequest = _service.Products.List(_merchantID);
                productsRequest.MaxResults = maxResults;
                productsRequest.PageToken = pageToken;
                productsRequest.IncludeInvalidInsertedItems = true;
                productsResponse = productsRequest.Execute();

                pageToken = productsResponse.NextPageToken;
                if (productsResponse.Resources != null)
                {
                    allProducts.AddRange(productsResponse.Resources);
                }
            }
            while (pageToken != null);


            //Gives page the index value, aka. If you want index 0 "page 1", you input 1 as page
            //And it gives you the element at index 0.
            //Defaults to 0, even if the the parameter 'page' is null.
            if (page > 0)
            {
                page--;
            }
            else
            {
                page = 0;
            }

            if ((int)(maxResults * page) < allProducts.Count)
            {
                allProducts.RemoveRange(0, (int)(maxResults * page));
            }
            else
            {
                return null;
            }


            return allProducts.Take((int)maxResults).ToList();
        }


        /// <summary>
        /// Returns a page of the Merchant's Google Shopping productstatuses
        /// <para>A productstatus can include errors and ushc of a product</para>
        /// <para>Min 1 per page, Max 250 per page</para>
        /// <para>Returns null if page doesn't exist.</para>
        /// </summary>
        /// <param name="maxResults">Max result of the page you're getting. Defaults to 20</param>
        /// <param name="page">Which page you're getting. Defaults to 1</param>
        /// <returns>Returns a list of Google Shopping Products</returns>
        public List<Google.Apis.ShoppingContent.v2.Data.ProductStatus> ProductStatusesReturn(int? maxResults, int? page)
        {

            if (maxResults == null)
            {
                maxResults = 20;
            }

            if (maxResults < 1 || maxResults > 250)
            {
                maxResults = 20;
            }
            string pageToken = null;

            List<Google.Apis.ShoppingContent.v2.Data.ProductStatus> allProductStatuses = new List<Google.Apis.ShoppingContent.v2.Data.ProductStatus>();

            ProductstatusesListResponse productStatusesResponse = null;
            do
            {
                ProductstatusesResource.ListRequest productStatusesRequest = _service.Productstatuses.List(_merchantID);
                productStatusesRequest.MaxResults = maxResults;
                productStatusesRequest.PageToken = pageToken;
                productStatusesRequest.IncludeInvalidInsertedItems = true;
                productStatusesResponse = productStatusesRequest.Execute();

                pageToken = productStatusesResponse.NextPageToken;
                if (productStatusesResponse.Resources != null)
                {
                    allProductStatuses.AddRange(productStatusesResponse.Resources);
                }
            }
            while (pageToken != null);


            //Gives page the index value, aka. If you want index 0 "page 1", you input 1 as page
            //And it gives you the element at index 0.
            //Defaults to 0, even if the the parameter 'page' is null.
            if (page > 0)
            {
                page--;
            }
            else
            {
                page = 0;
            }

            if ((int)(maxResults * page) < allProductStatuses.Count)
            {
                allProductStatuses.RemoveRange(0, (int)(maxResults * page));
            }
            else
            {
                return null;
            }


            return allProductStatuses.Take((int)maxResults).ToList();
        }

        /// <summary>
        /// Returns Google Shopping Product versions (if any) of the products in the url.
        /// <para>Takes a product url as parameter.</para>
        /// <para>Returns null if faulty.</para>
        /// </summary>
        /// <param name="productUrl">Parameter</param>
        /// <returns>A list of google products</returns>
        public List<Google.Apis.ShoppingContent.v2.Data.Product> ProductGetSpecificProducts(string productUrl)
        {
            JArray selectedProducts = ProductGet(productUrl);

            ProductsCustomBatchResponse batchResponse = null;
            ProductsCustomBatchRequest batchRequest = new ProductsCustomBatchRequest();
            batchRequest.Entries = new List<ProductsCustomBatchRequestEntry>();
            List<Google.Apis.ShoppingContent.v2.Data.Product> productsToReturn = new List<Google.Apis.ShoppingContent.v2.Data.Product>();
            
            foreach (var product in selectedProducts)
            {
                ProductsCustomBatchRequestEntry newEntry = new ProductsCustomBatchRequestEntry()
                {
                    BatchId = (long)product["ProductID"],
                    MerchantId = _merchantID,
                    Method = "get",
                    //Bygg upp riktig- channel:content_language:TARGET_COUNTRY:OfferId
                    ProductId = "online:" + (string)product["GoogleContentLanguage"] + ":" +  (string)product["GoogleTargetCountry"] + ":" + (string)product["ProductID"]
                };
                batchRequest.Entries.Add(newEntry);
            }

            try
            {
                ProductsResource.CustombatchRequest reeeq = _service.Products.Custombatch(batchRequest);
                batchResponse = reeeq.Execute();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("EXCEPTION THROWN @ProductGetSpecificProducts()");
                System.Diagnostics.Debug.WriteLine("Message: " + e.Message);
                System.Diagnostics.Debug.WriteLine("Stack Trace: " + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("Target Site: " + e.TargetSite);
                return null;
            }

            foreach (var entry in batchResponse.Entries)
            {
                productsToReturn.Add(entry.Product);
            }

            return productsToReturn;
        }

        /// <summary>
        /// Gets all the the Google Shopping products belonging to the Merchant's account.
        /// </summary>
        /// <returns>Returns a List of Google Shopping products</returns>
        public List<Google.Apis.ShoppingContent.v2.Data.Product> ProductGetAllProducts()
        {
            List<Google.Apis.ShoppingContent.v2.Data.Product> allProducts = new List<Google.Apis.ShoppingContent.v2.Data.Product>();
            string pageToken = null;

            ProductsListResponse productsResponse = null;
            do
            {
                ProductsResource.ListRequest productsRequest = _service.Products.List(_merchantID);
                productsRequest.MaxResults = 250;
                productsRequest.PageToken = pageToken;
                productsRequest.IncludeInvalidInsertedItems = true;
                productsResponse = productsRequest.Execute();

                pageToken = productsResponse.NextPageToken;
                if (productsResponse.Resources != null)
                {
                    allProducts.AddRange(productsResponse.Resources);
                }
            }
            while (pageToken != null);


            return allProducts;
        }

        /// <summary>
        /// Gets all the the Google Shopping productStatuses belonging to the Merchant's account.
        /// </summary>
        /// <returns>Returns a List of Google Shopping productStatuses</returns>
        public List<Google.Apis.ShoppingContent.v2.Data.ProductStatus> ProductGetAllProductStatuses()
        {
            List<Google.Apis.ShoppingContent.v2.Data.ProductStatus> allProductStatuses = new List<Google.Apis.ShoppingContent.v2.Data.ProductStatus>();
            string pageToken = null;

            ProductstatusesListResponse productStatusesResponse = null;
            do
            {
                ProductstatusesResource.ListRequest productStatusesRequest = _service.Productstatuses.List(_merchantID);
                productStatusesRequest.MaxResults = 250;
                productStatusesRequest.PageToken = pageToken;
                productStatusesRequest.IncludeInvalidInsertedItems = true;
                productStatusesResponse = productStatusesRequest.Execute();

                pageToken = productStatusesResponse.NextPageToken;
                if (productStatusesResponse.Resources != null)
                {
                    allProductStatuses.AddRange(productStatusesResponse.Resources);
                }
            }
            while (pageToken != null);


            return allProductStatuses;
        }

        public bool NextPageExists(int? maxResults, int? page)
        {
            List<Google.Apis.ShoppingContent.v2.Data.Product> productList = ProductsReturn(maxResults, page + 1);

            if (productList == null || productList.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool PreviousPageExists(int? maxResults, int? page)
        {
            List<Google.Apis.ShoppingContent.v2.Data.Product> productList = ProductsReturn(maxResults, page - 1);

            if (productList == null || productList.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //Returns the index +1 of the x:th occurance of a char.
        private int _customIndexOf(string source, char toFind, int occurrence)
        {
            int index = -1;
            for (int i = 0; i < occurrence; i++)
            {
                index = source.IndexOf(toFind, index + 1);

                if (index == -1)
                    break;
            }
            return index;
        }


    }
}



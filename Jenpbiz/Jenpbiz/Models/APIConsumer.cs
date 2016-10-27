using System;
using System.Diagnostics;
using System.Threading;
using Google.Apis.ShoppingContent.v2;
using Google.Apis.ShoppingContent.v2.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;

namespace Jenpbiz.Models
{
    internal class ShoppingContent
    {
        private static string CLIENT_ID = "896777409399-ghva93bgs7qpv293tqj1vp4eefi7n82c.apps.googleusercontent.com";
        private static string CLIENT_SECRET = "vq_UOyNpiO2q6uTb-QKcCykt";
        private static ulong MERCHANT_ID = 113298073;
        private static ulong MCA_MERCHANT_ID = 0;

        [STAThread]
        internal static void Main(string[] args)
        {
            GoogleWebAuthorizationBroker.Folder = "";
            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = CLIENT_ID,
                    ClientSecret = CLIENT_SECRET
                },
                new string[] { ShoppingContentService.Scope.Content },
                "user",
                CancellationToken.None).Result;

            // Create the service.
            var service = new ShoppingContentService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "jenpbiz",

            });

            if (MCA_MERCHANT_ID == 0)
            {
                ApiConsumer consumah = new ApiConsumer(service);
                consumah.RunCalls(MERCHANT_ID);
            }
        }
    }

    internal class ApiConsumer
    {
        private readonly ShoppingContentService _service;

        public ApiConsumer(ShoppingContentService service)
        {
            this._service = service;
        }

        public void RunCalls(ulong merchantId)
        {
            // Products
            GetAllProducts(merchantId);
        }

        private ProductsListResponse GetAllProducts(ulong merchantId)
        {
            // Retrieve account list in pages and display data as we receive it.
            string pageToken = null;
            ProductsListResponse productsResponse = null;
            do
            {
                ProductsResource.ListRequest accountRequest = _service.Products.List(merchantId);
                accountRequest.PageToken = pageToken;
                productsResponse = accountRequest.Execute();

                if (productsResponse.Resources != null && productsResponse.Resources.Count != 0)
                {

                }
                pageToken = productsResponse.NextPageToken;
            } while (pageToken != null);
            // Return the last page of accounts.
            return productsResponse;
        }
    }



       
     
}


using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.ShoppingContent.v2;
using Google.Apis.ShoppingContent.v2.Data;

namespace Jenpbiz.Controllers
{
    public class GoogleApiController : Controller
    {
        private static string CLIENT_ID = "896777409399-ghva93bgs7qpv293tqj1vp4eefi7n82c.apps.googleusercontent.com";
        private static string CLIENT_SECRET = "vq_UOyNpiO2q6uTb-QKcCykt";
        private static ulong MERCHANT_ID = 113298073;
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
            string[] scopes = new string[] { ShoppingContentService.Scope.Content };

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
            return View();
        }

    }


}
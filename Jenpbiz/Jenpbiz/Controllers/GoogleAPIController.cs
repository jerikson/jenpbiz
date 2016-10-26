using Microsoft.Owin.Security.Google;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Json;
using Google.Apis.Http;
using Google.Apis.Requests;
using Google.Apis.Services;
using Google.Apis.ShoppingContent;
using Google.Apis.ShoppingContent.v2;
using Google.Apis.ShoppingContent.v2.Data;
using Google.Apis.Util;
using Google.Apis.Util.Store;


namespace Jenpbiz.Controllers
{
    public class GoogleAPIController : Controller
    {
        // GET: GoogleAPI
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult API_GetProducts()
        {

            string[] scopes = new string[] { ShoppingContentService.Scope.Content };

            //var clientIdWeb = "896777409399-k6hpbpab88tbv6qn7233thc0nhkncu2g.apps.googleusercontent.com";
            //var clientSecretWeb = "VCpvtenKfMz40axOJIqBH_EC";

            var clientId = "896777409399-ghva93bgs7qpv293tqj1vp4eefi7n82c.apps.googleusercontent.com";
            var clientSecret = "vq_UOyNpiO2q6uTb-QKcCykt";
            ulong merchantId = 113298073;

            //GoogleWebAuthorizationBroker.Folder = "ShoppingContent.Sample";

            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret
                },
                scopes,
                "jenpbiz",
                CancellationToken.None).Result;



            // Utkommenterad för jag tror det bara behövs när man ska göra calls,
            // inte vid autentisering.
            var service = new ShoppingContentService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Jenpbiz"

            });

            string pageToken = null;
            long maxResults = 10;
            ProductsListResponse productsResponse = null;

            do
            {
                ProductsResource.ListRequest accountRequest = service.Products.List(merchantId);
                accountRequest.MaxResults = maxResults;
                accountRequest.PageToken = pageToken;
                accountRequest.IncludeInvalidInsertedItems = true;
                productsResponse = accountRequest.Execute();

                if (productsResponse.Resources != null && productsResponse.Resources.Count != 0)
                {
                    foreach (var product in productsResponse.Resources)
                    {
                        //System.Console.WriteLine(
                        //    "Product with ID \"{0}\" and title \" {1}\" was found.",
                        //    product.Id, product.Title);
                        Debug.WriteLine("Product with ID \"{0}\" and title \" {1}\" was found.",
                            product.Id, product.Title);
                    }
                }
                else
                {
                    //System.Console.WriteLine("No accounts found.");
                    Debug.WriteLine("No accounts found.");
                }

                pageToken = productsResponse.NextPageToken;
            }
            while (pageToken != null);
            //System.Console.WriteLine();
            Debug.WriteLine("");



            return View(productsResponse.Resources.ToList());
        }
    }
}
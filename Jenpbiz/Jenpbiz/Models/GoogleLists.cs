using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jenpbiz.Models
{
    public class GoogleLists
    {
        public GoogleLists()
        {

        }

        public List<Google.Apis.ShoppingContent.v2.Data.Product> Products { get; set; }
        public List<Google.Apis.ShoppingContent.v2.Data.ProductStatus> ProductsStatuses { get; set; }



    }
}
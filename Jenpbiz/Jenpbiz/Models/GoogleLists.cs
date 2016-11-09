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

        public Google.Apis.ShoppingContent.v2.Data.ProductsListResponse Products { get; set; }
        public Google.Apis.ShoppingContent.v2.Data.ProductstatusesListResponse ProductsStatuses { get; set; }


    }
}
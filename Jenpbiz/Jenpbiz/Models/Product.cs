using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jenpbiz.Models
{
    public class Product
    {
            //[Key] / Fluet API? Code first? DataAnnotations
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public string PriductTitle { get; set; }
            public string PruductDescription { get; set; }
            public uint ProductPrice { get; set; }
            public string ProductUri { get; set; }


        }
}


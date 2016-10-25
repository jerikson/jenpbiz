using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Data.Entity.Infrastructure.Annotations;


namespace Jenpbiz.Models
{
    public class Product
    {
            //[Key] / Fluet API? Code first? DataAnnotations
            public int ProductId { get; set; }
            public string ProductTitle{ get; set; }
            public string ProductDescription { get; set; }
            public uint ProductPrice { get; set; }
            public string ProductLink { get; set; }
            public string ProductImageLink { get; set; }


        }
}


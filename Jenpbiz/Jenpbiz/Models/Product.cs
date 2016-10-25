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
            public string ProductLink { get; set; }
            public string ProductImageLink { get; set; }
            public string ProductGtin { get; set; }
            public string ProductMpn { get; set; }
            public string ProductBrand { get; set; }
            public DateTime ProductAvailabilityDate { get; set; }

            public ProductCategoryEnum ProductCategory { get; set; }
            public ProductConditionEnum ProductCondition { get; set; }
            public ProductAvailabilityEnum ProductAvailability { get; set; }


        public enum ProductCategoryEnum
        {
            Electronics,
            Apparel,
            Books,
            Tools,
            Media,
            Consumable,
        };

        public enum ProductConditionEnum
        {
            New,
            Refurbished,
            Used
        };

        public enum ProductAvailabilityEnum
        {
            In_stock,
            Out_of_stock,
            Pre_order
        };



            public Price Price { get; set; }

        }
}


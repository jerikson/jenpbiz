﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Data.Entity.Infrastructure.Annotations;
using Microsoft.Owin.Security.Google;
using Newtonsoft.Json.Converters;


namespace Jenpbiz.Models
{
    public class Product
    {
            public int ProductId { get; set; }
            public string ProductTitle{ get; set; }
            public string ProductDescription { get; set; }
            public string ProductLink { get; set; }
            public string ProductImageLink { get; set; }
            public string ProductGtin { get; set; }
            public string ProductMpn { get; set; }
            public string ProductBrand { get; set; }
            public string ProductColor { get; set; }
            public string ProductMaterial { get; set; }
            public string ProductSize { get; set; }
            public string ProductPattern { get; set; }
            public DateTime? ProductAvailabilityDate { get; set; }

            public ProductCategoryEnum ProductCategory { get; set; }
            public ProductConditionEnum ProductCondition { get; set; }
            public ProductAvailabilityEnum ProductAvailability { get; set; }

            public virtual Price Price { get; set; }

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




    }
}


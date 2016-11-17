using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jenpbiz.Models
{

    public class Model
    {
        public class Rootobject
        {
            public Model response { get; set; }
        }
        public class Product
        {
            public string StockCode { get; set; }
            public int ProductID { get; set; }
            public int ProductType { get; set; }
            public object ProductTypeText { get; set; }
            public string Description { get; set; }
            public object Description2 { get; set; }
            public object OriginalName { get; set; }
            public object ModelIdentifier { get; set; }
            public string LangDescription { get; set; }
            public string ProductGroup { get; set; }
            public float NetWeight { get; set; }
            public float GrossWeight { get; set; }
            public float Volume { get; set; }
            public string DisplayStockCode { get; set; }
            public Grossprice GrossPrice { get; set; }
            public Grosspriceinclvat GrossPriceInclVAT { get; set; }
            public Netprice NetPrice { get; set; }
            public Netpriceinclvat NetPriceInclVAT { get; set; }
            public float TotalDiscount { get; set; }
            public object[] PriceLadders { get; set; }
            public object[] PriceLaddersInclVat { get; set; }
            public Costprice CostPrice { get; set; }
            public object CustomPrice1 { get; set; }
            public object CustomPrice2 { get; set; }
            public object CustomPrice3 { get; set; }
            public object VirtualMinProduct { get; set; }
            public object VirtualMaxProduct { get; set; }
            public string CurrencyShortName { get; set; }
            public string PriceLadder { get; set; }
            public string PriceLadderInclVAT { get; set; }
            public string CurrencyCode { get; set; }
            public string VATCode { get; set; }
            public float VATPercentage { get; set; }
            public bool BlockedForOrder { get; set; }
            public float QtyUniOutDel { get; set; }
            public float Discount { get; set; }
            public float MultiLineDiscount { get; set; }
            public float Available { get; set; }
            public float StockBalance { get; set; }
            public DateTime DateConfirmed { get; set; }
            public DateTime LeadTimeTarget { get; set; }
            public DateTime ReplacementStartDate { get; set; }
            public DateTime CustomDate1 { get; set; }
            public DateTime CustomDate2 { get; set; }
            public string UnitCodeSale { get; set; }
            public object UnitNameSale { get; set; }
            public string ReplacementStockCode { get; set; }
            public bool ShowOnWebBtoC { get; set; }
            public bool ShowOnWebBtoB { get; set; }
            public object ExtraDescription { get; set; }
            public object ProductTreeNodes { get; set; }
            public object Attributes { get; set; }
            public object[] ProductRelations { get; set; }
            public object ParentProductRelations { get; set; }
            public object ExplodedPositions { get; set; }
            public object Documents { get; set; }
            public object DocumentCategories { get; set; }
            public object ProductAttributeGroups { get; set; }
            public object ProductFilterGroups { get; set; }
            public bool Favorite { get; set; }
            public bool SellableProduct { get; set; }
            public object Combinations { get; set; }
            public Additionalvalues AdditionalValues { get; set; }
            public object LinePlanValues { get; set; }
            public object ProductFieldsTexts { get; set; }
            public object ProductFieldsImages { get; set; }
            public object Thumbnail { get; set; }
            public object Images { get; set; }
            public int RelationCategoryID { get; set; }
            public int RelationSortOrder { get; set; }
            public int RelationTypeID { get; set; }
            public int PageID { get; set; }
            public object LayoutSectionID { get; set; }
            public object[] FilteredChildProductID { get; set; }
            public object[] FilteredChildProduct { get; set; }
            public object ProductSuppliers { get; set; }
            public object DiagramBookItems { get; set; }
            public string CombinedSearchText { get; set; }
            public bool IsAttributsSet { get; set; }
            public bool IsReplaced { get; set; }
            public object DisplayOption { get; set; }
        }

        public class Grossprice
        {
            public float Amount { get; set; }
            public Currency Currency { get; set; }
        }

        public class Currency
        {
            public string Code { get; set; }
            public object Name { get; set; }
            public bool CurrencySymbol { get; set; }
        }

        public class Grosspriceinclvat
        {
            public float Amount { get; set; }
            public Currency1 Currency { get; set; }
        }

        public class Currency1
        {
            public string Code { get; set; }
            public object Name { get; set; }
            public bool CurrencySymbol { get; set; }
        }

        public class Netprice
        {
            public float Amount { get; set; }
            public Currency2 Currency { get; set; }
        }

        public class Currency2
        {
            public string Code { get; set; }
            public object Name { get; set; }
            public bool CurrencySymbol { get; set; }
        }

        public class Netpriceinclvat
        {
            public float Amount { get; set; }
            public Currency3 Currency { get; set; }
        }

        public class Currency3
        {
            public string Code { get; set; }
            public object Name { get; set; }
            public bool CurrencySymbol { get; set; }
        }

        public class Costprice
        {
            public float Amount { get; set; }
            public Currency4 Currency { get; set; }
        }

        public class Currency4
        {
            public string Code { get; set; }
            public object Name { get; set; }
            public bool CurrencySymbol { get; set; }
        }

        public class Additionalvalues
        {
            public float NetPrice { get; set; }
            public string PriceLists { get; set; }
            public string Status { get; set; }
            public DateTime LastUpdate { get; set; }
            public float NetPriceInclVAT { get; set; }
            public float GrossPriceInclVAT { get; set; }
            public string Active { get; set; }
            public string CombinedSearchTextde { get; set; }
            public string CombinedSearchTextfr { get; set; }
            public string CombinedSearchTextda { get; set; }
            public string ProductType { get; set; }
            public string CombinedSearchTextsv { get; set; }
            public string CombinedSearchTextit { get; set; }
            public string CombinedSearchTextfi { get; set; }
            public string CombinedSearchTextlt { get; set; }
            public bool HoldStatusFlag { get; set; }
            public string CacheProductChildProduct { get; set; }
            public string FCPartID { get; set; }
            public bool WebPart { get; set; }
            public string test1 { get; set; }
            public string CustomerStockCode { get; set; }
            public string CustomerStockCodeDescription { get; set; }
            public int row { get; set; }
            public string NetPriceInclVat { get; set; }
            public string DetailLink { get; set; }
            public string ImageUrl { get; set; }
        }


    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MocoApp.Models
{
    public class Product : BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        // added
        public string TotalPrice { get
            {
                var prop = Price * Quantity;
                return prop.ToString("C", App.AppCurrent.CompanyCulture);
                //return prop.ToString("C", System.Globalization.CultureInfo.CurrentCulture);
            }
        }

        public decimal TotalPriceDecimal { get { return Price * Quantity; } }

        public int Quantity { get; set; }
        public string CompanyId { get; set; }
        public Company Company { get; set; }

        public string LocationId { get; set; }
        public Location Location { get; set; }

        public string ImageUri { get; set; }
        public string CategoryGroupId { get; set; }
        //public object ProductCategory { get; set; }

        public string InformativeMenuId { get; set; }
        public InformativeMenu InformativeMenu { get; set; }
        public EProductSegment ProductSegment { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsFavorited { get; set; }
        public decimal Rating { get; set; }
        public int TotalRating { get; set; }
        public int OrderingNumber { get; set; }
        public int TotalItens { get; set; }
        public string PriceStr { get { return String.Format(App.AppCurrent.CompanyCulture, "{0:C}", Price); } }
        public string ProductStarImage
        {
            get
            {
                if (Rating < 1)
                    return "ic_star";
                else if (Rating < 2)
                    return "ic_1star_list";
                else if (Rating < 3)
                    return "ic_2star_list";
                else if (Rating < 4)
                    return "ic_3star_list";
                else if (Rating <= (decimal)4.9)
                    return "ic_4star_list";
                else if (Rating == 5)
                    return "ic_5star_list";

                return "ic_5star_list";
            }
        }

        public bool ShowPrice
        {
            get
            {
                return Price == 0 ? false : true;
            }
        }

        public bool ShowElementBySegment
        {
            get
            {
                return ProductSegment == EProductSegment.Product ? true : false;
            }
        }
    }

    public enum EProductSegment
    {
        Product = 0,
        Service = 1,
        InformativeMenu =  2
    }


}

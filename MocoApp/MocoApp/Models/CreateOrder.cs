using MocoApp.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MocoApp.Models
{
    public class CreateOrder
    {
        public string Observation { get; set; }
        public int ProductQuantity { get; set; }
        public string ProductId { get; set; }
        public string CompanyId { get; set; }
        public string LocationId { get; set; }
        public Company Company { get; set; }


        public string ClientId { get; set; }

        public string UserId { get; set; }

        public DateTime CreatedAt { get; set; }

        public string CreatedAtStr { get; set; }

        public Product Product { get; set; }

        public string CheckinSubId { get; set; }
        public string WhoRequested { get; set; }
        public string CheckinId { get; set; }

        // todo: lembrar de comentar pq ja tava assim marco fez merda
        public string TotalPrice
        {
            get
            {
                decimal prop = 0;
                if (Product == null)
                    prop = 0;
                else
                    prop = Product.Price * ProductQuantity;

                return prop.ToString("C", App.AppCurrent.CompanyCulture);
            }
        }

        public decimal TotalPriceDecimal
        {
            get
            {
                if (Product == null)
                    return 0;
                else
                    return Product.Price * ProductQuantity;
            }
        }

        public string Version { get; set; }
        public string Device { get; set; }
    }

    public class CreateOrderCommandV2
    {
        public string Observation { get; set; }
        public int ProductQuantity { get; set; }
        public string ProductId { get; set; }
        public string UserId { get; set; }
        public string CheckinId { get; set; }
        public string CheckinSubId { get; set; }
        public string Device { get; set; }
        public string Version { get; set; }
    }
}

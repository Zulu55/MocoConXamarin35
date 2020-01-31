using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MocoApp.Models
{
    public class CheckoutTax
    {
        public string CheckinId { get; set; }
        public string CompanyId { get; set; }
        public string ClientId { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal PriceTipPaid { get; set; }
        public string PriceTipPaidStr { get; set; }
        public decimal PriceDiscount { get; set; }


        public decimal PaidInCard { get; set; }

        public decimal PaidInCash { get; set; }
    }
}

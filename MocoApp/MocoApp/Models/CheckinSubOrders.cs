using MocoApp.Views.CompanyFluxo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MocoApp.Models.Enums;

namespace MocoApp.Models
{
    public class CheckinSubOrders
    {
        public string Id { get; set; }
        public virtual string LocationId { get; set; }
        public virtual Location Location { get; set; }

        public CheckinSubStatus CheckinSubStatus { get; set; }

        //CheckinSub values
        public decimal SubTotal { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal PriceTaxPaid { get; set; }
        public decimal PriceTipPaid { get; set; }
        public decimal PriceDiscount { get; set; }
        public decimal TotalPaid { get; set; }


        public decimal PaidInCard { get; set; }
        public decimal PaidInCardHiCharlie { get; set; }

        public decimal PaidInCash { get; set; }

        public decimal TipPercentage { get; set; }
        public decimal TaxPercentage { get; set; }

        public int TotalOrders { get; set; }
        public int TotalCancelledOrders { get; set; }

        public bool Paid { get; set; }
        public ClientDTO Client { get; set; }
        public List<Order> Orders { get; set; }

        public ETimeToOrder TimeToOrder { get; set; }
        public DateTimeOffset? DateTimeToOrder { get; set; }

        public decimal PaidCardTotal { get { return PaidInCard + PaidInCardHiCharlie; } }
    }
}

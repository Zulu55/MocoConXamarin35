using MocoApp.Views.CompanyFluxo;
using System.Collections.Generic;

namespace MocoApp.DTO
{
    public class CompanyReportDTO
    {
        public int TotalOrders { get; set; }
        public int TotalOrdersCanceled { get; set; }
        public int TotalClients { get; set; }
        public int TotalCheckins { get; set; }
        public int TotalCarts { get; set; }
        public decimal OrdersPriceCanceled { get; set; }
        public decimal OrdersPrice { get; set; }
        public decimal TotalGross { get; set; }
        public decimal TotalTip { get; set; }
        public decimal TotalTax { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalInCash { get; set; }
        public decimal TotalInCard { get; set; }
        public decimal PaidInCardHiCharlie { get; set; }
        public List<ClientDTO> Clients { get; set; }
        public int TotalClientAttended { get; set; }
    }
}

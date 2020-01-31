using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MocoApp.Models.ReportCommand
{
    //Historico de Pedidos
    public class OrderHistoryFilter
    {
        public string Id { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public DateTimeOffset LocalDate { get; set; }
        public string LocationId { get; set; }
    }
}

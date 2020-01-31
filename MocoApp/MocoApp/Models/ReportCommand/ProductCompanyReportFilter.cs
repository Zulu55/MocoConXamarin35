using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MocoApp.Models.ReportCommand
{
    public class ProductCompanyReportFilter
    {
        public string CompanyId { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public DateTimeOffset LocalDate { get; set; }
        public string ProductId { get; set; }
        public string CategoryId { get; set; }
        public string LocationId { get; set; }

    }
}

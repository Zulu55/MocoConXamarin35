using System;

namespace MocoApp.Models.ReportCommand
{
    public class CheckinFilter
    {
        public string Id { get; set; }
        public string LocationId { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public DateTimeOffset? LocalDate { get; set; }
    }
}

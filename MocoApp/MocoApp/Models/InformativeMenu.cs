using static MocoApp.Models.Enums;

namespace MocoApp.Models
{
    public class InformativeMenu : BaseModel
    {
        public string Name { get; set; }

        public int OrderingNumber { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string ImageUri { get; set; }
        public string Url { get; set; }
        public string LocationId { get; set; }
        public string CategoryGroupId { get; set; }
        public string CompanyId { get; set; }
        public bool IsDisabled { get; set; }
        public int TotalItens { get; set; }
    }
}

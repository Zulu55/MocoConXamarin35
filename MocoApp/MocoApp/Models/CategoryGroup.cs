using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MocoApp.Models.Enums;

namespace MocoApp.Models
{
    public class CategoryGroup : BaseModel
    {
        public CategoryGroup()
        {
            Products = new List<Product>();
        }

        public string Name { get; set; }
        public string ImageUri { get; set; }
        public bool IsDisabled { get; set; }
        public virtual string CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public virtual string CompanyId { get; set; }
        public virtual Company Company { get; set; }
        public int OrderingNumber { get; set; }
        public int TotalItens { get; set; }
        public EMenuType MenuType { get; set; }

        public virtual List<Product> Products { get; set; }
    }
}

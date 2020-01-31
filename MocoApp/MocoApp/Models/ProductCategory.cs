using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MocoApp.Models
{
    public class ProductCategory : BaseModel
    {
        public string Name { get; set; }
        public string ImageUri { get; set; }

        public virtual string CompanyId { get; set; }
        public virtual Company Company { get; set; }
        public virtual string LocationId { get; set; }
        public virtual Location Location { get; set; }

        public bool IsDisabled { get; set; }
    }
}

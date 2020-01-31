using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MocoApp.Models
{
    public class CompanyPhoto : BaseModel
    {
        public string CompanyId { get; set; }

        public string ImageUri { get; set; }
    }
}

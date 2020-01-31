using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MocoApp.Models
{
    public class CompanyRating : BaseModel
    {
        public virtual string CompanyId { get; set; }
        public virtual Company Company { get; set; }

        public string Message { get; set; }

        public virtual string ClientId { get; set; }
        public virtual Client Client { get; set; }

        public int Rating { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MocoApp.Models
{
    public class Employee : User
    {
        public string Photo { get; set; }

        public virtual string CompanyId { get; set; }
        public virtual Company Company { get; set; }

        public string Cellphone { get; set; }
        public string CityName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MocoApp.Models
{
    public class Manager : User
    {
        public virtual ICollection<Company> Companies { get; set; }

        public string Cellphone { get; set; }
        public string CityName { get; set; }
    }
}

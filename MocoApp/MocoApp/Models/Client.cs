using MocoApp.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MocoApp.Models.Enums;

namespace MocoApp.Models
{
    public class Client : User
    {
        public string Cellphone { get; set; }
        public ClientStatus ClientStatus { get; set; }

        public string CityName { get; set; }
        //public virtual string CityId { get; set; }
        //public virtual City City { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }
        
        public string DesirableLanguage { get; set; }
        public virtual ICollection<Company> Companies { get; set; }

        public string ClientName
        {
            get
            {
                if (!string.IsNullOrEmpty(Surname) || !string.IsNullOrEmpty(Email))
                    return !string.IsNullOrEmpty(Surname) ? Surname : Name;
                else
                    return AppResource.textNotAHiCharlieUser;
            }
        }

    }
}

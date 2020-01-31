using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MocoApp.Models.Enums;

namespace MocoApp.Models
{
    public class User : BaseModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Photo { get; set; }
        public UserRole UserRole { get; set; }
        public string DeviceType { get; set; }
        public string DeviceToken { get; set; }

        public string CompanyId { get; set; }

        public string DesirableLanguage { get; set; }

        public Company Company { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MocoApp.Models.Enums;

namespace MocoApp.Models
{
    public class Notification : BaseModel
    {
        public virtual string ClientCompanyId { get; set; }
        public virtual ClientCompany ClientCompany { get; set; }

        public string Title { get; set; }
        public string Body { get; set; }

        public NotificationType NotificationType { get; set; }
    }
}

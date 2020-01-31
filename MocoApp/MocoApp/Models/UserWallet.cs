using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MocoApp.Models
{
    public class UserWallet : BaseModel
    {
        public string UserId { get;  set; }

        public string Token { get; set; }


        public string CardName { get; set; }
        public string CVV { get; set; }
        public string CardNumber { get; set; }
        public string CardBrand { get; set; }

        public string ExpirationDate { get; set; }
        public string StripeId { get; set; }
        public bool Default { get; set; }

        public string ShowName { get {
                return CardBrand + " - " + CardNumber;
            }  }
    }
}

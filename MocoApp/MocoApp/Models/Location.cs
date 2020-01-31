using MocoApp.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MocoApp.Models.Enums;

namespace MocoApp.Models
{
    public class Location : BaseModel
    {
        public string Name { get; set; }
        public string ImageUri { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsRoom { get; set; }
        public virtual string CompanyId { get; set; }
        public virtual Company Company { get; set; }
        public CheckinAction CheckinAction { get; set; }
        public string Prefix { get; set; }
        public EMenuType MenuType { get; set; }
        public LocationType LocationType { get; set; }
        public int OrderingNumber { get; set; }
        public int TotalItens { get; set; }

        public string LocationTypeStr
        {
            get
            {
                string value = "";
                switch (LocationType)
                {
                    case LocationType.Undefined:
                        value = "Indefinido";
                        break;
                    case LocationType.Room:
                        value = "Quarto: ";
                        break;
                    case LocationType.Bar:
                        value = "Numero cadeira ou mesa: ";
                        break;
                    case LocationType.Restaurant:
                        value = "Mesa: ";
                        break;
                    case LocationType.MeetingRoom:
                        value = "Indefinido";
                        break;
                    case LocationType.Spa:
                        value = "Indefinido";
                        break;
                    case LocationType.Valet:
                        value = "Indefinido";
                        break;
                    case LocationType.Concierge:
                        value = "Indefinido";
                        break;
                    case LocationType.SwimmingPool:
                        value = "Indefinido";
                        break;
                    default:
                        break;      
                }

                return value;
            }
        }

    }
}

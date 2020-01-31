using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;
using static MocoApp.Models.Enums;
using MocoApp.Resources;
using MocoApp.Extensions;

namespace MocoApp.Models
{
    ///checkin
    public class ClientCompany : BaseModel
    {
        public virtual string ClientId { get; set; }
        public virtual Client Client { get; set; }

        public virtual string CompanyId { get; set; }
        public virtual Company Company { get; set; }

        public string ClientQuantity { get; set; }

        public ClientCompanyStatus ClientCompanyStatus { get; set; }

        public string ClientCompanyStatusStr
        {
            get
            {
                return ClientCompanyStatus.ToResource();
                //switch (ClientCompanyStatus)
                //{
                //    case ClientCompanyStatus.CheckinPending:
                //        return "Pendente";
                //    case ClientCompanyStatus.Checkin:
                //        return AppResource.textCheckin;
                //    case ClientCompanyStatus.RequestedCheckout:
                //        return "Solicitado \n check-out";
                //    case ClientCompanyStatus.Checkout:
                //        return "Encerrada";
                //    case ClientCompanyStatus.RequestedCheckin:
                //        return "Checkin Solicitado";
                //    case ClientCompanyStatus.Denied:
                //        return "Recusado";
                //    default:
                //        return "";
                //}
            }
        }

        public string ClientCompanyStatusColor
        {
            get
            {
                switch (ClientCompanyStatus)
                {
                    case ClientCompanyStatus.CheckinPending:
                        return "#FF7417";
                    case ClientCompanyStatus.Checkin:
                        return "#651FFF";
                    case ClientCompanyStatus.RequestedCheckout:
                        return "#0D47A1";
                    case ClientCompanyStatus.Checkout:
                        return "#1B5E20";
                    case ClientCompanyStatus.RequestedCheckin:
                        return "#0a7cd4";
                    case ClientCompanyStatus.Denied:
                        return "#8A2839";
                    default:
                        return "";
                }
            }
        }

        public string Occupation { get; set; }

        public string OccupationStr {
            get
            {
                return AppResource.lblAllocation +  ": " + Occupation;
                //switch (Company.CompanyType)
                //{
                //    case CompanyType.Hotel:
                //        return AppResource.alertRoom + Occupation;
                //    case CompanyType.Restaurante:
                //        return "Mesa " + Occupation;
                //    case CompanyType.Praia:
                //        return "Mesa " + Occupation;
                //    default:
                //        return "";
                //}
            }
        }

        public string QtdPeopleStr
        {
            get
            {
                return AppResource.lblQuantityOfPeople + ": " + ClientQuantity;
                //switch (Company.CompanyType)
                //{
                //    case CompanyType.Hotel:
                //        return AppResource.alertRoom + Occupation;
                //    case CompanyType.Restaurante:
                //        return "Mesa " + Occupation;
                //    case CompanyType.Praia:
                //        return "Mesa " + Occupation;
                //    default:
                //        return "";
                //}
            }
        }

        public int CheckInNumber { get; set; }

        public string CheckInNumberStr { get { return ClientCompanyStatusStr + " #" + CheckInNumber; } }

        public string CheckInCreatedStr { get { return CreatedAt.ToString("dd/MM/yyyy HH:mm"); } }


        public List<Order> Orders { get; set; }

        public decimal PriceTipPaid { get; set; }
        public decimal PriceDiscount { get; set; }
        public decimal TotalPaid { get; set; }

        public decimal TipPercentage { get; set; }

    }
}

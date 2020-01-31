using MocoApp.Extensions;
using MocoApp.Resources;
using System.Collections.Generic;
using static MocoApp.Models.Enums;

namespace MocoApp.Models
{
    public partial class Checkin
    {
        public virtual string ClientId { get; set; }
        public virtual Client Client { get; set; }

        public virtual string CompanyId { get; set; }
        public virtual Company Company { get; set; }

        public string ClientQuantity { get; set; }

        public CheckinStatus CheckinStatus { get; set; }
        public string Occupation { get; set; }

        public string CheckinStatusStr
        {
            get
            {
                return CheckinStatus.ToResource();
            }
        }

        public string CheckinStatusColor
        {
            get
            {
                switch (CheckinStatus)
                {
                    case CheckinStatus.CheckinPending:
                        return "#FF7417";
                    case CheckinStatus.Checkin:
                        return "#651FFF";
                    case CheckinStatus.RequestedCheckout:
                        return "#0D47A1";
                    case CheckinStatus.Checkout:
                        return "#1B5E20";
                    case CheckinStatus.RequestedCheckin:
                        return "#0a7cd4";
                    case CheckinStatus.Denied:
                        return "#8A2839";
                    default:
                        return "";
                }
            }
        }


        public string OccupationStr
        {
            get
            {

                switch (Company.CompanyType)
                {
                    case CompanyType.Hotel:
                        return AppResource.lblRoom + " " + Occupation;
                    case CompanyType.Restaurante:
                        return AppResource.lblTable + " " + Occupation;
                    case CompanyType.Praia:
                        return AppResource.lblTable + " " + Occupation;
                    default:
                        return AppResource.lblLocation + " " + Occupation;
                }
            }
        }

        public string QtdPeopleStr
        {
            get
            {
                return AppResource.lblQtdPeople + ClientQuantity;
            }
        }

        public int CheckInNumber { get; set; }

        public string CheckInNumberStr { get { return CheckinStatusStr + " #" + CheckInNumber; } }

        public string CheckInCreatedStr { get { return CreatedAt.ToString("dd/MM/yyyy HH:mm"); } }


        public List<Order> Orders { get; set; }

        public decimal PriceTipPaid { get; set; }
        public decimal PriceDiscount { get; set; }
        public decimal PriceTaxPaid { get; set; }
        public decimal TaxPercentage { get; set; }

        public decimal SubTotal { get; set; }

        public decimal TotalPaid { get; set; }
        public decimal TotalToBePaid { get; set; }

        public decimal TotalSpent { get; set; }

        public decimal TipPercentage { get; set; }

        public string LocationId { get; set; }
        public string LocationName { get; set; }

        public string AllocationNumber { get; set; }

        public bool ShowAllocationNumber { get { return string.IsNullOrEmpty(AllocationNumber) ? false : true; } }

        public string Prefix { get; set; }

        public string PrefixAllocation { get { return Prefix + ": " + AllocationNumber; } }

        public string CheckinSubId { get; set; }

        public decimal PaidInCard { get; set; }
        public decimal PaidInCash { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public LocationType LocationType { get; set; }

        public List<CheckinSub> CheckinSubs { get; set; }

        public CheckinSub ActiveLocation { get; set; }

    }
}

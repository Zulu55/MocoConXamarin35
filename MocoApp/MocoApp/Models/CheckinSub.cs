using MocoApp.Extensions;
using MocoApp.Resources;
using System;
using Xamarin.Forms;
using static MocoApp.Models.Enums;

namespace MocoApp.Models
{
    public class CheckinSub : BaseModel
    {
        public virtual string CheckinId { get; set; }
        public virtual Checkin Checkin { get; set; }
        public virtual string LocationId { get; set; }
        public virtual Location Location { get; set; }
        public CheckinSubStatus CheckinSubStatus { get; set; }
        public decimal PaidInCard { get; set; }
        public decimal PaidInCash { get; set; }
        public decimal PaidInCardHiCharlie { get; set; }
        public bool PaidFromAdmin { get; set; }
        public decimal TotalToBePaid { get; set; }
        public string ClientId { get; set; }
        public decimal TotalSpent { get; set; }

        public string LocationNameWithValue
        {
            get
            {
                if (Location == null)
                    return "";
                if (CheckinSubStatus == Enums.CheckinSubStatus.Closed || (CheckinSubStatus == CheckinSubStatus.Pending && Paid))
                    return Location.Name + " (" + AppResource.lblSeeDetails + ") ";
                else if (Location != null && CheckinSubStatus == CheckinSubStatus.RequestedCheckout)
                    return Location.Name + " (" + AppResource.lblSeeDetails + ") ";
                else if (Location != null)
                    return $"{Location.Name} ({AppResource.lblSeeDetails}) ";
                else
                    return $"{Location.Name} ({AppResource.lblSeeDetails})";
            }
        }
        public string CheckinSubStatusStr
        {
            get
            {
                return CheckinSubStatus.ToResource();
            }
        }

        public string CheckinSubStatusStrWithValue
        {
            get
            {
                string status = CheckinSubStatus.ToResource();
                string prefix = "";
                if (PaidInCard != 0)
                    prefix = " " + AppResource.lblCardObs;
                else if (PaymentMethod == PaymentMethod.Money)
                    prefix = " " + AppResource.lblMoneyObs;
                else if (!Paid)
                    prefix = " " + AppResource.lblTransfer;

                if (CheckinSubStatus == CheckinSubStatus.Closed && TotalSpent != TotalPaid)
                    return $"{AppResource.lblTotalMissing}{prefix}";
                else if (CheckinSubStatus == CheckinSubStatus.Pending && Paid)
                {
                    if (PaidInCard != 0)
                        return $"{AppResource.lblCheckinPending}{prefix}";
                    else if (PaidInCash != 0)
                        return $"{AppResource.lblCheckinPending}{prefix}";
                }
                else if (CheckinSubStatus == CheckinSubStatus.Active && (PaymentMethod == PaymentMethod.Money || PaymentMethod == PaymentMethod.Transferred) && !PaidFromAdmin)
                    return $"{AppResource.ECheckinStatusCheckinPending}{prefix}";
                if (CheckinSubStatus == CheckinSubStatus.Pending)
                    return $"{AppResource.lblCheckinPending}{prefix}";

                return status + prefix;
            }
        }

        public string CheckinSubStatusColor
        {
            get
            {
                switch (CheckinSubStatus)
                {
                    case CheckinSubStatus.Pending:
                        return "Orange";
                    case CheckinSubStatus.Active:
                        return "Green";
                    case CheckinSubStatus.RequestedCheckout:
                        return "Red";
                    case CheckinSubStatus.Closed:
                        return "Red";
                    case CheckinSubStatus.Denied:
                        return "Red";
                    default:
                        return "Red";
                }
            }
        }
        public string CheckinSubStatusColorV2
        {
            get
            {
                switch (CheckinSubStatus)
                {
                    case CheckinSubStatus.Pending:
                        {
                            return "Red";
                        }
                    case CheckinSubStatus.Active:
                        if (Paid && PaidInCard == 0)
                            return "Orange";
                        else if (Paid && PaidInCard != 0)
                            return "Green";
                        else if (!PaidFromAdmin && (PaymentMethod == PaymentMethod.Money || PaymentMethod == PaymentMethod.Transferred))
                            return "Orange";
                        return "Blue";
                    case CheckinSubStatus.RequestedCheckout:
                        return "Black";
                    case CheckinSubStatus.Closed:
                        {
                            if (TotalSpent != TotalPaid)
                                return "Orange";
                            else
                                return "Green";
                        }
                    case CheckinSubStatus.Denied:
                        return "Red";
                    default:
                        return "Red";
                }
            }
        }
        public string CheckinSubStatusColorValue
        {
            get
            {
                if (TotalToBePaid != 0)
                    return "Red";
                switch (CheckinSubStatus)
                {
                    case CheckinSubStatus.Pending:
                        {
                            if (Paid && PaidInCash == 0)
                                return "Green";
                            return "Red";
                        }
                    case CheckinSubStatus.Active:
                        if (Paid && PaidInCard != 0)
                            return "Green";
                        if (PaidFromAdmin && PaidInCash != 0)
                            return "Green";
                        return "Red";
                    case CheckinSubStatus.RequestedCheckout:
                        return "Red";
                    case CheckinSubStatus.Closed:
                        {
                            if (TotalSpent != TotalPaid)
                                return "Red";
                            else
                                return "Green";
                        }
                    case CheckinSubStatus.Denied:
                        return "Red";
                    default:
                        return "Red";
                }
            }
        }
        public Color CheckinSubStatusColorValueInColor
        {
            get
            {
                if (TotalToBePaid != 0)
                    return Color.Red;

                switch (CheckinSubStatus)
                {
                    case CheckinSubStatus.Pending:
                        {
                            if (Paid && PaidInCard != 0)
                                return Color.Green;
                            return Color.Red;
                        }
                    case CheckinSubStatus.Active:
                        if (TotalSpent != TotalPaid)
                            return Color.Red;
                        if (Paid && PaidInCard != 0)
                            return Color.Green;
                        if (PaidFromAdmin && PaidInCash != 0)
                            return Color.Green;

                        return Color.Red;
                    case CheckinSubStatus.RequestedCheckout:
                        return Color.Red;
                    case CheckinSubStatus.Closed:
                        {
                            if (TotalSpent != TotalPaid)
                                return Color.Red;
                            else
                                return Color.Green;
                        }
                    case CheckinSubStatus.Denied:
                        return Color.Red;
                    default:
                        return Color.Red;
                }
            }
        }
        public string CheckinSubStatusStrWithValueV2
        {
            get
            {
                if (CheckinSubStatus == CheckinSubStatus.Closed && TotalSpent != TotalPaid)
                    return $"{String.Format(App.AppCurrent.CompanyCulture, "{0:C}", TotalSpent)}";
                else if (CheckinSubStatus == CheckinSubStatus.Pending && Paid)
                {
                    if (PaidInCard != 0)
                        return $"{String.Format(App.AppCurrent.CompanyCulture, "{0:C}", TotalSpent)}";
                    else if (PaidInCash != 0)
                        return $"{String.Format(App.AppCurrent.CompanyCulture, "{0:C}", TotalSpent)}";
                }

                return $"{String.Format(App.AppCurrent.CompanyCulture, "{0:C}", TotalSpent)}";
            }
        }

        public string DetailInfo
        {
            get
            {
                string val = "";
                var resource = AppResource.lblPayLater;
                if (Checkin != null && Checkin.Company != null && (Checkin.Company.CompanyType == CompanyType.Restaurante || Checkin.Company.CompanyType == CompanyType.Praia))
                    resource = AppResource.lblPayLaterTwo;

                if (!Paid && CheckinSubStatus == CheckinSubStatus.Closed)
                    val = resource + " \n" + String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", TotalPaid);
                else if (!Paid)
                    val = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", TotalPaid);

                return val;
            }
        }



        public bool DetailInfoVisible
        {
            get
            {
                return Paid || (!Paid && (CheckinSubStatus == CheckinSubStatus.Closed || CheckinSubStatus == CheckinSubStatus.RequestedCheckout)) ? true : false;

            }
        }

        public bool ShowPaidIcon
        {
            get
            {
                if (Paid && PaidInCard != 0 && CheckinSubStatus == CheckinSubStatus.Pending)
                    return true;

                return Paid && CheckinSubStatus == CheckinSubStatus.Closed && TotalPaid == TotalSpent ? true : false;
            }
        }

        public decimal SubTotal { get; set; }
        public decimal PriceTaxPaid { get; set; }
        public decimal PriceTipPaid { get; set; }
        public decimal PriceDiscount { get; set; }
        public decimal TotalPaid { get; set; }

        public decimal TipPercentage { get; set; }
        public decimal TaxPercentage { get; set; }

        public bool Paid { get; set; }


        public string AllocationNumber { get; set; }

        public string Occupation { get; set; }

        public string ClientQuantity { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public decimal PaidCardTotal { get { return PaidInCard + PaidInCardHiCharlie; } }

    }
}

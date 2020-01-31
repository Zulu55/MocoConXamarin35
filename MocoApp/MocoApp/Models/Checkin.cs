using System;
using System.Collections.Generic;
using static MocoApp.Models.Enums;
using MocoApp.Resources;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MocoApp.Extensions;

namespace MocoApp.Models
{
    public partial class Checkin : BaseModel, INotifyPropertyChanged
    {
   
        public decimal CalcTotalToBePaidByTip(decimal totalTobePaid, decimal companyRecommendedTipPercentage, decimal companyTax, decimal newTip, CheckinSub sub = null)
        {
            if (sub == null)
            {
                decimal percentage = (companyRecommendedTipPercentage + companyTax) / 100;

                var a = (totalTobePaid * percentage);
                var b = (1 + percentage);
                var tipAndTax = a / b;

                var subTotal = totalTobePaid - tipAndTax;
                var tip = (subTotal * companyRecommendedTipPercentage) / 100;

                tipAndTax = tipAndTax - tip;

                return subTotal + tipAndTax + newTip;
            }
            else
            {
                var tip = sub.SubTotal * companyRecommendedTipPercentage / 100;
                var tax = sub.SubTotal * companyTax / 100;

                decimal toSum = totalTobePaid - sub.SubTotal - tip - tax;
                if (toSum < 0)
                    toSum = 0;

                var total = sub.SubTotal + tax + newTip + toSum;
                return total;
            }
        }

        public decimal GetSubTotal(decimal? totalTobePaid = null, decimal? companyRecommendedTipPercentage = null, decimal? companyTax = null)
        {
            if (totalTobePaid == null)
                totalTobePaid = TotalToBePaid;
            if (companyRecommendedTipPercentage == null)
            {
                companyRecommendedTipPercentage = Company.RecommendedTipPercentage;

                if (companyRecommendedTipPercentage != TipPercentage)
                    companyRecommendedTipPercentage = TipPercentage;
            }
            if (companyTax == null)
                companyTax = Company.TaxPercentage;

            decimal percentage = (companyRecommendedTipPercentage.Value + companyTax.Value) / 100;

            var a = (totalTobePaid.Value * percentage);
            var b = (1 + percentage);
            var tipAndTax = a / b;

            var subTotal = totalTobePaid.Value - tipAndTax;

            return subTotal;
        }

        public decimal GetTip(decimal? totalTobePaid = null, decimal? companyRecommendedTipPercentage = null, decimal? companyTax = null, CheckinSub sub = null)
        {
            if (sub == null)
            {
                if (totalTobePaid == null)
                    totalTobePaid = TotalToBePaid;
                if (companyRecommendedTipPercentage == null)
                {
                    companyRecommendedTipPercentage = Company.RecommendedTipPercentage;

                    if (companyRecommendedTipPercentage != TipPercentage)
                        companyRecommendedTipPercentage = TipPercentage;
                }
                if (companyTax == null)
                    companyTax = Company.TaxPercentage;

                decimal percentage = (companyRecommendedTipPercentage.Value + companyTax.Value) / 100; 

                var a = (totalTobePaid.Value * percentage);
                var b = (1 + percentage);
                var tipAndTax = a / b;

                var subTotal = totalTobePaid.Value - tipAndTax; 
                var tip = (subTotal * companyRecommendedTipPercentage.Value) / 100; 

                return tip;
            }
            else
            {
                if (sub.CheckinSubStatus == CheckinSubStatus.RequestedCheckout)
                {
                    return sub.PriceTipPaid;
                }
                else
                {
                    var tip = sub.SubTotal * Company.RecommendedTipPercentage / 100;
                    return tip;
                }
            }

        }

        public decimal GetTax(decimal? totalTobePaid = null, decimal? companyRecommendedTipPercentage = null, decimal? companyTax = null, CheckinSub sub = null)
        {
            if (sub == null)
            {
                if (totalTobePaid == null)
                    totalTobePaid = TotalToBePaid;
                if (companyRecommendedTipPercentage == null)
                {
                    companyRecommendedTipPercentage = Company.RecommendedTipPercentage;

                    if (companyRecommendedTipPercentage != TipPercentage)
                        companyRecommendedTipPercentage = TipPercentage;
                }
                if (companyTax == null)
                    companyTax = Company.TaxPercentage;

                decimal percentage = (companyRecommendedTipPercentage.Value + companyTax.Value) / 100; 

                var a = (totalTobePaid.Value * percentage);
                var b = (1 + percentage);
                var tipAndTax = a / b;//4,5

                var subTotal = totalTobePaid.Value - tipAndTax; 
                var tax = (subTotal * companyTax.Value) / 100; 

                return tax;
            }
            else
            {
                var tax = sub.SubTotal * Company.TaxPercentage / 100;

                return tax;
            }

        }

        public decimal GetTipPercentage(decimal tip, decimal subTotal)
        {
            if (subTotal == 0)
                return 0;

            return Math.Round((tip * 100 / subTotal), 2);
        }

        public decimal GetTaxPercentage(decimal tax, decimal subTotal)
        {
            if (subTotal == 0)
                return 0;

            return Math.Round((tax * 100 / subTotal), 2);
        }

        public List<Location> Locations { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private decimal _cardValue;
        public decimal CardValue
        {
            get
            {
                return _cardValue;
            }
            set
            {
                _cardValue = value;
                Notify("CardValue");

            }
        }

        private decimal _cashValue;
        public decimal CashValue
        {
            get
            {
                return _cashValue;
            }
            set
            {
                _cashValue = value;
                Notify("CashValue");

            }
        }

        protected void Notify(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

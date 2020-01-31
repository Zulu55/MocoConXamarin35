using MocoApp.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MocoApp.Models.Enums;

namespace MocoApp.Extensions
{
    public static class EnumModelExtesion
    {
        public static string ToResource(this CheckinStatus s, bool toLower = false)
        {
            string result = "";

            switch (s)
            {
                case CheckinStatus.CheckinPending:
                    result = AppResource.ECheckinStatusCheckinPending;
                    break;
                case CheckinStatus.Checkin:
                    result = AppResource.ECheckinStatusCheckin;
                    break;
                case CheckinStatus.RequestedCheckout:
                    result = AppResource.ECheckinStatusRequestedCheckout;
                    break;
                case CheckinStatus.Checkout:
                    result = AppResource.ECheckinStatusCheckout;
                    break;
                case CheckinStatus.RequestedCheckin:
                    result = AppResource.ECheckinStatusRequestedCheckin;
                    break;
                case CheckinStatus.Denied:
                    result = AppResource.ECheckinStatusDenied;
                    break;
                default:
                    break;
            }

            if (toLower)
                result = result.ToLower();

            return result;
        }

        public static string ToResource(this ClientCompanyStatus s, bool toLower = false)
        {
            string result = "";

            switch (s)
            {
                case ClientCompanyStatus.CheckinPending:
                    result = AppResource.ECheckinStatusCheckinPending;
                    break;
                case ClientCompanyStatus.Checkin:
                    result = AppResource.ECheckinStatusCheckin;
                    break;
                case ClientCompanyStatus.RequestedCheckout:
                    result = AppResource.ECheckinStatusRequestedCheckout;
                    break;
                case ClientCompanyStatus.Checkout:
                    result = AppResource.ECheckinStatusCheckout;
                    break;
                case ClientCompanyStatus.RequestedCheckin:
                    result = AppResource.ECheckinStatusRequestedCheckin;
                    break;
                case ClientCompanyStatus.Denied:
                    result = AppResource.ECheckinStatusDenied;
                    break;
                default:
                    break;
            }

            if (toLower)
                result = result.ToLower();

            return result;
        }

        public static string ToResource(this CheckinSubStatus s, bool toLower = false)
        {
            string result = "";

            switch (s)
            {
                case CheckinSubStatus.Undefined:
                    result = AppResource.ECheckinSubStatusUndefined;
                    break;
                case CheckinSubStatus.Pending:
                    result = AppResource.ECheckinSubStatusPending;
                    break;
                case CheckinSubStatus.Active:
                    result = AppResource.ECheckinSubStatusActive;
                    break;
                case CheckinSubStatus.Closed:
                    result = AppResource.ECheckinSubStatusClosed;
                    break;
                case CheckinSubStatus.RequestedCheckout:
                    result = AppResource.ECheckinSubStatusRequestedCheckout;
                    break;
                case CheckinSubStatus.Checkout:
                    result = AppResource.ECheckinSubStatusCheckout;
                    break;
                case CheckinSubStatus.Denied:
                    result = AppResource.ECheckinSubStatusDenied;
                    break;
                default:
                    result = AppResource.ECheckinSubStatusUndefined;
                    break;
            }

            if (toLower)
                result = result.ToLower();

            return result;
        }

        public static string ToResource(this OrderStatus s, bool toLower = true)
        {
            string result = "";

            switch (s)
            {
                case OrderStatus.Pending:
                    result = AppResource.EOrderStatusPending;
                    break;
                case OrderStatus.Accepted:
                    result = AppResource.EOrderStatusAccepted;
                    break;
                case OrderStatus.OnGoing:
                    result = AppResource.EOrderStatusOnGoing;
                    break;
                case OrderStatus.Canceled:
                    result = AppResource.EOrderStatusCanceled;
                    break;
                case OrderStatus.Done:
                    result = AppResource.EOrderStatusDone;
                    break;
                case OrderStatus.Closed:
                    result = AppResource.EOrderStatusClosed;
                    break;
                default:
                    break;
            }

            if (toLower)
                result = result.ToLower();

            return result;
        }

        public static string ToCultureStr(this ECurrencyType type)
        {
            string result = "en-us";

            switch (type)
            {
                case ECurrencyType.USD:
                    return "en-us";
                case ECurrencyType.BRT:
                    return "pt-br";
                case ECurrencyType.EUR:
                    return "es-ES";
                case ECurrencyType.COL:
                    return "es-CO";
            }

            return result;
        }

        public static CultureInfo ToCultureInfo(this ECurrencyType type)
        {
            CultureInfo result;

            switch (type)
            {
                case ECurrencyType.USD:
                    result = new CultureInfo("en-us");
                    break;
                case ECurrencyType.BRT:
                    result = new CultureInfo("pt-br");
                    break;
                case ECurrencyType.EUR:
                    result = new CultureInfo("es-es");
                    result.NumberFormat.CurrencyPositivePattern = 0;
                    break;
                case ECurrencyType.COL:
                    result = new CultureInfo("es-co");
                    //result.NumberFormat.CurrencyPositivePattern = 0;
                    break;
                default:
                    result = new CultureInfo("en-us");
                    break;
            }

            return result;
        }
    }
}

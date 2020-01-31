using System.Globalization;

namespace MocoApp.Helpers
{
    public static class Utils
    {
        public static NumberFormatInfo GetCurrencyCUlture(bool force = true)
        {
            return App.AppCurrent.CompanyCulture.NumberFormat;
        }
    }
}

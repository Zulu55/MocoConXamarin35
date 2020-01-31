using System;

namespace MocoApp.Extensions
{
    public static class DoubleExtension
    {
        //public static decimal Floor(this double d, int decimals)
        //{
        //    return Convert.ToDecimal(Math.Floor(d * Math.Pow(10, decimals)) / Math.Pow(10, decimals));
        //}

        public static decimal FloorDecimal(this decimal d, int decimals)
        {
            var input = Convert.ToDouble(d);
            return Convert.ToDecimal(Math.Floor(input * Math.Pow(10, decimals)) / Math.Pow(10, decimals));
        }
    }
}

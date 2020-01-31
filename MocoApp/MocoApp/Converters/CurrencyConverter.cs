﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MocoApp.Converters
{
    public class CurrencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)

        {
            //NumberFormatInfo nfi = App.AppCurrent.Culture.NumberFormat;

            //if (App.AppCurrent.Culture.Name == new CultureInfo("es-ES").Name)
            //{
            //    nfi = new CultureInfo("en-US").NumberFormat;
            //}
            //var nfi = new CultureInfo("pt-BR").NumberFormat;
            var nfi = new CultureInfo("en-US").NumberFormat;

            return Decimal.Parse(value.ToString()).ToString("C", nfi);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string valueFromString = Regex.Replace(value.ToString(), @"\D", "");

            if (valueFromString.Length <= 0)
                return 0m;

            long valueLong;
            if (!long.TryParse(valueFromString, out valueLong))
                return 0m;

            if (valueLong <= 0)
                return 0m;

            return valueLong / 100m;
        }
    }
}

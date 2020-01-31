using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MocoApp.Droid.Renderers;
using MocoApp.Interfaces;
using Xamarin.Forms;
using System.Globalization;

[assembly: Xamarin.Forms.Dependency(typeof(StatusBarImplementation))]
namespace MocoApp.Droid.Renderers
{
    public class StatusBarImplementation : IStatusBar
    {
        public StatusBarImplementation()
        {
        }

        int red;
        int blue;
        int green;
        WindowManagerFlags _originalFlags;

        #region IStatusBar implementation


        public void ShowStatusBar()
        {
            try
            {
                HexToColor("#0194CA");

                var activity = (Activity)Forms.Context;
                //activity.Window.SetStatusBarColor(Android.Graphics.Color.Rgb(red, green, blue));

            }
            catch (Exception)
            {


            }

        }

        public void HideStatusBar(string color)
        {
            try
            {
                HexToColor(color);
                var activity = (Activity)Forms.Context;
                activity.Window.AddFlags(WindowManagerFlags.KeepScreenOn);
                activity.Window.SetStatusBarColor(Android.Graphics.Color.Rgb(red, green, blue));
            }
            catch (Exception ex)
            {


            }


        }

        private void HexToColor(string hexColor)
        {
            //Remove # if present
            if (hexColor.IndexOf('#') != -1)
                hexColor = hexColor.Replace("#", "");

            red = 0;
            green = 0;
            blue = 0;

            if (hexColor.Length == 6)
            {
                //#RRGGBB
                red = int.Parse(hexColor.Substring(0, 2), NumberStyles.AllowHexSpecifier);
                green = int.Parse(hexColor.Substring(2, 2), NumberStyles.AllowHexSpecifier);
                blue = int.Parse(hexColor.Substring(4, 2), NumberStyles.AllowHexSpecifier);
            }
            else if (hexColor.Length == 3)
            {
                //#RGB
                red = int.Parse(hexColor[0].ToString() + hexColor[0].ToString(), NumberStyles.AllowHexSpecifier);
                green = int.Parse(hexColor[1].ToString() + hexColor[1].ToString(), NumberStyles.AllowHexSpecifier);
                blue = int.Parse(hexColor[2].ToString() + hexColor[2].ToString(), NumberStyles.AllowHexSpecifier);
            }


        }
        #endregion
    }
}
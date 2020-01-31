using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using MocoApp.iOS.Renderers;
using MocoApp.Interfaces;

[assembly: Xamarin.Forms.Dependency(typeof(PhoneDialer))]
namespace MocoApp.iOS.Renderers
{
    public class PhoneDialer : IDialer
    {
        /// <summary>  
        /// Gets Number from user and Dial   
        /// </summary>  
        /// <param name="number"></param>  
        /// <returns></returns>  
        public bool Dial(string number)
        {
            return UIApplication.SharedApplication.OpenUrl(
                new NSUrl("tel:" + number));
        }
    }
}
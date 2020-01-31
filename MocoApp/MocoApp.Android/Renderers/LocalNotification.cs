using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

namespace MocoApp.Droid.Renderers
{
    public class LocalNotification
    {

        static readonly int NOTIFICATION_ID = 1000;
        static readonly string CHANNEL_ID = "location_notification";
        internal static readonly string COUNT_KEY = "count";
        //public void CreateNotification(string title, string message)
        //{
        //    var builder = new NotificationCompat.Builder(, CHANNEL_ID)
        //                 .SetAutoCancel(true) // Dismiss the notification from the notification area when the user clicks on it
        //                 .SetContentTitle("Button Clicked") // Set the title
        //                 .SetNumber(1) // Display the count in the Content Info
        //                 .SetSmallIcon(Resource.Drawable.appIcon) // This is the icon to display
        //                 .SetContentText($"The button has been clicked times."); // the message to display.


        //}
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using MocoApp.Interfaces;
using MocoApp.iOS.Renderers;
using UIKit;


[assembly: Xamarin.Forms.Dependency(typeof(LocalNotification))]
namespace MocoApp.iOS.Renderers
{
    public class LocalNotification : ILocalNotification
    {

        public void CreateNotification(string title, string message)
        {
            // create the notification
            var notification = new UILocalNotification();

            //// set the fire date (the date time in which it will fire)
            //notification.FireDate = NSDate.FromTimeIntervalSinceNow(secs);

            // configure the alert

            notification.AlertBody = message;
            notification.AlertTitle = title;
            //notification.AlertAction = date;



            // modify the badge
            notification.ApplicationIconBadgeNumber = 1;

            // set the sound to be the default sound
            notification.SoundName = UILocalNotification.DefaultSoundName;


            // schedule it
            UIApplication.SharedApplication.ScheduleLocalNotification(notification);
        }
    }
}
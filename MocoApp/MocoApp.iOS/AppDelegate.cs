using System;
using System.Collections.Generic;
using System.Linq;
using MocoApp;
using Foundation;
using UIKit;
using FFImageLoading.Forms.Touch;
using ImageCircle.Forms.Plugin.iOS;
using WindowsAzure.Messaging;
using Xamarin.Forms;
using ObjCRuntime;
using Xamarin;
using Com.OneSignal;

namespace MocoApp.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public App CurrentApp { get; private set; }
        private SBNotificationHub Hub { get; set; }
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                var pushSettings = UIUserNotificationSettings.GetSettingsForTypes(
                       UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
                       new NSSet());

                UIApplication.SharedApplication.RegisterUserNotificationSettings(pushSettings);
                UIApplication.SharedApplication.RegisterForRemoteNotifications();
            }
            else
            {
                UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound;
                UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(notificationTypes);
            }

            ImageCircleRenderer.Init();
            CachedImageRenderer.Init();
            Xamarin.FormsMaps.Init();
            Rg.Plugins.Popup.Popup.Init();

            IQKeyboardManager.SharedManager.Enable = true;
            IQKeyboardManager.SharedManager.EnableAutoToolbar = false;



            CurrentApp = new App();


            LoadApplication(CurrentApp);
            OneSignal.Current.StartInit("0f3334c6-d6e1-46ab-9dc5-99d96b0e8c29").EndInit();


            return base.FinishedLaunching(app, options);
        }

        public override async void OnActivated(UIApplication uiApplication)
        {
            base.OnActivated(uiApplication);

            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;

            if (!CurrentApp.Initialized)
            {
                await CurrentApp.InitializeApp();
            }


        }

        [Export("oneSignalApplicationDidBecomeActive:")]
        public void OneSignalApplicationDidBecomeActive(UIApplication application)
        {
            Console.WriteLine("oneSignalApplicationDidBecomeActive:");
            OnActivated(application);

        }
        [Export("oneSignalDidFailRegisterForRemoteNotification:error:")]
        public void OneSignalDidFailRegisterForRemoteNotification(UIApplication app, NSError error)
        {
            Console.WriteLine("oneSignalDidFailRegisterForRemoteNotification:error:");
        }

        [Export("oneSignalApplicationWillResignActive:")]
        public void OneSignalApplicationWillResignActive(UIApplication application)
        {
            Console.WriteLine("OneSignalApplicationWillResignActive");

            OnResignActivation(application);
        }

        public override void ReceivedRemoteNotification(UIApplication app, NSDictionary userInfo)
        {
            try
            {
                UILocalNotification notification = new UILocalNotification();
                notification.ApplicationIconBadgeNumber = 1;
                notification.SoundName = UILocalNotification.DefaultSoundName;

                MessagingCenter.Send<object, string>(this, "Push", "PushReceived");
            }
            catch (Exception)
            {


            }

        }

    }
}

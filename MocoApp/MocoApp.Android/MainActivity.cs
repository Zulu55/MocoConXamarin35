using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.Forms;
using FFImageLoading.Forms.Droid;
using ImageCircle.Forms.Plugin.Droid;
using Acr.UserDialogs;
using Com.OneSignal;
using Plugin.LocalNotifications;

namespace MocoApp.Droid
{
    [Activity(Label = "HiCharlie", Icon = "@drawable/AppIcon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public App CurrentApp { get; private set; }

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            Rg.Plugins.Popup.Popup.Init(this, bundle);
            UserDialogs.Init(this);
            ImageCircleRenderer.Init();
            CachedImageRenderer.Init();

            CurrentApp = new App();


            OneSignal.Current.StartInit("0f3334c6-d6e1-46ab-9dc5-99d96b0e8c29")
        .EndInit();

            LoadApplication(CurrentApp);


            LocalNotificationsImplementation.NotificationIconId = Resource.Drawable.appIcon;
            

        }

        protected override async void OnStart()
        {
            base.OnStart();

            if (!CurrentApp.Initialized)
            {
                await CurrentApp.InitializeApp();
            }

        }

        public override void OnBackPressed()
        {
            if (Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed))
            {
                // Do something if there are some pages in the `PopupStack`
            }
            else
            {
                // Do something if there are not any pages in the `PopupStack`
            }
        }

    }
}


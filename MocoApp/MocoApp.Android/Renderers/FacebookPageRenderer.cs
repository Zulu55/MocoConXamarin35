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
using MocoApp.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using MocoApp.Droid.Renderers;
using Xamarin.Auth;

[assembly: ExportRenderer(typeof(FacebookSyncPage), typeof(FacebookPageRenderer))]
namespace MocoApp.Droid.Renderers
{

    public class FacebookPageRenderer : PageRenderer
    {
        public FacebookPageRenderer()
        {
            var activity = this.Context as Activity;

            var auth = new OAuth2Authenticator(
                clientId: "1859340717412698",
                scope: "email",
                authorizeUrl: new Uri("https://m.facebook.com/dialog/oauth/"),
                redirectUrl: new Uri("https://www.facebook.com/connect/login_success.html"));

            auth.Completed += async (sender, eventArgs) => {
                if (eventArgs.IsAuthenticated)
                {
                    var accessToken = eventArgs.Account.Properties["access_token"].ToString();
                    App.NavigateToProfile(accessToken);
                }
                else                
                    App.NavigateToProfile("Erro");
                
            };

            activity.Window.SetTitle("Entrar com Facebook");
            activity.StartActivity(auth.GetUI(activity));
        }
    }
   
}
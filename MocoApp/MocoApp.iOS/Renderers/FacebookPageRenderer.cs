using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Forms.Platform.iOS;
using MocoApp.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Auth;
using MocoApp.Views;

[assembly: ExportRenderer(typeof(FacebookSyncPage), typeof(FacebookPageRenderer))]
namespace MocoApp.iOS.Renderers
{
    public class FacebookPageRenderer : PageRenderer
    {
        bool done = false;
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            if (done)
                return;

            var auth = new OAuth2Authenticator(
                clientId: "1859340717412698", // your OAuth2 client id
                scope: "email", // the scopes for the particular API you're accessing, delimited by "+" symbols
                authorizeUrl: new Uri("https://m.facebook.com/dialog/oauth/"),
                redirectUrl: new Uri("https://www.facebook.com/connect/login_success.html"));

            auth.Completed += async (sender, eventArgs) =>
            {
                DismissViewController(true, null);
                App.HideLoginView();

                if (eventArgs.IsAuthenticated)
                {
                    var accessToken = eventArgs.Account.Properties["access_token"].ToString();


                    await App.NavigateToProfile(accessToken);
                }
                else
                {
                    await App.NavigateToProfile("Erro");
                }
            };

            done = true;
            PresentViewController(auth.GetUI(), true, null);
        }
    }
}
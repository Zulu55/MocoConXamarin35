using MocoApp.ViewModels;
using Naylah.Xamarin.Controls.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MocoApp.Views
{
    public class SplashPage : ContentPageBase
    {
        SplashViewModel VM => BindingContext as SplashViewModel;
        public SplashPage()
        {

            BindingContext = new SplashViewModel();

            NavigationPage.SetHasNavigationBar(this, false);

            //BackgroundColor = Color.FromHex("#2196F3");
            BackgroundColor = Color.White;

            Content = new Grid()
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children =
                {
                    new Image() { Source = "splash", Aspect = Aspect.AspectFit },
                    
                    new StackLayout
                    {
                        HorizontalOptions = LayoutOptions.EndAndExpand,
                        VerticalOptions = LayoutOptions.EndAndExpand,
                        Spacing = 0,
                        Children =
                        {
                                new ActivityIndicator() { IsEnabled = true, IsRunning = true, Color = Color.White },
                                 new Label() { Text = "Carregando..", TextColor = Color.White }
                        }
                    }
                }
            };
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MocoApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImageFullScreenPage : ContentPage
    {
        public ImageFullScreenPage(string uri)
        {
            InitializeComponent();

            imgPage.Source = uri;
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void OnBackTapped(object sender, EventArgs e)
        {
            await App.AppCurrent.NavigationService.ModalGoBack();
        }
    }
}
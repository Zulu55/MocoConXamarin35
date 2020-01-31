using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MocoApp.Views.CompanyFluxo
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReportPage : ContentPage
    {
        public ReportPage()
        {
            InitializeComponent();

            if (Device.OS == TargetPlatform.iOS)
            {
                // move layout under the status bar
                this.Padding = new Thickness(0, 0, 0, 0);

                var menu = new ToolbarItem("Menu", "", () =>
                {
                    App app = Application.Current as App;
                    Naylah.Xamarin.Controls.Pages.MasterDetailNavigationPage md = (Naylah.Xamarin.Controls.Pages.MasterDetailNavigationPage)app.MainPage;

                    md.IsPresented = true;

                }, 0, 0);
                menu.Priority = 0;

                ToolbarItems.Add(menu);
            }
        }

        private async void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            await App.AppCurrent.NavigationService.NavigateAsync(new ReportProductPage(), null, true);
        }

        private async void TapGestureRecognizer_Tapped_4(object sender, EventArgs e)
        {
            await App.AppCurrent.NavigationService.NavigateAsync(new ReportCompanyPage(), null, true);
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await App.AppCurrent.NavigationService.NavigateAsync(new OrderHistoryPage(), null, true);
        }
    }
}
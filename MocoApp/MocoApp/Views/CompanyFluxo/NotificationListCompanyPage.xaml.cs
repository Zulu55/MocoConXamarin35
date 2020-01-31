using MocoApp.Models;
using MocoApp.Resources;
using MocoApp.Services;
using Newtonsoft.Json;
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
    public partial class NotificationListCompanyPage : ContentPage
    {
        public NotificationListCompanyPage()
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

            LoadNotifications();
        }

        public async void LoadNotifications()
        {
            ApiService service = new ApiService();

            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);


                //var result = await service.GetAsync("notification/getNotificationsByCompanyId?id=" + Helpers.Settings.DisplayUserCompany);
                var result = await service.GetAsync("notification/getNotificationsByUserId?id=" + Helpers.Settings.DisplayUserId);

                var list = JsonConvert.DeserializeObject<List<Notification>>(result);

                listView.ItemsSource = list;
                listView.ItemTapped += ListView_ItemTapped;

            }
            catch (Exception ex)
            {

            }
            finally
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
        }

        private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (listView.SelectedItem == null)
                return;


            listView.SelectedItem = null;
        }

    }
}
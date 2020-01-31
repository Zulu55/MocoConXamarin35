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

namespace MocoApp.Views.Cliente
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListNotificationClientPage : ContentPage
    {
        public ListNotificationClientPage()
        {
            InitializeComponent();

            if (Device.RuntimePlatform == "Android")
                frmXaml.CornerRadius = 30;

            NavigationPage.SetHasNavigationBar(this, false);


            LoadNotifications();
        }

        public async void LoadNotifications()
        {
            ApiService service = new ApiService();

            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);


                var result = await service.GetAsync("notification/getNotificationsByClientId?id=" + Helpers.Settings.DisplayUserId);
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

            //var item = listView.SelectedItem as ClientCompany;

            //await App.AppCurrent.NavigationService.NavigateModalAsync(new ListPedidoPage(item), null, false);

            listView.SelectedItem = null;
        }


        private async void OnMenuTapped(object sender, EventArgs e)
        {
            App app = Application.Current as App;
            Naylah.Xamarin.Controls.Pages.MasterDetailNavigationPage md = (Naylah.Xamarin.Controls.Pages.MasterDetailNavigationPage)app.MainPage;
            md.IsPresented = true;
        }
    }
}
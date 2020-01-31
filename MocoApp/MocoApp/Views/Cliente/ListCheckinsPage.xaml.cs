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
    public partial class ListCheckinsPage : ContentPage
    {

        List<Checkin> ListCheckin;
        public ListCheckinsPage()
        {
            InitializeComponent();
            if (Device.RuntimePlatform == "Android")
                frmXaml.CornerRadius = 30;

            NavigationPage.SetHasNavigationBar(this, false);

            LoadCheckins();
            
            
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if(App.AppCurrent.IsToRefresh)
            {
                LoadCheckins();
                App.AppCurrent.IsToRefresh = false;
            }

            listView.ItemTapped += ListView_ItemTapped;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            listView.ItemTapped -= ListView_ItemTapped;
        }

        public async void LoadCheckins()
        {
            OrderService Orderservice = new OrderService();

            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);


                var result = await Orderservice.GetcheckinsByClientId();
                ListCheckin = JsonConvert.DeserializeObject<List<Checkin>>(result);

                listView.ItemsSource = ListCheckin;


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

            var item = listView.SelectedItem as Checkin;         

            if(item.Company.HasLocation)
                await App.AppCurrent.NavigationService.NavigateModalAsync(new ListLocationBillsPage(item), null, false);
            else
                await App.AppCurrent.NavigationService.NavigateModalAsync(new ListPedidoPage(item), null, false);

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
using MocoApp.Extensions;
using MocoApp.Models;
using MocoApp.Resources;
using MocoApp.Services;
using MocoApp.Views.CompanyFluxo;
using MocoApp.Views.ManagerCheckinFlow;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MocoApp.Views.ManagerCheckinFlow
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClientListPage : ContentPage
    {
        List<Checkin> ListCheckins;
        List<Location> ListLocation;
        string _locationId = "";
        public ClientListPage()
        {
            InitializeComponent();

            listView.RefreshCommand = new Command(() => LoadCheckins());

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

            var hist = new ToolbarItem(AppResource.lblHistory, "ic_history", async () =>
            {
                await App.AppCurrent.NavigationService.NavigateAsync(new CheckinHistoryPage(), null, false);
            }, 0, 0);
            hist.Priority = 1;
            ToolbarItems.Add(hist);

            var chatIcon = new ToolbarItem("Chat", "ic_chat_message", async () =>
            {

                await App.AppCurrent.NavigationService.NavigateSetRootAsync(new Chat.CompanyChatList(), null, false);
            }, 0, 0);
            chatIcon.Priority = 1;
            ToolbarItems.Add(chatIcon);

            LoadLocations();

       
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await App.AppCurrent.NavigationService.NavigateAsync(new AddNewClientPage(), null, true);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            LoadCheckins();

            MessagingCenter.Subscribe<object>(this, "Push", (sender) => {
                var page = sender as Page;
                if (page is ClientListPage)
                {
                    LoadCheckins();
                }
            });

            listView.ItemTapped += ListView_ItemTapped;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            MessagingCenter.Unsubscribe<object, string>(this, "Push");
            listView.ItemsSource = null;
            listView.ItemTapped -= ListView_ItemTapped;
        }

        private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (listView.SelectedItem == null)
                return;

            var item = listView.SelectedItem as Checkin;

            if (item.CheckinStatus == Enums.CheckinStatus.RequestedCheckin)
                await App.AppCurrent.NavigationService.NavigateAsync(new AcceptDeclineCheckinPage(item), null, false);
            else
                await App.AppCurrent.NavigationService.NavigateAsync(new CheckinDetailPage(item), null, false);

            listView.SelectedItem = null;
        }

        public async void LoadLocations()
        {
            CompanyService companyService = new CompanyService();

            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);


                var result = await companyService.GetLocationsByCompanyId();
                ListLocation = JsonConvert.DeserializeObject<List<Location>>(result);


                if (ListLocation != null)
                {
                    stkLocation.IsVisible = true;
                    pckLocations.Items.Add(AppResource.txtAll);
                    foreach (var item in ListLocation)
                    {
                        pckLocations.Items.Add(item.Name);
                    }

                    pckLocations.SelectedIndexChanged += PckLocations_SelectedIndexChanged;
                }
                else
                    stkLocation.IsVisible = false;

                
            }
            catch (Exception ex)
            {
                this.DisplayAlert(MocoApp.Resources.AppResource.alertAlert, ex.Message, AppResource.textOk);
            }
            finally
            {
                listView.IsRefreshing = false;
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
        }

        private void PckLocations_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = sender as Picker;

            if (item.SelectedIndex < 0)
                return;

            if (item.Items[item.SelectedIndex] == AppResource.txtAll)
            {
                _locationId = "";
            }
            else
            {
                _locationId = ListLocation.Where(m => m.Name == item.Items[item.SelectedIndex]).FirstOrDefault().Id;
                listView.ItemsSource = null;
            }

            LoadCheckins();
        }

        public async void LoadCheckins()
        {
            OrderService orderService = new OrderService();

            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);
                listView.IsRefreshing = false;


                var result = await orderService.GetCheckinsByCompanyId(_locationId);
                ListCheckins = new List<Checkin>();
                ListCheckins = JsonConvert.DeserializeObject<List<Checkin>>(result);

                if (ListCheckins.Count > 0)
                {
                    App.AppCurrent.CompanyCulture = ListCheckins.FirstOrDefault().Company.CurrencyType.ToCultureInfo();
                    listView.ItemsSource = null;
                    listView.ItemsSource = ListCheckins;
                    lblEmpty.IsVisible = false;
                }
                else
                {
                    listView.ItemsSource = ListCheckins;
                    lblEmpty.IsVisible = true;
                }

            }
            catch (Exception ex)
            {
                this.DisplayAlert(MocoApp.Resources.AppResource.alertAlert, ex.Message, AppResource.textOk);
            }
            finally
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
        }
    }
}
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
    public partial class OrderAcceptedListPage : ContentPage
    {
        List<Order> ListOrder;
        public OrderAcceptedListPage()
        {
            InitializeComponent();
            LoadOrders();

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

            var chatIcon = new ToolbarItem("Chat", "ic_chat_message", async () =>
            {

                await App.AppCurrent.NavigationService.NavigateSetRootAsync(new Chat.CompanyChatList(), null, false);
            }, 0, 0);
            chatIcon.Priority = 1;
            ToolbarItems.Add(chatIcon);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            listView.ItemTapped += ListView_ItemTapped;
            listView.ItemsSource = ListOrder;


        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            listView.ItemsSource = null;
            listView.ItemTapped -= ListView_ItemTapped;
        }

        private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (listView.SelectedItem == null)
                return;

            var item = listView.SelectedItem as Order;

            await App.AppCurrent.NavigationService.NavigateAsync(new OrderDetailPage(item), null, false);

            listView.SelectedItem = null;
        }

        public async void LoadOrders()
        {
            OrderService orderService = new OrderService();

            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);


                var result = await orderService.GetMyOrdersAccepted();
                ListOrder = JsonConvert.DeserializeObject<List<Order>>(result);

                if (ListOrder.Count > 0)
                {
                    listView.ItemsSource = ListOrder;
                    lblEmpty.IsVisible = false;
                }
                else
                    lblEmpty.IsVisible = true;

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
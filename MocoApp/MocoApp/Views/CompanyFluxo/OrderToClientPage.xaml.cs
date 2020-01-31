using MocoApp.Models;
using MocoApp.Resources;
using MocoApp.Services;
using Newtonsoft.Json;
using Plugin.DeviceInfo;
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
    public partial class OrderToClientPage : ContentPage
    {
        Checkin _checkin;
        string _entregaTapped = "";
        Product Product;
        CheckinSub _checkinSub;
        public OrderToClientPage(Checkin checkin, Product product, CheckinSub checkinSub  = null)
        {
            InitializeComponent();

            _checkin = checkin;
            Product = product;
            _checkinSub = checkinSub;
            lblProductName.Text = Product.Name;
            lblProductDescription.Text = product.Description;
            imgProduct.Source = product.ImageUri;

            lblPrice.Text = product.PriceStr;

            this.BindingContext = _checkin;
        }

        public async void CreateOrder()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);

                CreateOrderCommandV2 createOrder = new CreateOrderCommandV2()
                {
                    CheckinId = _checkin.Id,
                    CheckinSubId = _checkinSub?.Id,
                    Observation = txtObs.Text,
                    ProductId = Product.Id,
                    ProductQuantity = Convert.ToInt32(txtQtd.Text),
                    UserId = Helpers.Settings.DisplayUserId
                };

                if (Device.RuntimePlatform == "iOS")
                {
                    createOrder.Version = CrossDeviceInfo.Current.AppBuild;
                    createOrder.Device = "iOS";
                }
                else
                {
                    createOrder.Version = CrossDeviceInfo.Current.AppVersion;
                    createOrder.Device = "Android";
                }

                OrderService orderService = new OrderService();

                var result = await orderService.CreateOrderV2(createOrder);

                var order = JsonConvert.DeserializeObject<Order>(result);
                if (_checkin.Orders == null)
                    _checkin.Orders = new List<Order>();

                _checkin.Orders.Add(order);

                Acr.UserDialogs.UserDialogs.Instance.Toast(AppResource.alertOrderSucess);
                await App.AppCurrent.NavigationService.GoBack();

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

        private async void OnChurrasqueiraTapped(object sender, EventArgs e)
        {
            if (_entregaTapped != "Churrasqueira")
            {
                _entregaTapped = "Churrasqueira";
                imgChurrasqueira.Source = "ic_check";
                imgPraia.Source = "ic_uncheck";
                imgQuarto.Source = "ic_uncheck";
            }

        }

        private async void OnQuartoTapped(object sender, EventArgs e)
        {
            if (_entregaTapped != "Quarto")
            {
                _entregaTapped = "Quarto";
                imgQuarto.Source = "ic_check";
                imgChurrasqueira.Source = "ic_uncheck";
                imgPraia.Source = "ic_uncheck";
            }

        }

        private async void OnPraiaTapped(object sender, EventArgs e)
        {
            if (_entregaTapped != AppResource.lblBeachs)
            {
                _entregaTapped = AppResource.lblBeachs;
                imgPraia.Source = "ic_check";
                imgChurrasqueira.Source = "ic_uncheck";
                imgQuarto.Source = "ic_uncheck";
            }

        }

        private void btnAdd_Clicked(object sender, EventArgs e)
        {
            CreateOrder();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MocoApp.Models;
using MocoApp.Resources;
using MocoApp.Services;
using MocoApp.Services.V2;
using MocoApp.Views.CartFlow;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static MocoApp.Models.Enums;
using static MocoApp.Views.CartFlow.CheckoutCartPage;

namespace MocoApp.Views.Cliente
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainBillCheckoutPage : ContentPage
    {
        List<SelectionItem> paymentMethods;
        Checkin _checkin;
        decimal _pricetippaid;
        bool _fromCompanyHome;
        decimal _paidInCash;
        decimal _paidInCard;
        bool _ignoreOccupation;
        private string _stripeId;
        public MainBillCheckoutPage(Checkin checkin, decimal pricetippaid, bool FromCompanyHome = false, decimal PaidInCash = 0, decimal PaidInCard = 0, bool ignoreOccupation = false, PaymentMethod method = PaymentMethod.NotInformed)
        {
            InitializeComponent();

            if (Device.RuntimePlatform == "Android")
                frmXaml.CornerRadius = 30;

            _checkin = checkin;
            _pricetippaid = pricetippaid;
            _fromCompanyHome = FromCompanyHome;
            _paidInCash = PaidInCash;
            _paidInCard = PaidInCard;
            _ignoreOccupation = ignoreOccupation;

            _stripeId = "";
        }

        protected override async void OnAppearing()
        {
            await LoadPage();

            base.OnAppearing();
        }

        private async Task LoadPage()
        {
            btnSendOrder.Text = AppResource.lblCloseBill + " - " + String.Format(App.AppCurrent.CompanyCulture, "{0:C}", _checkin.TotalToBePaid);
            if (!_checkin.Company.CreditCardAllowed)
                grdAddCard.IsVisible = false;
            else
                grdAddCard.IsVisible = true;

            await LoadWallet();
        }

        public async Task LoadWallet()
        {
            paymentMethods = new List<SelectionItem>()
                {
                    new SelectionItem() { Id = "2", Label = AppResource.textCartCash },
                };

            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);
                if (_checkin.Company.CreditCardAllowed)
                {
                    var userService = new UserServicev2();
                    var myWallet = await userService.GetMyWalletV2();

                    foreach (var item in myWallet)
                    {
                        paymentMethods.Add(
                                new SelectionItem() { Id = item.StripeId, Label = item.ShowName, IsSelected = item.Default }
                        );
                        if (item.Default)
                            _stripeId = item.StripeId;
                    }
                }

                listPaymentMethods.ItemsSource = paymentMethods;
                listPaymentMethods.HeightRequest = paymentMethods.Count * 40 + 48;
                listPaymentMethods.ItemTapped += async (sender, args) =>
                {
                    foreach (var item in paymentMethods)
                    {
                        item.IsSelected = false;
                    }

                    var selected = args.Item as SelectionItem;
                    selected.IsSelected = true;

                    _stripeId = selected.Id;

                    //App.AppCurrent.Cart.PaymentMethod = selected.Id;
                };
                //else
                //{
                //    grdAddCard.IsVisible = false;
                //}
            }
            catch (Exception ex)
            {

            }
            finally
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
        }

        private async void OnMenuTapped(object sender, EventArgs e)
        {
            await App.AppCurrent.NavigationService.ModalGoBack();
        }

        private async void OnCheckout_Tapped(object sender, EventArgs e)
        {
            try
            {
                if (!paymentMethods.Any(s => s.IsSelected))
                {
                    await DisplayAlert(AppResource.lblSelect, AppResource.alertPaymentMethod, AppResource.textOk);
                    return;
                }

                PaymentMethod method = PaymentMethod.NotInformed;
                if (paymentMethods.Any(s => s.IsSelected && s.Id == "2"))
                    method = PaymentMethod.Money;
                else
                    method = PaymentMethod.Card;

                CompanyService service = new CompanyService();
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);
                //var result = await service.RequestCheckoutFromClient(_company.Id, _checkoutTax.PriceTipPaid, false, valordCash, valorCard, true, paymentType);

                var result = await service.RequestCheckoutFromClient(_checkin.CompanyId, _pricetippaid, true, _paidInCash, _paidInCard, true, method, _stripeId);

                _checkin.Company.CheckedIn = JsonConvert.DeserializeObject<bool>(result);
                _checkin.CheckinStatus = Enums.CheckinStatus.RequestedCheckout;

                App.AppCurrent.IsToRefresh = true;

                Acr.UserDialogs.UserDialogs.Instance.Toast(AppResource.alertRequestSucess, TimeSpan.FromSeconds(5));
                await App.AppCurrent.NavigationService.ModalGoBack();
                await App.AppCurrent.NavigationService.ModalGoBack();
                await App.AppCurrent.NavigationService.ModalGoBack();
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

        private async void OnAddCard_Tapped(object sender, EventArgs e)
        {
            App.AppCurrent.NavigationService.NavigateModalAsync(new AddCardPage(true), null, true);

        }
    }
}
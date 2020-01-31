using MocoApp.Controls;
using MocoApp.Helpers;
using MocoApp.Models;
using MocoApp.Resources;
using MocoApp.Services;
using MocoApp.Views.CompanyFluxo;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static MocoApp.Models.Enums;

namespace MocoApp.Views.Cliente
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LocationBillOrderPage : ContentPage
    {


        CheckinSubOrders _checkinSubOrders;

        OrderService Orderservice = new OrderService();
        decimal _tip;
        decimal _total;
        decimal _discount;
        decimal _subTotal;
        decimal _tax;
        Checkin _checkin;
        CheckinSub _sub;
        public LocationBillOrderPage(CheckinSub sub, Checkin checkin)
        {
            InitializeComponent();

            if (Device.RuntimePlatform == "Android")
                frmXaml.CornerRadius = 30;

            NavigationPage.SetHasNavigationBar(this, false);

            _sub = sub;
            _checkin = checkin;
            lblTitle.Text = sub.Location.Name;

            ColorPage();
            LoadOrders();

            //if(_checkin.Company.CompanyType == CompanyType.Hotel)
            //{
            //    btnPayNow.IsVisible = true;
            //    btnPayNowCard.IsVisible = true;
            //}

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<object>(this, "Push", (sender) =>
            {
                if (App.AppCurrent.NavigationService.ModalCurrentPage is LocationBillOrderPage)
                {
                    LoadOrders();
                }
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<Page, string>(this, "Push");
        }

        private async void OnMenuTapped(object sender, EventArgs e)
        {
            await App.AppCurrent.NavigationService.ModalGoBack();
        }

        public async void LoadOrders()
        {
            ApiService apiService = new ApiService();

            try
            {
                listView.ItemsSource = null;
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);

                var result = await apiService.GetAsync("order/getOrdersByCheckinSubId?id=" + _sub.Id);
                _checkinSubOrders = JsonConvert.DeserializeObject<CheckinSubOrders>(result);





                if (_checkinSubOrders.Paid)
                {
                    if (_checkinSubOrders.CheckinSubStatus == CheckinSubStatus.Closed)
                        imgPaid.IsVisible = true;

                    lblCardPaid.Text = AppResource.lblCard + ": " + String.Format( App.AppCurrent.CompanyCulture, "{0:C}", _checkinSubOrders.PaidCardTotal);
                    lblCashPaid.Text = AppResource.lblMoney + ": " + String.Format(App.AppCurrent.CompanyCulture, "{0:C}", _checkinSubOrders.PaidInCash);
                    stkBtuttonOrder.IsVisible = false;
                }
                else if (_checkinSubOrders.CheckinSubStatus == CheckinSubStatus.RequestedCheckout
                    || _checkinSubOrders.CheckinSubStatus == CheckinSubStatus.Checkout
                    || _checkinSubOrders.CheckinSubStatus == CheckinSubStatus.Closed)
                {
                    stkBtuttonOrder.IsVisible = false;
                }
                else
                {
                    imgPaid.IsVisible = false;
                    stkBtuttonOrder.IsVisible = true;
                }



                listView.ItemsSource = _checkinSubOrders.Orders;
                LoadPrices();

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

        public void LoadPrices()
        {

            //lblSubTotal.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", _checkinSubOrders.TotalSpent);
            lblSubTotal.Text = String.Format(App.AppCurrent.CompanyCulture, "{0:C}", _checkinSubOrders.SubTotal);
            _total = _checkinSubOrders.TotalPaid;

            lblTaxsTitle.Text = AppResource.lblTaxs + "(" + _checkinSubOrders.TaxPercentage + "%)";
            lblTipTitle.Text = AppResource.lblTips + "(" + +_checkinSubOrders.TipPercentage + "%)";
            lblDesconto.Text = String.Format(App.AppCurrent.CompanyCulture, "{0:C}", _checkinSubOrders.PriceDiscount);
            lblTotal.Text = String.Format(   App.AppCurrent.CompanyCulture, "{0:C}", _total);
            lblTax.Text = String.Format(     App.AppCurrent.CompanyCulture, "{0:C}", _checkinSubOrders.PriceTaxPaid);
            lblTip.Text = String.Format(App.AppCurrent.CompanyCulture, "{0:C}", _checkinSubOrders.PriceTipPaid);

            //decimal totalWithTip = _total * _checkinSubOrders.TipPercentage / 100;
            //lblTip.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", totalWithTip);

            //decimal totalWithTax = _total * _checkin.Company.TaxPercentage / 100;
            //lblTax.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", totalWithTax);




            //var newTotal = (_total + totalWithTax + totalWithTip) - _checkin.PriceDiscount;
            //_total = newTotal;



            _discount = _checkinSubOrders.PriceDiscount;
            _tip = _checkinSubOrders.PriceTipPaid;
            _subTotal = _checkinSubOrders.SubTotal;
            _tax = _checkinSubOrders.PriceTaxPaid;


            if (_checkinSubOrders.CheckinSubStatus == CheckinSubStatus.Active)
                lblTapToAlter.IsVisible = true;

            if (_sub.PaymentMethod == PaymentMethod.BillStillOpen)
            {
                stkBtuttonOrder.IsVisible = true;
            }
            else
            {
                stkBtuttonOrder.IsVisible = false;
            }
        }

        public void ColorPage()
        {
            switch (_checkin.Company.CompanyType)
            {
                case Models.Enums.CompanyType.Hotel:
                    this.BackgroundColor = (Color)App.Current.Resources["HotelColor"];
                    lblTitle.TextColor = (Color)App.Current.Resources["HotelColor"];
                    imgBack.Source = "ic_voltar_hotel";
                    break;
                case Models.Enums.CompanyType.Restaurante:
                    this.BackgroundColor = (Color)App.Current.Resources["RestauranteColor"];
                    lblTitle.TextColor = (Color)App.Current.Resources["RestauranteColor"];
                    imgBack.Source = "ic_voltar_restaurante";
                    break;
                case Models.Enums.CompanyType.Praia:
                    this.BackgroundImage = "bg_praia";
                    lblTitle.TextColor = (Color)App.Current.Resources["BarracaColor"];
                    imgBack.Source = "ic_voltar_praia";
                    break;
                case Enums.CompanyType.EsporteEvento:
                    this.BackgroundColor = (Color)App.Current.Resources["EsportesColor"];
                    lblTitle.TextColor = (Color)App.Current.Resources["EsportesColor"];
                    imgBack.Source = "ic_voltar_esportes";
                    break;
                default:
                    break;
            }
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            listView.SelectedItem = null;
        }

        private async void btnPayLater_Clicked(object sender, EventArgs e)
        {
            try
            {
                PaymentMethod paymentType = PaymentMethod.NotInformed;

                var titleChoose = AppResource.alertTitleChoosePaymentMethod;
                var message = AppResource.alertBodyChoosePaymentMethod;
                var resource = AppResource.lblPayLater;
                if (_checkin.Company.CompanyType == CompanyType.Restaurante || _checkin.Company.CompanyType == CompanyType.Praia)
                    resource = AppResource.lblPayLaterTwo;

                var action = await DisplayActionSheet(message, AppResource.alertCancel, null, AppResource.lblMoney, AppResource.lblCard, resource);
                if (action == AppResource.lblMoney)
                    paymentType = PaymentMethod.Money;
                else if (action == AppResource.lblCard)
                    paymentType = PaymentMethod.Card;
                else if (action == AppResource.lblPayLater || action == AppResource.lblPayLaterTwo)
                    paymentType = PaymentMethod.PayLater;
                else
                    return;

                if (paymentType == PaymentMethod.Money)
                {
                    btnPayCash_Clicked(null, null);
                }
                else if (paymentType == PaymentMethod.Card)
                {
                    btnPayCard_Clicked(null, null);
                }
                else
                {
                    Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);

                    var totalPaid = _total;

                    CompanyService companyService = new CompanyService();

                    var result = await companyService.RequestPayLater(_checkinSubOrders.Id, _tip, 0, 0, false);

                    await App.AppCurrent.NavigationService.ModalGoBack();
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

        //private async void btnPayLater_Clicked(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);


        //        var totalPaid = _total;

        //        CompanyService companyService = new CompanyService();

        //        var result = await companyService.RequestPayLater(_checkinSubOrders.Id, _tip, 0, 0, false);



        //        await App.AppCurrent.NavigationService.ModalGoBack();

        //    }
        //    catch (Exception ex)
        //    {
        //        this.DisplayAlert(MocoApp.Resources.AppResource.alertAlert, ex.Message, AppResource.textOk);

        //    }
        //    finally
        //    {
        //        Acr.UserDialogs.UserDialogs.Instance.HideLoading();
        //    }
        //}


        private void OnTipsTapped(object sender, EventArgs e)
        {
            if (_checkinSubOrders.CheckinSubStatus != CheckinSubStatus.Active)
                return;

            var popup = new EntryPopup(AppResource.alertFillTips, "", AppResource.textOk, AppResource.alertCancel);
            popup.PopupClosed += async (o, closedArgs) =>
            {
                if (closedArgs.Button == AppResource.textOk)
                {
                    try
                    {
                        _checkin.PriceTipPaid = Convert.ToDecimal(closedArgs.Text);
                        lblTip.Text = String.Format(App.AppCurrent.CompanyCulture, "{0:C}", _checkin.PriceTipPaid);
                        _total = _subTotal + Convert.ToDecimal(_checkin.PriceTipPaid) + _tax;
                        _tip = Convert.ToDecimal(_checkin.PriceTipPaid);
                        var newtotal = _total - _checkin.PriceDiscount;


                        if (newtotal < 0)
                            newtotal = 0;

                        lblTotal.Text = String.Format(App.AppCurrent.CompanyCulture, "{0:C}", newtotal);

                        lblTipTitle.Text = AppResource.lblTips;

                    }
                    catch (Exception ex)
                    {
                        this.DisplayAlert(MocoApp.Resources.AppResource.alertAlert, AppResource.alertOnlyNumber, AppResource.textOk);

                    }


                }


            };

            popup.Show();
        }

        private void btnPayCash_Clicked(object sender, EventArgs e)
        {
            var _cashValue = _total;
            lblCashValue.Text = String.Format(App.AppCurrent.CompanyCulture, "{0:C}", _cashValue);

            stkPayNow.IsVisible = true;
            stkMoneyValue.IsVisible = true;
            OnCheckoutTapped(null, null);
            //var popup = new EntryPopup(AppResource.lblInsertMoneyPaid, "", AppResource.textOk, AppResource.alertCancel);
            //popup.PopupClosed += async (o, closedArgs) =>
            //{
            //    if (closedArgs.Button == AppResource.textOk)
            //    {
            //        try
            //        {
            //            var _cashValue = Convert.ToDecimal(closedArgs.Text);
            //            lblCashValue.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", _cashValue);

            //            stkPayNow.IsVisible = true;
            //            stkMoneyValue.IsVisible = true;
            //        }
            //        catch (Exception ex)
            //        {
            //            this.DisplayAlert(MocoApp.Resources.AppResource.alertAlert, AppResource.alertOnlyNumber, AppResource.textOk);

            //        }


            //    }


            //};

            //popup.Show();


        }

        private void btnPayCard_Clicked(object sender, EventArgs e)
        {

            var _cardValue = _total;
            lblCardValue.Text = String.Format(App.AppCurrent.CompanyCulture, "{0:C}", _cardValue);

            stkPayNow.IsVisible = true;
            stkCardValue.IsVisible = true;
            OnCheckoutTapped(null, null);
            //var popup = new EntryPopup(AppResource.lblInsertCardPaid, "", AppResource.textOk, AppResource.alertCancel);
            //popup.PopupClosed += async (o, closedArgs) =>
            //{
            //    if (closedArgs.Button == AppResource.textOk)
            //    {
            //        try
            //        {
            //            var _cardValue = Convert.ToDecimal(closedArgs.Text);
            //            lblCardValue.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", _cardValue);

            //            stkPayNow.IsVisible = true;
            //            stkCardValue.IsVisible = true;
            //        }
            //        catch (Exception ex)
            //        {
            //            this.DisplayAlert(MocoApp.Resources.AppResource.alertAlert, AppResource.alertOnlyNumber, AppResource.textOk);

            //        }


            //    }


            //};

            //popup.Show();


        }

        private async void OnCheckoutTapped(object sender, EventArgs e)
        {
            try
            {
                //string msg = "";
                //string title = "";
                //msg =  AppResource.textWantCloseLocationBillOrder  +  _sub.Location.Name + "?";
                //title = AppResource.textCheckout;

                //var answer = await DisplayAlert(title, msg, AppResource.textOk, AppResource.alertCancel);

                //if (!answer)
                //{
                //    lblCashValue.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", 0);
                //    stkMoneyValue.IsVisible = false;


                //    lblCardValue.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", 0);
                //    stkCardValue.IsVisible = true;

                //    stkPayNow.IsVisible = false;

                //    return;
                //}


                var culture = App.AppCurrent.Culture;
                var currency = new CultureInfo(CultureInfo.CurrentCulture.Name).NumberFormat.CurrencySymbol;

                var tipValue = lblTip.Text.Replace("R$", "").Replace("€", "").Replace("$", "").Replace(currency, "").TrimStart();
                var cardValue = lblCardValue.Text.Replace("R$", "").Replace("€", "").Replace("$", "").Replace(currency, "").TrimStart();
                var cashValue = lblCashValue.Text.Replace("R$", "").Replace("€", "").Replace("$", "").Replace(currency, "").TrimStart();

                var valorCard = Decimal.Parse(cardValue, Utils.GetCurrencyCUlture(true));
                var valordCash = Decimal.Parse(cashValue, Utils.GetCurrencyCUlture(true));
                var valorTip = Decimal.Parse(tipValue, Utils.GetCurrencyCUlture(true));



                if (_total == (valordCash + valorCard))
                {


                    Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);


                    var totalPaid = _total;

                    CompanyService companyService = new CompanyService();

                    var result = await companyService.RequestPayLater(_checkinSubOrders.Id, _tip, valorCard, valordCash, true);


                    Acr.UserDialogs.UserDialogs.Instance.Toast(AppResource.alertAttendentComming);
                    await App.AppCurrent.NavigationService.ModalGoBack();






                }
                else
                    this.DisplayAlert(MocoApp.Resources.AppResource.alertAlert, AppResource.alertCardMoneyNeedBeEqualTotal, AppResource.textOk);



                //_checkoutTax.PriceTipPaid = valorTip;
                //_checkoutTax.PaidInCard = valorCard;
                //_checkoutTax.PaidInCash = valordCash;




                //CompanyService service = new CompanyService();
                //Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);
                //var result = await service.RequestCheckoutFromClient(_company.Id, _checkoutTax.PriceTipPaid);

                //_checkin.Company.CheckedIn = JsonConvert.DeserializeObject<bool>(result);
                //_checkin.CheckinStatus = Enums.CheckinStatus.RequestedCheckout;

                //App.AppCurrent.IsToRefresh = true;

                //Acr.UserDialogs.UserDialogs.Instance.Toast(AppResource.alertRequestSucess);


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
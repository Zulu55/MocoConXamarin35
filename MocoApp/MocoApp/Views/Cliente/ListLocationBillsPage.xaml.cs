using MocoApp.Controls;
using MocoApp.Models;
using MocoApp.Resources;
using MocoApp.Services;
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
    public partial class ListLocationBillsPage : ContentPage
    {
        CheckinSub _sub;
        Checkin _checkin;
        Company _company;
        decimal total = 0;
        decimal subTotal = 0;
        int totalOrders = 0;
        int totalOrder = 0;
        CheckoutTax _checkoutTax = new CheckoutTax();
        bool goBackOneMore;
        public ListLocationBillsPage(Checkin checkin = null, Company company = null, bool back = false)
        {
            InitializeComponent();

            if (Device.RuntimePlatform == "Android")
                frmXaml.CornerRadius = 30;

            NavigationPage.SetHasNavigationBar(this, false);

            _checkin = checkin;
            _company = checkin.Company;
            _sub = checkin.ActiveLocation;

            ColorPage();
            lblTitle.Text = checkin.Company.Title;

            goBackOneMore = back;

            listView.ItemSelected += ListView_ItemSelected;

            //20190409
            if (_company.CompanyType == CompanyType.Hotel)
            {
                //btnPayNow.IsVisible = true;
                //btnPayNowCard.IsVisible = true;//20190412
            }


        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadLocationsBill();

            MessagingCenter.Subscribe<object>(this, "Push", (sender) =>
            {
                if (App.AppCurrent.NavigationService.ModalCurrentPage is ListLocationBillsPage)
                {
                    LoadLocationsBill();
                }
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<Page, string>(this, "Push");
        }

        private async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (listView.SelectedItem == null)
                return;

            var item = listView.SelectedItem as CheckinSub;

            await App.AppCurrent.NavigationService.NavigateModalAsync(new LocationBillOrderPage(item, _checkin), null, false);

            listView.SelectedItem = null;
        }

        private async void OnMenuTapped(object sender, EventArgs e)
        {
            await App.AppCurrent.NavigationService.ModalGoBack();
        }

        public void ColorPage()
        {
            switch (_company.CompanyType)
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

        public async void LoadLocationsBill()
        {
            OrderService Orderservice = new OrderService();
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);

                var resultCheckin = await Orderservice.GetAsync("check/getCheckinById?id=" + _checkin.Id);
                _checkin = JsonConvert.DeserializeObject<Checkin>(resultCheckin);

                var result = await Orderservice.GetCheckinsSubsByCheckinId(_checkin.Id);
                var ListSubCheckin = JsonConvert.DeserializeObject<List<CheckinSub>>(result);
                foreach (var s in ListSubCheckin)
                {
                    s.Location.Company = _company;
                }
                LoadInfos();


                listView.ItemsSource = null;
                listView.ItemsSource = ListSubCheckin.OrderByDescending(m => m.CreatedAt).ToList();


                if (_checkin.CheckinStatus == Enums.CheckinStatus.Checkout)
                {
                    imgPaid.IsVisible = true;
                    stkBtuttonOrder.IsVisible = false;
                    lblCardPaid.Text = AppResource.lblCard + ": " + String.Format(App.AppCurrent.CompanyCulture, "{0:C}", _checkin.PaidInCard);
                    lblCashPaid.Text = AppResource.lblMoney + ": " + String.Format(App.AppCurrent.CompanyCulture, "{0:C}", _checkin.PaidInCash);
                    btnRequestCheckout.IsVisible = false;
                    stkFaltante.IsVisible = false;
                }
                else
                {
                    btnRequestCheckout.IsVisible = true;
                    imgPaid.IsVisible = false;
                    stkBtuttonOrder.IsVisible = true;
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
        }

        public void LoadInfos()
        {
            stkDesconto.IsVisible = false;

            lblTaxsTitle.Text = AppResource.lblTaxs + "(" + _checkin.Company.TaxPercentage + "%)";
            lblTipTitle.Text = AppResource.lblTips + "(" + _checkin.TipPercentage + "%)";

            if (_checkin.CheckinStatus != Enums.CheckinStatus.Checkout && _checkin.CheckinStatus != Enums.CheckinStatus.RequestedCheckout)
            {

                btnRequestCheckout.IsVisible = true;

                total = _checkin.TotalSpent;

                if (_sub != null && (_sub.CheckinSubStatus == Enums.CheckinSubStatus.RequestedCheckout || _sub.CheckinSubStatus == Enums.CheckinSubStatus.Checkout))
                {
                    lblTipTitle.Text = AppResource.lblTips + "(" + _sub.TipPercentage + "%)";
                }

                var tip = _checkin.GetTip(null, null, null, _sub);
                var tax = _checkin.GetTax(null, null, null, _sub);
                lblTip.Text = String.Format(App.AppCurrent.CompanyCulture, "{0:C}", tip);
                lblDesconto.Text = String.Format(App.AppCurrent.CompanyCulture, "{0:C}", _checkin.PriceDiscount);
                lblTax.Text = String.Format(App.AppCurrent.CompanyCulture, "{0:C}", tax);


                lblSubTotal.Text = String.Format(App.AppCurrent.CompanyCulture, "{0:C}", _checkin.TotalSpent);

                lblTotal.Text = String.Format(App.AppCurrent.CompanyCulture, "{0:C}", _checkin.TotalPaid);
                //20190219 mudei pq a linha debaixo n parece fazer sentido
                //decimal totalToBePaid = total - _checkin.TotalPaid;
                decimal totalToBePaid = _checkin.TotalToBePaid;

                lblTotalBePaid.Text = String.Format(App.AppCurrent.CompanyCulture, "{0:C}", totalToBePaid);

                //exibe o adicionar gorjeta
                if (_checkin.CheckinStatus == Enums.CheckinStatus.Checkin)
                    lblTapToAlter.IsVisible = true;


                if (string.IsNullOrEmpty(_checkin.Occupation) && _company.CompanyType == Enums.CompanyType.Hotel && totalToBePaid < 0)
                {
                    stkPayNow.IsVisible = false;
                    stkBtuttonOrder.IsVisible = false;
                    //btnPayNow.IsVisible = false;
                    //btnPayNowCard.IsVisible = false;//20190412
                    lblPaymentOption.IsVisible = false;

                    if (_sub == null)
                    {
                        btnRequestCheckout.IsVisible = true;
                    }
                    else
                        btnRequestCheckout.IsVisible = false;


                    btnRequestCheckout.Text = AppResource.textCloseBill;
                }
                else
                {
                    //20190403
                    if (_company.CompanyType != CompanyType.Hotel)
                    {
                        //stkPayNow.IsVisible = true;
                        //btnPayNow.IsVisible = true;
                        //btnPayNowCard.IsVisible = true;
                        //lblPaymentOption.IsVisible = true;
                    }
                    else
                    {
                        stkPayNow.IsVisible = true;
                        //btnPayNow.IsVisible = true;
                        //btnPayNowCard.IsVisible = true; //20190412
                        lblPaymentOption.IsVisible = true;
                    }

                    stkBtuttonOrder.IsVisible = true;
                    stkWarning.IsVisible = false;
                    lblLocation.IsVisible = false;
                    lblLocation.Text = "";
                }

            }
            else
            {
                LoadPricesIfCheckout();
                btnRequestCheckout.IsVisible = false;


            }
        }

        public void LoadPricesIfCheckout()
        {
            lblDesconto.Text = String.Format(App.AppCurrent.CompanyCulture, "{0:C}", _checkin.PriceDiscount);
            lblTax.Text = String.Format(App.AppCurrent.CompanyCulture, "{0:C}", _checkin.PriceTaxPaid);
            lblTip.Text = String.Format(App.AppCurrent.CompanyCulture, "{0:C}", _checkin.PriceTipPaid);
            lblSubTotal.Text = String.Format(App.AppCurrent.CompanyCulture, "{0:C}", _checkin.TotalSpent);
            lblTotal.Text = String.Format(App.AppCurrent.CompanyCulture, "{0:C}", _checkin.TotalSpent + _checkin.PriceTipPaid + _checkin.PriceTaxPaid - _checkin.PriceDiscount);
        }

        private void btnPayCard_Clicked(object sender, EventArgs e)
        {

            var _cardValue = _checkin.TotalToBePaid;
            lblCardValue.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", _cardValue);

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

                PaymentMethod paymentType = PaymentMethod.NotInformed;
                
                var tipValue = lblTip.Text.Replace("R$", "").Replace("€", "").Replace("$", "").TrimStart();
                var cardValue = lblCardValue.Text.Replace("R$", "").Replace("€", "").Replace("$", "").TrimStart();
                var cashValue = lblCashValue.Text.Replace("R$", "").Replace("€", "").Replace("$", "").TrimStart();

                var valorCard = Decimal.Parse(cardValue, App.AppCurrent.Culture);
                var valordCash = Decimal.Parse(cashValue, App.AppCurrent.Culture);
                decimal valorTip = 0;
                var tipResult = Decimal.TryParse(tipValue, NumberStyles.None, App.AppCurrent.Culture, out valorTip);
                if (!tipResult)
                {
                    tipValue = tipValue.Replace(".", "").Replace(",", "");
                    valorTip = (decimal)((decimal)Convert.ToInt64(tipValue) / (decimal)100);
                }

                if (App.AppCurrent.Culture.Name == "es-ES" && !valorTip.ToString().Contains("."))
                {
                    _checkoutTax.PriceTipPaid = valorTip / 100;
                    _checkoutTax.PaidInCard = valorCard / 100;
                    _checkoutTax.PaidInCash = valordCash / 100;

                }
                else
                {
                    _checkoutTax.PriceTipPaid = valorTip;
                    _checkoutTax.PaidInCard = valorCard;
                    _checkoutTax.PaidInCash = valordCash;

                }

                if (_checkin.TotalToBePaid != 0)
                {
                    await App.AppCurrent.NavigationService.NavigateModalAsync(new MainBillCheckoutPage(_checkin, _checkoutTax.PriceTipPaid, true, valordCash, valorCard, true, paymentType), null, false);

                    return;
                    //var titleChoose = AppResource.alertTitleChoosePaymentMethod;
                    //var message = AppResource.alertBodyChoosePaymentMethod;

                    //var action = await DisplayActionSheet(message, AppResource.alertCancel, null, AppResource.lblMoney, AppResource.lblCard);
                    //if (action == AppResource.lblMoney)
                    //    paymentType = PaymentMethod.Money;
                    //else if (action == AppResource.lblCard)
                    //    paymentType = PaymentMethod.Card;
                    //else
                    //    return;
                }

                CompanyService service = new CompanyService();
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);
                //var result = await service.RequestCheckoutFromClient(_company.Id, _checkoutTax.PriceTipPaid, false, valordCash, valorCard, true, paymentType);
                var result = await service.RequestCheckoutFromClient(_company.Id, _checkoutTax.PriceTipPaid, true, valordCash, valorCard, true, paymentType);

                _checkin.Company.CheckedIn = JsonConvert.DeserializeObject<bool>(result);
                _checkin.CheckinStatus = Enums.CheckinStatus.RequestedCheckout;

                App.AppCurrent.IsToRefresh = true;

                Acr.UserDialogs.UserDialogs.Instance.Toast(AppResource.alertRequestSucess, TimeSpan.FromSeconds(5));
                stkBtuttonOrder.IsVisible = false;
                await App.AppCurrent.NavigationService.ModalGoBack();
                await App.AppCurrent.NavigationService.ModalGoBack();
                if (goBackOneMore)
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

        private void OnTipsTapped(object sender, EventArgs e)
        {
            if (_checkin.CheckinStatus != Enums.CheckinStatus.Checkin)
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
                        //var newtotal = _checkin.SubTotal + Convert.ToDecimal(_checkin.PriceTipPaid) - _checkin.PriceDiscount;


                        var newtotal = _checkin.CalcTotalToBePaidByTip(_checkin.TotalToBePaid, _checkin.Company.RecommendedTipPercentage, _checkin.Company.TaxPercentage, _checkin.PriceTipPaid, _sub);

                        if (newtotal < 0)
                            newtotal = 0;

                        lblTotalBePaid.Text = String.Format(App.AppCurrent.CompanyCulture, "{0:C}", newtotal);
                        //lblTotalBePaid.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", newtotal);

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
            var _cashValue = _checkin.TotalToBePaid;
            lblCashValue.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", _cashValue);

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
    }
}
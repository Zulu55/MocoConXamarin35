using MocoApp.Controls;
using MocoApp.Helpers;
using MocoApp.Models;
using MocoApp.Resources;
using MocoApp.Services;
using MocoApp.Views.Empresa;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class ListPedidoPage : ContentPage
    {

        Checkin _checkin;
        Company Company;
        bool isClosed = false;
        CheckoutTax _checkoutTax = new CheckoutTax();

        private ObservableCollection<OrdersGroupedByLocation> _groupedList;

        public ObservableCollection<OrdersGroupedByLocation> GroupedList { get { return _groupedList; } set { _groupedList = value; base.OnPropertyChanged(); } }

        decimal total = 0;
        decimal newTotal = 0;
        public ListPedidoPage(Checkin checkin = null, Company company = null)
        {
            InitializeComponent();
            if (Device.RuntimePlatform == "Android")
                frmXaml.CornerRadius = 30;

            NavigationPage.SetHasNavigationBar(this, false);

            if (checkin != null)
            {
                _checkin = checkin;
                Company = checkin.Company;
                lblQtdPeopleStr.Text = _checkin.QtdPeopleStr;
                lblOcupationStr.Text = _checkin.OccupationStr;


                LoadOrders();
            }
            else
            {
                Company = company;
                LoadOrdersByClienteIdAndCompanyId();
            }

            BindingContext = GroupedList;


            ColorPage();
            lblTitle.Text = Company.Title;



            //< Label Text = "{Binding OccupationStr}" Style = "{StaticResource label1}" FontSize = "14" ></ Label >

            //                             < Label Text = "{Binding QtdPeopleStr}" Style = "{StaticResource label1}" FontSize = "14" ></ Label >
            listView.ItemSelected += ListView_ItemSelected;

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<object>(this, "Push", (sender) =>
            {
                if (App.AppCurrent.NavigationService.ModalCurrentPage is ListPedidoPage)
                {
                    if (_checkin != null)
                    {
                        lblQtdPeopleStr.Text = _checkin.QtdPeopleStr;
                        lblOcupationStr.Text = _checkin.OccupationStr;
                        LoadOrders();
                    }
                    else
                    {
                        LoadOrdersByClienteIdAndCompanyId();
                    }
                }
            });
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            listView.SelectedItem = null;
        }

        public void ColorPage()
        {
            switch (Company.CompanyType)
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


        private async void OnMenuTapped(object sender, EventArgs e)
        {
            await App.AppCurrent.NavigationService.ModalGoBack();
        }

        public async void LoadOrders()
        {
            OrderService Orderservice = new OrderService();

            try
            {
                stkFooter.IsVisible = true;
                actPiece.IsRunning = false;
                actPiece.IsEnabled = false;
                actPiece.IsVisible = false;

                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);
                listView.ItemsSource = null;

                var result = await Orderservice.GetAsync("check/getCheckinById?id=" + _checkin.Id);
                _checkin = JsonConvert.DeserializeObject<Checkin>(result);

                result = await Orderservice.GetOrderGrouped(_checkin.Id, _checkin.CompanyId);
                GroupedList = JsonConvert.DeserializeObject<ObservableCollection<OrdersGroupedByLocation>>(result);


                var list = GroupedList.SelectMany(m => m.Orders);

                var groupedList = new List<GroupedList>();

                foreach (var item in GroupedList)
                {
                    var group = new Cliente.GroupedList(item.Name);
                    group.AddRange(item.Orders);

                    groupedList.Add(group);
                }

                listView.ItemsSource = groupedList;

                //listView.ItemsSource = list;

                if (_checkin.CheckinStatus == Enums.CheckinStatus.Checkout)
                {
                    imgPaid.IsVisible = true;
                    stkBtuttonOrder.IsVisible = false;
                    lblCardPaid.Text = AppResource.lblCard + ": " + String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", _checkin.PaidInCard);
                    lblCashPaid.Text = AppResource.lblMoney + ": " + String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", _checkin.PaidInCash);
                    grdCheckout.IsVisible = false;
                }
                else
                {
                    grdCheckout.IsVisible = true;
                    imgPaid.IsVisible = false;
                    stkBtuttonOrder.IsVisible = true;
                }

                total = list.Where(m => m.OrderStatus != Enums.OrderStatus.Canceled).Sum(x => x.TotalPrice);

                lblSubTotal.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", total);

                lblTaxsTitle.Text = AppResource.lblTaxs + "(" + _checkin.Company.TaxPercentage + "%)";
                lblTipTitle.Text = AppResource.lblTips + "(" + _checkin.TipPercentage + "%)";


                decimal totalWithTip = 0;
                //totalWithTip = _checkin.PriceTipPaid != 0 ? _checkin.PriceTipPaid : total * _checkin.Company.RecommendedTipPercentage / 100;
                if (_checkin.PriceTipPaid != 0)
                {
                    totalWithTip = _checkin.PriceTipPaid;
                }
                else if (_checkin.Company.RecommendedTipPercentage != 0)
                {
                    totalWithTip = total * _checkin.Company.RecommendedTipPercentage / 100;
                }
                lblTip.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", totalWithTip);

                var totalWithTax = total * _checkin.Company.TaxPercentage / 100;
                lblTax.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", totalWithTax);

                if (_checkin.CheckinStatus == Enums.CheckinStatus.Checkout)
                {
                    lblEncerrada.IsVisible = true;
                    isClosed = true;

                    if (_checkin.PriceDiscount > 0)
                    {
                        stkDesconto.IsVisible = true;
                        lblDesconto.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", _checkin.PriceDiscount);
                    }
                }
                else
                    lblTapToAlter.IsVisible = true;



                newTotal = (total + totalWithTax + totalWithTip) - _checkin.PriceDiscount;
                total = (total + totalWithTax) - _checkin.PriceDiscount;

                lblTotals.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", newTotal);

            }
            catch (Exception ex)
            {

            }
            finally
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
        }

        public async void LoadOrdersByClienteIdAndCompanyId()
        {
            OrderService Orderservice = new OrderService();

            try
            {


                var resultCk = await Orderservice.GetCheckinByClienteAndCompanyId(Company.Id);
                _checkin = JsonConvert.DeserializeObject<Checkin>(resultCk);
                listView.ItemsSource = null;

                var result = await Orderservice.GetOrderGrouped(_checkin.Id, _checkin.CompanyId);
                GroupedList = JsonConvert.DeserializeObject<ObservableCollection<OrdersGroupedByLocation>>(result);
                lblQtdPeopleStr.Text = _checkin.QtdPeopleStr;
                lblOcupationStr.Text = _checkin.OccupationStr;

                var list = GroupedList.SelectMany(m => m.Orders);

                var groupedList = new List<GroupedList>();

                foreach (var item in GroupedList)
                {
                    var group = new Cliente.GroupedList(item.Name);
                    group.AddRange(item.Orders);

                    groupedList.Add(group);
                }



                listView.ItemsSource = groupedList;

                //listView.ItemsSource = list;

                if (_checkin.CheckinStatus != Enums.CheckinStatus.Checkout)
                    grdCheckout.IsVisible = true;
                else
                    grdCheckout.IsVisible = false;

                total = list.Where(m => m.OrderStatus != Enums.OrderStatus.Canceled).Sum(x => x.TotalPrice);

                lblSubTotal.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", total);

                lblTaxsTitle.Text = AppResource.lblTaxs + "(" + _checkin.Company.TaxPercentage + "%)";
                lblTipTitle.Text = AppResource.lblTips + "(" + _checkin.TipPercentage + "%)";

                decimal totalWithTip = 0;
                //totalWithTip = _checkin.PriceTipPaid != 0 ? _checkin.PriceTipPaid : total * _checkin.Company.RecommendedTipPercentage / 100;
                if (_checkin.PriceTipPaid != 0)
                {
                    totalWithTip = _checkin.PriceTipPaid;
                }
                else if (_checkin.Company.RecommendedTipPercentage != 0)
                {
                    totalWithTip = total * _checkin.Company.RecommendedTipPercentage / 100;
                }
                lblTip.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", totalWithTip);

                //var totalWithTip = _checkin.PriceTipPaid;
                //lblTip.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", totalWithTip);

                var totalWithTax = total * _checkin.Company.TaxPercentage / 100;
                lblTax.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", totalWithTax);



                if (_checkin.CheckinStatus == Enums.CheckinStatus.Checkout)
                {
                    lblEncerrada.IsVisible = true;
                    isClosed = true;

                    if (_checkin.PriceDiscount > 0)
                    {
                        stkDesconto.IsVisible = true;
                        lblDesconto.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", _checkin.PriceDiscount);
                    }
                }
                else
                    lblTapToAlter.IsVisible = true;



                newTotal = (total + totalWithTax + totalWithTip) - _checkin.PriceDiscount;
                total = (total + totalWithTax) - _checkin.PriceDiscount;

                lblTotals.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", newTotal);


            }
            catch (Exception ex)
            {

            }
            finally
            {

                stkFooter.IsVisible = true;
                actPiece.IsRunning = false;
                actPiece.IsEnabled = false;
                actPiece.IsVisible = false;
            }
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await App.AppCurrent.NavigationService.NavigateModalAsync(new CompanyDetailsPage(Company), null, true);
        }

        private async void OnTipsTapped(object sender, EventArgs e)
        {
            if (isClosed)
                return;

            var popup = new EntryPopup(AppResource.alertFillTips, "", AppResource.textOk, AppResource.alertCancel);
            popup.PopupClosed += async (o, closedArgs) =>
            {
                if (closedArgs.Button == AppResource.textOk)
                {
                    try
                    {
                        _checkin.PriceTipPaid = Convert.ToDecimal(closedArgs.Text);
                        lblTip.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", _checkin.PriceTipPaid);
                        var newtotal = total + Convert.ToDecimal(_checkin.PriceTipPaid);


                        if (newtotal < 0)
                            newtotal = 0;

                        lblTotals.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", newtotal);
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

        //private async void OnCheckoutTapped(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        string msg = "";
        //        string title = "";
        //        msg = AppResource.alertWantCloseLocationBill;
        //        title = AppResource.textCheckout;

        //        var answer = await DisplayAlert(title, msg, AppResource.textOk, AppResource.alertCancel);

        //        if (!answer)
        //            return;




        //        var tipValue = lblTip.Text.Replace("R$", "").Replace("$", "").TrimStart();
        //        var cardValue = lblCardValue.Text.Replace("R$", "").Replace("$", "").TrimStart();
        //        var cashValue = lblCashValue.Text.Replace("R$", "").Replace("$", "").TrimStart();

        //        var valorCard = Decimal.Parse(cardValue, Utils.GetCurrencyCUlture());
        //        var valordCash = Decimal.Parse(cashValue, Utils.GetCurrencyCUlture());
        //        var valorTip = Decimal.Parse(tipValue, Utils.GetCurrencyCUlture());

        //        _checkoutTax.PriceTipPaid = valorTip;
        //        _checkoutTax.PaidInCard = valorCard;
        //        _checkoutTax.PaidInCash = valordCash;

        //        _checkoutTax.PriceTipPaidStr = lblTip.Text.Replace(',', '.').Replace("R$", "").Replace("$", "").TrimStart();

        //        CompanyService service = new CompanyService();
        //        Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);
        //        //var result = await service.AddRemoveCheckin(Company.Id, _checkoutTax.PriceTipPaidStr);
        //        var result = await service.RequestCheckoutFromClient(Company.Id, _checkoutTax.PriceTipPaid, false, _checkoutTax.PaidInCash, _checkoutTax.PaidInCard);


        //        _checkin.Company.CheckedIn = JsonConvert.DeserializeObject<bool>(result);
        //        _checkin.CheckinStatus = Enums.CheckinStatus.RequestedCheckout;

        //        App.AppCurrent.IsToRefresh = true;

        //        Acr.UserDialogs.UserDialogs.Instance.Toast(AppResource.alertRequestSucess);


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
        //private void btnPayCash_Clicked(object sender, EventArgs e)
        //{
        //    var _cashValue = _checkin.TotalToBePaid;
        //    lblCashValue.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", _cashValue);

        //    stkPayNow.IsVisible = true;
        //    stkMoneyValue.IsVisible = true;
        //    OnCheckoutTapped(null, null);
        //    //var popup = new EntryPopup(AppResource.lblInsertMoneyPaid, "", AppResource.textOk, AppResource.alertCancel);
        //    //popup.PopupClosed += async (o, closedArgs) =>
        //    //{
        //    //    if (closedArgs.Button == AppResource.textOk)
        //    //    {
        //    //        try
        //    //        {
        //    //            var _cashValue = Convert.ToDecimal(closedArgs.Text);
        //    //            lblCashValue.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", _cashValue);

        //    //            stkPayNow.IsVisible = true;
        //    //            stkMoneyValue.IsVisible = true;
        //    //        }
        //    //        catch (Exception ex)
        //    //        {
        //    //            this.DisplayAlert(MocoApp.Resources.AppResource.alertAlert, AppResource.alertOnlyNumber, AppResource.textOk);

        //    //        }


        //    //    }


        //    //};

        //    //popup.Show();


        //}
        //private void btnPayCard_Clicked(object sender, EventArgs e)
        //{

        //    var _cardValue = _checkin.TotalToBePaid;
        //    lblCardValue.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", _cardValue);

        //    stkPayNow.IsVisible = true;
        //    stkCardValue.IsVisible = true;
        //    OnCheckoutTapped(null, null);
        //    //var popup = new EntryPopup(AppResource.lblInsertCardPaid, "", AppResource.textOk, AppResource.alertCancel);
        //    //popup.PopupClosed += async (o, closedArgs) =>
        //    //{
        //    //    if (closedArgs.Button == AppResource.textOk)
        //    //    {
        //    //        try
        //    //        {
        //    //            var _cardValue = Convert.ToDecimal(closedArgs.Text);
        //    //            lblCardValue.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", _cardValue);

        //    //            stkPayNow.IsVisible = true;
        //    //            stkCardValue.IsVisible = true;
        //    //        }
        //    //        catch (Exception ex)
        //    //        {
        //    //            this.DisplayAlert(MocoApp.Resources.AppResource.alertAlert, AppResource.alertOnlyNumber, AppResource.textOk);

        //    //        }


        //    //    }


        //    //};

        //    //popup.Show();


        //}


        private async void OnCheckoutTapped(object sender, EventArgs e)
        {
            try
            {
                PaymentMethod paymentType = PaymentMethod.NotInformed;

                var titleChoose = AppResource.alertTitleChoosePaymentMethod;
                var message = AppResource.alertBodyChoosePaymentMethod;

                var action = await DisplayActionSheet(message, AppResource.alertCancel, null, AppResource.lblMoney, AppResource.lblCard);
                if (action == AppResource.lblMoney)
                    paymentType = PaymentMethod.Money;
                else if (action == AppResource.lblCard)
                    paymentType = PaymentMethod.Card;
                else
                    return;
                var currency = new CultureInfo(CultureInfo.CurrentCulture.Name).NumberFormat.CurrencySymbol;

                var tipValue = lblTip.Text.Replace("R$", "").Replace("$", "").Replace(currency, "").TrimStart();
                var cardValue = lblCardValue.Text.Replace("R$", "").Replace("$", "").Replace(currency, "").TrimStart();
                var cashValue = lblCashValue.Text.Replace("R$", "").Replace("$", "").Replace(currency, "").TrimStart();

                var valorCard = Decimal.Parse(cardValue, Utils.GetCurrencyCUlture());
                var valordCash = Decimal.Parse(cashValue, Utils.GetCurrencyCUlture());
                var valorTip = Decimal.Parse(tipValue, Utils.GetCurrencyCUlture());

                _checkoutTax.PriceTipPaid = valorTip;
                _checkoutTax.PaidInCard = valorCard;
                _checkoutTax.PaidInCash = valordCash;

                _checkoutTax.PriceTipPaidStr = lblTip.Text.Replace(',', '.').Replace("R$", "").Replace("$", "").Replace(currency, "").TrimStart();

                CompanyService service = new CompanyService();
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);
                //var result = await service.AddRemoveCheckin(Company.Id, _checkoutTax.PriceTipPaidStr);
                var result = await service.RequestCheckoutFromClient(Company.Id, _checkoutTax.PriceTipPaid, false, _checkoutTax.PaidInCash, _checkoutTax.PaidInCard, false, paymentType);


                _checkin.Company.CheckedIn = JsonConvert.DeserializeObject<bool>(result);
                _checkin.CheckinStatus = Enums.CheckinStatus.RequestedCheckout;

                App.AppCurrent.IsToRefresh = true;

                Acr.UserDialogs.UserDialogs.Instance.Toast(AppResource.alertRequestSucess);

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

    }

    public class OrdersGroupedByLocation
    {
        public string Name { get; set; }

        public string PriceTipPaid { get; set; }
        public ObservableCollection<Order> Orders { get; set; }
    }


    public class GroupedList : List<Order>
    {
        string title = "Group";


        public GroupedList(string title)
        {
            this.title = title;
        }

        public string Title { get => title; set => title = value; }
    }









}
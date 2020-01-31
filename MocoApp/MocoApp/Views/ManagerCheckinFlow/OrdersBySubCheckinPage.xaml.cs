using MocoApp.Controls;
using MocoApp.Helpers;
using MocoApp.Interfaces;
using MocoApp.Models;
using MocoApp.Resources;
using MocoApp.Services;
using MocoApp.Views.CompanyFluxo;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static MocoApp.Models.Enums;

namespace MocoApp.Views.ManagerCheckinFlow
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OrdersBySubCheckinPage : ContentPage
    {
        private CheckinSub _sub;
        private OrderService _orderservice = new OrderService();
        private bool _payNow = true;
        private CheckinSubOrders _checkinSubOrders;
        private decimal _discount;
        private decimal _tip;
        private decimal _total;
        private decimal _subTotal;
        private Checkin _checkin;
        private static OrdersBySubCheckinPage _instance;

        public OrdersBySubCheckinPage(CheckinSub sub, Checkin checkin)
        {
            InitializeComponent();

            BindingContext = new OrderBySubCheckinPageViewModel();
            _instance = this; 
            _sub = sub;
            _checkin = checkin;

            listView.ItemTapped += ListView_ItemTapped;
            Title = sub.Location.Name;

            var printer = new ToolbarItem(AppResource.lblPrint, "", () =>
            {
                PrintNow();
            }, 0, 0);

            printer.Priority = 1;
            ToolbarItems.Add(printer);

            btnPayNowCard.IsVisible = true;
        }

        public OrderBySubCheckinPageViewModel Vm => this.BindingContext as OrderBySubCheckinPageViewModel;

        public List<Order> Orders => _checkinSubOrders.Orders;
        
        public static OrdersBySubCheckinPage GetInstance()
        {
            return _instance;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            LoadOrders();

            MessagingCenter.Subscribe<object>(this, "Push", (sender) =>
            {
                var page = sender as Page;
                if (page is OrdersBySubCheckinPage)
                {
                    LoadOrders();
                }
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<object, string>(this, "Push");
        }

        private void LoadPrices()
        {
            lblSubTotal.Text = String.Format(App.AppCurrent.CompanyCulture, "{0:C}", _checkinSubOrders.SubTotal);

            lblTaxsTitle.Text = AppResource.lblTaxs + "(" + _checkinSubOrders.TaxPercentage + "%)";
            lblTipTitle.Text = AppResource.lblTips + "(" + _checkinSubOrders.TipPercentage + "%)";
            lblDesconto.Text = String.Format(App.AppCurrent.CompanyCulture, "{0:C}", _checkinSubOrders.PriceDiscount);

            lblTax.Text = String.Format(App.AppCurrent.CompanyCulture, "{0:C}", _checkinSubOrders.PriceTaxPaid);
            lblTip.Text = String.Format(App.AppCurrent.CompanyCulture, "{0:C}", _checkinSubOrders.PriceTipPaid);

            lblTotal.Text = String.Format(App.AppCurrent.CompanyCulture, "{0:C}", _checkinSubOrders.TotalPaid);


            _total = _checkinSubOrders.TotalPaid;
            _discount = _checkinSubOrders.PriceDiscount;
            _tip = _checkinSubOrders.PriceTipPaid;
            _subTotal = _checkinSubOrders.SubTotal;
        }

        public async void LoadOrders()
        {
            ApiService apiService = new ApiService();

            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);

                var result = await apiService.GetAsync("order/getOrdersByCheckinSubId?id=" + _sub.Id);
                _checkinSubOrders = JsonConvert.DeserializeObject<CheckinSubOrders>(result);
                listView.ItemsSource = _checkinSubOrders.Orders;
                stkBtuttonOrder.IsVisible = true;

                if (_checkinSubOrders.Paid && _checkinSubOrders.CheckinSubStatus != CheckinSubStatus.RequestedCheckout && _checkinSubOrders.CheckinSubStatus != CheckinSubStatus.Pending && _checkinSubOrders.CheckinSubStatus != CheckinSubStatus.Active)
                {
                    txtCardValue.Text = String.Format(App.AppCurrent.CompanyCulture, "{0:C}", _checkinSubOrders.PaidCardTotal);
                    txtMoneyValue.Text = String.Format(App.AppCurrent.CompanyCulture, "{0:C}", _checkinSubOrders.PaidInCash);


                    //ALAIN PEDIU ESSA MUDANCA NO HOTEL no DIA 07/11/2018 A NOITE NO DIA QUE EU PEDI UM SUBWAY PQ SO TEM Q APARECER PAID SE TIVER REALMENTE PAGO 
                    if (_checkinSubOrders.CheckinSubStatus == CheckinSubStatus.Closed)
                    {
                        imgPaid.IsVisible = true;
                    }

                    stkPayNow.IsVisible = true;
                    btnAddProduct.IsVisible = false;
                    stkBtuttonOrder.IsVisible = false;
                    lblCardPaid.Text = AppResource.lblCard + ": " + String.Format(App.AppCurrent.CompanyCulture, "{0:C}", _checkinSubOrders.PaidCardTotal);
                    lblCashPaid.Text = AppResource.lblMoney + ": " + String.Format(App.AppCurrent.CompanyCulture, "{0:C}", _checkinSubOrders.PaidInCash);
                }
                else
                {
                    stkPayNow.IsVisible = false;
                    imgPaid.IsVisible = false;
                }

                if(_checkinSubOrders.CheckinSubStatus == CheckinSubStatus.Denied)
                {
                    stkPayNow.IsVisible = false;
                    imgPaid.IsVisible = false;
                    stkBtuttonOrder.IsVisible = false;
                }

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

        private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (listView.SelectedItem == null)
                return;

            var item = listView.SelectedItem as Order;

            await App.AppCurrent.NavigationService.NavigateAsync(new OrderDetailPage(item, _checkin), null, false);

            listView.SelectedItem = null;
        }

        private async void btnCheckout_Clicked(object sender, EventArgs e)
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);

                var b = txtMoneyValue.Text.Replace("R$", "").Replace("$", "").TrimStart();
                var c = txtCardValue.Text.Replace("R$", "").Replace("$", "").TrimStart();

                var cardValue = Decimal.Parse(b, Utils.GetCurrencyCUlture());
                var cashValue = Decimal.Parse(c, Utils.GetCurrencyCUlture());

                var totalPaid = _total;


                var result = await _orderservice.UpdateRequestedSubCheckoutTax(new CheckoutSubTax()
                {
                    CheckinSubId = _sub.Id,
                    CheckinSubStatus = CheckinSubStatus.Closed,
                    Paid = _payNow,
                    PaidInCard = cardValue,
                    PaidInCash = cashValue,
                    PriceDiscount = _discount,
                    PriceTipPaid = _tip,
                    TotalPaid = totalPaid
                });

                Acr.UserDialogs.UserDialogs.Instance.Toast(AppResource.alertCheckinClosed);


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


        private void OnTipsTapped(object sender, EventArgs e)
        {
            var popup = new EntryPopup(AppResource.alertFillTips, "", AppResource.textOk, AppResource.alertCancel);
            popup.PopupClosed += async (o, closedArgs) =>
            {
                if (closedArgs.Button == AppResource.textOk)
                {
                    try
                    {
                        _tip = Convert.ToDecimal(closedArgs.Text);
                        lblTip.Text = String.Format(App.AppCurrent.CompanyCulture, "{0:C}", _tip);
                        _total = _subTotal + _tip + _checkinSubOrders.PriceTaxPaid - _discount;


                        if (_total < 0)
                            _total = 0;

                        lblTotal.Text = String.Format(App.AppCurrent.CompanyCulture, "{0:C}", _total);
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

        private async void OnDescontoTapped(object sender, EventArgs e)
        {
            var popup = new EntryPopup(AppResource.alertFillDiscount, "", AppResource.textOk, AppResource.alertCancel);
            popup.PopupClosed += async (o, closedArgs) =>
            {
                if (closedArgs.Button == AppResource.textOk)
                {
                    try
                    {
                        _discount = Convert.ToDecimal(closedArgs.Text);
                        lblDesconto.Text = String.Format(App.AppCurrent.CompanyCulture, "{0:C}", _discount);
                        _total = _subTotal + _tip + _checkinSubOrders.PriceTaxPaid - _discount;


                        if (_total < 0)
                            _total = 0;

                        lblTotal.Text = String.Format(App.AppCurrent.CompanyCulture, "{0:C}", _total);

                    }
                    catch (Exception ex)
                    {
                        this.DisplayAlert(MocoApp.Resources.AppResource.alertAlert, AppResource.alertOnlyNumber, AppResource.textOk);

                    }


                }


            };

            popup.Show();
        }

        private async void btnPayLater_Clicked(object sender, EventArgs e)
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);


                var totalPaid = _total;


                var result = await _orderservice.UpdateRequestedSubCheckoutTax(new CheckoutSubTax()
                {
                    CheckinSubId = _sub.Id,
                    Paid = false,
                    CheckinSubStatus = CheckinSubStatus.Closed,
                    TotalPaid = totalPaid,
                    PriceTipPaid = _tip,
                    PriceDiscount = _discount,
                    PaymentMethod = PaymentMethod.PayLater
                });


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

        private async void btnPayCard_Clicked(object sender, EventArgs e)
        {
            try
            {
                PaymentMethod paymentType = PaymentMethod.NotInformed;
                decimal totals = 0;

                var b = lblTotal.Text.Replace("R$", "").Replace("$", "").Replace("€", "").TrimStart();


                totals += Decimal.Parse(b, Utils.GetCurrencyCUlture());

                if (totals != 0)
                {

                    var titleChoose = AppResource.alertTitleChoosePaymentMethod;
                    var message = AppResource.alertBodyChoosePaymentMethod;
                    var action = await DisplayActionSheet(message, AppResource.alertCancel, null, AppResource.lblMoney, AppResource.lblCard/*, resource*/);
                    if (action == AppResource.lblMoney)
                        paymentType = PaymentMethod.Money;
                    else if (action == AppResource.lblCard)
                        paymentType = PaymentMethod.Card;
                    else if (action == AppResource.lblPayLater || action == AppResource.lblPayLaterTwo)
                        paymentType = PaymentMethod.PayLater;
                    else
                        return;
                }

                if (paymentType == PaymentMethod.Money)
                {
                    txtMoneyValue.Text = lblTotal.Text;
                    btnPayNow_Clicked(null, null);
                }
                else if (paymentType == PaymentMethod.Card)
                {
                    txtCardValue.Text = lblTotal.Text;
                    btnPayNow_Clicked(null, null);
                }
                else if(paymentType == PaymentMethod.NotInformed)
                {
                    btnPayNow_Clicked(null, null);
                }
                else
                {
                    btnPayLater_Clicked(null, null);
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


        private void btnPayCash_Clicked(object sender, EventArgs e)
        {
            stkPayNow.IsVisible = true;
            stkMoneyValue.IsVisible = true;
        }

        private async void btnPayNow_Clicked(object sender, EventArgs e)
        {
            try
            {
                stkPayNow.IsVisible = true;

                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);

                var b = txtMoneyValue.Text.Replace("R$", "").Replace("$", "").Replace("€", "").TrimStart();
                var c = txtCardValue.Text.Replace("R$", "").Replace("$", "").Replace("€", "").TrimStart();

                var cashValue = Decimal.Parse(b, Utils.GetCurrencyCUlture());
                var cardValue = Decimal.Parse(c, Utils.GetCurrencyCUlture());

                var totalPaid = _total;
                PaymentMethod method;
                if (cardValue > 0)
                    method = PaymentMethod.Card;
                else if (cashValue > 0)
                    method = PaymentMethod.Money;
                else
                    method = PaymentMethod.NotInformed;
                var result = await _orderservice.UpdateRequestedSubCheckoutTax(new CheckoutSubTax()
                {
                    CheckinSubId = _sub.Id,
                    CheckinSubStatus = CheckinSubStatus.Closed,
                    Paid = true,
                    PaidInCard = cardValue,
                    PaidInCash = cashValue,
                    PriceDiscount = _discount,
                    PriceTipPaid = _tip,
                    TotalPaid = totalPaid,
                    PaymentMethod = method
                });

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

        private async void btnAddOrder_Clicked(object sender, EventArgs e)
        {
            await App.AppCurrent.NavigationService.NavigateAsync(new AddOrderToClientPage(_checkin, _checkin.LocationId, _sub), null, true);
        }

        public async void PrintNow()
        {
            //if (_checkin.CheckinStatus == Enums.CheckinStatus.Checkout)
            //{

            //}
            //else
            //    this.DisplayAlert(MocoApp.Resources.AppResource.alertAlert, "A conta precisa estar fechada para realizar a impressão.", AppResource.textOk);

            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertPrinting, Acr.UserDialogs.MaskType.Black);
                await Task.Delay(500);
                await DependencyService.Get<IBluetoothManager>().OpenOutputStream();
                PrinterService _printer = new PrinterService();

                await _printer.PrintText(PrintText.PrintLocationBill(_checkin, _checkinSubOrders));


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

    public class CheckoutSubTax
    {
        public string CheckinSubId { get; set; }
        public CheckinSubStatus CheckinSubStatus { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal PriceTipPaid { get; set; }
        public decimal PriceDiscount { get; set; }
        public bool Paid { get; set; }
        public decimal PaidInCard { get; set; }
        public decimal PaidInCash { get; set; }
        public PaymentMethod PaymentMethod { get; set; }

    }

    public class OrderBySubCheckinPageViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private decimal _cardValue;
        public decimal CardValue
        {
            get
            {
                return _cardValue;
            }
            set
            {
                _cardValue = value;
                Notify("CardValue");

            }
        }

        private decimal _cashValue;
        public decimal CashValue
        {
            get
            {
                return _cashValue;
            }
            set
            {
                _cashValue = value;
                Notify("CashValue");

            }
        }

        protected void Notify(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
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
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static MocoApp.Models.Enums;

namespace MocoApp.Views.ManagerCheckinFlow
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CheckinDetailPage : ContentPage
    {
        private Checkin _checkin { get; set; }
        private decimal _total = 0;
        private decimal _subTotal = 0;
        private int _totalOrders = 0;
        private int _totalOrder = 0;
        private bool _clicked = false;
        private CheckoutTax _checkoutTax = new CheckoutTax();
        private bool _hasActive;
        private List<CheckinSub> _listSubCheckin;

        public CheckinDetailPage(Checkin checkin)
        {
            InitializeComponent();

            if (Device.RuntimePlatform == "iOS")
            {
                btnAddProduct.WidthRequest = 110;
                btnCheckout.WidthRequest = 100;
            }

            var printer = new ToolbarItem(AppResource.lblPrint, "", () =>
            {
                PrintNow();
            }, 0, 0);

            printer.Priority = 1;
            ToolbarItems.Add(printer);

            _checkin = checkin;

            this.BindingContext = _checkin;

            _hasActive = false;

            listView.ItemSelected += ListView_ItemSelected;
            listView.RefreshCommand = new Command(() => LoadCheckin());
        }

        public OrderBySubCheckinPageViewModel Vm => this.BindingContext as OrderBySubCheckinPageViewModel;

        private async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (listView.SelectedItem == null)
                return;

            var item = listView.SelectedItem as Order;

            await App.AppCurrent.NavigationService.NavigateAsync(new OrderDetailPage(item, _checkin), null, false);

            listView.SelectedItem = null;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<object>(this, "Push", (sender) =>
            {
                var page = sender as Page;
                if (page is CheckinDetailPage)
                {
                    LoadCheckin();
                }

            });

            stkLocations.IsVisible = false;
            LoadCheckin();

            if (_checkin.Company.HasLocation)
            {
                stkSubCheckins.IsVisible = true;
            }
            else
            {
                stkSubCheckins.IsVisible = false;
                btnAddProduct.IsVisible = true;
            }

        }


        public void LoadInfos()
        {
            if (_checkin.CheckinStatus == Enums.CheckinStatus.RequestedCheckout)
            {
                if (_checkin.PaymentMethod == Enums.PaymentMethod.Card)
                    lblPaymentTypeInfo.Text = AppResource.lblCardObs;
                else if (_checkin.PaymentMethod == Enums.PaymentMethod.Money)
                    lblPaymentTypeInfo.Text = AppResource.lblMoneyObs;
                else
                    lblPaymentTypeInfo.Text = AppResource.lblNotInformedObs;
            }

            if (!_checkin.Company.HasLocation)
            {
                _total = _checkin.TotalToBePaid;
                _subTotal = _checkin.SubTotal;

                listView.ItemsSource = _checkin.Orders;

                lblSubTotalSpentText.Text = AppResource.lblSubTotal;
                lblSubTotal.Text = String.Format(App.AppCurrent.CompanyCulture, "{0:C}", _checkin.SubTotal);
                _totalOrders = _checkin.Orders.Count();

                lblTotalOrders.Text = AppResource.alertOrders + "(" + _totalOrders + ")";

                lblTaxsTitle.Text = AppResource.lblTaxs + "(" + _checkin.Company.TaxPercentage + "%)";
                var tipPerc = _checkin.TipPercentage;
                lblTipTitle.Text = AppResource.lblTips + "(" + _checkin.TipPercentage + "%)";
                if (_checkin.TipPercentage == 0 && _checkin.SubTotal != 0)
                {
                    lblTipTitle.Text = AppResource.lblTips + "(" + _checkin.Company.RecommendedTipPercentage + "%)";
                    tipPerc = _checkin.Company.RecommendedTipPercentage;
                }

                _checkin.PriceTipPaid = _subTotal * tipPerc / 100;

                lblTip.Text = String.Format(App.AppCurrent.CompanyCulture, "{0:C}", _checkin.PriceTipPaid);

                _checkin.PriceTaxPaid = _subTotal * _checkin.Company.TaxPercentage / 100;
                lblTax.Text = String.Format(App.AppCurrent.CompanyCulture, "{0:C}", _checkin.PriceTaxPaid);
            }
            else
            {
                _total = _checkin.TotalToBePaid;
                LoadOrdersByCheckinId();
                listView.ItemsSource = null;
                lblSubTotalSpentText.Text = AppResource.lblTotalSpent;
                lblSubTotal.Text = String.Format(App.AppCurrent.CompanyCulture, "{0:C}", _checkin.TotalSpent);
            }

            lblDesconto.Text = String.Format(App.AppCurrent.CompanyCulture, "{0:C}", _checkin.PriceDiscount);

            lblTotal.Text = String.Format(App.AppCurrent.CompanyCulture, "{0:C}", _checkin.TotalPaid);
            lblTotalBePaid.Text = String.Format(App.AppCurrent.CompanyCulture, "{0:C}", _checkin.TotalToBePaid);

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            MessagingCenter.Unsubscribe<object, string>(this, "Push");

            listView.ItemsSource = null;
        }

        private async void btnCheckout_Clicked(object sender, EventArgs e)
        {
            try
            {

                _clicked = true;

                PaymentMethod paymentType = PaymentMethod.NotInformed;

                if ((_checkin.TotalToBePaid != 0 && _checkin.Company.HasLocation) || (_checkin.SubTotal != 0 && !_checkin.Company.HasLocation))
                {
                    var titleChoose = AppResource.alertTitleChoosePaymentMethod;
                    var message = AppResource.alertBodyChoosePaymentMethod;

                    var action = await DisplayActionSheet(message, AppResource.alertCancel, null, AppResource.lblMoney, AppResource.lblCard);
                    if (action == AppResource.lblMoney)
                    {
                        txtMoneyValue.Text = lblTotalBePaid.Text;
                        paymentType = PaymentMethod.Money;

                    }
                    else if (action == AppResource.lblCard)
                    {
                        txtCardValue.Text = lblTotalBePaid.Text;
                        paymentType = PaymentMethod.Card;
                    }
                    else
                        return;
                }

                var bePaid = lblTotalBePaid.Text.Replace("R$", "").Replace("€", "").Replace("$", "").TrimStart();
                var beDescount = lblDesconto.Text.Replace("R$", "").Replace("€", "").Replace("$", "").TrimStart();
                var beTip = lblTip.Text.Replace("R$", "").Replace("€", "").Replace("$", "").TrimStart();
           
                var cardValue = txtCardValue.Text.Replace("R$", "").Replace("€", "").Replace("$", "").TrimStart();
                var cashValue = txtMoneyValue.Text.Replace("R$", "").Replace("€", "").Replace("$", "").TrimStart();

                var valorTotal = Decimal.Parse(bePaid, Utils.GetCurrencyCUlture());
                var valorDesconto = Decimal.Parse(beDescount, Utils.GetCurrencyCUlture());
                var valorTip = Decimal.Parse(beTip, Utils.GetCurrencyCUlture(false));
                var valorCard = Decimal.Parse(cardValue, Utils.GetCurrencyCUlture());
                var valordCash = Decimal.Parse(cashValue, Utils.GetCurrencyCUlture());


                _checkoutTax.ClientId = _checkin.ClientId;
                _checkoutTax.CompanyId = _checkin.CompanyId;
                _checkoutTax.TotalPaid = valorTotal;
                _checkoutTax.PriceDiscount = valorDesconto;
                _checkoutTax.PriceTipPaid = valorTip;
                _checkoutTax.PaidInCard = valorCard;
                _checkoutTax.PaidInCash = valordCash;
                _checkoutTax.CheckinId = _checkin.Id;

                _checkoutTax.PriceTipPaid = valorTip;

                if (_checkin.Company.HasLocation)
                {
                    _checkoutTax.PriceTipPaid = 0;
                }

                CompanyService companyService = new CompanyService();
                await companyService.MakeCheckoutTax(_checkoutTax);

                _checkin.CheckinStatus = Enums.CheckinStatus.Checkout;
                Acr.UserDialogs.UserDialogs.Instance.Toast(AppResource.alertCheckinClosed);
                App.AppCurrent.NavigationService.GoBack();

            }
            catch (Exception ex)
            {
                this.DisplayAlert(MocoApp.Resources.AppResource.alertAlert, ex.Message, AppResource.textOk);
            }
            finally
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                _clicked = false;
            }
        }


        private async void btnAddProduct_Clicked(object sender, EventArgs e)
        {
            await App.AppCurrent.NavigationService.NavigateAsync(new AddOrderToClientPage(_checkin, _checkin.LocationId), null, true);
        }

        private async void btnAddClient_Clicked(object sender, EventArgs e)
        {
            if (_hasActive)
            {
                await DisplayAlert(AppResource.alertAlert, AppResource.alertCloseAllActiveLocations, AppResource.alertCancel);
                return;
            }

            await App.AppCurrent.NavigationService.NavigateAsync(new AddNewClientPage(_checkin.Client, _checkin.Occupation, _checkin), null, true);
        }


        private void btnPayCard_Clicked(object sender, EventArgs e)
        {
            stkPayNow.IsVisible = true;
            stkCardValue.IsVisible = true;
        }

        private void btnPayCash_Clicked(object sender, EventArgs e)
        {
            stkPayNow.IsVisible = true;
            stkMoneyValue.IsVisible = true;
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
                        _checkin.PriceDiscount = Convert.ToDecimal(closedArgs.Text);
                        lblDesconto.Text = String.Format(App.AppCurrent.CompanyCulture, "{0:C}", _checkin.PriceDiscount);
                        var newtotal = (_total - Convert.ToDecimal(_checkin.PriceDiscount));

                        if (newtotal < 0)
                            newtotal = 0;

                        lblTotalBePaid.Text = String.Format(App.AppCurrent.CompanyCulture, "{0:C}", newtotal);

                    }
                    catch (Exception ex)
                    {
                        this.DisplayAlert(MocoApp.Resources.AppResource.alertAlert, AppResource.alertOnlyNumber, AppResource.textOk);
                    }
                }
            };

            popup.Show();
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
                        _checkin.PriceTipPaid = Convert.ToDecimal(closedArgs.Text);
                        lblTip.Text = String.Format(App.AppCurrent.CompanyCulture, "{0:C}", _checkin.PriceTipPaid);
                        _total = _subTotal + _checkin.PriceTipPaid + _checkin.PriceTaxPaid;
                        var newtotal = _total - _checkin.PriceDiscount;


                        if (newtotal < 0)
                            newtotal = 0;

                        lblTotalBePaid.Text = String.Format(App.AppCurrent.CompanyCulture, "{0:C}", newtotal);

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

        public async void LoadOrdersByCheckinId()
        {
            OrderService Orderservice = new OrderService();

            try
            {
                var result = await Orderservice.GetCheckinsSubsByCheckinId(_checkin.Id);
                _listSubCheckin = JsonConvert.DeserializeObject<List<CheckinSub>>(result);
                listView.IsRefreshing = false;
                stkLocations.Children.Clear();
                stkLocations.IsVisible = true;
                stkTipsAndTax.IsVisible = false;

                foreach (var item in _listSubCheckin.OrderByDescending(m => m.CreatedAt).ToList())
                {

                    var grid = new Grid
                    {
                        ColumnDefinitions =
                            {
                                new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                                new ColumnDefinition { Width = new GridLength(6, GridUnitType.Star) },
                                new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) }
                            }
                    };

                    StackLayout stk = new StackLayout();
                    stk.Margin = new Thickness(0, 0, 0, 10);
                    stk.Orientation = StackOrientation.Vertical;
                    stk.Spacing = 2;
                    stk.StyleId = item.Id;

                    ImageCircle.Forms.Plugin.Abstractions.CircleImage image =
                        new ImageCircle.Forms.Plugin.Abstractions.CircleImage();
                    image.HeightRequest = 40;
                    image.WidthRequest = 40;
                    image.FillColor = Color.FromHex("#ccc");
                    image.HorizontalOptions = LayoutOptions.Center;
                    image.VerticalOptions = LayoutOptions.Center;
                    image.Aspect = Aspect.AspectFill;
                    image.Source = item.Location.ImageUri;
                    image.StyleId = item.Id;

                    Label lblName = new Label();
                    lblName.FontSize = 14;
                    lblName.TextColor = Color.FromHex("#212121");
                    lblName.HorizontalTextAlignment = TextAlignment.Start;
                    lblName.VerticalTextAlignment = TextAlignment.Center;
                    lblName.Text = $"{item.LocationNameWithValue}";

                    Label lblStatus = new Label();
                    lblStatus.FontSize = 12;
                    lblStatus.Text = item.CheckinSubStatusStrWithValue;

                    lblStatus.IsVisible = true;
                    switch (item.CheckinSubStatus)
                    {
                        case CheckinSubStatus.Pending:
                            lblStatus.TextColor = Color.Red;
                            break;
                        case CheckinSubStatus.Active:
                            if (item.Paid && item.PaidInCard == 0)
                                lblStatus.TextColor = Color.Orange;
                            else if (item.Paid && item.PaidInCard != 0)
                                lblStatus.TextColor = Color.Green;
                            else if(item.PaymentMethod == PaymentMethod.Money && !item.PaidFromAdmin)
                                lblStatus.TextColor = Color.Orange;
                            else if(item.PaymentMethod == PaymentMethod.Transferred && !item.PaidFromAdmin)
                                lblStatus.TextColor = Color.Orange;
                            else
                                lblStatus.TextColor = Color.Blue;
                            break;
                        case CheckinSubStatus.Closed:
                            if (item.TotalSpent != item.TotalPaid)
                                lblStatus.TextColor = Color.Orange;
                            else
                                lblStatus.TextColor = Color.Green;
                            break;
                        case CheckinSubStatus.RequestedCheckout:
                            lblStatus.TextColor = Color.Black;
                            break;
                        case CheckinSubStatus.Checkout:
                            lblStatus.TextColor = Color.Red;
                            break;
                        case CheckinSubStatus.Denied:
                            lblStatus.TextColor = Color.Red;
                            break;
                    }

                    Label lblPaidIn = new Label();
                    lblPaidIn.FontSize = 12;

                    lblPaidIn.TextColor = Color.Gray;
                    lblPaidIn.IsVisible = false;
                    if (item.CheckinSubStatus == Enums.CheckinSubStatus.RequestedCheckout && !item.Paid)
                    {
                        lblPaidIn.IsVisible = true;

                        if (item.PaymentMethod == PaymentMethod.Card)
                        {
                            lblPaidIn.Text = AppResource.lblCardObs;
                        }
                        else if (item.PaymentMethod == PaymentMethod.Money)
                        {
                            lblPaidIn.Text = AppResource.lblMoneyObs;
                        }
                        else
                        {
                            lblPaidIn.IsVisible = false;
                        }
                    }

                    Label lblPaid = new Label();
                    lblPaid.FontSize = 12;
                    lblPaid.Text = AppResource.lblPaid;
                    lblPaid.TextColor = Color.Green;

                    Label lblPaidLater = new Label();
                    lblPaidLater.FontSize = 12;

                    ////ESSA MUDANÇA FOI FEITA NO DIA QUE O ALAIN TAVA EM CURITIBA ANTES DE IR EMBORA, PEDIMOS BERA E BURGUER
                    ////ELE PEDIU PARA CASO PEÇA FECHAR A CONTA DO CHECKIN COM LOCATION ATIVA, TEM Q APARECER PENDENTE PAGAMENTO
                    stk.Children.Add(lblName);
                    stk.Children.Add(lblStatus);
                    stk.Children.Add(lblPaidIn);
                   
                    if (!item.Paid && item.CheckinSubStatus == Enums.CheckinSubStatus.Closed || item.CheckinSubStatus == Enums.CheckinSubStatus.RequestedCheckout)
                        stk.Children.Add(lblPaidLater);

                    var tpReco = new TapGestureRecognizer();
                    tpReco.Tapped += OnLocationTapped;
                    stk.GestureRecognizers.Add(tpReco);

                    Label lblValuePaid = new Label();
          
                    lblValuePaid.TextColor = item.CheckinSubStatusColorValueInColor;
                    lblValuePaid.Text = item.CheckinSubStatusStrWithValueV2;
                    lblValuePaid.FontSize = 12;

                    grid.Padding = new Thickness(10, 6, 10, 0);
                    grid.ColumnSpacing = 8;
                    grid.Children.Add(image, 0, 0);
                    grid.Children.Add(stk, 1, 0);
                    grid.Children.Add(lblValuePaid, 2, 0);

                    stkLocations.Children.Add(grid);
                   
                }

            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
        }

        public async void LoadCheckin()
        {
            OrderService orderService = new OrderService();

            if (!_checkin.Company.HasLocation)
            {
                lblAllocationPrefixText.Text = _checkin.OccupationStr;
                stkDiscount.IsVisible = true;
                boxViewDisc.IsVisible = true;
            }
            else
            {
                boxViewDisc.IsVisible = false;
                stkDiscount.IsVisible = false;
            }
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);

                listView.IsRefreshing = false;
                var result = await orderService.GetAsync("check/getCheckinById?id=" + _checkin.Id);
                _checkin = JsonConvert.DeserializeObject<Checkin>(result);

                this.BindingContext = _checkin;

                listView.ItemsSource = null;

                LoadInfos();
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

        private async void OnLocationTapped(object sender, EventArgs e)
        {
            var item = sender as StackLayout;
            var locationSelected = _listSubCheckin.FirstOrDefault(x => x.Id == item.StyleId);

            if (locationSelected.CheckinSubStatus == Enums.CheckinSubStatus.Pending)
                await App.AppCurrent.NavigationService.NavigateAsync(new AcceptDeclineCheckinPage(_checkin, locationSelected), null, false);
            else
                await App.AppCurrent.NavigationService.NavigateAsync(new OrdersBySubCheckinPage(locationSelected, _checkin), null, true);

        }

        private async void OnCkGestureTapped_Tapped(object sender, EventArgs e)
        {
            var item = sender as Image;

            await App.AppCurrent.NavigationService.NavigateAsync(new OrdersBySubCheckinPage(_listSubCheckin.FirstOrDefault(x => x.Id == item.StyleId), _checkin), null, true);
        }

        public async void PrintNow()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertPrinting, Acr.UserDialogs.MaskType.Black);
                await Task.Delay(500);
                await DependencyService.Get<IBluetoothManager>().OpenOutputStream();
                PrinterService _printer = new PrinterService();

                await _printer.PrintText(PrintText.PrintCheckin(_checkin, _listSubCheckin));
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
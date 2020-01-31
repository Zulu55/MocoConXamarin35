using MocoApp.Controls;
using MocoApp.Models;
using MocoApp.Resources;
using MocoApp.Services;
using MocoApp.Views.ManagerCheckinFlow;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MocoApp.Views.CompanyFluxo
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CheckinHistoryDetailPage : ContentPage
    {
        Checkin _checkin;
        decimal total = 0;
        decimal subTotal = 0;
        CheckoutTax _checkoutTax = new CheckoutTax();
        List<CheckinSub> ListSubCheckin;
        public CheckinHistoryDetailPage(Checkin checkin)
        {
            InitializeComponent();

            _checkin = checkin;

            this.BindingContext = _checkin;

            if (_checkin.Company.HasLocation)
            {
                lblSubTotalSpent.Text = AppResource.lblTotalSpent;
                stkTax.IsVisible = false;
                stkTip.IsVisible = false;
                boxTax.IsVisible = false;
                boxTip.IsVisible = false;
                LoadOrdersByCheckinId();
            }
            else
                lblSubTotalSpent.Text = AppResource.lblSubTotal;

            listView.ItemSelected += ListView_ItemSelected;
        }

        private async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            //if (listView.SelectedItem == null)
            //    return;

            //var item = listView.SelectedItem as Order;

            //await App.AppCurrent.NavigationService.NavigateAsync(new OrderDetailPage(item, _checkin), null, false);

            listView.SelectedItem = null;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            listView.ItemsSource = _checkin.Orders;

            if(_checkin.CheckinStatus != Enums.CheckinStatus.Checkout)
            {
                var subTotal = _checkin.GetSubTotal();
                var tip = _checkin.GetTip();
                var tax = _checkin.GetTax();
                var tipPercentage = _checkin.GetTipPercentage(tip, subTotal);
                var taxPercentage = _checkin.GetTaxPercentage(tax, subTotal);

                lblTaxsTitle.Text = AppResource.lblTaxs + "(" + taxPercentage + "%)";
                lblTipTitle.Text = AppResource.lblTips + "(" + tipPercentage + "%)";
                lblDesconto.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", _checkin.PriceDiscount);
                lblTotal.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", _checkin.TotalPaid);
                lblTax.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", tax);
                lblTip.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", tip);
            }
            else
            {
                //20180904
                lblTaxsTitle.Text = AppResource.lblTaxs + "(" + _checkin.TaxPercentage.ToString("#.##") + "%)";
                lblTipTitle.Text = AppResource.lblTips + "(" + _checkin.TipPercentage.ToString("#.##") + "%)";
                lblDesconto.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", _checkin.PriceDiscount);
                lblTotal.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", _checkin.TotalPaid);
                lblTax.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", _checkin.PriceTaxPaid);
                lblTip.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", _checkin.PriceTipPaid);
            }

            if (_checkin.Company.HasLocation)
                lblSubTotal.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", _checkin.TotalSpent);
            else
                lblSubTotal.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", _checkin.SubTotal);




            lblCashValue.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", _checkin.PaidInCash);
            lblCardValue.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", _checkin.PaidInCard);


            //total = 0;
            //foreach (var item in _checkin.Orders.Where(m => m.OrderStatus != Enums.OrderStatus.Canceled))
            //{
            //    total = total + item.TotalPrice;
            //}

            //lblSubTotal.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", _checkin.SubTotal);


            //lblTaxsTitle.Text = AppResource.lblTaxs + "(" + _checkin.Company.TaxPercentage + "%)";
            //lblTipTitle.Text = AppResource.lblTips + "(" + _checkin.TipPercentage + "%)";


            ////var totalWithTip = total * _checkin.TipPercentage / 100;
            //decimal totalWithTip = 0;
            ////totalWithTip = _checkin.PriceTipPaid != 0 ? _checkin.PriceTipPaid : total * _checkin.Company.RecommendedTipPercentage / 100;
            //if (_checkin.PriceTipPaid != 0)
            //{
            //    totalWithTip = _checkin.PriceTipPaid;
            //}
            //else if (_checkin.Company.RecommendedTipPercentage != 0)
            //{
            //    totalWithTip = total * _checkin.Company.RecommendedTipPercentage / 100;
            //}
            //lblTip.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", totalWithTip);

            //lblDesconto.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", _checkin.PriceDiscount);

            //var totalWithTax = total * _checkin.Company.TaxPercentage / 100;
            //lblTax.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", totalWithTax);
            //subTotal = total + totalWithTax;


            //var newTotal = (total + totalWithTax + totalWithTip) - _checkin.PriceDiscount;
            //total = newTotal;


            //lblTotal.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", newTotal);
            //lblCashValue.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", _checkin.PaidInCash);
            //lblCardValue.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", _checkin.PaidInCard);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            listView.ItemsSource = null;
        }

        private async void btnCheckout_Clicked(object sender, EventArgs e)
        {
            try
            {
                var answer = await DisplayAlert(AppResource.textCloseBill, AppResource.alertWantcancelCheckout, AppResource.textOk, AppResource.alertCancel);

                if (!answer)
                    return;

                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);
                var culture = App.AppCurrent.Culture;

                var a = lblTotal.Text.Replace("R$", "").Replace("$", "").TrimStart().Replace(',', '-');
                var b = lblDesconto.Text.Replace("R$", "").Replace("$", "").TrimStart().Replace(',', '-');
                var c = lblTip.Text.Replace("R$", "").Replace("$", "").TrimStart().Replace(',', '-');

                var valorTotal = Decimal.Parse(a.Replace('.', ',').Replace('-', '.'), culture.NumberFormat);
                var valorDesconto = Decimal.Parse(b.Replace('.', ',').Replace('-', '.'), culture.NumberFormat);
                var valorTip = Decimal.Parse(c.Replace('.', ',').Replace('-', '.'), culture.NumberFormat);

                _checkoutTax.ClientId = _checkin.ClientId;
                _checkoutTax.CompanyId = _checkin.CompanyId;
                _checkoutTax.TotalPaid = valorTotal;
                _checkoutTax.PriceDiscount = valorDesconto;
                _checkoutTax.PriceTipPaid = valorTip;


                _checkin.PriceTipPaid = valorTip;
                CompanyService companyService = new CompanyService();
                await companyService.MakeCheckoutTax(_checkoutTax);

                _checkin.CheckinStatus = Enums.CheckinStatus.Checkout;
                Acr.UserDialogs.UserDialogs.Instance.Toast(AppResource.alertCheckinClosed);

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

        private async void btnAddProduct_Clicked(object sender, EventArgs e)
        {
            await App.AppCurrent.NavigationService.NavigateAsync(new AddOrderToClientPage(_checkin, _checkin.LocationId), null, true);
        }

        private async void OnDescontoTapped(object sender, EventArgs e)
        {
            var popup = new EntryPopup(AppResource.alertFillDiscount, "", AppResource.textOk, AppResource.alertCancel);
            popup.PopupClosed += async (o, closedArgs) => {
                if (closedArgs.Button == AppResource.textOk)
                {
                    try
                    {
                        _checkin.PriceDiscount = Convert.ToDecimal(closedArgs.Text);
                        lblDesconto.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", _checkin.PriceDiscount);
                        var newtotal = (total - Convert.ToDecimal(_checkin.PriceDiscount));


                        if (newtotal < 0)
                            newtotal = 0;

                        lblTotal.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", newtotal);

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
            popup.PopupClosed += async (o, closedArgs) => {
                if (closedArgs.Button == AppResource.textOk)
                {
                    try
                    {
                        _checkin.PriceTipPaid = Convert.ToDecimal(closedArgs.Text);
                        lblTip.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", _checkin.PriceTipPaid);
                        total = subTotal + Convert.ToDecimal(_checkin.PriceTipPaid);
                        var newtotal = total - _checkin.PriceDiscount;


                        if (newtotal < 0)
                            newtotal = 0;

                        lblTotal.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", newtotal);

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
                ListSubCheckin = JsonConvert.DeserializeObject<List<CheckinSub>>(result);


                stkLocations.IsVisible = true;

                foreach (var item in ListSubCheckin)
                {

                    var grid = new Grid
                    {
                        ColumnDefinitions =
                            {
                                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) }
                            }
                    };


                    StackLayout stk = new StackLayout();
                    stk.Margin = new Thickness(0, 0, 0, 10);
                    stk.Orientation = StackOrientation.Horizontal;
                    stk.Spacing = 0;
                    stk.StyleId = item.Id;



                    ImageCircle.Forms.Plugin.Abstractions.CircleImage image = new ImageCircle.Forms.Plugin.Abstractions.CircleImage();
                    image.HeightRequest = 26;
                    image.WidthRequest = 26;
                    image.FillColor = Color.FromHex("#ccc");
                    image.HorizontalOptions = LayoutOptions.Center;
                    image.VerticalOptions = LayoutOptions.Center;
                    image.Aspect = Aspect.AspectFill;
                    image.Source = item.Location.ImageUri;
                    image.StyleId = item.Id;

                    Label lblName = new Label();
                    lblName.FontSize = 12;
                    lblName.Margin = new Thickness(10, 0, 0, 0);
                    lblName.TextColor = Color.FromHex("#212121");
                    lblName.HorizontalTextAlignment = TextAlignment.Start;
                    lblName.VerticalTextAlignment = TextAlignment.Center;
                    lblName.Text = item.Location.Name + " - Total: " + AppResource.currency + item.TotalPaid.ToString("F");

                    stk.Children.Add(image);
                    stk.Children.Add(lblName);

                    //var tpReco = new TapGestureRecognizer();
                    //tpReco.Tapped += OnLocationTapped;
                    //stk.GestureRecognizers.Add(tpReco);


                    var stkTwo = new StackLayout();


                    Image imgCk = new Image();
                    //imgCk.IsVisible = item.CheckinSubStatus != Enums.CheckinSubStatus.Closed ? true : false;
                    imgCk.IsVisible = false;
                    imgCk.WidthRequest = 28;
                    imgCk.Source = "ic_detail_checkout";

                    Label lblStatus = new Label();
                    lblStatus.FontSize = 12;
                    lblStatus.Text = item.CheckinSubStatusStr;
                    lblStatus.IsVisible = true;

                    stkTwo.Children.Add(lblStatus);
                    stkTwo.Children.Add(imgCk);


                    grid.Children.Add(stk, 0, 0);
                    grid.Children.Add(stkTwo, 2, 0);


                    //var detailsRecognizer = new TapGestureRecognizer();
                    //detailsRecognizer.Tapped += OnCkGestureTapped_Tapped;
                    //imgCk.GestureRecognizers.Add(detailsRecognizer);
                    imgCk.StyleId = item.Id;


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

        private async void OnLocationTapped(object sender, EventArgs e)
        {
            var item = sender as StackLayout;

            await App.AppCurrent.NavigationService.NavigateAsync(new OrdersBySubCheckinPage(ListSubCheckin.FirstOrDefault(x => x.Id == item.StyleId), _checkin), null, true);

        }

        private async void OnCkGestureTapped_Tapped(object sender, EventArgs e)
        {
            var item = sender as Image;

            OrderService Orderservice = new OrderService();

            try
            {

                var result = await Orderservice.UpdateRequestedSubCheckout(item.StyleId, Enums.CheckinSubStatus.Closed);

                Acr.UserDialogs.UserDialogs.Instance.Toast(AppResource.alertCheckinClosed);



            }
            catch (Exception ex)
            {
                this.DisplayAlert(MocoApp.Resources.AppResource.alertAlert, ex.Message, AppResource.textOk);

            }

        }
    }
}
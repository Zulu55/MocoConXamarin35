using MocoApp.Controls;
using MocoApp.Extensions;
using MocoApp.Models;
using MocoApp.Resources;
using MocoApp.Services;
using MocoApp.Services.V2;
using MocoApp.Views.Cliente;
using MocoApp.Views.Empresa;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static MocoApp.Views.CartFlow.CheckoutCartPage;

namespace MocoApp.Views.CartFlow
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CheckoutCartPage : ContentPage
    {

        public class HourSelectionItem : SelectionItem
        {
            TimeSpan _hour;
            public TimeSpan Hour
            {
                get => _hour;
                set
                {
                    _hour = value;
                    //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Hour"));
                    OnPropertyChanged("Hour");
                }
            }

            // padrao
            public bool LabelVisible { get; set; } = true;
            public bool HourVisible { get; set; } = false;

            public void ToggleVisible()
            {
                LabelVisible = !LabelVisible;
                HourVisible = !HourVisible;
                OnPropertyChanged("LabelVisible");
                OnPropertyChanged("HourVisible");
            }

            public void ResetToDefaultState()
            {
                LabelVisible = true;
                HourVisible = false;
                OnPropertyChanged("LabelVisible");
                OnPropertyChanged("HourVisible");
            }
        }

        public class SelectionItem : INotifyPropertyChanged
        {

            public string Id { get; set; }

            string _lbl;
            public string Label
            {
                get => _lbl;
                set
                {
                    _lbl = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Label"));
                }
            }

            // usar isso pra definir a ação
            public string OptionTag { get; set; }

            bool _sel;
            public bool IsSelected
            {
                get => _sel;
                set
                {
                    _sel = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedColor"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedTextColor"));

                }
            }

            public Color CustomColor { get; set; } = Color.White;
            public Color SelectedColor => IsSelected ? Color.FromHex("#0bd479") : (CustomColor == Color.Default ? Color.Default : CustomColor);

            // se precisar arrumar a cor do texto
            public Color SelectedTextColor => IsSelected ? Color.Default : Color.Default;

            public event PropertyChangedEventHandler PropertyChanged;

            public override string ToString()
            {
                return Label;
            }

            protected void OnPropertyChanged(string _prop)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(_prop));
            }
        }

        List<SelectionItem> orderTimes;
        List<SelectionItem> paymentOptions;
        List<SelectionItem> paymentMethods;

        //SelectionItem datePickItem;
        Checkin checkin;

        private string _stripeId = "";

        public CheckoutCartPage()
        {
            // criar antes de mudar o bindingcontext
            orderTimes = new List<SelectionItem>()
            {
                new SelectionItem() { Label = AppResource.textCartAsap },
                new HourSelectionItem() { Label = AppResource.textCartSpecifiedTime}
            };

            // guardar esse, com find so pra n usar index fixo
            //datePickItem = orderTimes.Find(x => x.OptionTag == "OpenDatePicker");

            paymentOptions = new List<SelectionItem>()
            {
                new SelectionItem() { Id = "1",  Label = AppResource.textCartTransferBill},
                new SelectionItem() { Id = "2", Label = AppResource.textCartPayNow}
            };

            paymentMethods = new List<SelectionItem>()
                {
                    new SelectionItem() { Id = "2", Label = AppResource.textCartCash },
                };

            InitializeComponent();
            ColorPage();

            LoadPage();


            BindingContext = App.AppCurrent.Cart;

            //LoadPck();
            //pckTime.SelectedIndexChanged += PckTime_SelectedIndexChanged;
            //pckPaymentOption.SelectedIndexChanged += PckPaymentOption_SelectedIndexChanged;
            //pckPaymentMethod.SelectedIndexChanged += PckPaymentMethod_SelectedIndexChanged;

            _stripeId = "";
        }

        protected override async void OnAppearing()
        {
            await LoadWallet();
            base.OnAppearing();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();



        }

        private async void OnCheckout_Tapped(object sender, EventArgs e)
        {
            if (Helpers.Settings.DisplayUserRole == Enums.UserRole.Guest.ToString())
            {
                App.AppCurrent.AlertToGuest();
                return;
            }

            if (App.AppCurrent.Cart.TimeToOrder == Models.ETimeToOrder.None)
            {
                await DisplayAlert(AppResource.lblSelect, AppResource.alertSelectTimeOrder, AppResource.textOk);
                return;
            }
            if (App.AppCurrent.Cart.TimeToOrder == Models.ETimeToOrder.SpecifiedTime)
            {
                App.AppCurrent.Cart.DateTimeToOrder = new DateTimeOffset(dtDate.Date.Year, dtDate.Date.Month, dtDate.Date.Day, hourPicker.Time.Hours, hourPicker.Time.Minutes, 00, DateTimeOffset.Now.Offset);


            }

            if (App.AppCurrent.Cart.PaymentOption == Models.EPaymentOption.None)
            {
                await DisplayAlert(AppResource.lblSelect, AppResource.alertSelectPaymentType, AppResource.textOk);
                return;
            }

            if (App.AppCurrent.Cart.PaymentMethod == "" && App.AppCurrent.Cart.PaymentOption == Models.EPaymentOption.CloseBill)
            {
                await DisplayAlert(AppResource.lblSelect, AppResource.alertPaymentMethod, AppResource.textOk);
                return;
            }


            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);

                var orderService = new OrderServicev2();
                var result = await orderService.MakeCartOrder(App.AppCurrent.Cart, Helpers.Settings.DisplayUserToken);

                if (result)
                {
                    Acr.UserDialogs.UserDialogs.Instance.Toast(AppResource.AlertCartRequestedSucessfull);
                    App.AppCurrent.Cart = new Models.CartModel();


                    await App.AppCurrent.NavigationService.ModalGoBack(true);
                    await App.AppCurrent.NavigationService.ModalGoBack(true);

                    await App.AppCurrent.NavigationService.NavigateModalAsync(new CompanyDetailsPage(checkin.Company), null, false);


                    //zera 

                    //await App.AppCurrent.NavigationService.NavigateSetRootAsync(new ListCheckinsPage(), null, true);
                }
                else
                {
                    await DisplayAlert(AppResource.alertAlert, AppResource.alertErrorOcured, AppResource.textOk);

                }



            }
            catch (Exception ex)
            {

                await DisplayAlert(AppResource.alertAlert, ex.Message, AppResource.textOk);

            }
            finally
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }


        }

        public void ColorPage()
        {
            switch (App.CompanyTypeSelected)
            {
                case Models.Enums.CompanyType.Hotel:
                    lblCompanyTitle.TextColor = (Color)App.Current.Resources["HotelColor"];
                    lblHeader.TextColor = (Color)App.Current.Resources["HotelColor"];
                    lblLocationName.TextColor = (Color)App.Current.Resources["HotelColor"];
                    imgBack.Source = "ic_voltar_hotel";
                    break;
                case Models.Enums.CompanyType.Restaurante:
                    lblCompanyTitle.TextColor = (Color)App.Current.Resources["RestauranteColor"];
                    lblHeader.TextColor = (Color)App.Current.Resources["RestauranteColor"];
                    lblLocationName.TextColor = (Color)App.Current.Resources["RestauranteColor"];
                    imgBack.Source = "ic_voltar_restaurante";
                    break;
                case Models.Enums.CompanyType.Praia:
                    lblCompanyTitle.TextColor = (Color)App.Current.Resources["BarracaColor"];
                    lblHeader.TextColor = (Color)App.Current.Resources["BarracaColor"];
                    lblLocationName.TextColor = (Color)App.Current.Resources["BarracaColor"];
                    imgBack.Source = "ic_voltar_praia";
                    break;
                case Models.Enums.CompanyType.EsporteEvento:
                    lblCompanyTitle.TextColor = (Color)App.Current.Resources["EsportesColor"];
                    lblHeader.TextColor = (Color)App.Current.Resources["EsportesColor"];
                    lblLocationName.TextColor = (Color)App.Current.Resources["EsportesColor"];
                    imgBack.Source = "ic_voltar_esportes";
                    break;
                default:
                    break;
            }
        }


        public async Task LoadCheckin()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);
                App.AppCurrent.Cart.PaymentMethod = "";
                App.AppCurrent.Cart.PaymentOption = Models.EPaymentOption.None;
                App.AppCurrent.Cart.TimeToOrder = Models.ETimeToOrder.None;
                App.AppCurrent.Cart.Checkin = null;

                lblCompanyTitle.Text = App.AppCurrent.Cart.Company.Title;
                lblLocationName.Text = App.AppCurrent.Cart.Location.Name;


                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);

                var checkinService = new CheckinServicev2();
                checkin = await checkinService.GetActiveCheckin(App.AppCurrent.Cart.Company.Id, Helpers.Settings.DisplayUserToken);

                App.AppCurrent.Cart.CheckinId = checkin.Id;
                App.AppCurrent.Cart.Company.CreditCardAllowed = checkin.Company.CreditCardAllowed;

                lblTaxsTitle.Text = AppResource.lblTaxs + "(" + checkin.Company.TaxPercentage + "%)";
                lblTipTitle.Text = AppResource.lblTips + "(" + checkin.Company.RecommendedTipPercentage + "%)";

                //TA PEGANDO DOS EUA, VERIFICAR SE TEM Q ALTERAR

                var culture = checkin.Company.CurrencyType.ToCultureInfo();

                decimal totalTips = 0;
                decimal totalTax = 0;
                if (checkin.Company.CurrencyType == Enums.ECurrencyType.COL)
                {
                    totalTips = App.AppCurrent.Cart.SubTotal * (checkin.Company.RecommendedTipPercentage / 100);
                    totalTax = (App.AppCurrent.Cart.SubTotal * (checkin.Company.TaxPercentage / 100));
                }
                else
                {
                    totalTips = App.AppCurrent.Cart.SubTotal * (checkin.Company.RecommendedTipPercentage / 100).FloorDecimal(2);
                    totalTax = (App.AppCurrent.Cart.SubTotal * (checkin.Company.TaxPercentage / 100)).FloorDecimal(2);
                }

                lblTip.Text = String.Format(culture, "{0:C}", totalTips);


                lblTax.Text = String.Format(culture, "{0:C}", totalTax);

                lblSubTotal.Text = String.Format(culture, "{0:C}", App.AppCurrent.Cart.SubTotal);

                App.AppCurrent.Cart.PriceTipPaid = totalTips;
                App.AppCurrent.Cart.PriceTaxPaid = totalTax;
                App.AppCurrent.Cart.UpdateTotalBill(culture);

                //I dont have checkin, so I need make open the page to make the checkin?
                //Here is the follow without Checkin
                if (checkin == null)
                {




                }
                else
                {



                    //preciso verificar se o produtos são da location do meu ultimo checkin e se ele esta ativo
                    //Here follow the right flow, to make the order
                    if (checkin.Company.Id == App.AppCurrent.Cart.Company.Id)
                    {
                        //verifica se possui checkin na mesma location
                        if (App.AppCurrent.Cart.Company.HasLocation)
                        {
                            if (checkin.CheckinSubs.Where(m => m.LocationId == App.AppCurrent.Cart.Location.Id).FirstOrDefault() != null)
                            {
                                //tem checkin na location
                            }
                        }

                        lblOccupationName.Text = checkin.OccupationStr;
                    }
                    else
                    {
                        //I have an openned checkin, but isn't the same location
                        //Bad Flow, so the app need go back her and advertise 
                        await DisplayAlert(AppResource.alertAlert, "Você possuo checkin em outra empresa. Impossivel continuar.", "Ok");
                        //await App.AppCurrent.NavigationService.ModalGoBack();

                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
        }

        public async void LoadPage()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);

                await LoadCheckin();
                //await LoadWallet();

                invisibleDatePicker.Date = DateTime.Today;
                debugDate.Text = "Selected date:" + invisibleDatePicker.Date.ToString("dd/MM/yyyy");

                //listOrderTime.ItemsSource = orderTimes;
                listPaymentOptions.ItemsSource = paymentOptions;
                listPaymentMethods.ItemsSource = paymentMethods;



                orderTimeASAPStack.BindingContext = orderTimes[0];
                orderTimePickerStack.BindingContext = orderTimes[1];

                // as alturas tem que ser calculadas aqui pra altura da lista não dar scroll
                //listOrderTime.HeightRequest = orderTimes.Count * 40 + 48;
                listPaymentOptions.HeightRequest = paymentOptions.Count * 40 + 48;
                listPaymentMethods.HeightRequest = paymentMethods.Count * 40 + 48;


                listPaymentOptions.ItemTapped += (sender, args) =>
                {
                    foreach (var item in paymentOptions)
                    {
                        item.IsSelected = false;
                    }

                    var selected = args.Item as SelectionItem;
                    selected.IsSelected = true;


                    App.AppCurrent.Cart.PaymentOption = (MocoApp.Models.EPaymentOption)Convert.ToInt32(selected.Id);


                    //deseja deixar a conta aberta, entao vai para o quarto
                    if (App.AppCurrent.Cart.PaymentOption == Models.EPaymentOption.KeepBillOpen)
                    {
                        stkPaymentMethods.IsVisible = false;
                        App.AppCurrent.Cart.PaymentMethod = "";

                        //zera a listview de tipo metodos de pagamento
                        listPaymentMethods.SelectedItem = null;
                        foreach (var item in paymentMethods)
                            item.IsSelected = false;


                    }
                    if (App.AppCurrent.Cart.PaymentOption == Models.EPaymentOption.CloseBill)
                    {
                        stkPaymentMethods.IsVisible = true;
                        App.AppCurrent.Cart.PaymentMethod = "";
                    }
                };

            }
            catch (Exception)
            {


            }
            finally
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                stkTimes.IsVisible = true;
            }


        }

        public async Task LoadWallet()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);
                if (App.AppCurrent.Cart.Company.CreditCardAllowed)
                {
                    grdAddCard.IsVisible = true;
                    var userService = new UserServicev2();
                    var myWallet = await userService.GetMyWalletV2();
                    paymentMethods = new List<SelectionItem>();
                    paymentMethods.Add(new SelectionItem() { Id = "2", Label = AppResource.textCartCash });
                    foreach (var item in myWallet)
                    {
                        paymentMethods.Add(
                                new SelectionItem() { Id = item.StripeId, Label = item.ShowName, IsSelected = item.Default }
                        );
                        if (item.Default)
                            _stripeId = item.StripeId;
                    }


                    listPaymentMethods.ItemTapped += async (sender, args) =>
                    {
                        foreach (var item in paymentMethods)
                        {
                            item.IsSelected = false;
                        }

                        var selected = args.Item as SelectionItem;
                        selected.IsSelected = true;
                        App.AppCurrent.Cart.StripeId = selected.Id;

                        App.AppCurrent.Cart.PaymentMethod = selected.Id;
                    };
                }
                else
                {
                    paymentMethods = new List<SelectionItem>();
                    paymentMethods.Add(new SelectionItem() { Id = "2", Label = AppResource.textCartCash });

                    listPaymentMethods.ItemTapped += async (sender, args) =>
                    {
                        foreach (var item in paymentMethods)
                        {
                            item.IsSelected = false;
                        }

                        var selected = args.Item as SelectionItem;
                        selected.IsSelected = true;
                        App.AppCurrent.Cart.StripeId = selected.Id;

                        App.AppCurrent.Cart.PaymentMethod = selected.Id;
                    };

                    grdAddCard.IsVisible = false;
                }

                listPaymentMethods.ItemsSource = paymentMethods;
                listPaymentMethods.HeightRequest = paymentMethods.Count * 40 + 48;
            }
            catch (Exception ex)
            {


            }
            finally
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }


        }

        private async void OnAddCard_Tapped(object sender, EventArgs e)
        {
            App.AppCurrent.NavigationService.NavigateModalAsync(new AddCardPage(), null, true);

        }

        private void ASAP_Tapped(object sender, EventArgs e)
        {
            App.AppCurrent.Cart.TimeToOrder = Models.ETimeToOrder.Asap;

            (orderTimeASAPStack.BindingContext as SelectionItem).IsSelected = true;
            (orderTimePickerStack.BindingContext as SelectionItem).IsSelected = false;
            (orderTimes[1] as HourSelectionItem).ResetToDefaultState();

        }

        private void TimePicker_Tapped(object sender, EventArgs e)
        {
            App.AppCurrent.Cart.TimeToOrder = Models.ETimeToOrder.SpecifiedTime;

            (orderTimeASAPStack.BindingContext as SelectionItem).IsSelected = false;
            (orderTimePickerStack.BindingContext as SelectionItem).IsSelected = true;

            (orderTimes[1] as HourSelectionItem).ToggleVisible();
            hourPicker.Focus();

        }

        private async void OnTipsTapped(object sender, EventArgs e)
        {


            var popup = new EntryPopup(AppResource.alertFillTips, "", AppResource.textOk, AppResource.alertCancel);
            popup.PopupClosed += async (o, closedArgs) =>
            {
                if (closedArgs.Button == AppResource.textOk)
                {
                    try
                    {
                        App.AppCurrent.Cart.PriceTipPaid = Convert.ToDecimal(closedArgs.Text);
                        lblTip.Text = String.Format(App.AppCurrent.CompanyCulture, "{0:C}", App.AppCurrent.Cart.PriceTipPaid);
                        lblTipTitle.Text = AppResource.lblTips;

                        App.AppCurrent.Cart.UpdateTotalPrice();



                    }
                    catch (Exception ex)
                    {
                        this.DisplayAlert(MocoApp.Resources.AppResource.alertAlert, AppResource.alertOnlyNumber, AppResource.textOk);

                    }
                }


            };

            popup.Show();
        }


        #region old
        //private void PckPaymentMethod_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    var item = sender as Picker;

        //    var itemSelected = item.SelectedItem;

        //    if(item.SelectedItem == "Add Credit Card")
        //    {
        //        App.AppCurrent.NavigationService.NavigateModalAsync(new AddCardPage(), null, true);
        //    }
        //}

        //private void PckPaymentOption_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //}

        //private void PckTime_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    var item = sender as Picker;

        //    var itemSelected = item.SelectedItem;

        //    if (item.SelectedItem as string == "Specific Time")
        //    {
        //        orderDatePicker.Focus();
        //    }
        //    else if (item.SelectedItem as string == "")
        //    {
        //        item.SelectedIndex = 0;
        //    }
        //}
        #endregion

        #region old
        //public void LoadPck()
        //{

        //    //LoadPckTime();
        //    LoadPckPaymentOption();
        //    LoadPckPaymentMethod();
        //}

        ////public void LoadPckTime()
        ////{
        ////    pckTime.Items.Add("Asap");
        ////    pckTime.Items.Add("Specific Time");
        ////}

        //public void LoadPckPaymentOption()
        //{

        //    pckPaymentOption.Items.Add("Keep Bill Open - Pay Later");
        //    pckPaymentOption.Items.Add("Close Bill - Pay Now");
        //}

        //public void LoadPckPaymentMethod()
        //{
        //    pckPaymentMethod.Items.Add("Cash");
        //    pckPaymentMethod.Items.Add("Bill to Room");
        //    pckPaymentMethod.Items.Add("Amex - 4343");
        //    pckPaymentMethod.Items.Add("Add Credit Card");

        //}


        //private void OrderDate_Selected(object sender, DateChangedEventArgs e)
        //{
        //    orderDatePicker.IsVisible = true;
        //    pckTimeLabel.IsVisible = false;
        //}

        //async void SelectTime_Tapped(object sender, EventArgs e)
        //{
        //    var answer = await DisplayActionSheet("Opções", null, null, "ASAP", "Specific time");

        //    if (answer == "Specific time")
        //    {
        //        orderDatePicker.Date = DateTime.Today.AddDays(-1);
        //        orderDatePicker.Focus();
        //    }
        //}
        #endregion

        private async void OnContOnBackinue_Tapped(object sender, EventArgs e)
        {
            await App.AppCurrent.NavigationService.ModalGoBack();
        }


    }
}
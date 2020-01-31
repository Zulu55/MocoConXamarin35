using MocoApp.DTO;
using MocoApp.Models;
using MocoApp.Resources;
using MocoApp.Services;
using MocoApp.Views.Cliente;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MocoApp.Views.ClientCheckinFlow
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MultiLocationRequestCheckin : ContentPage
    {
        Company _company;
        List<Location> _locations;
        Checkin _checkin;
        string _locationId = "";
        MultiLocationRequestCheckinDTO _resultDTO;
        public MultiLocationRequestCheckin(Company company, string locationId = ""  )
        {
            InitializeComponent();
            _company = company;

            if (Device.RuntimePlatform == "Android")
                frmXaml.CornerRadius = 30;


            ColorPage();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<object>(this, "Push", (sender) =>
            {
                if (App.AppCurrent.NavigationService.ModalCurrentPage is MultiLocationRequestCheckin)
                {
                    stkCheckout.IsVisible = false;
                    LoadPage();
                }
                //var page = sender as Page;
                //if (page is MultiLocationRequestCheckin)
                //{
                //    LoadPage();
                //}

            });

            //stkLocations.IsVisible = false;
            LoadPage();

            //base.OnAppearing();
            //LoadPage();
        }

        public void ColorPage()
        {


            switch (_company.CompanyType)
            {
                case Models.Enums.CompanyType.Hotel:
                    this.BackgroundColor = Color.FromHex("#6dadeb");
                    lblName.TextColor = Color.FromHex("#6dadeb");
                    imgBack.Source = "ic_voltar_hotel";
                    //txtOccupation.Placeholder = AppResource.textNotGuestHotel;
                    break;
                case Models.Enums.CompanyType.Restaurante:
                    this.BackgroundColor = Color.FromHex("#fdbf2c");
                    lblName.TextColor = Color.FromHex("#fdbf2c");
                    imgBack.Source = "ic_voltar_restaurante";
                    txtOccupation.IsVisible = false;
                    lblOcuupation.IsVisible = false;
                    //txtOccupation.Placeholder = AppResource.textNotHaveReservation;
                    break;
                case Models.Enums.CompanyType.Praia:
                    this.BackgroundImage = "bg_praia";
                    this.BackgroundColor = Color.Transparent;
                    lblName.TextColor = Color.FromHex("#009145");
                    imgBack.Source = "ic_voltar_praia";
                    txtOccupation.IsVisible = false;
                    lblOcuupation.IsVisible = false;
                    //txtOccupation.Placeholder = AppResource.textNotHaveReservation;
                    break;
                case Models.Enums.CompanyType.EsporteEvento:
                    this.BackgroundColor = (Color)App.Current.Resources["EsportesColor"];
                    lblName.TextColor = (Color)App.Current.Resources["EsportesColor"];
                    imgBack.Source = "ic_voltar_esportes";
                    txtOccupation.IsVisible = false;
                    lblOcuupation.IsVisible = false;
                    //txtOccupation.Placeholder = AppResource.textNotHaveReservation;
                    break;
            }
        }

        public async void LoadPage()
        {
            try
            {
                //imgCheck.Source = "ic_detail_checkin";
                stkItens.IsVisible = false;
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);

                //lblOcuupation.Text = string.Format(AppResource.lblPleaseFill, _company.OccupationPrefix);
                lblOcuupation.Text = _company.OccupationPrefix;

                OrderService orderService = new OrderService();
                var result = await orderService.GetCheckinByClienteAndCompanyId(_company.Id);
                _resultDTO = JsonConvert.DeserializeObject<MultiLocationRequestCheckinDTO>(result);

                _checkin = _resultDTO.Checkin;
                _company = _resultDTO.Company;
                _locations = _resultDTO.Locations;

                if (_checkin != null)
                {
                    stkCheckout.IsVisible = true;
                    lblCheckOut.Text = AppResource.textRequestCheckout + " " + _checkin.Company.Title;
                }

                if (_locations != null)
                {
                    //LoadLocations(_locations);
                    lblTextRoomNumber.IsVisible = false;
                    txtQtd.IsVisible = false;

                    if (_checkin != null && !string.IsNullOrEmpty(_checkin.Occupation))
                    {
                        txtOccupation.Text = _checkin.Occupation;
                        txtOccupation.IsEnabled = false;
                    }
                    else
                        //lblOcuupation.Text = AppResource.textRoomNumber;
                        //lblOcuupation.Text = string.Format(AppResource.lblPleaseFill, _company.OccupationPrefix);
                        lblOcuupation.Text = _company.OccupationPrefix;


                }
                else
                {

                    stkItens.IsVisible = true;
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                }

            }
            catch (Exception ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast(AppResource.alertNoneCheckinFound);
                App.AppCurrent.NavigationService.ModalGoBack();
            }
            finally
            {
                stkAct.IsVisible = false;
                stkItens.IsVisible = true;
                await Task.Delay(500);
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();

            }

        }


        public async void LoadLocations(List<Location> list)
        {
            stkAct.IsVisible = true;
            stkLocations.IsVisible = true;
            stkCk.IsVisible = false;
            stkLocations.Children.Clear();
            foreach (var item in list)
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



                ImageCircle.Forms.Plugin.Abstractions.CircleImage image = new ImageCircle.Forms.Plugin.Abstractions.CircleImage();
                image.HeightRequest = 26;
                image.WidthRequest = 26;
                image.FillColor = Color.FromHex("#ccc");
                image.HorizontalOptions = LayoutOptions.Center;
                image.VerticalOptions = LayoutOptions.Center;
                image.Aspect = Aspect.AspectFill;
                image.Source = item.ImageUri;
                image.StyleId = item.Id;

                Label lblName = new Label();
                lblName.FontSize = 12;
                lblName.Margin = new Thickness(10, 0, 0, 0);
                lblName.TextColor = Color.FromHex("#212121");
                lblName.HorizontalTextAlignment = TextAlignment.Start;
                lblName.VerticalTextAlignment = TextAlignment.Center;
                lblName.Text = item.Name;

                stk.Children.Add(image);
                stk.Children.Add(lblName);



                Image imgCk = new Image();
                imgCk.WidthRequest = 28;


                if (item.CheckinAction == CheckinAction.EnableLocationCheckin || item.CheckinAction == CheckinAction.HasAnotherLocationPending)
                {
                    imgCk.Source = "ic_detail_checkin";
                    var detailsRecognizer = new TapGestureRecognizer();
                    detailsRecognizer.Tapped += OnCkGestureTapped_Tapped;
                    grid.GestureRecognizers.Add(detailsRecognizer);
                    grid.StyleId = item.Id;
                }
                //if (item.CheckinAction == CheckinAction.HasAnotherLocationPending)
                //{
                //    imgCk.Source = "ic_detail_checkin";
                //    var detailsRecognizer = new TapGestureRecognizer();
                //    detailsRecognizer.Tapped += onMessageTapped_Tapped;
                //    grid.GestureRecognizers.Add(detailsRecognizer);
                //}
                if (item.CheckinAction == CheckinAction.CheckoutLocationCheckin)
                {
                    imgCk.Source = "ic_detail_checkout";
                    //var detailsRecognizer = new TapGestureRecognizer();
                    //detailsRecognizer.Tapped += OnCancelTapped;
                    //grid.GestureRecognizers.Add(detailsRecognizer);
                    grid.StyleId = item.Id;

                }
                //posso fazer checkin na empresa
                //if (item.CheckinAction == CheckinAction.EnableLocationCheckin)
                //{
                //    imgCk.Source = "ic_detail_checkin";
                //    var detailsRecognizer = new TapGestureRecognizer();
                //    detailsRecognizer.Tapped += OnCkGestureTapped_Tapped;
                //    grid.GestureRecognizers.Add(detailsRecognizer);
                //    grid.StyleId = item.Id;
                //}
                //if (item.CheckinAction == CheckinAction.HasAnotherLocationPending)
                //{
                //    imgCk.Source = "ic_detail_checkin";
                //    var detailsRecognizer = new TapGestureRecognizer();
                //    detailsRecognizer.Tapped += onMessageTapped_Tapped;
                //    grid.GestureRecognizers.Add(detailsRecognizer);
                //}
                //if (item.CheckinAction == CheckinAction.CheckoutLocationCheckin)
                //{
                //    imgCk.Source = "ic_detail_checkout";
                //    var detailsRecognizer = new TapGestureRecognizer();
                //    detailsRecognizer.Tapped += OnCancelTapped;
                //    grid.GestureRecognizers.Add(detailsRecognizer);
                //    grid.StyleId = item.Id;

                //}



                grid.Children.Add(stk, 0, 0);
                grid.Children.Add(imgCk, 2, 0);

                stkLocations.Children.Add(grid);



            }

            stkAct.IsVisible = false;
            stkItens.IsVisible = true;
            await Task.Delay(500);
            Acr.UserDialogs.UserDialogs.Instance.HideLoading();
        }

        private void onMessageTapped_Tapped(object sender, EventArgs e)
        {
            Acr.UserDialogs.UserDialogs.Instance.Toast(AppResource.alertNeedCheckoutLocation);
        }


        private async void OnCkGestureTapped_Tapped(object sender, EventArgs e)
        {
            var grd = sender as Grid;
            _locationId = grd.StyleId;


            var location = _locations.Where(m => m.Id == _locationId).FirstOrDefault();
            //20190403 
            if (location.LocationType == Enums.LocationType.Room)
            {
                if (string.IsNullOrEmpty(txtOccupation.Text))
                {
                    await this.DisplayAlert(MocoApp.Resources.AppResource.alertAlert, AppResource.alertFillRoomNumber, AppResource.textOk);
                    return;
                }

                OnOkTapped(null, null);
            }

            if (string.IsNullOrEmpty(txtOccupation.Text))
            {
                await this.DisplayAlert(MocoApp.Resources.AppResource.alertAlert, AppResource.alertFillRoomNumber, AppResource.textOk);
                return;
            }

            if (location.LocationType == Enums.LocationType.Room)
            {
                OnOkTapped(null, null);
            }
            else
                await App.AppCurrent.NavigationService.NavigateModalAsync(new RequestCheckinByLocationPage(_company, _locationId, "", txtOccupation.Text, _checkin, location), null, false);
            //OnOkTapped(null, null);

        }

        private async void OnOkTapped(object sender, EventArgs e)
        {
            CompanyService service = new CompanyService();

            try
            {

                RequestCheckin requestCheckin = new RequestCheckin();

                requestCheckin.CompanyId = _company.Id;
                requestCheckin.ClientQuantity = txtQtd.Text;
                requestCheckin.ClientId = Helpers.Settings.DisplayUserId;
                requestCheckin.Occupation = txtOccupation.Text;
                requestCheckin.LocationId = _locationId;


                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);
                bool result = false;

                if (_company.CheckedIn && !string.IsNullOrEmpty(_locationId))
                {
                    var sub = new CheckinSub();
                    sub.LocationId = _locationId;
                    sub.Occupation = requestCheckin.Occupation;
                    sub.ClientQuantity = requestCheckin.ClientQuantity;
                    sub.ClientId = Helpers.Settings.DisplayUserId;

                    result = await service.RequestCheckinSub(sub);
                }
                else
                {

                    if (string.IsNullOrEmpty(txtQtd.Text) && txtQtd.IsVisible)
                    {
                        await this.DisplayAlert(MocoApp.Resources.AppResource.alertAlert, AppResource.alertInformQtdPeople, AppResource.textOk);
                        return;
                    }
                    else if (string.IsNullOrEmpty(txtOccupation.Text) && txtOccupation.IsVisible)
                    {
                        await this.DisplayAlert(MocoApp.Resources.AppResource.alertAlert, string.Format(AppResource.alertNeedInformOccupation, txtOccupation.Text), AppResource.textOk);
                        return;
                    }
                    result = await service.AddRemoveCheckinPost(requestCheckin);
                }





                if (result)
                {
                    await this.DisplayAlert(AppResource.textCheckin, AppResource.alertCheckinSucess, AppResource.textOk);
                    await App.AppCurrent.NavigationService.ModalGoBack();
                }
                else
                    await this.DisplayAlert(AppResource.alertAlert, AppResource.alertErrorOcured, AppResource.textOk);




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

        private async void OnCancelTapped(object sender, EventArgs e)
        {
            //ALAIN PEDIU ESSA ALTERAÇÂO DIA 08/11 as 00:22, a noite que comprou acai e subway,  https://trello.com/c/4hB44Vko/482-when-client-click-check-out-in-main-estabilimento-page-or-location-screen-it-should-be-redirect-payment-screen-pending-marco


            await App.AppCurrent.NavigationService.NavigateModalAsync(new ListLocationBillsPage(_checkin, _company), null, true);
            //CompanyService service = new CompanyService();
            //string msg = AppResource.alertWantCloseLocationBill;
            //string title = AppResource.textCheckout;

            //var answer = await DisplayAlert(title, msg, AppResource.textOk, AppResource.alertCancel);

            //if (!answer)
            //    return;

            //Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);

            //try
            //{
            //    var result = await service.RequestCheckoutFromClient(_company.Id, 0);
            //    Acr.UserDialogs.UserDialogs.Instance.Toast(AppResource.alertRequestSucess, TimeSpan.FromSeconds(7));


            //}
            //catch (Exception ex)
            //{
            //    this.DisplayAlert(MocoApp.Resources.AppResource.alertAlert, ex.Message, AppResource.textOk);

            //}
            //finally
            //{
            //    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            //    App.AppCurrent.NavigationService.ModalGoBack();

            //}

        }

        private async void OnBackTapped(object sender, EventArgs e)
        {
            await App.AppCurrent.NavigationService.ModalGoBack();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<object, string>(this, "Push");
        }

    }
}
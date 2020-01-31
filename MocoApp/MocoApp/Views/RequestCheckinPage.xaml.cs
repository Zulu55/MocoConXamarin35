using MocoApp.Models;
using MocoApp.Resources;
using MocoApp.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MocoApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RequestCheckinPage : ContentPage
    {
        Company _company;
        string _locationId = "";
        List<Location> _locations;
        Checkin _checkin;
        public RequestCheckinPage(Company company, string locationId = "", List<Location> locations = null, Checkin checkin = null, string occupation = "")
        {
            InitializeComponent();
            _company = company;
            _locationId = locationId;
            txtOccupation.Text = occupation;
            _checkin = checkin;

            if (Device.RuntimePlatform == "Android")
                frmXaml.CornerRadius = 30;

            LoadPage();
           

        }

        public void LoadLocations(List<Location> list)
        {
            stkLocations.IsVisible = true;
            stkCk.IsVisible = false;

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
                imgCk.Source = "ic_detail_checkin";


                grid.Children.Add(stk, 0, 0);
                grid.Children.Add(imgCk, 2, 0);

                var detailsRecognizer = new TapGestureRecognizer();
                detailsRecognizer.Tapped += OnCkGestureTapped_Tapped;
                grid.GestureRecognizers.Add(detailsRecognizer);
                grid.StyleId = item.Id;


                stkLocations.Children.Add(grid);

            }
        }

        private async void OnCkGestureTapped_Tapped(object sender, EventArgs e)
        {
            var grd = sender as Grid;
            _locationId = grd.StyleId;


            var location = _locations.Where(m => m.Id == _locationId).FirstOrDefault();

            if (location.LocationType == Enums.LocationType.Room) {
                if (string.IsNullOrEmpty(txtOccupation.Text))
                {
                    await this.DisplayAlert(MocoApp.Resources.AppResource.alertAlert, "Insira o numero do quarto.", AppResource.textOk);
                    return;
                }

                OnOkTapped(null, null);
            }

                
            else
                await App.AppCurrent.NavigationService.NavigateModalAsync(new RequestCheckinByLocationPage(_company, _locationId, "", txtOccupation.Text, null, location), null, false);
            //OnOkTapped(null, null);

        }

        public void ColorPage()
        {
            //https://trello.com/c/7PGnZeNB/424-categoria-praia-hotel-ecc-except-hotel-should-not-ask-room-number-at-check-in
            //setei para false txtOccupation e lblOcuupation quando não é hotel

            switch (_company.CompanyType)
            {
                case Models.Enums.CompanyType.Hotel:
                    this.BackgroundColor = Color.FromHex("#6dadeb");
                    lblName.TextColor = Color.FromHex("#6dadeb");
                    imgBack.Source = "ic_voltar_hotel";
                    txtOccupation.Placeholder = "Não sou convidado do hotel";
                    break;
                case Models.Enums.CompanyType.Restaurante:
                    this.BackgroundColor = Color.FromHex("#fdbf2c");
                    lblName.TextColor = Color.FromHex("#fdbf2c");
                    imgBack.Source = "ic_voltar_restaurante";
                    txtOccupation.IsVisible = false;
                    lblOcuupation.IsVisible = false;
                    txtOccupation.Placeholder = "Não possuo reserva";
                    break;
                case Models.Enums.CompanyType.Praia:                       
                    this.BackgroundImage = "bg_praia";
                    this.BackgroundColor = Color.Transparent;
                    lblName.TextColor = Color.FromHex("#009145");
                    imgBack.Source = "ic_voltar_praia";
                    txtOccupation.IsVisible = false;
                    lblOcuupation.IsVisible = false;
                    txtOccupation.Placeholder = "Não possuo reserva";
                    break;
                case Models.Enums.CompanyType.EsporteEvento:
                    this.BackgroundColor = (Color)App.Current.Resources["EsportesColor"];
                    lblName.TextColor = (Color)App.Current.Resources["EsportesColor"];
                    imgBack.Source = "ic_voltar_esportes";
                    txtOccupation.IsVisible = false;
                    lblOcuupation.IsVisible = false;
                    txtOccupation.Placeholder = "Não possuo reserva";
                    break;
            }
        }

        public async void LoadPage()
        {
            try
            {
                OrderService orderService = new OrderService();
                var resultCk = await orderService.GetCheckinByClienteAndCompanyId(_company.Id);
                _checkin = JsonConvert.DeserializeObject<Checkin>(resultCk);

                if (_checkin != null)
                {
                    stkCheckout.IsVisible = true;
                    lblCheckOut.Text = "Solicitar Checkout " + _checkin.Company.Title;
                }


                if (_checkin != null && !string.IsNullOrEmpty(_checkin.Occupation))
                    txtOccupation.IsEnabled = false;

                ColorPage();

                if (_company.HasLocation)
                {
                    if (_locations != null)
                    {
                        LoadLocations(_locations);
                        lblTextRoomNumber.IsVisible = false;
                        txtQtd.IsVisible = false;
                        lblOcuupation.Text = "Número do Quarto";
                    }
                    else
                    {

                    }
                }
                else
                {
                    lblOcuupation.IsVisible = true;
                    lblTextRoomNumber.IsVisible = true;
                    txtOccupation.IsVisible = true;
                    txtQtd.IsVisible = true;
                }

            }
            catch (Exception ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Nenhum checkin encontrado.");
                App.AppCurrent.NavigationService.ModalGoBack();
            }
          
        }

        private async void OnCancelTapped(object sender, EventArgs e)
        {
            CompanyService service = new CompanyService();
            string msg = AppResource.alertWantCloseLocationBill;
            string title = AppResource.textCheckout;

            var answer = await DisplayAlert(title, msg, AppResource.textOk, AppResource.alertCancel);

            if (!answer)
                return;

            Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);

            try
            {
                var result = await service.RequestCheckoutFromClient(_company.Id, 0);
                Acr.UserDialogs.UserDialogs.Instance.Toast(AppResource.alertRequestSucess);


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


        private async void OnBackTapped(object sender, EventArgs e)
        {
            await App.AppCurrent.NavigationService.ModalGoBack();
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
                        await this.DisplayAlert(MocoApp.Resources.AppResource.alertAlert, "Você precisa informar o número de pessoas.", AppResource.textOk);
                        return;
                    }
                    else if (string.IsNullOrEmpty(txtOccupation.Text) && txtOccupation.IsVisible)
                    {
                        await this.DisplayAlert(MocoApp.Resources.AppResource.alertAlert, "Você precisa preencher " + lblOcuupation.Text + ". Por favor contate o atendente.", AppResource.textOk);
                        return;
                    }
                    result = await service.AddRemoveCheckinPost(requestCheckin);
                }





                if (result)
                {
                    await this.DisplayAlert(AppResource.textCheckin, "Checkin feito com sucesso. Aguarde a aprovação realizar pedidos.", AppResource.textOk);
                    await App.AppCurrent.NavigationService.ModalGoBack();
                }
                else
                    await this.DisplayAlert(AppResource.alertAlert, "Ocorreu um erro. Tente novamente em alguns minutos.", AppResource.textOk);




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
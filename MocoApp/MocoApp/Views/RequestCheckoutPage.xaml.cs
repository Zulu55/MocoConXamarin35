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
    public partial class RequestCheckoutPage : ContentPage
    {

        Company _company;
        Checkin _checkin;
        List<LocationCheckedInDTO> List;
        public RequestCheckoutPage(Checkin checkin, Company company)
        {
            InitializeComponent();
            _company = company;
            _checkin = checkin;
            txtOccupation.Text = checkin.Occupation;

            lblCheckOut.Text = "Solicitar Checkout " + checkin.Company.Title;

            ColorPage();
            LoadPage(checkin.Id);
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
                var result = await service.RequestCheckoutFromClient(_company.Id, 0, true);

                Acr.UserDialogs.UserDialogs.Instance.Toast(AppResource.alertRequestSucess);

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


        public async void LoadPage(string id)
        {
            OrderService service = new OrderService();

            try
            {
                var result = await service.GetLocationCheckedInDTO(id);

                List = JsonConvert.DeserializeObject<List<LocationCheckedInDTO>>(result);

                if (!string.IsNullOrEmpty(_checkin.Occupation))
                    txtOccupation.IsEnabled = false;

                LoadLocations(List);

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

       

        public void LoadLocations(List<LocationCheckedInDTO> list)
        {
            stkLocations.IsVisible = true;
            

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
                image.StyleId = item.LocationId;

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
                imgCk.Source = item.IsCheckedIn ? "ic_detail_checkout" : "ic_detail_checkin";


                grid.Children.Add(stk, 0, 0);
                grid.Children.Add(imgCk, 2, 0);

                if(item.IsCheckedIn)
                {
                    var detailsRecognizer = new TapGestureRecognizer();
                    detailsRecognizer.Tapped += OnCkGestureTapped_Tapped;
                    grid.GestureRecognizers.Add(detailsRecognizer);
                    grid.StyleId = item.CheckinSubId;
                }
                else
                {
                    var detailsRecognizer = new TapGestureRecognizer();
                    detailsRecognizer.Tapped += onMessageTapped_Tapped;
                    grid.GestureRecognizers.Add(detailsRecognizer);
                }
                


                stkLocations.Children.Add(grid);

            }
        }

        private void onMessageTapped_Tapped(object sender, EventArgs e)
        {
            Acr.UserDialogs.UserDialogs.Instance.Toast("Você precisa fazer check-out na localização atual antes de efetuar novo check-in");
        }

        private async void OnCkGestureTapped_Tapped(object sender, EventArgs e)
        {
            var item = sender as Grid;

            CompanyService service = new CompanyService();
            string msg = "Deseja realmente fechar sua conta nessa localização?";
            string title = AppResource.textCheckout;

            var answer = await DisplayAlert(title, msg, AppResource.textOk, AppResource.alertCancel);

            if (!answer)
                return;

            Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);

            try
            {
                var result = await service.RequestPayLater(item.StyleId, 0, 0, 0, false);

                Acr.UserDialogs.UserDialogs.Instance.Toast(AppResource.alertRequestSucess);
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

        public void ColorPage()
        {
            switch (_company.CompanyType)
            {
                case Models.Enums.CompanyType.Hotel:
                    this.BackgroundColor = Color.FromHex("#6dadeb");
                    lblName.TextColor = Color.FromHex("#6dadeb");
                    imgBack.Source = "ic_voltar_hotel";
                    break;
                case Models.Enums.CompanyType.Restaurante:
                    this.BackgroundColor = Color.FromHex("#fdbf2c");
                    lblName.TextColor = Color.FromHex("#fdbf2c");
                    imgBack.Source = "ic_voltar_restaurante";
                    break;
                case Models.Enums.CompanyType.Praia:
                    //this.BackgroundColor = Color.FromHex("#009145");     
                    this.BackgroundImage = "bg_praia";
                    this.BackgroundColor = Color.Transparent;
                    lblName.TextColor = Color.FromHex("#009145");
                    imgBack.Source = "ic_voltar_praia";
                    break;
                default:
                    break;
            }
        }

        private async void OnBackTapped(object sender, EventArgs e)
        {
            await App.AppCurrent.NavigationService.ModalGoBack();
        }
    }

    public class LocationCheckedInDTO
    {
        public string LocationId { get; set; }
        public string Name { get; set; }
        public string ImageUri { get; set; }
        public bool IsCheckedIn { get; set; }

        public string CheckinSubId { get; set; }
    }
}
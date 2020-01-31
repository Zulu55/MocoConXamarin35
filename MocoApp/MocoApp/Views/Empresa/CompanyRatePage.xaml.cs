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

namespace MocoApp.Views.Empresa
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CompanyRatePage : ContentPage
    {
        Company Company;
        ApiService service = new ApiService();
        int rate = 0;
        public CompanyRatePage(Company company)
        {
            InitializeComponent();
            Company = company;

            if (Device.RuntimePlatform == "Android")
                frmXaml.CornerRadius = 30;

            imgHeart.Source = company.IsFavorited ? "ic_heart_detail" : "ic_heart_detail_off";
            imgStar.Source = company.CompanyStarImageDetail;
            lblAvaliacoes.Text = "(" + company.TotalRating + " " +  AppResource.textRatings;
            lblName.Text = company.Title;
            lblAddress.Text = company.Address;
            lblPhone.Text = company.Cellphone;
            imgUser.Source = company.ImageUri;
            lblInfoDistance.Text = company.DistanceString;





            ColorPage();
            LoadPage();
            
        }

        public void ColorPage()
        {
            switch (App.CompanyTypeSelected)
            {
                case Models.Enums.CompanyType.Hotel:
                    this.BackgroundColor = (Color)App.Current.Resources["HotelColor"];
                    lblName.TextColor = (Color)App.Current.Resources["HotelColor"];
                    actPiece.Color = (Color)App.Current.Resources["HotelColor"];
                    imgBack.Source = "ic_voltar_hotel";
                    break;
                case Models.Enums.CompanyType.Restaurante:
                    this.BackgroundColor = (Color)App.Current.Resources["RestauranteColor"];
                    lblName.TextColor = (Color)App.Current.Resources["RestauranteColor"];
                    actPiece.Color = (Color)App.Current.Resources["RestauranteColor"];
                    imgBack.Source = "ic_voltar_restaurante";
                    break;
                case Models.Enums.CompanyType.Praia:
                    //this.BackgroundColor = Color.FromHex("#009145");     
                    this.BackgroundImage = "bg_praia";
                    this.BackgroundColor = Color.Transparent;
                    lblName.TextColor = (Color)App.Current.Resources["BarracaColor"];
                    actPiece.Color = (Color)App.Current.Resources["BarracaColor"];
                    imgBack.Source = "ic_voltar_praia";
                    break;
                case Enums.CompanyType.EsporteEvento:
                    this.BackgroundColor = (Color)App.Current.Resources["EsportesColor"];
                    lblName.TextColor = (Color)App.Current.Resources["EsportesColor"];
                    actPiece.Color = (Color)App.Current.Resources["EsportesColor"];
                    imgBack.Source = "ic_voltar_esportes";
                    break;
                default:
                    break;
            }
        }

        private async void OnOkTapped(object sender, EventArgs e)
        {
            try
            {
                ShowLoading(true);
                

                RatingCreate rc = new RatingCreate();

                if (rate == 0)
                {
                    throw new Exception(AppResource.alertSelectStar);
                    //throw new Exception(AppResource.alertFillRating);
                }

                rc.Rating = rate;
                rc.ClientId = Helpers.Settings.DisplayUserId;
                rc.CompanyId = Company.Id;
                rc.Message = edtMensagem.Text;

                var json = JsonConvert.SerializeObject(rc);

                var result = await service.PostAsync(json, "company/rating/create");
                Acr.UserDialogs.UserDialogs.Instance.Toast(AppResource.alertRatingSucess);

                var _company = JsonConvert.DeserializeObject<Company>(result);            

                Company.TotalRating = _company.TotalRating;
                Company.Rating = _company.Rating;  


                LoadPage();
            }
            catch (Exception ex)
            {

                this.DisplayAlert(MocoApp.Resources.AppResource.alertAlert, ex.Message, AppResource.textOk);
                ShowLoading(false);
            }
            finally
            {
                
            }
        }

        private async void OnInfoTapped(object sender, EventArgs e)
        {
            await App.AppCurrent.NavigationService.NavigateModalAsync(new CompanyInfoPage(Company), null, true);
        }

        private async void OnBackTapped(object sender, EventArgs e)
        {
            await App.AppCurrent.NavigationService.ModalGoBack();
        }

        private async void OnFavoriteTapped(object sender, EventArgs e)
        {
            //var obj = sender as Image;
            //Device.BeginInvokeOnMainThread(() =>
            //{
            //    try
            //    {
            //        obj.ScaleTo(1.3, 75).ContinueWith((t) =>
            //        {
            //            try
            //            {
            //                obj.ScaleTo(1.0, 75);
            //            }
            //            catch
            //            {
            //            }
            //        },
            //        scheduler: TaskScheduler.FromCurrentSynchronizationContext());
            //    }
            //    catch
            //    {
            //    }
            //});

            //obj.Source = "ic_heart_detail_off";
        }

        public async void LoadPage()
        {

            try
            {
                ShowLoading(true);

                var result = await service.GetAsync("company/rating/getCompanyRatingsByCompanyId?companyId=" + Company.Id);
                var list = JsonConvert.DeserializeObject<List<CompanyRating>>(result);

                if (list.Count < 1)
                    return;


                stkItens.Children.Clear();
                foreach (var item in list)
                {
                    var grid = new Grid
                    {
                        ColumnDefinitions =
                            {
                                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                            }
                    };


                    StackLayout stk = new StackLayout();
                    stk.Margin = new Thickness(0, 0, 0, 10);
                    stk.Spacing = 0;

                    ImageCircle.Forms.Plugin.Abstractions.CircleImage image = new ImageCircle.Forms.Plugin.Abstractions.CircleImage();
                    image.FillColor = Color.Gray;
                    image.HeightRequest = 50;
                    image.WidthRequest = 50;
                    image.HorizontalOptions = LayoutOptions.Center;
                    image.VerticalOptions = LayoutOptions.Center;
                    image.Aspect = Aspect.AspectFill;
                    image.Source = item.Client.Photo;
                    image.StyleId = item.Id;

                    Label lblName = new Label();
                    lblName.FontSize = 11;
                    lblName.TextColor = Color.FromHex("#212121");
                    lblName.HorizontalTextAlignment = TextAlignment.Start;
                    lblName.Text = item.Client.Name + " - " + item.CreatedAt.ToString("dd/MM/yyyy");

                    Label lblComentario = new Label();
                    lblComentario.FontSize = 10;
                    lblComentario.TextColor = Color.Silver;
                    lblComentario.HorizontalTextAlignment = TextAlignment.Start;
                    lblComentario.Text = item.Message;

                    BoxView box = new BoxView();
                    box.HeightRequest = 1;
                    box.WidthRequest = 500;
                    box.Color = Color.FromHex("#ccc");

                    stk.Children.Add(lblName);
                    stk.Children.Add(lblComentario);

                    grid.Children.Add(stk, 1, 0);
                    grid.Children.Add(image, 0, 0);

                    stkItens.Children.Add(grid);
                    stkItens.Children.Add(box);
                }
            }
            catch (Exception)
            {


            }
            finally
            {
                ShowLoading(false);
            }
            
        }

        private void ShowLoading(bool show)
        {
            actPiece.IsRunning = show;
            actPiece.IsVisible = show;
            actPiece.IsEnabled = show;
        }


        private async void OnImageUmTapped(object sender, EventArgs e)
        {
            rate = 1;
            imgStarUm.Source = "ic_star";
            imgStarDois.Source = "ic_graystar";
            imgStarTres.Source = "ic_graystar";
            imgStarQuatro.Source = "ic_graystar";
            imgStarCinco.Source = "ic_graystar";
        }
        private async void OnImageDoisTapped(object sender, EventArgs e)
        {
            rate = 2;
            imgStarUm.Source = "ic_star";
            imgStarDois.Source = "ic_star";
            imgStarTres.Source = "ic_graystar";
            imgStarQuatro.Source = "ic_graystar";
            imgStarCinco.Source = "ic_graystar";
        }
        private async void OnImageTresTapped(object sender, EventArgs e)
        {
            rate = 3;
            imgStarUm.Source = "ic_star";
            imgStarDois.Source = "ic_star";
            imgStarTres.Source = "ic_star";
            imgStarQuatro.Source = "ic_graystar";
            imgStarCinco.Source = "ic_graystar";
        }
        private async void OnImageQuatroTapped(object sender, EventArgs e)
        {
            rate = 4;
            imgStarUm.Source = "ic_star";
            imgStarDois.Source = "ic_star";
            imgStarTres.Source = "ic_star";
            imgStarQuatro.Source = "ic_star";
            imgStarCinco.Source = "ic_graystar";
        }
        private async void OnImageCincoTapped(object sender, EventArgs e)
        {
            rate = 5;
            imgStarUm.Source = "ic_star";
            imgStarDois.Source = "ic_star";
            imgStarTres.Source = "ic_star";
            imgStarQuatro.Source = "ic_star";
            imgStarCinco.Source = "ic_star";

        }
    }

    class RatingCreate
    {
        public string ClientId { get; set; }
        public int Rating { get; set; }
        public string CompanyId { get; set; }
        public string Message { get; set; }
    }

}
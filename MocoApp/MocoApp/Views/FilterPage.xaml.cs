using MocoApp.Views.Empresa;
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
    public partial class FilterPage : ContentPage
    {
        public FilterPage()
        {
            InitializeComponent();
            if (Device.RuntimePlatform == "Android")
                frmXaml.CornerRadius = 30;


            ColorPage();

        }

        public void ColorPage()
        {
            switch (App.CompanyTypeSelected)
            {
                case Models.Enums.CompanyType.Hotel:
                    this.BackgroundColor = (Color)App.Current.Resources["HotelColor"];
                    imgBack.Source = "ic_voltar_hotel";
                    break;
                case Models.Enums.CompanyType.Restaurante:
                    this.BackgroundColor = (Color)App.Current.Resources["RestauranteColor"];
                    imgBack.Source = "ic_voltar_restaurante";
                    break;
                case Models.Enums.CompanyType.Praia:
                    //this.BackgroundColor = Color.FromHex("#009145");     
                    this.BackgroundImage = "bg_praia";
                    this.BackgroundColor = Color.Transparent;
                    imgBack.Source = "ic_voltar_praia";
                    break;
                case Models.Enums.CompanyType.EsporteEvento:
                    this.BackgroundColor = (Color)App.Current.Resources["EsportesColor"];
                    imgBack.Source = "ic_voltar_esportes";
                    break;
                default:
                    break;
            }

            switch (App.FilterType)
            {
                case Models.Enums.FilterType.Proximidade:
                    OnProximidadeTapped(null, null);
                    break;
                case Models.Enums.FilterType.OrderByName:
                    OnNameTapped(null, null);
                    break;
                case Models.Enums.FilterType.Favorites:
                    OnFavoritesTapped(null, null);
                    break;
                case Models.Enums.FilterType.Ratings:
                    OnRatingsTapped(null, null);
                    break;
                default:
                    break;
            }
        }

        private async void OnOkTapped(object sender, EventArgs e)
        {
            App.AppCurrent.NavigationService.ModalGoBack();
            await App.AppCurrent.NavigationService.NavigateSetRootAsync(new CompanyListPage(), null, false);
        }

        private void OnRatingsTapped(object sender, EventArgs e)
        {
            imgFavorites.Source = "ic_uncheck";
            imgProximidade.Source = "ic_uncheck";
            imgName.Source = "ic_uncheck";
            imgRatings.Source = "ic_check";            
            App.FilterType = Models.Enums.FilterType.Ratings;

        }

        private void OnFavoritesTapped(object sender, EventArgs e)
        {
            imgFavorites.Source = "ic_check";
            imgProximidade.Source = "ic_uncheck";
            imgRatings.Source = "ic_uncheck";
            imgName.Source = "ic_uncheck";
            App.FilterType = Models.Enums.FilterType.Favorites;
        }

        private void OnProximidadeTapped(object sender, EventArgs e)
        {
            imgFavorites.Source = "ic_uncheck";
            imgProximidade.Source = "ic_check";
            imgRatings.Source = "ic_uncheck";
            imgName.Source = "ic_uncheck";
            App.FilterType = Models.Enums.FilterType.Proximidade;
        }

        private void OnNameTapped(object sender, EventArgs e)
        {
            imgFavorites.Source = "ic_uncheck";
            imgProximidade.Source = "ic_uncheck";
            imgRatings.Source = "ic_uncheck";
            imgName.Source = "ic_check";
            App.FilterType = Models.Enums.FilterType.OrderByName;
        }

        private async void OnBackTapped(object sender, EventArgs e)
        {
            await App.AppCurrent.NavigationService.ModalGoBack();
        }
    }
}
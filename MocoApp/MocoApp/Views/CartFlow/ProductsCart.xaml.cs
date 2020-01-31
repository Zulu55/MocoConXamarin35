using MocoApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MocoApp.Views.CartFlow
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProductsCart : ContentPage
    {
        Random r = new Random();

        public ProductsCart()
        {
            InitializeComponent();
            if (Device.RuntimePlatform == "Android")
                frmXaml.CornerRadius = 30;

            BindingContext = App.AppCurrent.Cart;
            ColorPage();
            //var product = new Product()
            //{
            //    ImageUri = "img_restaurantes",
            //    Name = "Product 1",
            //    Price = 30
            //};

            //for (int i = 0; i < 5; i++)
            //{
            //    App.AppCurrent.Cart.AddOrder(new CreateOrder()
            //    {
            //        Product = product,
            //        ProductQuantity = r.Next(1, 10)
            //    }
            //   );
            //}
        }

        private async void OnContinue_Tapped(object sender, EventArgs e)
        {
            if (App.AppCurrent.Cart.TotalOrdersInCart > 0)
                await App.AppCurrent.NavigationService.NavigateModalAsync(new CheckoutCartPage(), null, true);
            else
                await DisplayAlert("Ops", "You need add orders to continue.", "Ok");
        }


        private async void OnEmptyCart_Clicked(object sender, EventArgs e)
        {
            if (App.AppCurrent.Cart.TotalOrdersInCart > 0)
            {
                var action = await DisplayAlert("Empty Cart", "Do you really want to empty your cart?", "Yes", "No");

                if (action)
                {
                    App.AppCurrent.Cart.ClearOrders();
                }
            }
                
        }

        public void ColorPage()
        {
            switch (App.CompanyTypeSelected)
            {
                case Models.Enums.CompanyType.Hotel:
                    this.BackgroundColor = (Color)App.Current.Resources["HotelColor"];
                    lblLocationName.TextColor = (Color)App.Current.Resources["HotelColor"];
                    imgBack.Source = "ic_voltar_hotel";
                    break;
                case Models.Enums.CompanyType.Restaurante:
                    this.BackgroundColor = (Color)App.Current.Resources["RestauranteColor"];

                    lblLocationName.TextColor = (Color)App.Current.Resources["RestauranteColor"];
                    imgBack.Source = "ic_voltar_restaurante";
                    break;
                case Models.Enums.CompanyType.Praia:
                    this.BackgroundImage = "bg_praia";
                    this.BackgroundColor = Color.Transparent;
                    lblLocationName.TextColor = (Color)App.Current.Resources["BarracaColor"];
                    imgBack.Source = "ic_voltar_praia";
                    break;
                case Models.Enums.CompanyType.EsporteEvento:
                    this.BackgroundColor = (Color)App.Current.Resources["EsportesColor"];
                    lblLocationName.TextColor = (Color)App.Current.Resources["EsportesColor"];
                    imgBack.Source = "ic_voltar_esportes";
                    break;
                default:
                    break;
            }
        }

       

        private async void OnContOnBackinue_Tapped(object sender, EventArgs e)
        {
            await App.AppCurrent.NavigationService.ModalGoBack();
        }
    }
}
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
    public partial class CarouselImagePage : ContentPage
    {
        public CarouselImagePage(List<string> uris)
        {
            InitializeComponent();
            //Thickness padding;

            //this.Title = "Galeria";

            //switch (Device.RuntimePlatform)
            //{
            //    case Device.iOS:
            //    case Device.Android:
            //        padding = new Thickness(0, 0, 0, 0);
            //        break;
            //    default:
            //        padding = new Thickness();
            //        break;
            //}

            //foreach (var item in uris)
            //{
            //    var detailsRecognizer = new TapGestureRecognizer();
            //    detailsRecognizer.Tapped += DetailsRecognizer_Tapped;
            //    var img = new Image();
            //    img.Source = "ic_voltar";
            //    img.WidthRequest = 24;
            //    img.HorizontalOptions = LayoutOptions.Start;
            //    img.VerticalOptions = LayoutOptions.Start;
            //    img.Margin = new Thickness(10, 5, 5, 5);

            //    var lbl = new Label();
            //    lbl.Text = "Voltar";
            //    lbl.FontSize = 18;
            //    lbl.TextColor = Color.Gray;
            //    lbl.VerticalTextAlignment = TextAlignment.Center;

            //    var stkHor = new StackLayout();
            //    stkHor.Orientation = StackOrientation.Horizontal;

            //    stkHor.Children.Add(img);
            //    stkHor.Children.Add(lbl);

            //    var imgPhoto = new Image();
            //    imgPhoto.Source = item;
            //    imgPhoto.HorizontalOptions = LayoutOptions.CenterAndExpand;
            //    imgPhoto.VerticalOptions = LayoutOptions.CenterAndExpand;

            //    img.GestureRecognizers.Add(detailsRecognizer);

            //    var content = new ContentPage();                
            //    var stk = new StackLayout();
            //    stk.Children.Add(stkHor);
            //    stk.Children.Add(imgPhoto);
            //    content.Content = stk;

            //    Children.Add(content);
            //}

            //Acr.UserDialogs.UserDialogs.Instance.Toast("Puxe para o lado para ver mais fotos.");

            
            
        }

        

        private async void DetailsRecognizer_Tapped(object sender, EventArgs e)
        {
            await App.AppCurrent.NavigationService.ModalGoBack();

        }
    }
}
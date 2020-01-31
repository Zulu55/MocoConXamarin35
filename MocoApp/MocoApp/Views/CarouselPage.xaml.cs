using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MocoApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CarouselPage : ContentPage
    {
        public CarouselPage(List<string> listUri)
        {
            InitializeComponent();

            List<ImgGaleria> list = new List<ImgGaleria>();

            var count = 1;
            foreach (var item in listUri)
            {
                list.Add(new ImgGaleria() { ImageUri = item, Position = "carousel" + count.ToString() });
                count++;
            }
        }

        private async void OnBackTapped(object sender, EventArgs e)
        {
            await App.AppCurrent.NavigationService.ModalGoBack();
        }


        class ImgGaleria
        {
            public string ImageUri { get; set; }

            public string Position { get; set; }
        }
            
    }
}
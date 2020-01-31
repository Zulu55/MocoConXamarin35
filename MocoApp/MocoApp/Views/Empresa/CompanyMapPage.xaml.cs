using MocoApp.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace MocoApp.Views.Empresa
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CompanyMapPage : ContentPage
    {
        public CompanyMapPage(Company company)
        {
            InitializeComponent();

            var lat = company.Latitude;
            var log = company.Longitude;


            var a = double.Parse(lat, new CultureInfo("en-US"));
            var b = double.Parse(log, new CultureInfo("en-US"));

            MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(a, b), Distance.FromMeters(400)));

            Pin pin = new Pin();
            pin.Address = company.Address;
            pin.Label = company.Title;



            var position = new Position(a, b);

            pin.Position = position;

            MyMap.Pins.Add(pin);
        }

        private void OnBackTapped(object sender, EventArgs e)
        {
            App.AppCurrent.NavigationService.ModalGoBack();
        }
    }
}
using MocoApp.Resources;
using MocoApp.Views.Empresa;
using Plugin.Geolocator;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MocoApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            if(Device.RuntimePlatform == "Android")
                frmXaml.CornerRadius = 30;


            var a = AppResource.lblHotels;
            var b = AppResource.LblEnterAsGuest;
        }

        private async void OnMenuTapped(object sender, EventArgs e)
        {
            var obj = sender as Image;
            Device.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    obj.ScaleTo(1.4, 75).ContinueWith((t) =>
                    {
                        try
                        {
                            obj.ScaleTo(1.0, 75);
                        }
                        catch
                        {
                        }
                    },
                    scheduler: TaskScheduler.FromCurrentSynchronizationContext());
                }
                catch
                {
                }
            });


            App app = Application.Current as App;
            Naylah.Xamarin.Controls.Pages.MasterDetailNavigationPage md = (Naylah.Xamarin.Controls.Pages.MasterDetailNavigationPage)app.MainPage;
            md.IsPresented = true;

        }
        
        private async void OnBarracasTapped(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(App.AppCurrent.Latitude))
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertTakingLocation);
                try
                {
                    var locator = CrossGeolocator.Current;

                    if (!locator.IsGeolocationEnabled)
                        throw new Exception();

                    if (locator.IsGeolocationAvailable && locator.IsGeolocationEnabled)
                    {
                        locator.DesiredAccuracy = 50;
                        var position = await locator.GetPositionAsync(1000);

                        App.AppCurrent.Latitude = position.Latitude.ToString().Replace(',', '.');
                        App.AppCurrent.Longitude = position.Longitude.ToString().Replace(',', '.');

                    }
                    else
                    {
                        //throw new Exception("Você precisa ativar seu GPS para dar continuidade. Verifique nas configurações.");

                    }
                }
                catch (Exception ex)
                {
                    App.AppCurrent.Latitude = "";
                    App.AppCurrent.Longitude = "";
                }
            }
            App.CompanyTypeSelected = Models.Enums.CompanyType.Praia;
            await App.AppCurrent.NavigationService.NavigateSetRootAsync(new CompanyListPage(), null, true);
        }

        private async void OnHotelTapped(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(App.AppCurrent.Latitude))
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertTakingLocation);
                try
                {
                    var locator = CrossGeolocator.Current;

                    if (!locator.IsGeolocationEnabled)
                        throw new Exception();

                    if (locator.IsGeolocationAvailable && locator.IsGeolocationEnabled)
                    {
                        locator.DesiredAccuracy = 50;
                        var position = await locator.GetPositionAsync(1000);

                        App.AppCurrent.Latitude = position.Latitude.ToString().Replace(',', '.');
                        App.AppCurrent.Longitude = position.Longitude.ToString().Replace(',', '.');

                    }
                    else
                    {
                        //throw new Exception("Você precisa ativar seu GPS para dar continuidade. Verifique nas configurações.");

                    }
                }
                catch (Exception ex)
                {
                    App.AppCurrent.Latitude = "";
                    App.AppCurrent.Longitude = "";
                }
            }
            App.CompanyTypeSelected = Models.Enums.CompanyType.Hotel;
            await App.AppCurrent.NavigationService.NavigateSetRootAsync(new CompanyListPage(), null, true);
        }

        private async void OnRestauranteTapped(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(App.AppCurrent.Latitude))
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertTakingLocation);
                try
                {
                    var locator = CrossGeolocator.Current;

                    if (!locator.IsGeolocationEnabled)
                        throw new Exception();

                    if (locator.IsGeolocationAvailable && locator.IsGeolocationEnabled)
                    {
                        locator.DesiredAccuracy = 50;
                        var position = await locator.GetPositionAsync(1000);

                        App.AppCurrent.Latitude = position.Latitude.ToString().Replace(',', '.');
                        App.AppCurrent.Longitude = position.Longitude.ToString().Replace(',', '.');

                    }
                    else
                    {
                        //throw new Exception("Você precisa ativar seu GPS para dar continuidade. Verifique nas configurações.");

                    }
                }
                catch (Exception ex)
                {
                    App.AppCurrent.Latitude = "";
                    App.AppCurrent.Longitude = "";
                }
            }
            App.CompanyTypeSelected = Models.Enums.CompanyType.Restaurante;
            await App.AppCurrent.NavigationService.NavigateSetRootAsync(new CompanyListPage(), null, true);
        }

        private async void OnEsportesEventosTapped(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(App.AppCurrent.Latitude))
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertTakingLocation);
                try
                {
                    var locator = CrossGeolocator.Current;

                    if (!locator.IsGeolocationEnabled)
                        throw new Exception();

                    if (locator.IsGeolocationAvailable && locator.IsGeolocationEnabled)
                    {
                        locator.DesiredAccuracy = 50;
                        var position = await locator.GetPositionAsync(1000);

                        App.AppCurrent.Latitude = position.Latitude.ToString().Replace(',', '.');
                        App.AppCurrent.Longitude = position.Longitude.ToString().Replace(',', '.');

                    }
                    else
                    {
                        //throw new Exception("Você precisa ativar seu GPS para dar continuidade. Verifique nas configurações.");

                    }
                }
                catch (Exception ex)
                {
                    App.AppCurrent.Latitude = "";
                    App.AppCurrent.Longitude = "";
                }
            }
            App.CompanyTypeSelected = Models.Enums.CompanyType.EsporteEvento;
            await App.AppCurrent.NavigationService.NavigateSetRootAsync(new CompanyListPage(), null, true);
        }
    }
}
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

namespace MocoApp.Views.CompanyFluxo
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LocationListPage : ContentPage
    {
        private List<Location> locations;
        public LocationListPage()
        {
            InitializeComponent();


            //variavel para passar para outra tela
            App.AppCurrent.LocationId = "";

            if (Device.OS == TargetPlatform.iOS)
            {
                // move layout under the status bar
                this.Padding = new Thickness(0, 0, 0, 0);

                var menu = new ToolbarItem("Menu", "", () =>
                {
                    App app = Application.Current as App;
                    Naylah.Xamarin.Controls.Pages.MasterDetailNavigationPage md = (Naylah.Xamarin.Controls.Pages.MasterDetailNavigationPage)app.MainPage;

                    md.IsPresented = true;

                }, 0, 0);
                menu.Priority = 0;

                ToolbarItems.Add(menu);
            }



            var addNew = new ToolbarItem(AppResource.lblNew, "", async () =>
            {
                await App.AppCurrent.NavigationService.NavigateAsync(new CreateEditLocationPage(), null, false);
            }, 0, 0);
            addNew.Priority = 1;
            ToolbarItems.Add(addNew);

            listView.ItemTapped += ListView_ItemTapped;


            this.Title = AppResource.lblLocation;
        }

        public async void LoadLocations()
        {
            CompanyService companyService = new CompanyService();

            try
            {
                listView.ItemsSource = null;
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);


                var result = await companyService.GetAllLocationsByCompanyId();
                var list = JsonConvert.DeserializeObject<List<Location>>(result);
                locations = list;
                foreach (var item in list)
                    item.Name += $" ({AppResource.lblSeeDetails})";

                listView.ItemsSource = list;

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

        protected override void OnAppearing()
        {
            base.OnAppearing();



            LoadLocations();
        }

        private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (listView.SelectedItem == null)
                return;

            var item = listView.SelectedItem as Location;
            App.AppCurrent.LocationId = item.Id;
            App.AppCurrent.LocationName = item.Name.Replace($"({AppResource.lblSeeDetails})", "");
            var me = locations.FirstOrDefault(x => x.Id == item.Id);
            //if (item.MenuType == Enums.EMenuType.Category)
            await App.AppCurrent.NavigationService.NavigateAsync(new CategoryListPage(me.MenuType), null, false);
            //else
            //    await App.AppCurrent.NavigationService.NavigateAsync(new InformativeMenuListPage(), null, false);

            listView.SelectedItem = null;
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            //await App.AppCurrent.NavigationService.NavigateAsync(new CategoryListPage(), null, false);
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var item = sender as Button;

            var _location = item.BindingContext as Location;
            _location.Name = _location.Name.Replace($"({AppResource.lblSeeDetails})", "");
            await App.AppCurrent.NavigationService.NavigateAsync(new CreateEditLocationPage(_location), null, false);
        }



        private void Button_Clicked_2(object sender, EventArgs e)
        {

        }

        private void OnImageTapped(object sender, EventArgs e)
        {

        }

        private void OnItemTapped(object sender, EventArgs e)
        {

        }
    }
}
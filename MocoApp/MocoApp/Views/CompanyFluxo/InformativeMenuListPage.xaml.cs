﻿using ImageCircle.Forms.Plugin.Abstractions;
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
    public partial class InformativeMenuListPage : ContentPage
    {
        private readonly string _locationId = "";
        private CategoryGroup _group;
        public InformativeMenuListPage(CategoryGroup group = null)
        {
            InitializeComponent();

            _locationId = App.AppCurrent.LocationId;
            _group = group;


            if (string.IsNullOrEmpty(_locationId))
            {
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
            }

            var addNew = new ToolbarItem(AppResource.lblNew, "", async () =>
            {
                await App.AppCurrent.NavigationService.NavigateAsync(new CreateEditInformativeMenuPage(null, _locationId), null, false);
            }, 0, 0);
            addNew.Priority = 1;
            ToolbarItems.Add(addNew);

            if (string.IsNullOrEmpty(App.AppCurrent.LocationId))
                this.Title = AppResource.lblMenu;
            else
                this.Title = App.AppCurrent.LocationName;

        }


        protected override void OnAppearing()
        {
            base.OnAppearing();

            listView.ItemsSource = null;
            LoadCategories();
            listView.ItemTapped += ListView_ItemTapped;
            //listView.ItemsSource = ListProducts;


        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();


            listView.ItemTapped -= ListView_ItemTapped;
        }



        public async void LoadCategories()
        {
            CompanyService companyService = new CompanyService();

            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);


                var result = await companyService.GetAllInformativeMenus(_locationId, true);
                var list = JsonConvert.DeserializeObject<List<InformativeMenu>>(result);

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

        private async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (listView.SelectedItem == null)
                return;

            var lbl = sender as StackLayout;
            var item = lbl.BindingContext as InformativeMenu;

            //await App.AppCurrent.NavigationService.NavigateAsync(new GroupListPage(item.Id, item.Name), null, false);

            listView.SelectedItem = null;
        }

        private async void OnImageTapped(object sender, ItemTappedEventArgs e)
        {
            if (listView.SelectedItem == null)
                return;

            var img = sender as CircleImage;
            var item = img.BindingContext as InformativeMenu;

            //await App.AppCurrent.NavigationService.NavigateAsync(new GroupListPage(item.Id, item.Name), null, false);

            listView.SelectedItem = null;
        }

        private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (listView.SelectedItem == null)
                return;

            var item = listView.SelectedItem as InformativeMenu;

            await App.AppCurrent.NavigationService.NavigateAsync(new CreateEditInformativeMenuPage(item, _locationId), null, false);

            listView.SelectedItem = null;
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var item = sender as Button;

            var productCategory = item.BindingContext as InformativeMenu;
            productCategory.Name = productCategory.Name.Replace($"({AppResource.lblSeeDetails})", "");

            await App.AppCurrent.NavigationService.NavigateAsync(new CreateEditInformativeMenuPage(productCategory, _locationId), null, false);
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            await App.AppCurrent.NavigationService.NavigateAsync(new CreateEditInformativeMenuPage(null, _locationId), null, false);
        }

        private void OnImageTapped(object sender, EventArgs e)
        {

        }
    }
}
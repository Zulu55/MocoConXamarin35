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
    public partial class ProductListPage : ContentPage
    {
        List<Product> ListProducts;
        CategoryGroup Group;
        public ProductListPage(CategoryGroup _group)
        {
            InitializeComponent();

            this.Title = _group.Name;

            if(_group.MenuType == Enums.EMenuType.Informative)
            {
                var addNew = new ToolbarItem(AppResource.lblNew, "", async () =>
                {
                    await App.AppCurrent.NavigationService.NavigateAsync(new CreateEditInformativeMenuPage(_group), null, false);
                }, 0, 0);
                addNew.Priority = 1;
                ToolbarItems.Add(addNew);
            }
            else
            {
                var addNew = new ToolbarItem(AppResource.lblNew, "", async () =>
                {
                    await App.AppCurrent.NavigationService.NavigateAsync(new CreateEditProductPage(Group), null, false);
                }, 0, 0);
                addNew.Priority = 1;
                ToolbarItems.Add(addNew);
            }

            Group = _group;


            listView.ItemTapped += ListView_ItemTapped;
          
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            listView.ItemsSource = null;
            LoadProducts();
            //listView.ItemTapped += ListView_ItemTapped;
            //listView.ItemsSource = ListProducts;


        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            //listView.ItemsSource = null;
            //listView.ItemTapped -= ListView_ItemTapped;
        }


        private async void Button_Clicked(object sender, EventArgs e)
        {
            if(Group.MenuType != Enums.EMenuType.Informative)
                await App.AppCurrent.NavigationService.NavigateAsync(new CreateEditProductPage(Group), null, false);
            else
                await App.AppCurrent.NavigationService.NavigateAsync(new InformativeMenuListPage(Group), null, false);

        }

        public async void LoadProducts()
        {
            CompanyService companyService = new CompanyService();

            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);

                var result = await companyService.GetAllProductsByCategoryAndCompanyId(Group.Id, true);
                ListProducts = JsonConvert.DeserializeObject<List<Product>>(result);

                if (ListProducts.Count > 0)
                {
                    listView.ItemsSource = ListProducts;
                    lblEmpty.IsVisible = false;
                }
                else
                    lblEmpty.IsVisible = true;

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

        private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (listView.SelectedItem == null)
                return;

            var item = listView.SelectedItem as Product;
            if(string.IsNullOrEmpty(item.InformativeMenuId))
            {
                await App.AppCurrent.NavigationService.NavigateAsync(new CreateEditProductPage(Group, item), null, false);
            }
            else
            {
                await App.AppCurrent.NavigationService.NavigateAsync(new CreateEditInformativeMenuPage(Group, item.InformativeMenuId), null, false);
            }

            listView.SelectedItem = null;
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {

        }
    }
}
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
    public partial class AddOrderToClientPage : ContentPage
    {
        List<Product> ListProducts;
        List<ProductCategory> ListCategories;
        ProductCategory ProductCategory;
        Checkin _checkin;
        string _locationId = "";
        CheckinSub _checkinSub;
        public AddOrderToClientPage(Checkin checkin, string locationId = "", CheckinSub checkinSub  = null)
        {
            InitializeComponent();

            _locationId = locationId;
            if (checkinSub != null)
                _locationId = checkinSub.LocationId;
            _checkinSub = checkinSub;
            _checkin = checkin;
            LoadCategories();

            listView.ItemTapped += ListView_ItemTapped;
        }
      
        public async void LoadProducts()
        {
            CompanyService companyService = new CompanyService();
            listView.ItemsSource = null;

            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);

                var result = await companyService.GetAllProductsByCategoryId(ProductCategory.Id, false);
                ListProducts = JsonConvert.DeserializeObject<List<Product>>(result);

                if (ListProducts.Count > 0)
                {
                    listView.ItemsSource = ListProducts;                    
                }     

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

            await App.AppCurrent.NavigationService.NavigateAsync(new OrderToClientPage(_checkin, item, _checkinSub), null, false);

            listView.SelectedItem = null;
        }

        public async void LoadCategories()
        {
            CompanyService companyService = new CompanyService();

            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);


                var result = await companyService.GetAllProductCategoryByLocationId(_locationId, false);
                var list = JsonConvert.DeserializeObject<List<ProductCategory>>(result);

                ListCategories = list;
                foreach (var item in list)
                {
                    pckCategory.Items.Add(item.Name);
                }

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

        private void pckCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = sender as Picker;

            if (item.SelectedIndex < 0)
                return;

            ProductCategory = ListCategories.Where(m => m.Name == item.Items[item.SelectedIndex]).FirstOrDefault();

            LoadProducts();
        }
    }
}
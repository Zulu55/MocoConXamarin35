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
    public partial class ReportProductPage : ContentPage
    {
        List<Product> ListProducts;
        List<ProductCategory> ListCategories;
        List<Location> ListLocations;
        ProductCategory ProductCategory;
        Product Product;
        private string categoryId;
        private Location Location;
        public ReportProductPage()
        {
            InitializeComponent();

            Product = new Product();
            Location = new Location();

            categoryId = "";

            dtInit.Date = DateTime.Now.AddDays(-7);
            dtEnd.Date = DateTime.Now;

            listView.ItemTapped += ListView_ItemTapped;
            LoadLocations();
            LoadCategories();
            LoadProducts();

            //if (Device.OS == TargetPlatform.iOS)
            //{
            //    // move layout under the status bar
            //    this.Padding = new Thickness(0, 0, 0, 0);

            //    var menu = new ToolbarItem("Menu", "", () =>
            //    {
            //        App app = Application.Current as App;
            //        Naylah.Xamarin.Controls.Pages.MasterDetailNavigationPage md = (Naylah.Xamarin.Controls.Pages.MasterDetailNavigationPage)app.MainPage;

            //        md.IsPresented = true;

            //    }, 0, 0);
            //    menu.Priority = 0;

            //    ToolbarItems.Add(menu);
            //}
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            listView.SelectedItem = null;
        }

        public async void LoadLocations()
        {
            CompanyService companyService = new CompanyService();

            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);


                var result = await companyService.GetAllLocationsByCompanyId();
                var list = JsonConvert.DeserializeObject<List<Location>>(result);

                ListLocations = list;
                pckLocations.Items.Add(AppResource.txtAll);
                foreach (var item in list)
                {
                    pckLocations.Items.Add(item.Name);
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


        public async void LoadCategories()
        {
            CompanyService companyService = new CompanyService();

            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);


                var result = await companyService.GetProductCategory();
                var list = JsonConvert.DeserializeObject<List<ProductCategory>>(result);

                ListCategories = list;
                pckCategoria.Items.Add(AppResource.txtAll);
                foreach (var item in list)
                {
                    pckCategoria.Items.Add(item.Name);
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

        private void pckCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            Product = new Product();

            var item = sender as Picker;

            if (item.SelectedIndex < 0)
                return;

            var selected = item.Items[item.SelectedIndex];
            if (selected == AppResource.txtAll)
            {
                ProductCategory = new ProductCategory();
                Product = new Product();
            }
            else
                ProductCategory = ListCategories.Where(m => m.Name == selected).FirstOrDefault();

            categoryId = ProductCategory.Id;

            pckProduto.Items.Clear();
            LoadProducts();
        }

        public async void LoadProducts()
        {
            CompanyService companyService = new CompanyService();


            try
            {
                pckProduto.Items.Clear();
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);
                if (string.IsNullOrEmpty(categoryId) && Location != null && !string.IsNullOrEmpty(Location.Id))
                {
                    var result = await companyService.GetProductsByLocationId(Location.Id);
                    ListProducts = JsonConvert.DeserializeObject<List<Product>>(result);

                    pckProduto.Items.Add(AppResource.txtAll);
                    foreach (var item in ListProducts)
                    {
                        pckProduto.Items.Add(item.Name);
                    }
                }
                else if (string.IsNullOrEmpty(categoryId))
                {
                    var result = await companyService.GetProductsByCompanyId(Helpers.Settings.DisplayUserCompany);
                    ListProducts = JsonConvert.DeserializeObject<List<Product>>(result);

                    pckProduto.Items.Add(AppResource.txtAll);
                    foreach (var item in ListProducts)
                    {
                        pckProduto.Items.Add(item.Name);
                    }
                }
                else
                {
                    var result = await companyService.GetProductsByCategoryAndCompanyId(ProductCategory.Id, Helpers.Settings.DisplayUserCompany);
                    ListProducts = JsonConvert.DeserializeObject<List<Product>>(result);

                    pckProduto.Items.Add(AppResource.txtAll);
                    foreach (var item in ListProducts)
                    {
                        pckProduto.Items.Add(item.Name);
                    }
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

        private void pckProduto_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = sender as Picker;

            if (item.SelectedIndex < 0)
                return;

            var selected = item.Items[item.SelectedIndex];
            if (selected == AppResource.txtAll)
                Product = new Product();
            else
                Product = ListProducts.Where(m => m.Name == item.Items[item.SelectedIndex]).FirstOrDefault();


        }

        private async void btnFiltrar_Clicked(object sender, EventArgs e)
        {
            CompanyService companyService = new CompanyService();


            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);
                listView.ItemsSource = null;


                if (Location == null)
                    Location = new Location();

                if (Product == null)
                    Product = new Product();



                //var result = await companyService.GetProductReport(dtInit.Date.ToString("dd/MM/yyyy HH:mm"), dtEnd.Date.ToString("dd/MM/yyyy HH:mm"), Product.Id);
                var result = await companyService.GetProductReport(dtInit.Date, dtEnd.Date, Product.Id, categoryId, Location.Id);

                var list = JsonConvert.DeserializeObject<List<ProductReport>>(result);

                if (list.Count > 0)
                {
                    lblEmpty.IsVisible = false;
                    listView.ItemsSource = list;
                }
                else
                {
                    lblEmpty.IsVisible = true;
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

        private void pckLocations_SelectedIndexChanged(object sender, EventArgs e)
        {
            categoryId = null;
            Product = new Product();

            var item = sender as Picker;

            if (item.SelectedIndex < 0)
                return;

            var currentCategories = ListCategories.Where(x => x.Location != null && x.Location.Name == item.Items[item.SelectedIndex]).ToList();

            pckCategoria.Items.Clear();

            foreach (var cat in currentCategories)
                pckCategoria.Items.Add(cat.Name);

            Location = ListLocations.Where(m => m.Name == item.Items[item.SelectedIndex]).FirstOrDefault();

            LoadProducts();
        }

    }

    class ProductReport
    {
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public int QuantityLeft { get; set; }
        public int QuantitySold { get; set; }

        public string SoldStr { get { return string.Format("{0} {1}", QuantitySold, AppResource.lblSold); } }
        public string LeftStr { get { return string.Format("{0} {1}", QuantityLeft, AppResource.lblStoque); } }
    }
}
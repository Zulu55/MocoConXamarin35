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
using static MocoApp.Models.Enums;

namespace MocoApp.Views.CompanyFluxo
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GroupListPage : ContentPage
    {
        string CategoryId = "";
        EMenuType _type;
        public GroupListPage(string categoryId, string categoryName, EMenuType type)
        {
            InitializeComponent();

            CategoryId = categoryId;
            _type = type;
            var addNew = new ToolbarItem(AppResource.lblNew, "", async () =>
            {

                await App.AppCurrent.NavigationService.NavigateAsync(new CreateEditGroupPage(null, categoryId, _type), null, false);

            }, 0, 0);
            addNew.Priority = 1;
            ToolbarItems.Add(addNew);

            this.Title = categoryName;

            listView.ItemTapped += ListView_ItemTapped;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            listView.ItemsSource = null;
            LoadGroups();
            //listView.ItemTapped += ListView_ItemTapped;
            //listView.ItemsSource = ListProducts;
        }

        public async void LoadGroups()
        {
            CompanyService companyService = new CompanyService();

            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);


                var result = await companyService.GetAllCategoryGroupByCategoryId(CategoryId, true);
                var list = JsonConvert.DeserializeObject<List<CategoryGroup>>(result);
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

        private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (listView.SelectedItem == null)
                return;

            var item = listView.SelectedItem as CategoryGroup;
            item.Name = item.Name.Replace($"({AppResource.lblSeeDetails})", "");
            await App.AppCurrent.NavigationService.NavigateAsync(new ProductListPage(item), null, false);

            listView.SelectedItem = null;

        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var item = sender as Button;

            var group = item.BindingContext as CategoryGroup;
            group.Name = group.Name.Replace($"({AppResource.lblSeeDetails})", "");
            await App.AppCurrent.NavigationService.NavigateAsync(new CreateEditGroupPage(group, CategoryId, _type), null, false);
        }

     
    }
}
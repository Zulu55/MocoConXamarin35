using MocoApp.Extensions;
using MocoApp.Models;
using MocoApp.Resources;
using MocoApp.Services;
using MocoApp.Services.V2;
using MocoApp.Views.ManagerCheckinFlow;
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
    public partial class ListCompanyPage : ContentPage
    {
        private readonly CompanyV2Service _companyService;
        Company CompanySelected;
        public ListCompanyPage()
        {
            InitializeComponent();

            LoadCompanies();

            listView.ItemTapped += ListView_ItemTapped;

            _companyService = new CompanyV2Service();
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

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (App.AppCurrent.FirstLogin)
            {
                App app = Application.Current as App;
                Naylah.Xamarin.Controls.Pages.MasterDetailNavigationPage md = (Naylah.Xamarin.Controls.Pages.MasterDetailNavigationPage)app.MainPage;
                md.IsPresented = true;
            }

            App.AppCurrent.FirstLogin = false;
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (listView.SelectedItem == null)
                return;

            CompanySelected = listView.SelectedItem as Company;
            btnAdd.IsEnabled = true;
        }

        public async void LoadCompanies()
        {
            ApiService apiService = new ApiService();

            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);

                System.Diagnostics.Debug.WriteLine(Helpers.Settings.DisplayUserId);
                      var result = await apiService.GetAsync("company/getcompaniesbymanagerid?id=" + Helpers.Settings.DisplayUserId);
                var list = JsonConvert.DeserializeObject<List<Company>>(result);

                listView.ItemsSource = list;

                if (string.IsNullOrEmpty(Helpers.Settings.DisplayUserCompany) && list.Count > 0)
                {
                    Helpers.Settings.DisplayUserCompany = list.FirstOrDefault().Id;
                    Helpers.Settings.DisplayHasLocation = list.FirstOrDefault().HasLocation;
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

        private async void btnAdd_Clicked(object sender, EventArgs e)
        {
            if (CompanySelected != null)
            {
                Helpers.Settings.DisplayUserCompany = CompanySelected.Id;
                Helpers.Settings.DisplayHasLocation = CompanySelected.HasLocation;
                App.AppCurrent.CompanyCulture = CompanySelected.CurrencyType.ToCultureInfo();

                await _companyService.UpdateCurrentCompany(CompanySelected.Id);

                await App.AppCurrent.ConfigureAppPhase();

                await App.AppCurrent.NavigationService.NavigateSetRootAsync(new ClientListPage(), null, true);
            }
            else
                await this.DisplayAlert(MocoApp.Resources.AppResource.alertAlert, AppResource.alertSelectCompany, AppResource.textOk);

            
        }
    }
}
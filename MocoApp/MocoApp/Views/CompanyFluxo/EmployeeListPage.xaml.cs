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
    public partial class EmployeeListPage : ContentPage
    {

        List<Employee> ListEmployee;
        public EmployeeListPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadEmployees();

            listView.ItemTapped += ListView_ItemTapped;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            listView.ItemTapped -= ListView_ItemTapped;
        }

        public async void LoadEmployees()
        {
            CompanyService companyService = new CompanyService();

            listView.ItemsSource = null;

            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);


                var result = await companyService.GetEmployees();
                ListEmployee = JsonConvert.DeserializeObject<List<Employee>>(result);

                listView.ItemsSource = ListEmployee;

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

            var item = listView.SelectedItem as Employee;

            await App.AppCurrent.NavigationService.NavigateAsync(new EditEmployeePage(item), null, false);

            listView.SelectedItem = null;
        }


        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {

            Label lbl = sender as Label;
            Employee employee = lbl.BindingContext as Employee;

            var answer = await DisplayAlert(AppResource.lblDeleteEmployee, AppResource.lblWantDeleteEmployee + employee.Name + "?", AppResource.lblYes, AppResource.lblNo);

            if (!answer)
                return;

            try
            {

                ApiService service = new ApiService();
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);


                var result = await service.GetAsync("employee/disableEmployee?id=" + employee.Id);


                Acr.UserDialogs.UserDialogs.Instance.Toast(AppResource.alertItemDeletedSucess);
                LoadEmployees();
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

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await App.AppCurrent.NavigationService.NavigateAsync(new AddNewEmployee(), null, true);
        }
    }
}
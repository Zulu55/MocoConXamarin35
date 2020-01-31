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
    public partial class AddDeliveryPlacePage : ContentPage
    {
        public AddDeliveryPlacePage()
        {
            InitializeComponent();
        }

        private async void btnAdd_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtName.Text))
                    throw new Exception(AppResource.alertInvalidFields);



                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);

                ApiService service = new ApiService();

                CompanyDelivery command = new CompanyDelivery();
                command.CompanyId = Helpers.Settings.DisplayUserCompany;
                command.Name = txtName.Text;

             

                var json = JsonConvert.SerializeObject(command);

                await service.PostAsync(json, "company/createCompanyDelivery");



                //this.DisplayAlert(AppResource.alertSucess, AppResource.alertEmployeeRegistered, AppResource.textOk);
                await App.AppCurrent.NavigationService.GoBack();

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
    }
}
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

namespace MocoApp.Views.Cliente
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditPasswordPage : ContentPage
    {
        public EditPasswordPage()
        {
            InitializeComponent();

            if (Device.RuntimePlatform == "Android")
                frmXaml.CornerRadius = 30;


            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void OnRegisterTapped(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtPassword.Text))
                    throw new Exception(AppResource.alertFillPassword);

                if (txtPassword.Text != txtConfirmPass.Text)
                    throw new Exception(AppResource.alertFillPasswordDifferent);

                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);

                ApiService service = new ApiService();

                UpdatePassword command = new UpdatePassword();

                command.Id = Helpers.Settings.DisplayUserId;
                command.Password = txtPassword.Text;

                var json = JsonConvert.SerializeObject(command);

                await service.PutAsync(json, "user/updatePassword");

                await this.DisplayAlert(AppResource.alertSucess, AppResource.alertProfileChanged, AppResource.textOk);

                txtPassword.Text = "";
                txtConfirmPass.Text = "";

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

        private async void OnVoltarTapped(object sender, EventArgs e)
        {
            await App.AppCurrent.NavigationService.ModalGoBack();
        }
    }

    class UpdatePassword
    {
        public string Id { get; set; }
        public string Password { get; set; }
    }
}
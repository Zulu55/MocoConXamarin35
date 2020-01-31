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

namespace MocoApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChangePasswordPage : ContentPage
    {
        public ChangePasswordPage()
        {
            InitializeComponent();

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

        private async void btnAdd_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtPassword.Text))
                    throw new Exception(AppResource.alertFillValid);

                if(txtPassword.Text != txtConfirm.Text)
                    throw new Exception(AppResource.alertFillPasswordDifferent);

                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);

                ApiService service = new ApiService();

                UpdatePassword command = new UpdatePassword();

                command.Id = Helpers.Settings.DisplayUserId;
                command.Password = txtPassword.Text;

                var json = JsonConvert.SerializeObject(command);

                await service.PutAsync(json, "user/updatePassword");

                await this.DisplayAlert(AppResource.alertSucess, AppResource.alertPasswordChanged, AppResource.textOk);

                txtPassword.Text = "";
                txtConfirm.Text = "";
          
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

    class UpdatePassword
    {
        public string Id { get; set; }
        public string Password { get; set; }
    }
}
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
    public partial class ContactUsPage : ContentPage
    {
        public ContactUsPage()
        {
            InitializeComponent();

            if (Device.RuntimePlatform == "Android")
                frmXaml.CornerRadius = 30;

            NavigationPage.SetHasNavigationBar(this, false);

            txtName.Text = Helpers.Settings.DisplayUserName;

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

        private async void OnOkTapped(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text) || string.IsNullOrEmpty(edtObs.Text))
            {
                await App.AppCurrent.MainPage.DisplayAlert(AppResource.alertInvalidFields, AppResource.txtFillNameMessage, AppResource.textOk);
                return;
            }

            Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);
            ApiService service = new ApiService();
            try
            {

                ContatoCommand contato = new ContatoCommand();
                contato.UsuarioId = Helpers.Settings.DisplayUserId;
                contato.Message = edtObs.Text;

                var json = JsonConvert.SerializeObject(contato);
                var result = await service.PostAsync(json, "info/contato");

                Acr.UserDialogs.UserDialogs.Instance.Toast(AppResource.alertMEessageSentSucess);

                                
                edtObs.Text = "";

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

        private void OnMenuTapped(object sender, EventArgs e)
        {
            App app = Application.Current as App;
            Naylah.Xamarin.Controls.Pages.MasterDetailNavigationPage md = (Naylah.Xamarin.Controls.Pages.MasterDetailNavigationPage)app.MainPage;
            md.IsPresented = true;
        }

      
    }

    public class ContatoCommand
    {
        public string Message { get; set; }

        public string UsuarioId { get; set; }
    }
}
using MocoApp.Resources;
using MocoApp.Services;
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
    public partial class ForgotPasswordPage : ContentPage
    {
        public ForgotPasswordPage()
        {
            InitializeComponent();
            if (Device.RuntimePlatform == "Android")
                frmXaml.CornerRadius = 30;

            
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            btnSend.Clicked += BtnSend_Clicked;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            btnSend.Clicked -= BtnSend_Clicked;
        }

        private async void BtnSend_Clicked(object sender, EventArgs e)
        {
            ApiService service = new ApiService();

            try
            {


                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);

                var result = await service.GetAsync("user/forgotPassword?email=" + txtEmail.Text);

                Acr.UserDialogs.UserDialogs.Instance.Toast(result);

            }
            catch (Exception ex)
            {

                this.DisplayAlert(AppResource.alertAlert, ex.Message, AppResource.textOk);
            }
            finally
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
        }
    }
}
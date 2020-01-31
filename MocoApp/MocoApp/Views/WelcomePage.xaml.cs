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

namespace MocoApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WelcomePage : ContentPage
    {
        bool needsToRecur = false;
        public WelcomePage()
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);

            Xamarin.Forms.Device.StartTimer(TimeSpan.FromSeconds(3), timerDelegate);
        }

        private bool timerDelegate()
        {

            if (!needsToRecur)
            {
                if(App.AppCurrent.GuestLogin)
                    App.AppCurrent.NavigationService.NavigateSetRootAsync(new LoginPage(), null, false);
                else
                    LoadUser();

                needsToRecur = true;
                
            }

            App.AppCurrent.GuestLogin = true;
            return false;

        }

        private async void LoadUser()
        {
            ApiService service = new ApiService();
            try
            {
                LoginVm loginVm = new LoginVm();

                loginVm.Email = "guest";
                loginVm.Password = "123456";

                string deviceType = "";

                switch (Device.OS)
                {
                    case TargetPlatform.Other:
                        deviceType = "Other";
                        break;
                    case TargetPlatform.iOS:
                        deviceType = "ios";
                        break;
                    case TargetPlatform.Android:
                        deviceType = "android";
                        break;
                    case TargetPlatform.WinPhone:
                        deviceType = "WindowsPhone";
                        break;
                    case TargetPlatform.Windows:
                        deviceType = "Windows";
                        break;
                    default:
                        deviceType = "None";
                        break;
                }


                loginVm.PushId = App.AppDeviceId;
                loginVm.PushToken = App.AppDeviceToken;
                loginVm.Latitude = App.AppCurrent.Latitude;
                loginVm.Longitude = App.AppCurrent.Longitude;

                decimal offset = (decimal)DateTimeOffset.Now.Offset.TotalHours;
                var userJson = JsonConvert.SerializeObject(loginVm);
                //var result = await service.PostAsync(userJson, "user/login");
                var resultToken = await service.Auth(loginVm.Email, loginVm.Password);

                var token = JsonConvert.DeserializeObject<AuthAcessToken>(resultToken);
                var result = await service.GetMe(loginVm.PushId, loginVm.PushToken, loginVm.Latitude, loginVm.Longitude, token.AcessToken);

                var user = JsonConvert.DeserializeObject<User>(result);

                //App.AppCurrent.StoreUser(user.Id, user.Name, user.Photo, user.UserRole, user.CompanyId);
                App.AppCurrent.StoreUser(user.Id, user.Name, user.Photo, user.UserRole, user.CompanyId, false, offset, user.DesirableLanguage, token.AcessToken);

                await App.AppCurrent.ConfigureAppPhase();

                

            }
            catch (Exception ex)
            {
                this.DisplayAlert(MocoApp.Resources.AppResource.alertAlert, ex.Message, AppResource.textOk);
                App.AppCurrent.NavigationService.NavigateSetRootAsync(new LoginPage(), null, false);

            }
            finally
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();

            }
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            //needsToRecur = true;
            //LoadUser();
            //await App.AppCurrent.NavigationService.NavigateSetRootAsync(new LoginPage(), null, false);
        }
    }
}
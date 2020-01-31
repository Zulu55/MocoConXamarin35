using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MocoApp.Extensions;
using MocoApp.Models;
using MocoApp.Resources;
using MocoApp.Services;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MocoApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            if (Device.RuntimePlatform == "Android")
                frmXaml.CornerRadius = 30;

            App.AppCurrent.GetUserLocation();

            txtPassword.Completed += TxtPassword_Completed;
        }

        private void TxtPassword_Completed(object sender, EventArgs e)
        {
            Login(txtEmail.Text, txtPassword.Text);
        }

        private async void OnEntrarTapped(object sender, EventArgs e)
        {
            Login(txtEmail.Text, txtPassword.Text);
        }

        private async void OnEsqueciSenhaTapped(object sender, EventArgs e)
        {
            var obj = sender as Image;
            Device.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    obj.ScaleTo(1.4, 75).ContinueWith((t) =>
                    {
                        try
                        {
                            obj.ScaleTo(1.0, 75);
                        }
                        catch
                        {
                        }
                    },
                    scheduler: TaskScheduler.FromCurrentSynchronizationContext());
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
            });

            await App.AppCurrent.NavigationService.NavigateAsync(new ForgotPasswordPage(), null, true);
        }

        private async void OnCadastrarTapped(object sender, EventArgs e)
        {
            await App.AppCurrent.NavigationService.NavigateModalAsync(new RegisterPage(), null, true);
        }

        public async void Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                await App.AppCurrent.MainPage.DisplayAlert(AppResource.alertInvalidFields, AppResource.alertFillEmailPassword, AppResource.textOk);
                return;
            }

            Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);
            ApiService service = new ApiService();
            try
            {
                LoginVm loginVm = new LoginVm();

                loginVm.Email = email.Trim();
                loginVm.Password = password;

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
                var resultToken = await service.Auth(loginVm.Email, loginVm.Password);
                var token = JsonConvert.DeserializeObject<AuthAcessToken>(resultToken);
                var result = await service.GetMe(loginVm.PushId, loginVm.PushToken, loginVm.Latitude, loginVm.Longitude, token.AcessToken);
                var user = JsonConvert.DeserializeObject<User>(result);

                App.AppCurrent.StoreUser(user.Id, user.Name, user.Photo, user.UserRole, user.CompanyId, false, offset, user.DesirableLanguage, token.AcessToken);

                App.AppCurrent.FirstLogin = true;


                if (Helpers.Settings.DisplayUserRole == Enums.UserRole.Manager.ToString())
                {
                    var resultCompanies = await service.GetAsync("company/getcompaniesbymanagerid?id=" + Helpers.Settings.DisplayUserId);
                    var list = JsonConvert.DeserializeObject<List<Company>>(resultCompanies);

                    if (list.Count >= 1)
                    {
                        Helpers.Settings.DisplayHasLocation = list.FirstOrDefault().HasLocation;
                        App.AppCurrent.CompanyCulture = list.FirstOrDefault().CurrencyType.ToCultureInfo();
                    }

                    await App.AppCurrent.ConfigureAppPhase();
                }
                else if (Helpers.Settings.DisplayUserRole == Enums.UserRole.Employee.ToString())
                {
                    if (user.Company != null && user.Company.HasLocation)
                        Helpers.Settings.DisplayHasLocation = true;

                    App.AppCurrent.CompanyCulture = user.Company.CurrencyType.ToCultureInfo();

                    await App.AppCurrent.ConfigureAppPhase();
                }
                else
                {
                    await App.AppCurrent.ConfigureAppPhase();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert(AppResource.alertAlert, ex.Message, AppResource.textOk);
            }
            finally
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
        }

        private void OnEntrarFacebookTapped(object sender, EventArgs e)
        {
            App.AppCurrent.NavigationService.NavigateModalAsync(new FacebookSyncPage(), null, true);
        }

        private async void OnVisitanteTapped(object sender, EventArgs e)
        {
            Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoging);
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
            }
            finally
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
        }
    }

    public class LoginVm
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string PushId { get; set; }

        public string PushToken { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }
    }

    class AuthAcessToken
    {
        [JsonProperty("access_token")]
        public string AcessToken { get; set; }
    }
}
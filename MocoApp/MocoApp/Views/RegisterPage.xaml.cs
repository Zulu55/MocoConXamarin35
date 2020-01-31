using MocoApp.Interfaces;
using MocoApp.Models;
using MocoApp.Resources;
using MocoApp.Services;
using Newtonsoft.Json;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MocoApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage : ContentPage
    {
        Stream imageStream = null;
        string photoUri = "ic_avatar";
        public RegisterPage()
        {
            InitializeComponent();
            if (Device.RuntimePlatform == "Android")
            {
                frmXaml.CornerRadius = 30;
                this.BackgroundImage = "bg_barraca";
            }
            else
                bgIos.IsVisible = true;

            NavigationPage.SetHasNavigationBar(this, false);

            if (!string.IsNullOrEmpty(App.AppCurrent.Latitude))
                App.AppCurrent.GetUserLocation();


            pckIdioma.Items.Add(AppResource.textPortugues);
            pckIdioma.Items.Add(AppResource.textEnglish);
            pckIdioma.Items.Add(AppResource.textSpain);
            pckIdioma.Items.Add(AppResource.textItalian);


            if (App.AppCurrent.User != null)
            {
                imgUser.Source = App.AppCurrent.User.Photo;
                txtName.Text = App.AppCurrent.User.Name;
                txtEmail.Text = App.AppCurrent.User.Email;
            }

        }

        private async void OnChangeImageTapped(object sender, EventArgs e)
        {
            ImageChoose();
        }
        private async void OnRegisterTapped(object sender, EventArgs e)
        {
            if (!CheckInfos())
            {
                return;
            }

            ApiService service = new ApiService();

            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);
                
                Client user = new Client();

                if (App.AppCurrent.User != null && !string.IsNullOrEmpty(App.AppCurrent.User.Photo))
                    user.Photo = App.AppCurrent.User.Photo;

                if (imageStream != null)
                {
                    //faz upload da imagem
                    Helpers.Settings.DisplayUserPhoto = await service.UploadImage(imageStream);
                    user.Photo = Helpers.Settings.DisplayUserPhoto;
                }

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

                
                user.Name = txtName.Text;
                user.Cellphone = txtPhone.Text;
                user.Email = txtEmail.Text;
                user.DeviceToken = App.AppDeviceToken;
                user.DeviceType = deviceType;
                user.CityName = txtCity.Text;
                user.Password = txtPassword.Text;
                user.Latitude = App.AppCurrent.Latitude;
                user.Longitude = App.AppCurrent.Longitude;
                user.DesirableLanguage = (pckIdioma.SelectedIndex + 1).ToString();
                user.Surname = txtSurname.Text;


                var json = JsonConvert.SerializeObject(user);
                var result = await service.PostAsync(json, "client/register");

                var userRetorno = JsonConvert.DeserializeObject<User>(result);

                var resultToken = await service.Auth(user.Email, user.Password);

                var token = JsonConvert.DeserializeObject<AuthAcessToken>(resultToken);

                App.AppCurrent.StoreUser(userRetorno.Id, userRetorno.Name, userRetorno.Photo, Enums.UserRole.Client, "", false, 0 , user.DesirableLanguage, token.AcessToken);

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

        public bool CheckInfos()
        {
            if (string.IsNullOrEmpty(txtName.Text))
            {
                App.AppCurrent.MainPage.DisplayAlert(AppResource.alertAtention, AppResource.alertFillName, AppResource.textOk);
                return false;
            }

            if (string.IsNullOrEmpty(txtPassword.Text))
            {
                App.AppCurrent.MainPage.DisplayAlert(AppResource.alertAtention, AppResource.alertFillPassword, AppResource.textOk);
                return false;
            }

            if (string.IsNullOrEmpty(txtPhone.Text))
            {
                App.AppCurrent.MainPage.DisplayAlert(AppResource.alertAtention, AppResource.alertFillPhone, AppResource.textOk);
                return false;
            }          


            if (string.IsNullOrEmpty(txtEmail.Text))
            {
                App.AppCurrent.MainPage.DisplayAlert(AppResource.alertAtention, AppResource.alertFillEmail, AppResource.textOk);
                return false;
            }

            if (string.IsNullOrEmpty(txtSurname.Text))
            {
                App.AppCurrent.MainPage.DisplayAlert(AppResource.alertAtention, AppResource.alertFillSurname, AppResource.textOk);
                return false;
            }

            if (!IsEmail(txtEmail.Text))
            {
                App.AppCurrent.MainPage.DisplayAlert(AppResource.alertAtention, AppResource.alertFillEmailValid, AppResource.textOk);
                return false;
            }


            return true;
        }

        public static bool IsEmail(string email)
        {
            string MatchEmailPattern =
                               @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
                        + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				                    [0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
                        + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				                    [0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
                        + @"([a-zA-Z0-9]+[\w-]+\.)+[a-zA-Z]{1}[a-zA-Z0-9-]{1,23})$";

            if (email != null) return Regex.IsMatch(email, MatchEmailPattern);
            else return false;
        }

        private async void ImageChoose()
        {
            try
            {

                await CrossMedia.Current.Initialize();

                //var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions() { PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium, CompressionQuality = 50 });
                var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions());


                if (file == null)
                {
                    return;
                }

                imgUser.Source = ImageSource.FromStream(() =>
                {
                    var img = file.GetStream();
                    return img;
                });

                Stream stream = file.GetStream();
                byte[] imageData;

                using (MemoryStream ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    imageData = ms.ToArray();
                }

                //byte[] resizedImage = DependencyService.Get<IImageResizer>().Resize(imageData, 220, 220);

                //imageStream = new MemoryStream(resizedImage);                
                imageStream = new MemoryStream(imageData);



            }
            catch (Exception)
            {


            }
            finally
            {

            }

        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri("http://hicharlieapp.com/VIPTECHLLCTOS.pdf"));
        }

        private async void OnBackTapped(object sender, EventArgs e)
        {
            await App.AppCurrent.NavigationService.ModalGoBack();
        }
    }
}
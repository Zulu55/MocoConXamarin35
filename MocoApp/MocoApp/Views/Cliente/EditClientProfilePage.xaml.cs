using MocoApp.Interfaces;
using MocoApp.Models;
using MocoApp.Resources;
using MocoApp.Services;
using MocoApp.Views.Menu;
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
using static MocoApp.Models.Enums;

namespace MocoApp.Views.Cliente
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditClientProfilePage : ContentPage
    {
        Stream imageStream = null;
        Client _user = new Client();
        UpdateUser _updateUser = new UpdateUser();
        DesirableLanguage lang;

        public EditClientProfilePage()
        {
            InitializeComponent();

            if (Device.RuntimePlatform == "Android")
                frmXaml.CornerRadius = 30;


            NavigationPage.SetHasNavigationBar(this, false);

            GetMe();

            pckIdioma.Items.Add(AppResource.textPortugues);
            pckIdioma.Items.Add(AppResource.textEnglish);
            pckIdioma.Items.Add(AppResource.textSpain);
            pckIdioma.Items.Add(AppResource.textItalian);

        }

        public async void GetMe()
        {
            ApiService service = new ApiService();

            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);


                var result = await service.GetAsync("client/getclientbyid?id=" + Helpers.Settings.DisplayUserId);
                _user = JsonConvert.DeserializeObject<Client>(result);

                txtEmail.Text = _user.Email;
                imgUser.Source = _user.Photo;
                txtName.Text = _user.Name;
                txtPhone.Text = _user.Cellphone;
                txtCity.Text = _user.CityName;
                txtSurname.Text = _user.Surname;

                if (_user.DesirableLanguage == "1")
                    pckIdioma.SelectedIndex = 0;
                if (_user.DesirableLanguage == "2")
                    pckIdioma.SelectedIndex = 1;
                if (_user.DesirableLanguage == "3")
                    pckIdioma.SelectedIndex = 2;
                if (_user.DesirableLanguage == "4")
                    pckIdioma.SelectedIndex = 3;

                _updateUser.Photo = _user.Photo;
                _updateUser.Name = _user.Name;
                _updateUser.CityName = _user.Cellphone;
                _updateUser.Cellphone = _user.CityName;
                _updateUser.DesirableLanguage = (DesirableLanguage)Convert.ToInt32(_user.DesirableLanguage);
                _updateUser.Surname = _user.Surname;

                lang = _updateUser.DesirableLanguage;


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

                _updateUser.Name = txtName.Text;
                _updateUser.CityName = txtCity.Text;
                _updateUser.Cellphone = txtPhone.Text;
                _updateUser.DesirableLanguage = (DesirableLanguage)pckIdioma.SelectedIndex + 1;
                _updateUser.Id = Helpers.Settings.DisplayUserId;
                _updateUser.Surname = txtSurname.Text;

                if (imageStream != null)
                {
                    _updateUser.Photo = await service.UploadImage(imageStream);
                    Helpers.Settings.DisplayUserPhoto = _updateUser.Photo;
                }

                //if (_updateUser.DesirableLanguage != lang)
                //{
                //AppResource.Culture = new CultureInfo(Helpers.Settings.DisplayUserIdiom);
                string idm = ((int)_updateUser.DesirableLanguage).ToString();
                App.AppCurrent.UpdateLanguage(idm);
                MenuListData.Reload();

                await App.AppCurrent.ConfigureAppPhase();
                //await App.AppCurrent.ConfigureAppPhase();
                //CurrentApp
                //}


                var json = JsonConvert.SerializeObject(_updateUser);
                var result = await service.PutAsync(json, "user/updateMe");

                var userRetorno = JsonConvert.DeserializeObject<User>(result);


                Helpers.Settings.DisplayUserPhoto = _updateUser.Photo;
                Helpers.Settings.DisplayUserName = txtName.Text;
                Acr.UserDialogs.UserDialogs.Instance.Toast(AppResource.alertProfileChanged);
                await App.AppCurrent.NavigationService.NavigateSetRootAsync(new HomePage(), null, false);
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


            if (string.IsNullOrEmpty(txtPhone.Text))
            {
                App.AppCurrent.MainPage.DisplayAlert(AppResource.alertAtention, AppResource.alertFillPhone, AppResource.textOk);
                return false;
            }

            if (string.IsNullOrEmpty(txtSurname.Text))
            {
                App.AppCurrent.MainPage.DisplayAlert(AppResource.alertAtention, AppResource.alertFillSurname, AppResource.textOk);
                return false;
            }

            return true;
        }



        private async void OnMenuTapped(object sender, EventArgs e)
        {
            App app = Application.Current as App;
            Naylah.Xamarin.Controls.Pages.MasterDetailNavigationPage md = (Naylah.Xamarin.Controls.Pages.MasterDetailNavigationPage)app.MainPage;
            md.IsPresented = true;

        }

        private void OnChangeImageTapped(object sender, EventArgs e)
        {
            ImageChoose();
        }

        private async void ImageChoose()
        {
            try
            {

                await CrossMedia.Current.Initialize();

                //var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions() { PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium, CompressionQuality = 70 });
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
            catch (Exception ex)
            {


            }
            finally
            {

            }

        }

        private async void OnAlterarSenhaTapped(object sender, EventArgs e)
        {
            await App.AppCurrent.NavigationService.NavigateModalAsync(new EditPasswordPage(), null, true);
        }
    }

    class UpdateUser
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Photo { get; set; }
        public string Cellphone { get; set; }
        public DesirableLanguage DesirableLanguage { get; set; }
        public string CityName { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Surname { get; set; }
    }
}
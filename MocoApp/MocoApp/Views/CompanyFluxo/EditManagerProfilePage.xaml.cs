using MocoApp.Interfaces;
using MocoApp.Models;
using MocoApp.Resources;
using MocoApp.Services;
using MocoApp.Views.Menu;
using Newtonsoft.Json;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static MocoApp.Models.Enums;

namespace MocoApp.Views.CompanyFluxo
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditManagerProfilePage : ContentPage
    {
        Stream imageStream = null;
        Manager _manager = new Manager();
        Manager _updateUser = new Manager();
        string lang;
        public EditManagerProfilePage()
        {
            InitializeComponent();






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


                var result = await service.GetAsync("user/getMe?id=" + Helpers.Settings.DisplayUserId);
                _manager = JsonConvert.DeserializeObject<Manager>(result);

                txtEmail.Text = _manager.Email;
                imgUser.Source = _manager.Photo;
                txtName.Text = _manager.Name;
                txtPhone.Text = _manager.Cellphone;
                txtCity.Text = _manager.CityName;


                _updateUser.Photo = _manager.Photo;
                _updateUser.Name = _manager.Name;
                _updateUser.CityName = _manager.Cellphone;
                _updateUser.Cellphone = _manager.CityName;

                if (_manager.DesirableLanguage == "1")
                    pckIdioma.SelectedIndex = 0;
                if (_manager.DesirableLanguage == "2")
                    pckIdioma.SelectedIndex = 1;
                if (_manager.DesirableLanguage == "3")
                    pckIdioma.SelectedIndex = 2;
                if (_manager.DesirableLanguage == "4")
                    pckIdioma.SelectedIndex = 3;

                lang = _manager.DesirableLanguage;
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
                _updateUser.DesirableLanguage = (pckIdioma.SelectedIndex + 1).ToString();
                _updateUser.Id = Helpers.Settings.DisplayUserId;
                if (imageStream != null)
                {
                    _updateUser.Photo = await service.UploadImage(imageStream);
                    Helpers.Settings.DisplayUserPhoto = _updateUser.Photo;
                }





                var json = JsonConvert.SerializeObject(_updateUser);
                var result = await service.PutAsync(json, "user/updateMe");

                //if(_updateUser.DesirableLanguage != lang)
                //{
                //AppResource.Culture = new CultureInfo(Helpers.Settings.DisplayUserIdiom);
                App.AppCurrent.UpdateLanguage(_updateUser.DesirableLanguage);
                MenuListData.Reload();

                await App.AppCurrent.ConfigureAppPhase();
                //await App.AppCurrent.ConfigureAppPhase();
                //CurrentApp
                //}

                var userRetorno = JsonConvert.DeserializeObject<Manager>(result);


                Helpers.Settings.DisplayUserPhoto = _updateUser.Photo;
                Helpers.Settings.DisplayUserName = txtName.Text;
                Acr.UserDialogs.UserDialogs.Instance.Toast(AppResource.lblItemUpdatedSucess);
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
    }
}
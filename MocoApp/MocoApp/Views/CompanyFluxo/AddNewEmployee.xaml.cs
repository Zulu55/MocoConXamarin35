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
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MocoApp.Views.CompanyFluxo
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddNewEmployee : ContentPage
    {
        Stream imageStream = null;
        string photoUri = "ic_avatar";

        public AddNewEmployee()
        {
            InitializeComponent();
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            ImageChoose();
        }

        private async void btnAdd_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtEmail.Text))
                    throw new Exception(AppResource.alertFillEmailValid);



                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);

                ApiService service = new ApiService();

                Employee command = new Employee();

                command.Name = txtName.Text;
                command.CityName = txtCity.Text;
                command.Cellphone = txtPhone.Text;
                command.Name = txtName.Text;
                command.Email = txtEmail.Text.TrimEnd().TrimStart();
                command.Password = txtPassword.Text;
                command.CompanyId = Helpers.Settings.DisplayUserCompany;
                command.Photo = photoUri;

                var json = JsonConvert.SerializeObject(command);

                await service.PostAsync(json, "employee/register");



                this.DisplayAlert(AppResource.alertSucess, AppResource.alertEmployeeRegistered, AppResource.textOk);
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

        private async void ImageChoose()
        {
            try
            {

                await CrossMedia.Current.Initialize();

                //var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions() { PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium, CompressionQuality = 80 });
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
    }
}
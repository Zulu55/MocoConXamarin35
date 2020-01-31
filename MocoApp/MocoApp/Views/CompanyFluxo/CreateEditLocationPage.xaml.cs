using MocoApp.Models;
using MocoApp.Resources;
using MocoApp.Services;
using Newtonsoft.Json;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static MocoApp.Models.Enums;

namespace MocoApp.Views.CompanyFluxo
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateEditLocationPage : ContentPage
    {
        private readonly Location _location;
        private Stream _imageStream = null;
        private readonly bool _isEdit = false;
        private readonly bool _firstLoad = true;
        private readonly CompanyService _companyService = new CompanyService();

        public CreateEditLocationPage(Location location = null)
        {
            InitializeComponent();

            LoadPck();

            _location = location;

            if (_location == null)
            {
                LoadTotals();
                Title = AppResource.lblCreateLocation;
                swtActive.IsToggled = true;
            }
            else
            {
                _isEdit = true;
                Title = AppResource.lblEditLocation;

                txtName.Text = location.Name;
                swtActive.IsToggled = !location.IsDisabled;
                pckLocationType.SelectedIndex = (int)location.LocationType;
                pckLocationMenuType.SelectedIndex = (int)location.MenuType;
                txtPrefixo.Text = location.Prefix;
                txtPosition.Text = location.OrderingNumber.ToString();
                lblTotalPositions.Text += location.TotalItens;
            }
            BindingContext = location;
        }

        public async void LoadTotals()
        {
            LocationService locationService = new LocationService();

            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);

                string result = await locationService.GetTotalPositions();
                int total = JsonConvert.DeserializeObject<int>(result);

                lblTotalPositions.Text += total;
                txtPosition.Text = (total + 1).ToString();
            }
            catch (Exception ex)
            {
                DisplayAlert(AppResource.alertAlert, ex.Message, AppResource.textOk);
            }
            finally
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }

        }

        public void LoadPck()
        {
            List<string> list = Enum.GetValues(typeof(LocationType)).Cast<LocationType>().Select(x => x.ToString()).ToList();
            foreach (string item in list)
            {
                pckLocationType.Items.Add(item);
            }

            List<string> listType = Enum.GetValues(typeof(EMenuType)).Cast<EMenuType>().Select(x => x.ToString()).ToList();
            foreach (string item in listType)
            {
                pckLocationMenuType.Items.Add(item);
            }
        }


        private void btnSave_Clicked(object sender, EventArgs e)
        {
            if (!_isEdit)
            {
                Create();
            }
            else
            {
                Edit();
            }
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            ImageChoose();
        }

        public async void Create()
        {
            try
            {

                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);

                if (pckLocationType.SelectedIndex < 1)
                {
                    await DisplayAlert(AppResource.alertAlert, AppResource.alertSelectedLocation, AppResource.textOk);
                    return;
                }
                if (pckLocationMenuType.SelectedIndex < 1)
                {
                    await DisplayAlert(MocoApp.Resources.AppResource.alertAlert, AppResource.alertSelectedLocationMenu, AppResource.textOk);
                    return;
                }

                Location _location = new Location();
                if (_imageStream != null)
                {
                    //faz upload da imagem
                    ApiService service = new ApiService();
                    _location.ImageUri = await service.UploadImage(_imageStream);
                }

                _location.Name = txtName.Text;
                _location.IsDisabled = !swtActive.IsToggled;
                _location.CompanyId = Helpers.Settings.DisplayUserCompany;
                _location.IsRoom = swtActive.IsToggled;
                _location.LocationType = (LocationType)pckLocationType.SelectedIndex;
                _location.Prefix = txtPrefixo.Text;
                _location.OrderingNumber = Convert.ToInt32(txtPosition.Text);
                _location.MenuType = (EMenuType)pckLocationMenuType.SelectedIndex;
                string result = await _companyService.CreateLocation(_location);

                Acr.UserDialogs.UserDialogs.Instance.Toast(AppResource.lblItemCreatedSucess);
                await App.AppCurrent.NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                DisplayAlert(AppResource.alertAlert, ex.Message, AppResource.textOk);
            }
            finally
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
        }

        public async void Edit()
        {
            try
            {
                if (pckLocationType.SelectedIndex < 1)
                {
                    await DisplayAlert(AppResource.alertAlert, AppResource.alertSelectedLocation, AppResource.textOk);
                    return;
                }
                if (pckLocationMenuType.SelectedIndex < 1)
                {
                    await DisplayAlert(AppResource.alertAlert, AppResource.alertSelectedLocationMenu, AppResource.textOk);
                    return;
                }

                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);

                if (_imageStream != null)
                {
                    ApiService service = new ApiService();
                    _location.ImageUri = await service.UploadImage(_imageStream);
                }

                _location.Name = txtName.Text;
                _location.IsDisabled = !swtActive.IsToggled;
                _location.LocationType = (LocationType)pckLocationType.SelectedIndex;
                _location.Prefix = txtPrefixo.Text;
                _location.OrderingNumber = Convert.ToInt32(txtPosition.Text);
                _location.MenuType = (EMenuType)pckLocationMenuType.SelectedIndex;
                Task<string> result = _companyService.EditLocation(_location);

                Acr.UserDialogs.UserDialogs.Instance.Toast(AppResource.lblItemUpdatedSucess);

            }
            catch (Exception ex)
            {
                DisplayAlert(MocoApp.Resources.AppResource.alertAlert, ex.Message, AppResource.textOk);

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

                Plugin.Media.Abstractions.MediaFile file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions() 
                { 
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium, 
                    CompressionQuality = 80 
                });

                if (file == null)
                {
                    return;
                }
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);

                imgProduct.Source = ImageSource.FromStream(() =>
                {
                    Stream img = file.GetStream();
                    return img;
                });

                Stream stream = file.GetStream();
                byte[] imageData;

                using (MemoryStream ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    imageData = ms.ToArray();
                }

                _imageStream = new MemoryStream(imageData);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            finally
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
        }

        private async void btnDelet_Clicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert(AppResource.alertDelete, AppResource.alertInfoDeleteCategory, AppResource.textOk, AppResource.alertCancel);

            if (!answer)
            {
                return;
            }

            try
            {
                ApiService service = new ApiService();
                await service.GetAsync("location/deleteLocation?id=" + _location.Id);
                Acr.UserDialogs.UserDialogs.Instance.Toast(AppResource.alertItemDeletedSucess);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
    }
}
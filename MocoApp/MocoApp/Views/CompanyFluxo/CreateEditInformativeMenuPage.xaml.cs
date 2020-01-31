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
    public partial class CreateEditInformativeMenuPage : ContentPage
    {
        Stream imageStream = null;
        InformativeMenu _menu;
        bool isEdit = false;
        string locationId = "";
        CompanyService companyService = new CompanyService();
        CategoryGroup _group;
        public CreateEditInformativeMenuPage(InformativeMenu menu = null, string _locationId = "", int? total = null)
        {
            InitializeComponent();

            locationId = _locationId;

            _menu = menu;

            if (_menu == null)
            {
                LoadTotals();
                this.Title = AppResource.lblCreateMenu;
                swtActive.IsToggled = true;
                _menu = new InformativeMenu();
            }
            else
            {
                isEdit = true;
                this.Title = AppResource.lblEditMenu;
                txtName.Text = _menu.Name;
                txtTitle.Text = _menu.Title;
                txtBody.Text = _menu.Body;
                txtUrl.Text = _menu.Url;
                swtActive.IsToggled = !_menu.IsDisabled;
                txtPosition.Text = _menu.OrderingNumber.ToString();
                lblTotalPositions.Text += _menu.TotalItens;

            }

            this.BindingContext = _menu;
        }
        private string _id = "";
        public CreateEditInformativeMenuPage(CategoryGroup group, string id = null)
        {
            InitializeComponent();
            _id = id;
            _group = group;


            LoadMenu(id);
        }
        public async void LoadTotals()
        {
            LocationService locationService = new LocationService();

            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);

                var result = await locationService.GetTotalPositionsProduct(_group.Id);
                var total = JsonConvert.DeserializeObject<int>(result);

                lblTotalPositions.Text += total;
                txtPosition.Text = (total + 1).ToString();
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
        public async void LoadMenu(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                CompanyService locationService = new CompanyService();

                try
                {
                    Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);

                    var result = await locationService.GetInformativeMenuById(id);
                    _menu = JsonConvert.DeserializeObject<InformativeMenu>(result);
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

            if (_menu == null)
            {
                LoadTotals();
                this.Title = AppResource.lblCreateMenu;
                swtActive.IsToggled = true;
                _menu = new InformativeMenu();
            }
            else
            {
                isEdit = true;
                this.Title = AppResource.lblEditMenu;
                txtName.Text = _menu.Name;
                txtTitle.Text = _menu.Title;
                txtBody.Text = _menu.Body;
                txtUrl.Text = _menu.Url;
                swtActive.IsToggled = !_menu.IsDisabled;
                txtPosition.Text = _menu.OrderingNumber.ToString();
                lblTotalPositions.Text += _menu.TotalItens;
            }

            this.BindingContext = _menu;
        }
        //public async void LoadTotals()
        //{
        //    LocationService locationService = new LocationService();

        //    try
        //    {
        //        Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);

        //        var result = await locationService.GetTotalPositionsMenu(locationId);
        //        var total = JsonConvert.DeserializeObject<int>(result);

        //        lblTotalPositions.Text += total;
        //        txtPosition.Text = (total + 1).ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        this.DisplayAlert(MocoApp.Resources.AppResource.alertAlert, ex.Message, AppResource.textOk);
        //    }
        //    finally
        //    {
        //        Acr.UserDialogs.UserDialogs.Instance.HideLoading();
        //    }

        //}

        private void btnSave_Clicked(object sender, EventArgs e)
        {

            if (!isEdit)
                Create();
            else
                Edit();
        }

        public async void Create()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);





                InformativeMenu menu = new InformativeMenu();
                if (imageStream != null)
                {
                    //faz upload da imagem
                    ApiService service = new ApiService();
                    menu.ImageUri = await service.UploadImage(imageStream);

                }
                menu.Title = txtTitle.Text;
                menu.Body = txtBody.Text;
                menu.Url = txtUrl.Text;
                menu.Name = txtName.Text;
                menu.IsDisabled = !swtActive.IsToggled;
                menu.OrderingNumber = Convert.ToInt32(txtPosition.Text);
                menu.CompanyId = Helpers.Settings.DisplayUserCompany;
                menu.LocationId = locationId;
                menu.CategoryGroupId = _group.Id;

                var result = await companyService.CreateInformativeMenu(menu);

                Acr.UserDialogs.UserDialogs.Instance.Toast(AppResource.lblItemUpdatedSucess);
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

        public async void Edit()
        {

            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);

                if (imageStream != null)
                {
                    //faz upload da imagem
                    ApiService service = new ApiService();
                    _menu.ImageUri = await service.UploadImage(imageStream);
                }

                _menu.Title = txtTitle.Text;
                _menu.Body = txtBody.Text;
                _menu.Url = txtUrl.Text;
                _menu.Name = txtName.Text;
                _menu.IsDisabled = !swtActive.IsToggled;
                _menu.OrderingNumber = Convert.ToInt32(txtPosition.Text);
                _menu.CompanyId = Helpers.Settings.DisplayUserCompany;
                _menu.LocationId = locationId;
                var result = await companyService.EditInformativeMenu(_menu);

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

        private async void ImageChoose()
        {
            try
            {

                await CrossMedia.Current.Initialize();
                var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions() { PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium, CompressionQuality = 80 });

                //var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions() { PhotoSize = Plugin.Media.Abstractions.PhotoSize.Small, CompressionQuality = 80 });
                //var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions());


                if (file == null)
                {
                    return;
                }

                imgProduct.Source = ImageSource.FromStream(() =>
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


                //byte[] resizedImage = DependencyService.Get<IImageResizer>().Resize(imageData, 140, 140);
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
            ImageChoose();
        }

        private async void btnDelet_Clicked(object sender, EventArgs e)
        {
            var answer = await DisplayAlert(AppResource.alertDelete, AppResource.alertInfoDeleteCategory, AppResource.textOk, AppResource.alertCancel);

            if (!answer)
                return;


            try
            {
                ApiService service = new ApiService();

                await service.GetAsync("v1/menu/delete/" + _menu.Id);

                Acr.UserDialogs.UserDialogs.Instance.Toast(AppResource.alertItemDeletedSucess);

            }
            catch (Exception ex)
            {


            }
        }
    }
}
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
    public partial class CreateEditCategoryPage : ContentPage
    {
        Stream imageStream = null;
        Category ProductCategory;
        bool isEdit = false;
        string locationId = "";
        CompanyService companyService = new CompanyService();
        public CreateEditCategoryPage(Category productCategory = null, string _locationId = "", int? total = null)
        {
            InitializeComponent();

            locationId = _locationId;

            ProductCategory = productCategory;

            if(ProductCategory == null)
            {
                LoadTotals();
                this.Title = AppResource.lblCreateCategory;
                swtActive.IsToggled = true;
                ProductCategory = new Category();
            }
            else
            {
                isEdit = true;                
                this.Title = AppResource.lblEditCategory;
                txtName.Text = productCategory.Name;                
                swtActive.IsToggled = !productCategory.IsDisabled;
                txtPosition.Text = productCategory.OrderingNumber.ToString();
                lblTotalPositions.Text += productCategory.TotalItens;
            }

            this.BindingContext = ProductCategory;
        }
        public async void LoadTotals()
        {
            LocationService locationService = new LocationService();

            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);

                var result = await locationService.GetTotalPositionsCategory(locationId);
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





                Category _productCategory = new Category();
                if (imageStream != null)
                {
                    //faz upload da imagem
                    ApiService service = new ApiService();
                    _productCategory.ImageUri = await service.UploadImage(imageStream);

                }

                _productCategory.Name = txtName.Text;
                _productCategory.IsDisabled = !swtActive.IsToggled;
                _productCategory.LocationId = locationId;
                _productCategory.CompanyId = Helpers.Settings.DisplayUserCompany;
                _productCategory.OrderingNumber = Convert.ToInt32(txtPosition.Text);

                var result = await companyService.CreateProductCategory(_productCategory);

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
                    ProductCategory.ImageUri = await service.UploadImage(imageStream);
                }

                ProductCategory.Name = txtName.Text;
                ProductCategory.IsDisabled = !swtActive.IsToggled;
                ProductCategory.OrderingNumber = Convert.ToInt32(txtPosition.Text);
                var result = await companyService.EditProductCategory(ProductCategory);

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

                await service.GetAsync("category/deleteCategory?id=" + ProductCategory.Id);

                Acr.UserDialogs.UserDialogs.Instance.Toast(AppResource.alertItemDeletedSucess);

            }
            catch (Exception ex)
            {


            }
        }
    }
}
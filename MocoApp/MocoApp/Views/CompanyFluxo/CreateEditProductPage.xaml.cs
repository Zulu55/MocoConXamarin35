using MocoApp.Helpers;
using MocoApp.Interfaces;
using MocoApp.Models;
using MocoApp.Resources;
using MocoApp.Services;
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

namespace MocoApp.Views.CompanyFluxo
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateEditProductPage : ContentPage
    {
        Product Product;
        Stream imageStream = null;
        CategoryGroup Group;
        bool isEdit = false;
        bool firstLoad = true;
        CompanyService companyService = new CompanyService();
        public CreateEditProductPage(CategoryGroup group, Product product = null )
        {
            InitializeComponent();

            Group = group;


            pckProductSegment.Items.Add("Produto");
            pckProductSegment.Items.Add("Serviço");


            if (product == null)
            {
                LoadTotals();
                this.Title = AppResource.lblCreateProduct + Group.Name +")";
                swtActive.IsToggled = true;
                pckProductSegment.SelectedIndex = 0;
                Product = new Product();
            }
            else
            {
                isEdit = true;
                Product = product;
                this.Title = AppResource.lblEditProduct;

                txtName.Text = product.Name;
                //txtPrice.Text = product.Price.ToString("00");
                txtDescription.Text = product.Description;
                txtQuantity.Text = product.Quantity.ToString();
                swtActive.IsToggled = !product.IsDisabled;
                txtPosition.Text = product.OrderingNumber.ToString();
                lblTotalPositions.Text += product.TotalItens;


                if (product.ProductSegment == EProductSegment.Product)
                    pckProductSegment.SelectedIndex = 0;

                if (product.ProductSegment == EProductSegment.Service)
                    pckProductSegment.SelectedIndex = 1;

            }

            this.BindingContext = Product;

            txtPrice.Focused += TxtPrice_Focused;
        }
        public async void LoadTotals()
        {
            LocationService locationService = new LocationService();

            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);

                var result = await locationService.GetTotalPositionsProduct(Group.Id);
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
        private void TxtPrice_Focused(object sender, FocusEventArgs e)
        {
            if (firstLoad && !isEdit)
                txtPrice.Text = "";

            firstLoad = false;
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

                if(pckProductSegment.SelectedIndex < 0)                
                    throw new Exception("Selecione o tipo do produto.");
                

                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);

                string value = txtPrice.Text.Replace(',', '.').Replace("R$", "").Replace("$", "");

                var culture = App.AppCurrent.Culture;
                var valorNow = Decimal.Parse(value, culture.NumberFormat);

                
                Product _product = new Product();
                if (imageStream != null)
                {
                    //faz upload da imagem
                    ApiService service = new ApiService();
                    _product.ImageUri = await service.UploadImage(imageStream);

                }

                _product.Name = txtName.Text;
                _product.Price = valorNow;
                _product.Quantity = Convert.ToInt32(txtQuantity.Text);
                _product.Description = txtDescription.Text;
                _product.IsDisabled = !swtActive.IsToggled;
                _product.CategoryGroupId = Group.Id;
                _product.CompanyId = Helpers.Settings.DisplayUserCompany;
                _product.OrderingNumber = Convert.ToInt32(txtPosition.Text);
                _product.ProductSegment = (EProductSegment)pckProductSegment.SelectedIndex;

                var result = await companyService.CreateProduct(_product);

                Acr.UserDialogs.UserDialogs.Instance.Toast(AppResource.lblItemCreatedSucess);
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
                    Product.ImageUri = await service.UploadImage(imageStream);
                    
                }

                //var culture = App.AppCurrent.Culture;
                //if (culture.Name  == new CultureInfo("es-ES").Name)
                //{
                //    culture = new CultureInfo("en-US");
                //}

                //var valorNow = Decimal.Parse(txtPrice.Text.Replace(',', '.').Replace("R$", "").Replace("$", "").TrimStart());
                //var valorNow = Decimal.Parse(txtPrice.Text.Replace("R$", "").Replace("$", "").TrimStart(), Utils.GetCurrencyCUlture(false));
                var valorNow = Convert.ToDecimal(txtPrice.Text.Replace("R$", "").Replace("$", "").TrimStart());
                Product.Name = txtName.Text;
                Product.Price = valorNow;
                Product.Quantity = Convert.ToInt32(txtQuantity.Text);
                Product.Description = txtDescription.Text;
                Product.IsDisabled = !swtActive.IsToggled;
                Product.OrderingNumber = Convert.ToInt32(txtPosition.Text);
                Product.ProductSegment = (EProductSegment)pckProductSegment.SelectedIndex;

                var result = companyService.EditProduct(Product);

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

                //var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions() { PhotoSize = Plugin.Media.Abstractions.PhotoSize.Small, CompressionQuality = 80 });
                //var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions());
                var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions() { PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium, CompressionQuality = 80 });


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


                imageStream = new MemoryStream(imageData);
                //byte[] resizedImage = DependencyService.Get<IImageResizer>().Resize(imageData, 140, 140);
                //imageStream = new MemoryStream(resizedImage);

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

                await service.GetAsync("product/deleteProduct?id=" + Product.Id);

                Acr.UserDialogs.UserDialogs.Instance.Toast(AppResource.alertItemDeletedSucess);

            }
            catch (Exception ex)
            {

                
            }
        }
    }
}
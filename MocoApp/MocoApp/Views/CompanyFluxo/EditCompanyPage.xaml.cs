using MocoApp.Extensions;
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
    public partial class EditCompanyPage : ContentPage
    {
        private Stream _imageStream = null;
        private ApiService _service = new ApiService();
        private List<CompanyPhoto> _listPhoto = new List<CompanyPhoto>();
        private List<CompanyDelivery> _listCompanyDelivery = new List<CompanyDelivery>();
        private CompanyService _cs = new CompanyService();
        private Company _company = new Company();

        public EditCompanyPage()
        {
            InitializeComponent();

            if (Device.OS == TargetPlatform.iOS)
            {
                // move layout under the status bar
                this.Padding = new Thickness(0, 0, 0, 0);

                var menu = new ToolbarItem("Menu", "", () =>
                {
                    App app = Application.Current as App;
                    Naylah.Xamarin.Controls.Pages.MasterDetailNavigationPage md = (Naylah.Xamarin.Controls.Pages.MasterDetailNavigationPage)app.MainPage;

                    md.IsPresented = true;

                }, 0, 0);
                menu.Priority = 0;

                ToolbarItems.Add(menu);
            }

            LoadImages();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadCompany();
        }

        private void btnAdd_Clicked(object sender, EventArgs e)
        {

        }

        public void LoadItens()
        {
            int count = 7;
            float totalRows = (float)count / (float)3;
            var intTotalRows = Math.Round(totalRows, MidpointRounding.AwayFromZero);

            var grid = new Grid
            {

                ColumnDefinitions =
                            {
                                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                            }
            };

            List<RowDefinition> rows = new List<RowDefinition>();

            for (int i = 0; i < intTotalRows; i++)
            {
                rows.Add(new RowDefinition() { Height = GridLength.Auto });
            }

            foreach (var item in rows)
            {
                grid.RowDefinitions.Add(item);
            }

            int column = 0;
            int row = 0;
            for (int i = 1; i < count; i++)
            {
                var image = new Image();
                image.HeightRequest = 80;
                image.BackgroundColor = Color.FromHex("#ccc");
                image.Aspect = Aspect.AspectFill;

                if (_listPhoto.Count >= i)
                    image.Source = ImageSource.FromUri(new Uri(_listPhoto[i - 1].ImageUri));

                if (_listPhoto.Count >= i)
                    image.StyleId = _listPhoto[i - 1].Id;

                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;
                image.GestureRecognizers.Add(tapGestureRecognizer);


                grid.Children.Add(image, column, row);

                column++;

                if (column > 2)
                    column = 0;

                if (column == 0)
                    row++;
            }

            stkPhotos.Children.Add(grid);
        }

        public async void LoadCompany()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);

                var result = await _cs.GetCompanyById(Helpers.Settings.DisplayUserCompany);
                _company = JsonConvert.DeserializeObject<Company>(result);



                txtGorjeta.Text = _company.RecommendedTipPercentage.ToString();
                txtTaxa.Text = _company.TaxPercentage.ToString();

                txtPrefix.IsVisible = true;
                lblPrefix.IsVisible = true;
                txtPrefix.Text = _company.OccupationPrefix;
                chkCreditCardAllowed.IsVisible = true;
                chkCreditCardAllowed.IsToggled = _company.CreditCardAllowed;
                chkAllowChatOnlyForCustomers.IsToggled = _company.AllowChatOnlyForCustomers;
                pickerCurrency.SelectedIndex = (int)_company.CurrencyType;
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

        private async void TapGestureRecognizer_Tapped1(object sender, EventArgs e)
        {
            var lbl = sender as Label;

            var answer = await DisplayAlert(AppResource.alertDelete, AppResource.alertDelete + "?", AppResource.lblYes, AppResource.lblNo);

            if (answer)
            {
                try
                {
                    Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.lblWait);
                    await _service.GetAsync("company/removeCompanyDelivery?id=" + lbl.StyleId);
                    lbl.StyleId = "";
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
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var img = sender as Image;

            if (!string.IsNullOrEmpty(img.StyleId))
            {

                var action = await DisplayActionSheet(AppResource.lblWhatYouWantToDo, AppResource.alertCancel, null, AppResource.lblSeeImage, AppResource.lblRemoveImage);

                if (action == AppResource.lblRemoveImage)
                {
                    try
                    {
                        Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.lblWait);

                        await _service.GetAsync("company/removeCompanyPhoto?id=" + img.StyleId);
                        img.StyleId = "";
                        img.Source = null;

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

                if (action == AppResource.lblSeeImage)
                {
                    await App.AppCurrent.NavigationService.NavigateModalAsync(new ImageFullScreenPage(_listPhoto.Where(m => m.Id == img.StyleId).FirstOrDefault().ImageUri), null, true);
                }
            }
            else
            {
                try
                {
                    await ImageChoose(img);

                    Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.lblWait);

                    string uriImage = await _service.UploadImage(_imageStream);
                    //insere a imagem
                    CompanyPhoto cp = new CompanyPhoto();
                    cp.CompanyId = Helpers.Settings.DisplayUserCompany;
                    cp.ImageUri = uriImage;

                    var json = JsonConvert.SerializeObject(cp);
                    var result = await _service.PostAsync(json, "company/createCompanyPhoto");

                    var cpCreated = JsonConvert.DeserializeObject<CompanyPhoto>(result);
                    img.StyleId = cpCreated.Id;

                }
                catch (Exception ex)
                {
                    await this.DisplayAlert(AppResource.alertAlert, ex.Message, AppResource.textOk);
                }
                finally
                {
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                }

            }

            _imageStream = null;
        }

        private async Task ImageChoose(Image fromImage)
        {
            try
            {
                await CrossMedia.Current.Initialize();
                var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions());


                if (file == null)
                {
                    return;
                }

                fromImage.Source = ImageSource.FromStream(() =>
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

                _imageStream = new MemoryStream(imageData);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            finally
            {
            }
        }

        public async void LoadImages()
        {
            try
            {

                var result = await _cs.GetCompanyPhotos(Helpers.Settings.DisplayUserCompany);

                _listPhoto = JsonConvert.DeserializeObject<List<CompanyPhoto>>(result);
            }
            catch (Exception ex)
            {
                await this.DisplayAlert(MocoApp.Resources.AppResource.alertAlert, ex.Message, AppResource.textOk);
            }
            finally
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                LoadItens();
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await App.AppCurrent.NavigationService.NavigateAsync(new AddDeliveryPlacePage(), null, true);
        }

        private async void btnSalvarValores(object sender, EventArgs e)
        {
            try
            {

                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.lblWait);
                var type = (ECurrencyType)pickerCurrency.SelectedIndex;
                App.AppCurrent.CompanyCulture = type.ToCultureInfo();

                var result = await _cs.EditCompanyTax(
                        txtTaxa.Text,
                        txtGorjeta.Text,
                        txtPrefix.Text,
                        chkCreditCardAllowed.IsToggled, 
                        chkAllowChatOnlyForCustomers.IsToggled,
                        pickerCurrency.SelectedIndex);

                Acr.UserDialogs.UserDialogs.Instance.Toast(AppResource.lblItemUpdatedSucess);
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
    }
}
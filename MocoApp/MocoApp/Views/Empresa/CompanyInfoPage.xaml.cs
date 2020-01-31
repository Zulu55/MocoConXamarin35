using MocoApp.Interfaces;
using MocoApp.Models;
using MocoApp.Resources;
using MocoApp.Services;
using Newtonsoft.Json;
using Plugin.ExternalMaps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MocoApp.Views.Empresa
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CompanyInfoPage : ContentPage
    {
        Company Company;
        List<CompanyPhoto> _listPhoto;
        public CompanyInfoPage(Company company)
        {
            InitializeComponent();
            if (Device.RuntimePlatform == "Android")
                frmXaml.CornerRadius = 30;

            NavigationPage.SetHasNavigationBar(this, false);

            ColorPage();
            Company = company;

            lblName.Text = company.Title;
            lblPhone.Text = company.Cellphone;
            lblAddress.Text = company.Address;
            lblDescription.Text = company.Description;
            lblHour.Text = company.Worktime;

            float paymentIconSize = 22f;
            float paymentLabelSize = 14f;

            if (Device.Idiom == TargetIdiom.Tablet)
            {
                paymentLabelSize = 20f;
                paymentIconSize = 40f;
            }

            if (company.Payments != null)
            {
                foreach (var item in company.Payments)
                {
                    StackLayout stk = new StackLayout();
                    stk.Orientation = StackOrientation.Horizontal;

                    Image img = new Image();
                    img.Source = item.ImageUri;
                    img.WidthRequest = paymentIconSize;

                    Label lbl = new Label();
                    lbl.FontSize = paymentLabelSize;
                    lbl.Text = item.Name;

                    stk.Children.Add(img);
                    stk.Children.Add(lbl);

                    stkPayments.Children.Add(stk);

                }
            }

            LoadImages();
        }

        private async void OnBackTapped(object sender, EventArgs e)
        {
            await App.AppCurrent.NavigationService.ModalGoBack();
        }

        public async void LoadImages()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);
                CompanyService cs = new CompanyService();
                var result = await cs.GetCompanyPhotos(Company.Id);

                _listPhoto = JsonConvert.DeserializeObject<List<CompanyPhoto>>(result);

                if (_listPhoto != null)
                    LoadItens(_listPhoto.Count, _listPhoto.Select(x => x.ImageUri).ToList());
            }
            catch (Exception ex)
            {
                await this.DisplayAlert(MocoApp.Resources.AppResource.alertAlert, ex.Message, AppResource.textOk);
            }
            finally
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();

            }
        }

        public void ColorPage()
        {
            switch (App.CompanyTypeSelected)
            {
                case Models.Enums.CompanyType.Hotel:
                    this.BackgroundColor = (Color)App.Current.Resources["HotelColor"];
                    imgBack.Source = "ic_voltar_hotel";
                    break;
                case Models.Enums.CompanyType.Restaurante:
                    this.BackgroundColor = (Color)App.Current.Resources["RestauranteColor"];
                    imgBack.Source = "ic_voltar_restaurante";
                    break;
                case Models.Enums.CompanyType.Praia:
                    //this.BackgroundColor = Color.FromHex("#009145");     
                    this.BackgroundImage = "bg_praia";
                    this.BackgroundColor = Color.Transparent;
                    imgBack.Source = "ic_voltar_praia";
                    break;
                case Enums.CompanyType.EsporteEvento:
                    this.BackgroundColor = (Color)App.Current.Resources["EsportesColor"];
                    imgBack.Source = "ic_voltar_esportes";
                    break;
                default:
                    break;
            }



        }

        public void LoadItens(int totalImages, List<string> images)
        {
            int count = totalImages;
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
            for (int i = 0; i < count; i++)
            {
                var image = new Image();
                image.HeightRequest = 65;
                image.BackgroundColor = Color.FromHex("#ccc");
                image.Aspect = Aspect.AspectFill;
                image.StyleId = images[i];
                image.Source = ImageSource.FromUri(new Uri(images[i]));

                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped1;
                image.GestureRecognizers.Add(tapGestureRecognizer);

                grid.Children.Add(image, column, row);

                column++;

                if (column > 2)
                    column = 0;

                if (column == 0)
                    row++;
            }

            stkFotos.Children.Add(grid);
        }

        private async void TapGestureRecognizer_Tapped1(object sender, EventArgs e)
        {
            var img = sender as Image;
            var list = _listPhoto.Select(m => m.ImageUri).ToList();
            list.Remove(img.StyleId);

            var lst = new List<string>();
            lst.Add(img.StyleId);
            lst.AddRange(list);


            await App.AppCurrent.NavigationService.NavigateModalAsync(new CarouselPage(lst), null, true);


        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            try
            {
                var answer = await DisplayAlert("", Company.Cellphone, AppResource.alertCall, AppResource.alertCancel);

                if (!answer)
                    return;

                if (Device.RuntimePlatform == "iOS")
                {
                    var dialer = DependencyService.Get<IDialer>();
                    dialer.Dial(Company.Cellphone.Replace(" ", ""));
                }
                else
                {

                    Device.OpenUri(new Uri("tel: " + Company.Cellphone.Replace(" ", "")));
                }

            }
            catch (Exception ex)
            {

            }


        }

        private async void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            try
            {
                if (Device.RuntimePlatform == "iOS")
                {
                    //var uri = string.Format("http://maps.apple.com/?q={0}&sll={1},{2}&z=2&t=s", Company.Address, Company.Latitude.Replace(",", "."), Company.Longitude.Replace(",", "."));
                    //Device.OpenUri(new Uri(uri));
                    await App.AppCurrent.NavigationService.NavigateModalAsync(new CompanyMapPage(Company), null, true);
                }
                else
                {
                    //await CrossExternalMaps.Current.NavigateTo(Company.Title, Convert.ToDouble(Company.Latitude), Convert.ToDouble(Company.Longitude));

                    var loc = string.Format("{0},{1}", Company.Latitude, Company.Longitude);
                    var name = Company.Title.Replace("&", "and"); // var name = Uri.EscapeUriString(place.Name);
                    
                    var request = string.Format("geo:0,0?q={0}({1})", loc, name);
                    Device.OpenUri(new Uri(request));
                }
            }
            catch (Exception ex)
            {
                this.DisplayAlert("Error", AppResource.alertErrorOcured, AppResource.textOk);
                
            }
            
        }
    }
}
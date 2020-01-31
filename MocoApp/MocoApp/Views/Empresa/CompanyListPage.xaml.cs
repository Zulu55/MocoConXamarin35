using ImageCircle.Forms.Plugin.Abstractions;
using MocoApp.Models;
using MocoApp.Resources;
using MocoApp.Services;
using MocoApp.Services.V2;
using Newtonsoft.Json;
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
    public partial class CompanyListPage : ContentPage
    {

        List<Company> ListCompany { get; set; }

        public CompanyListPage()
        {
            InitializeComponent();
            if (Device.RuntimePlatform == "Android")
                frmXaml.CornerRadius = 30;

            NavigationPage.SetHasNavigationBar(this, false);

            ListCompany = new List<Company>();
            ColorPage();
            LoadCompanies();

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
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            stkItens.Children.Clear();

            if (ListCompany.Count > 0)
                LoadItens(ListCompany.Count());
        }

        public void ColorPage()
        {
            switch (App.CompanyTypeSelected)
            {
                case Models.Enums.CompanyType.Hotel:
                    this.BackgroundColor = (Color)App.Current.Resources["HotelColor"];
                    lblHeaderTitle.TextColor = (Color)App.Current.Resources["HotelColor"];
                    imgSearch.Source = "ic_search_hotel";
                    //imgMenu.Source = "ic_menu_hotel";
                    imgMenu.Source = "ic_voltar_hotel";
                    lblHeaderTitle.Text = AppResource.lblHotels.ToUpper();
                    break;
                case Models.Enums.CompanyType.Restaurante:
                    this.BackgroundColor = (Color)App.Current.Resources["RestauranteColor"];
                    lblHeaderTitle.TextColor = (Color)App.Current.Resources["RestauranteColor"];
                    imgSearch.Source = "ic_search_restaurante";
                    //imgMenu.Source = "ic_menu_restaurante";
                    imgMenu.Source = "ic_voltar_restaurante";

                    lblHeaderTitle.Text = AppResource.lblRestaurants.ToUpper();
                    break;
                case Models.Enums.CompanyType.Praia:
                    //this.BackgroundColor = Color.FromHex("#009145");     
                    this.BackgroundImage = "bg_praia";
                    this.BackgroundColor = Color.Transparent;
                    lblHeaderTitle.TextColor = (Color)App.Current.Resources["BarracaColor"];
                    imgSearch.Source = "ic_search_praia";
                    //imgMenu.Source = "ic_menu_praia";
                    imgMenu.Source = "ic_voltar_praia";
                    lblHeaderTitle.Text = AppResource.lblBeachs.ToUpper();
                    break;
                case Enums.CompanyType.EsporteEvento:
                    this.BackgroundColor = (Color)App.Current.Resources["EsportesColor"];
                    lblHeaderTitle.TextColor = (Color)App.Current.Resources["EsportesColor"];
                    imgSearch.Source = "ic_search_esportes";
                    //imgMenu.Source = "ic_menu_esportes";
                    imgMenu.Source = "ic_voltar_esportes";
                    lblHeaderTitle.Text = AppResource.lblCruises.ToUpper();
                    break;
                default:
                    break;
            }



        }


        private async void OnMenuTapped(object sender, EventArgs e)
        {
            await App.AppCurrent.NavigationService.NavigateSetRootAsync(new HomePage(), null, false);

            //var obj = sender as Image;
            //Device.BeginInvokeOnMainThread(() =>
            //{
            //    try
            //    {
            //        obj.ScaleTo(1.4, 75).ContinueWith((t) =>
            //        {
            //            try
            //            {
            //                obj.ScaleTo(1.0, 75);
            //            }
            //            catch
            //            {
            //            }
            //        },
            //        scheduler: TaskScheduler.FromCurrentSynchronizationContext());
            //    }
            //    catch
            //    {
            //    }
            //});


            //App app = Application.Current as App;
            //Naylah.Xamarin.Controls.Pages.MasterDetailNavigationPage md = (Naylah.Xamarin.Controls.Pages.MasterDetailNavigationPage)app.MainPage;
            //md.IsPresented = true;

        }

        private async void OnSearchTapped(object sender, EventArgs e)
        {
            await App.AppCurrent.NavigationService.NavigateModalAsync(new FilterPage(), null, true);
        }

        public async void LoadCompanies()
        {
            //App.AppCurrent.GetUserLocation();

            CompanyService service = new CompanyService();

            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);


                var result = await service.GetCompaniesByFilter();
                ListCompany = JsonConvert.DeserializeObject<List<Company>>(result);

                if (ListCompany.Count > 0)
                    LoadItens(ListCompany.Count());




            }
            catch (Exception ex)
            {


            }
            finally
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
        }

        public void LoadItens(int totalImages)
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
            grid.VerticalOptions = LayoutOptions.CenterAndExpand;

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
                StackLayout stk = new StackLayout();
                stk.StyleId = ListCompany[i].Id;
                stk.VerticalOptions = LayoutOptions.CenterAndExpand;
                stk.HorizontalOptions = LayoutOptions.CenterAndExpand;
                stk.Margin = new Thickness(0, 0, 0, 10);
                stk.Spacing = 0;

                var detailsRecognizer = new TapGestureRecognizer();
                detailsRecognizer.Tapped += DetailsRecognizer_Tapped;
                stk.GestureRecognizers.Add(detailsRecognizer);


                double fontSize = 10;
                double distanceFontsize = 8;
                double imageSize = Constants.Constantes.CircleItensSize;
                double iconSize = 32;

                if (Device.Idiom == TargetIdiom.Tablet)
                {
                    imageSize = 120;
                    iconSize = 48;
                    fontSize = 14;
                    distanceFontsize = 12;
                }

                CircleImage image = new CircleImage();
                image.Aspect = Aspect.AspectFill;
                image.Source = ListCompany[i].ImageUri;
                image.StyleId = ListCompany[i].Id;
                image.WidthRequest = imageSize;
                image.HeightRequest = imageSize;
                image.FillColor = Color.Gray;

                image.VerticalOptions = LayoutOptions.Center;
                image.HorizontalOptions = LayoutOptions.Center;

                Image imageFavorite = new Image();
                imageFavorite.HeightRequest = iconSize;
                imageFavorite.Aspect = Aspect.AspectFit;
                if (ListCompany[i].IsFavorited)
                    imageFavorite.Source = "ic_heart_list";
                else
                    imageFavorite.Source = "ic_heart_list_off";

                imageFavorite.StyleId = i.ToString();
                imageFavorite.VerticalOptions = LayoutOptions.StartAndExpand;
                imageFavorite.HorizontalOptions = LayoutOptions.EndAndExpand;

                Image imageStar = new Image();
                imageStar.HeightRequest = iconSize;
                imageStar.Aspect = Aspect.AspectFit;
                imageStar.Source = ListCompany[i].CompanyStarImage;
                imageStar.StyleId = ListCompany[i].Id;
                imageStar.VerticalOptions = LayoutOptions.EndAndExpand;
                imageStar.HorizontalOptions = LayoutOptions.EndAndExpand;

                Label lblHotelName = new Label();
                lblHotelName.FontSize = fontSize;
                lblHotelName.Margin = new Thickness(0, 5, 0, 0);
                lblHotelName.TextColor = ListCompany[i].CompanyColor;
                lblHotelName.LineBreakMode = LineBreakMode.TailTruncation;
                lblHotelName.VerticalTextAlignment = TextAlignment.Center;
                lblHotelName.HorizontalTextAlignment = TextAlignment.Center;
                lblHotelName.Text = ListCompany[i].Title;

                Label lblDistance = new Label();
                lblDistance.FontSize = distanceFontsize;
                lblDistance.HorizontalTextAlignment = TextAlignment.Center;
                lblDistance.VerticalTextAlignment = TextAlignment.Center;
                lblDistance.Text = ListCompany[i].DistanceString;


                Grid grd = new Grid();
                grd.Children.Add(image);
                grd.Children.Add(imageFavorite);
                grd.Children.Add(imageStar);

                stk.Children.Add(grd);
                stk.Children.Add(lblHotelName);
                stk.Children.Add(lblDistance);

                grid.Children.Add(stk, column, row);

                column++;

                if (column > 2)
                    column = 0;

                if (column == 0)
                    row++;

            }



            stkItens.Children.Add(grid);

        }

        private async void DetailsRecognizer_Tapped(object sender, EventArgs e)
        {
            var obj = sender as StackLayout;
            Device.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    obj.ScaleTo(1.3, 75).ContinueWith((t) =>
                    {
                        try
                        {
                            obj.ScaleTo(1.0, 75);
                        }
                        catch
                        {
                        }
                    },
                    scheduler: TaskScheduler.FromCurrentSynchronizationContext());
                }
                catch
                {
                }
            });
            var company = ListCompany.FirstOrDefault(m => m.Id == obj.StyleId);
            await App.AppCurrent.NavigationService.NavigateModalAsync(new CompanyDetailsPage(company), null, true);

        }
    }
}
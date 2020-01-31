using ImageCircle.Forms.Plugin.Abstractions;
using MocoApp.Controls;
using MocoApp.DTO;
using MocoApp.Extensions;
using MocoApp.Models;
using MocoApp.Resources;
using MocoApp.Services;
using MocoApp.Views.CartFlow;
using MocoApp.Views.Chat;
using MocoApp.Views.ClientCheckinFlow;
using MocoApp.Views.Cliente;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Share;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MocoApp.Views.Empresa
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CompanyDetailsPage : ContentPage
    {
        Company Company;
        CompanyService service = new CompanyService();
        List<Category> categories = new List<Category>();

        public CompanyDetailsPage(Company company)
        {
            InitializeComponent();
            if (Device.RuntimePlatform == "Android")
                frmXaml.CornerRadius = 30;

            NavigationPage.SetHasNavigationBar(this, false);

            Company = company;

            imgHeart.Source = company.IsFavorited ? "ic_heart_detail" : "ic_heart_detail_off";
            imgStar.Source = company.CompanyStarImageDetail;
            lblAvaliacoes.Text = "(" + company.TotalRating + " " + AppResource.textRatings;
            lblName.Text = company.Title;
            lblAddress.Text = company.Address;
            lblPhone.Text = company.Cellphone;
            imgUser.Source = company.ImageUri;
            lblInfoDistance.Text = company.DistanceString;

            ColorPage();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            LoadCompany();

            MessagingCenter.Subscribe<object>(this, "Push", (sender) =>
            {
                if (App.AppCurrent.NavigationService.ModalCurrentPage is CompanyDetailsPage)
                {
                    LoadCompany();
                }
            });

            //LoadCartValues();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<Page, string>(this, "Push");
        }

        public void ColorPage()
        {
            switch (App.CompanyTypeSelected)
            {
                case Models.Enums.CompanyType.Hotel:
                    this.BackgroundColor = (Color)App.Current.Resources["HotelColor"];
                    lblName.TextColor = (Color)App.Current.Resources["HotelColor"];
                    actPiece.Color = (Color)App.Current.Resources["HotelColor"];
                    imgBack.Source = "ic_voltar_hotel";
                    break;
                case Models.Enums.CompanyType.Restaurante:
                    this.BackgroundColor = (Color)App.Current.Resources["RestauranteColor"];
                    lblName.TextColor = (Color)App.Current.Resources["RestauranteColor"];
                    actPiece.Color = (Color)App.Current.Resources["RestauranteColor"];
                    imgBack.Source = "ic_voltar_restaurante";
                    break;
                case Models.Enums.CompanyType.Praia:
                    this.BackgroundImage = "bg_praia";
                    this.BackgroundColor = Color.Transparent;
                    lblName.TextColor = (Color)App.Current.Resources["BarracaColor"];
                    actPiece.Color = (Color)App.Current.Resources["BarracaColor"];
                    imgBack.Source = "ic_voltar_praia";
                    break;
                case Enums.CompanyType.EsporteEvento:
                    this.BackgroundColor = (Color)App.Current.Resources["EsportesColor"];
                    lblName.TextColor = (Color)App.Current.Resources["EsportesColor"];
                    actPiece.Color = (Color)App.Current.Resources["EsportesColor"];
                    imgBack.Source = "ic_voltar_esportes";
                    break;
                default:
                    break;
            }
        }

        private async void OnRatingTapped(object sender, EventArgs e)
        {

            if (Helpers.Settings.DisplayUserRole == Enums.UserRole.Guest.ToString())
            {
                App.AppCurrent.AlertToGuest();
                return;
            }

            await App.AppCurrent.NavigationService.NavigateModalAsync(new CompanyRatePage(Company), null, true);
        }

        private async void OnMeusPedidosTapped(object sender, EventArgs e)
        {
            var obj = sender as Image;
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


            if (Helpers.Settings.DisplayUserRole == Enums.UserRole.Guest.ToString())
            {
                App.AppCurrent.AlertToGuest();
                return;
            }

            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading();

                OrderService Orderservice = new OrderService();
                var resultCk = await Orderservice.GetCheckinByClienteAndCompanyId(Company.Id);
                var resultMyOrders = JsonConvert.DeserializeObject<MultiLocationRequestCheckinDTO>(resultCk);

                if (resultMyOrders.Checkin != null)
                {
                    if (Company.HasLocation)
                        await App.AppCurrent.NavigationService.NavigateModalAsync(new ListLocationBillsPage(resultMyOrders.Checkin, Company), null, false);
                    else
                        await App.AppCurrent.NavigationService.NavigateModalAsync(new ListPedidoPage(resultMyOrders.Checkin, Company), null, false);
                }
                else
                {
                    await this.DisplayAlert(MocoApp.Resources.AppResource.alertAlert, AppResource.alertNeedMakeCheckin, AppResource.textOk);
                }





            }
            catch (Exception ex)
            {

                await this.DisplayAlert(MocoApp.Resources.AppResource.alertAlert, AppResource.alertErrorOcured, AppResource.textOk);

            }
            finally
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();

            }


        }

        private async void OnChatTapped(object sender, EventArgs e)
        {
            var obj = sender as Image;
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


            if (Helpers.Settings.DisplayUserRole == Enums.UserRole.Guest.ToString())
            {
                App.AppCurrent.AlertToGuest();
                return;
            }


            await App.AppCurrent.NavigationService.NavigateModalAsync(new ChatPage(Company.Id, Company.Title, Company.ImageUri), null, true);
        }
        private async void OnSolicitarAtendimentoTapped(object sender, EventArgs e)
        {
            var obj = sender as Image;
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


            if (Helpers.Settings.DisplayUserRole == Enums.UserRole.Guest.ToString())
            {
                App.AppCurrent.AlertToGuest();
                return;
            }

            try
            {
                var answer = await DisplayAlert(AppResource.alertAttedentRequest, AppResource.alertWantAttendent, AppResource.textOk, AppResource.alertCancel);

                if (!answer)
                    return;

                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);
                var result = await service.SolicitarAtendimento(Company.Id);

                await this.DisplayAlert(AppResource.alertWaitAttedent, AppResource.alertAttedentRequestSucess, AppResource.textOk);


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

        private async void OnCheckinTapped(object sender, EventArgs e)
        {
            var obj = sender as Image;
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

            if (Helpers.Settings.DisplayUserRole == Enums.UserRole.Guest.ToString())
            {
                App.AppCurrent.AlertToGuest();
                return;
            }

            try
            {
                if (Company.HasLocation)
                    await App.AppCurrent.NavigationService.NavigateModalAsync(new MultiLocationRequestCheckin(Company), null, true);
                else if (!Company.CheckedIn)
                    await App.AppCurrent.NavigationService.NavigateModalAsync(new SingleLocationRequestCheckinPage(Company), null, true);
                else
                {

                    CompanyService service = new CompanyService();
                    string msg = AppResource.alertWantCloseLocationBill;
                    string title = AppResource.textCheckout;

                    var answer = await DisplayAlert(title, msg, AppResource.textOk, AppResource.alertCancel);

                    if (!answer)
                        return;

                    Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);

                    try
                    {
                        var result = await service.RequestCheckoutFromClient(Company.Id, 0);
                        Acr.UserDialogs.UserDialogs.Instance.Toast(AppResource.alertRequestSucess);
                        App.AppCurrent.NavigationService.ModalGoBack();

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

        private async void OnShareTapped(object sender, EventArgs e)
        {
            var obj = sender as Image;
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
            await CrossShare.Current.Share(new Plugin.Share.Abstractions.ShareMessage
            {
                Title = "HiCharlie",
                Text = string.Format(AppResource.textHiCharlieShare, Company.Title)
            });


        }

        private async void OnBackTapped(object sender, EventArgs e)
        {
            await App.AppCurrent.NavigationService.ModalGoBack();
        }


        private async void OnInfoTapped(object sender, EventArgs e)
        {
            await App.AppCurrent.NavigationService.NavigateModalAsync(new CompanyInfoPage(Company), null, true);
        }

        private async void OnFavoriteTapped(object sender, EventArgs e)
        {
            var obj = sender as Image;
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

            if (Helpers.Settings.DisplayUserRole == Enums.UserRole.Guest.ToString())
            {
                App.AppCurrent.AlertToGuest();
                return;
            }

            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);


                var result = await service.AddRemoveCompanyFavorite(Company.Id);
                Company.IsFavorited = JsonConvert.DeserializeObject<bool>(result);
                imgHeart.Source = Company.IsFavorited ? "ic_heart_detail" : "ic_heart_detail_off";


            }
            catch (Exception ex)
            {


            }
            finally
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }

        }


        public async void LoadCompany()
        {


            try
            {
                ShowLoading(true);


                var result = await service.GetCompanyById(Company.Id);
                var _company = JsonConvert.DeserializeObject<Company>(result);
                Company.HasCheckinInAnotherCompany = _company.HasCheckinInAnotherCompany;
                Company.CompanyName = _company.CompanyName;
                Company.Differentials = _company.Differentials;
                Company.Payments = _company.Payments;
                Company.Categories = _company.Categories;
                Company.CheckedIn = _company.CheckedIn;
                Company.Locations = _company.Locations;
                Company.HasMessageNotRead = _company.HasMessageNotRead;
                Company.InformativeMenus = _company.InformativeMenus;

                App.AppCurrent.CompanyCulture = _company.CurrencyType.ToCultureInfo();

                if (_company.HasMessageNotRead)
                    imgChat.Source = "ic_chat_msg";


                lblAvaliacoes.Text = "(" + _company.TotalRating + " " + AppResource.textRatings;
                lblName.Text = _company.Title;
                lblAddress.Text = _company.Address;
                lblPhone.Text = _company.Cellphone;
                imgStar.Source = Company.CompanyStarImage;
                //20190421
                //lblCheckinText.Text = Company.CheckedIn ? AppResource.lblCloseBill : AppResource.lblStartOrder;
                //imgCheckin.Source = Company.CheckedIn ? "ic_detail_checkout" : "ic_detail_checkin";

                //if (_company.Differentials.Count > 0)
                //    LoadDifferentials(_company.Differentials.ToList());
                //else
                grdDiferenciais.IsVisible = false;


                if (_company.HasLocation)
                {
                    if (_company.Locations != null && _company.Locations.Count > 0)
                    {
                        LoadLocations(_company.Locations.Count());
                    }
                }
                else
                {
                    if (_company.Categories != null && _company.Categories.Count > 0)
                    {
                        LoadCategories(_company.Categories.Count());
                    }
                }



            }
            catch (Exception ex)
            {


            }
            finally
            {
                ShowLoading(false);
            }
        }

        public void LoadCategories(int totalImages)
        {
            stkItens.Children.Clear();
            int count = totalImages + Company.InformativeMenus.Count();
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
            for (int i = 0; i < totalImages; i++)
            {
                categories.Add(new Category() { Id = Company.Categories[i].Id, ImageUri = Company.Categories[i].ImageUri });

                StackLayout stk = new StackLayout();
                stk.StyleId = Company.Categories[i].Id;
                stk.VerticalOptions = LayoutOptions.CenterAndExpand;
                stk.HorizontalOptions = LayoutOptions.CenterAndExpand;
                stk.Margin = new Thickness(0, 0, 0, 10);
                stk.Spacing = 0;

                var detailsRecognizer = new TapGestureRecognizer();
                detailsRecognizer.Tapped += DetailsRecognizer_Tapped;
                stk.GestureRecognizers.Add(detailsRecognizer);

                CircleImage image = new CircleImage();
                image.Aspect = Aspect.AspectFill;
                image.FillColor = Color.FromHex("#ccc");
                image.Source = Company.Categories[i].ImageUri;
                image.StyleId = Company.Categories[i].Id;
                image.WidthRequest = Constants.Constantes.CircleItensSize;
                image.HeightRequest = Constants.Constantes.CircleItensSize;
                image.VerticalOptions = LayoutOptions.Center;
                image.HorizontalOptions = LayoutOptions.Center;

                double labelSize = 10;
                if (Device.Idiom == TargetIdiom.Tablet)
                    labelSize = 16;

                Label lblHotelName = new Label();
                lblHotelName.FontSize = labelSize;
                lblHotelName.TextColor = Color.FromHex("#212121");
                lblHotelName.Margin = new Thickness(0, 5, 0, 0);
                lblHotelName.VerticalTextAlignment = TextAlignment.Center;
                lblHotelName.HorizontalTextAlignment = TextAlignment.Center;
                lblHotelName.Text = Company.Categories[i].Name;


                stk.Children.Add(image);
                stk.Children.Add(lblHotelName);

                grid.Children.Add(stk, column, row);

                column++;

                if (column > 2)
                    column = 0;

                if (column == 0)
                    row++;
            }
            var aux = 0;
            for (int i = totalImages; i < count; i++)
            {
                StackLayout stk = new StackLayout();
                stk.StyleId = Company.InformativeMenus[aux].Id;
                stk.VerticalOptions = LayoutOptions.CenterAndExpand;
                stk.HorizontalOptions = LayoutOptions.CenterAndExpand;
                stk.Margin = new Thickness(0, 0, 0, 10);
                stk.Spacing = 0;

                //var detailsRecognizer = new TapGestureRecognizer();
                //detailsRecognizer.Tapped += DetailsInformativeMenuRecognizer_Tapped;
                //stk.GestureRecognizers.Add(detailsRecognizer);

                CircleImage image = new CircleImage();
                image.Aspect = Aspect.AspectFill;
                image.FillColor = Color.FromHex("#ccc");
                //image.Source = Company.Categories[i].ImageUri;
                image.StyleId = Company.InformativeMenus[aux].Id;
                image.WidthRequest = Constants.Constantes.CircleItensSize;
                image.HeightRequest = Constants.Constantes.CircleItensSize;
                image.VerticalOptions = LayoutOptions.Center;
                image.HorizontalOptions = LayoutOptions.Center;

                double labelSize = 10;
                if (Device.Idiom == TargetIdiom.Tablet)
                    labelSize = 16;

                Label lblHotelName = new Label();
                lblHotelName.FontSize = labelSize;
                lblHotelName.TextColor = Color.FromHex("#212121");
                lblHotelName.Margin = new Thickness(0, 5, 0, 0);
                lblHotelName.VerticalTextAlignment = TextAlignment.Center;
                lblHotelName.HorizontalTextAlignment = TextAlignment.Center;
                lblHotelName.Text = Company.InformativeMenus[aux].Name;

                stk.Children.Add(image);
                stk.Children.Add(lblHotelName);

                grid.Children.Add(stk, column, row);

                column++;

                if (column > 2)
                    column = 0;

                if (column == 0)
                    row++;

                aux++;
            }

            stkItens.Children.Add(grid);

        }

        public void LoadLocations(int totalImages)
        {
            stkItens.Children.Clear();
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
                stk.StyleId = Company.Locations[i].Id;
                stk.VerticalOptions = LayoutOptions.CenterAndExpand;
                stk.HorizontalOptions = LayoutOptions.CenterAndExpand;
                stk.Margin = new Thickness(0, 0, 0, 10);
                stk.Spacing = 0;

                var detailsRecognizer = new TapGestureRecognizer();
                detailsRecognizer.Tapped += Location_Tapped;
                stk.GestureRecognizers.Add(detailsRecognizer);

                CircleImage image = new CircleImage();
                image.Aspect = Aspect.AspectFill;
                image.FillColor = Color.FromHex("#ccc");
                image.Source = Company.Locations[i].ImageUri;
                image.StyleId = Company.Locations[i].Id;
                image.WidthRequest = Constants.Constantes.CircleItensSize;
                image.HeightRequest = Constants.Constantes.CircleItensSize;
                image.VerticalOptions = LayoutOptions.Center;
                image.HorizontalOptions = LayoutOptions.Center;


                float categoryFontSize = 10;
                if (Device.Idiom == TargetIdiom.Tablet)
                    categoryFontSize = 16;

                Label lblHotelName = new Label();
                lblHotelName.FontSize = categoryFontSize;
                lblHotelName.TextColor = Color.FromHex("#212121");
                lblHotelName.Margin = new Thickness(0, 5, 0, 0);
                lblHotelName.VerticalTextAlignment = TextAlignment.Center;
                lblHotelName.HorizontalTextAlignment = TextAlignment.Center;
                lblHotelName.Text = Company.Locations[i].Name;


                stk.Children.Add(image);
                stk.Children.Add(lblHotelName);

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
            var uri = categories.FirstOrDefault(x => x.Id == obj.StyleId).ImageUri;

            await App.AppCurrent.NavigationService.NavigateModalAsync(new CompanyProductList(obj.StyleId, Company, null, uri), null, true);
        }
        private async void DetailsInformativeMenuRecognizer_Tapped(object sender, EventArgs e)
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
            var uri = categories.FirstOrDefault(x => x.Id == obj.StyleId).ImageUri;

            await App.AppCurrent.NavigationService.NavigateModalAsync(new CompanyProductList(obj.StyleId, Company, null, uri), null, true);
        }
        private async void Location_Tapped(object sender, EventArgs e)
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
            var location = Company.Locations.Where(m => m.Id == obj.StyleId).FirstOrDefault();
            //if(location.MenuType == Enums.EMenuType.Category)
            await App.AppCurrent.NavigationService.NavigateModalAsync(new CompanyCategoryListPage(Company, location), null, true);
            //else
            //    await App.AppCurrent.NavigationService.NavigateModalAsync(new CompanyInformativeMenuListPage(Company, location), null, true);
        }

        public void LoadDifferentials(List<Differential> list)
        {
            //foreach (var item in list)
            //{
            //    Image img = new Image();
            //    img.Source = item.ImageUri;
            //    img.WidthRequest = 24;
            //    img.HeightRequest = 24;
            //    img.Aspect = Aspect.AspectFill;
            //    img.Margin = new Thickness(3, 0, 3, 0);

            //    stkDiferenciais.Children.Add(img);
            //}

        }

        private void ShowLoading(bool show)
        {
            actPiece.IsRunning = show;
            actPiece.IsVisible = show;
            actPiece.IsEnabled = show;
        }

        private async void OnCart_Tapped(object sender, EventArgs e)
        {
            await App.AppCurrent.NavigationService.NavigateModalAsync(new ProductsCart(), null, true);

        }
    }
}
using MocoApp.DTO;
using MocoApp.Models;
using MocoApp.Resources;
using MocoApp.Services;
using MocoApp.Views.Chat;
using MocoApp.Views.ClientCheckinFlow;
using MocoApp.Views.Cliente;
using Newtonsoft.Json;
using Plugin.Share;
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
    public partial class CompanyProductList : ContentPage
    {
        Company Company;
        Location Location;
        ApiService service = new ApiService();
        CompanyService companyService = new CompanyService();
        string _categoryId = "";
        List<CategoryGroup> ListProduct = new List<CategoryGroup>();

        public CompanyProductList(string categoryId, Company company = null, Location location = null, string categoryUri = "")
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            if (Device.RuntimePlatform == "Android")
                frmXaml.CornerRadius = 30;

            Company = company;
            Location = location;
            _categoryId = categoryId;
            ColorPage();

            if (Company.HasMessageNotRead)
                imgChat.Source = "ic_chat_msg";

            imgHeart.Source = company.IsFavorited ? "ic_heart_detail" : "ic_heart_detail_off";
            imgStar.Source = company.CompanyStarImageDetail;
            lblAvaliacoes.Text = "(" + company.TotalRating + " " + AppResource.textRatings;
            lblName.Text = company.Title;
            lblAddress.Text = company.Address;
            lblPhone.Text = company.Cellphone;
            imgUser.Source = company.ImageUri;

            if (Location != null)
            {
                lblName.Text = Location.Name;
                imgUser.Source = Location.ImageUri;
            }
            if (!string.IsNullOrEmpty(categoryUri))
                imgUser.Source = categoryUri;



            LoadItens();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadCompanyInfo();
            MessagingCenter.Subscribe<object>(this, "Push", (sender) =>
            {
                if (App.AppCurrent.NavigationService.ModalCurrentPage is CompanyProductList)
                {
                    LoadCompanyInfo();
                    LoadItens();
                }
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<object, string>(this, "Push");
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

        private async void OnBackTapped(object sender, EventArgs e)
        {
            await App.AppCurrent.NavigationService.ModalGoBack();
        }
        private async void OnInfoTapped(object sender, EventArgs e)
        {
            await App.AppCurrent.NavigationService.NavigateModalAsync(new CompanyInfoPage(Company), null, true);
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

            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);


                var result = await companyService.AddRemoveCompanyFavorite(Company.Id);
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

        private void ShowLoading(bool show)
        {
            actPiece.IsRunning = show;
            actPiece.IsVisible = show;
            actPiece.IsEnabled = show;
        }
        public async void LoadCompanyInfo()
        {
            //var checkin = await companyService.IsCheckedIn(Company.Id, Helpers.Settings.DisplayUserToken);
            //if (checkin)
            //{
            //    lblCheckinText.Text = AppResource.textCheckout;
            //    imgCheckin.Source = "ic_detail_checkout";
            //}
            //else
            //{
            //    lblCheckinText.Text = AppResource.textCheckin;
            //    imgCheckin.Source = "ic_detail_checkin";
            //}

        }
        public async void LoadItens()
        {
            try
            {
                ShowLoading(true);


                var result = await companyService.GetGroupsAndProductsByCategoryId(_categoryId);
                ListProduct = JsonConvert.DeserializeObject<List<CategoryGroup>>(result);

                if (ListProduct.Count > 0)
                {
                    LoadProducts();

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



        public async void LoadProducts()
        {
            stkItens.Children.Clear();
            foreach (var itemGroup in ListProduct)
            {

                float groupFontSize = 18;
                if (Device.Idiom == TargetIdiom.Tablet)
                    groupFontSize = 22;

                Label lblgroup = new Label();
                lblgroup.FontSize = groupFontSize;
                lblgroup.FontAttributes = FontAttributes.Bold;
                lblgroup.Margin = new Thickness(0, 15, 0, 15);
                lblgroup.TextColor = Color.FromHex("#212121");
                lblgroup.HorizontalTextAlignment = TextAlignment.Start;
                lblgroup.Text = itemGroup.Name;

                stkItens.Children.Add(lblgroup);

                foreach (var item in itemGroup.Products)
                {
                    var grid = new Grid
                    {
                        ColumnDefinitions =
                            {
                                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) }
                            }
                    };


                    StackLayout stk = new StackLayout();
                    stk.Margin = new Thickness(0, 0, 0, 10);
                    stk.Spacing = 0;

                    var detailsRecognizer = new TapGestureRecognizer();
                    detailsRecognizer.Tapped += DetailsRecognizer_Tapped;
                    grid.GestureRecognizers.Add(detailsRecognizer);
                    grid.StyleId = item.Id;

                    StackLayout stkImages = new StackLayout();
                    stkImages.Orientation = StackOrientation.Horizontal;
                    stkImages.HorizontalOptions = LayoutOptions.Center;
                    stkImages.Margin = new Thickness(0, 0, 0, 10);
                    stkImages.Spacing = 0;

                    StackLayout stkprice = new StackLayout();
                    stkprice.Margin = new Thickness(0, 0, 0, 10);
                    stkprice.Spacing = 0;

                    ImageCircle.Forms.Plugin.Abstractions.CircleImage image = new ImageCircle.Forms.Plugin.Abstractions.CircleImage();
                    image.HeightRequest = 50;
                    image.WidthRequest = 50;
                    image.FillColor = Color.FromHex("#ccc");
                    image.HorizontalOptions = LayoutOptions.Center;
                    image.VerticalOptions = LayoutOptions.Center;
                    image.Aspect = Aspect.AspectFill;
                    image.Source = item.ImageUri;
                    image.StyleId = item.Id;


                    float nameFontSize = 12;
                    float commentFontSize = 10;
                    float priceFontSize = 12;

                    if (Device.Idiom == TargetIdiom.Tablet)
                    {
                        nameFontSize = 18;
                        commentFontSize = 16;
                        priceFontSize = 18;
                    }

                    Label lblName = new Label();
                    lblName.FontSize = nameFontSize;
                    lblName.TextColor = Color.FromHex("#212121");
                    lblName.HorizontalTextAlignment = TextAlignment.Start;
                    lblName.Text = item.Name;

                    Label lblComentario = new Label();
                    lblComentario.FontSize = commentFontSize;
                    lblComentario.TextColor = Color.Silver;
                    lblComentario.HorizontalTextAlignment = TextAlignment.Start;
                    lblComentario.Text = item.Description;

                    Label lblPrice = new Label();
                    lblPrice.FontSize = priceFontSize;
                    lblPrice.TextColor = Color.Green;
                    lblPrice.HorizontalTextAlignment = TextAlignment.Start;
                    lblPrice.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", item.Price);

                    Image imgStar = new Image();
                    imgStar.WidthRequest = 16;
                    imgStar.Source = item.ProductStarImage;

                    Image imgFavorite = new Image();
                    imgFavorite.WidthRequest = 16;
                    imgFavorite.Source = "ic_heart_list";

                    BoxView box = new BoxView();
                    box.HeightRequest = 1;
                    box.WidthRequest = 500;
                    box.Color = Color.FromHex("#ccc");


                    if (item.IsFavorited)
                        stkImages.Children.Add(imgFavorite);

                    //stkImages.Children.Add(imgStar);
                    stkprice.Children.Add(lblPrice);
                    stkprice.Children.Add(stkImages);

                    stk.Children.Add(lblName);
                    stk.Children.Add(lblComentario);

                    if (!string.IsNullOrEmpty(item.ImageUri))
                        grid.Children.Add(image, 0, 0);

                    grid.Children.Add(stk, 1, 0);
                    grid.Children.Add(stkprice, 2, 0);

                    stkItens.Children.Add(grid);
                    stkItens.Children.Add(box);
                }
            }

            ShowLoading(false);
        }

        private async void DetailsRecognizer_Tapped(object sender, EventArgs e)
        {
            var obj = sender as Grid;
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

            var aa = ListProduct.Select(m => m.Products.Where(x => x.Id == obj.StyleId).FirstOrDefault()).ToList();
            var aux = new List<Product>();
            foreach (var item in aa)
            {
                if (item != null)
                    aux.Add(item);
            }

            var res = aux.LastOrDefault();
            if (res == null)
                res = aux.FirstOrDefault();

            await App.AppCurrent.NavigationService.NavigateModalAsync(new CompanyOrderPage(Company, res), null, true);
        }




        #region xnd
        private async void OnSolicitarAtendimentoTapped(object sender, EventArgs e)
        {
            if (Helpers.Settings.DisplayUserRole == Enums.UserRole.Guest.ToString())
            {
                App.AppCurrent.AlertToGuest();
                return;
            }

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
                var result = await companyService.SolicitarAtendimento(Company.Id);

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
                if (!Company.CheckedIn)
                {
                    await App.AppCurrent.NavigationService.NavigateModalAsync(new SingleLocationRequestCheckinPage(Company), null, true);
                }
                else
                {

                    string msg = AppResource.alertWantCloseLocationBill;
                    string title = AppResource.textCheckout;

                    var answer = await DisplayAlert(title, msg, AppResource.textOk, AppResource.alertCancel);

                    if (!answer)
                        return;

                    Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);

                    try
                    {
                        var result = await companyService.RequestCheckoutFromClient(Company.Id, 0);
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

        private async void OnMeusPedidosTapped(object sender, EventArgs e)
        {
            if (Helpers.Settings.DisplayUserRole == Enums.UserRole.Guest.ToString())
            {
                App.AppCurrent.AlertToGuest();
                return;
            }

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


            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading();

                Acr.UserDialogs.UserDialogs.Instance.ShowLoading();

                OrderService Orderservice = new OrderService();
                var resultCk = await Orderservice.GetCheckinByClienteAndCompanyId(Company.Id);
                var resultMyOrders = JsonConvert.DeserializeObject<MultiLocationRequestCheckinDTO>(resultCk);

                if (resultMyOrders.Checkin != null)
                {
                    await App.AppCurrent.NavigationService.NavigateModalAsync(new ListPedidoPage(resultMyOrders.Checkin, Company), null, false);
                }
                else
                {
                    await this.DisplayAlert(MocoApp.Resources.AppResource.alertAlert, AppResource.alertNeedMakeCheckin, AppResource.textOk);
                }


            }
            catch (Exception)
            {


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

        #endregion


    }
}
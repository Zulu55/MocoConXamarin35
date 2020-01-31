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
using Plugin.Share;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static MocoApp.Models.Enums;

namespace MocoApp.Views.Empresa
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CompanyCategoryListPage : ContentPage
    {
        private readonly Company _company;
        private readonly CompanyService _service = new CompanyService();
        private List<Category> _categories;
        private List<CategoryGroup> _listCategoryGroup;
        private List<Product> _listProduct = new List<Product>();
        private readonly Location _location;
        private CheckinSub _checkinSub;
        private string _categoryId;
        private readonly string _seletedCategory;
        private static CompanyCategoryListPage _instance;

        public CompanyCategoryListPage(Company company, Location location = null)
        {
            InitializeComponent();

            _instance = this;
            _location = location;

            if (Device.RuntimePlatform == "Android")
            {
                frmXaml.CornerRadius = 30;
            }

            _seletedCategory = string.Empty;
            _company = company;

            if (_company.HasMessageNotRead)
            {
                imgChat.Source = "ic_chat_msg";
            }

            lblName.Text = _location.Name;
            lblAddress.Text = company.Address;
            lblPhone.Text = company.Cellphone;
            imgUser.Source = _location.ImageUri;
            lblInfoDistance.Text = company.DistanceString;
            lblAvaliacoes.Text = "(" + _company.TotalRating + AppResource.textRatings;

            ColorPage();
        }

        public bool InformativeWithoutPrice => _location.MenuType == EMenuType.InformativeWithoutPrice;

        public static CompanyCategoryListPage GetInstance()
        {
            return _instance;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadCompany();

            MessagingCenter.Subscribe<object>(this, "Push", (sender) =>
            {
                object page = sender;
                if (App.AppCurrent.NavigationService.ModalCurrentPage is CompanyCategoryListPage)
                {
                    LoadCompany();
                }
            });

            LoadCartValues();
        }

        public void LoadCartValues()
        {
            grdCart.IsVisible = App.AppCurrent.Cart.TotalOrdersInCart > 0 ? true : false;


            if (!grdCart.IsVisible)
            {
                return;
            }

            lblCartTotalPrice.Text = App.AppCurrent.Cart.TotalPrice;
            lblCartOrdersCount.Text = App.AppCurrent.Cart.TotalOrdersInCart?.ToString();
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
                    BackgroundColor = (Color)App.Current.Resources["HotelColor"];
                    lblName.TextColor = (Color)App.Current.Resources["HotelColor"];
                    actPiece.Color = (Color)App.Current.Resources["HotelColor"];
                    imgBack.Source = "ic_voltar_hotel";
                    break;
                case Models.Enums.CompanyType.Restaurante:
                    BackgroundColor = (Color)App.Current.Resources["RestauranteColor"];
                    lblName.TextColor = (Color)App.Current.Resources["RestauranteColor"];
                    actPiece.Color = (Color)App.Current.Resources["RestauranteColor"];
                    imgBack.Source = "ic_voltar_restaurante";
                    break;
                case Models.Enums.CompanyType.Praia:
                    BackgroundImage = "bg_praia";
                    BackgroundColor = Color.Transparent;
                    lblName.TextColor = (Color)App.Current.Resources["BarracaColor"];
                    actPiece.Color = (Color)App.Current.Resources["BarracaColor"];
                    imgBack.Source = "ic_voltar_praia";
                    break;
                case Enums.CompanyType.EsporteEvento:
                    BackgroundColor = (Color)App.Current.Resources["EsportesColor"];
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

            await App.AppCurrent.NavigationService.NavigateModalAsync(new CompanyRatePage(_company), null, true);
        }

        private async void OnMeusPedidosTapped(object sender, EventArgs e)
        {
            if (Helpers.Settings.DisplayUserRole == Enums.UserRole.Guest.ToString())
            {
                App.AppCurrent.AlertToGuest();
                return;
            }

            Image obj = sender as Image;
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
                string resultCk = await Orderservice.GetCheckinByClienteAndCompanyId(_company.Id);
                MultiLocationRequestCheckinDTO resultMyOrders = JsonConvert.DeserializeObject<MultiLocationRequestCheckinDTO>(resultCk);

                if (resultMyOrders.Checkin != null)
                {
                    if (_company.HasLocation)
                    {
                        await App.AppCurrent.NavigationService.NavigateModalAsync(new ListLocationBillsPage(resultMyOrders.Checkin, _company, true), null, false);
                    }
                    else
                    {
                        await App.AppCurrent.NavigationService.NavigateModalAsync(new ListPedidoPage(resultMyOrders.Checkin, _company), null, false);
                    }
                }
                else
                {
                    await DisplayAlert(MocoApp.Resources.AppResource.alertAlert, AppResource.alertNeedMakeCheckin, AppResource.textOk);
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

        private async void OnSolicitarAtendimentoTapped(object sender, EventArgs e)
        {
            if (Helpers.Settings.DisplayUserRole == Enums.UserRole.Guest.ToString())
            {
                App.AppCurrent.AlertToGuest();
                return;
            }

            Image obj = sender as Image;
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
                bool answer = await DisplayAlert(AppResource.alertAttedentRequest, AppResource.alertWantAttendent, AppResource.textOk, AppResource.alertCancel);

                if (!answer)
                {
                    return;
                }

                string locationId = "";
                if (_location != null)
                {
                    locationId = _location.Id;
                }

                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);
                string result = await _service.SolicitarAtendimento(_company.Id, locationId);

                await DisplayAlert(AppResource.alertWaitAttedent, AppResource.alertAttedentRequestSucess, AppResource.textOk);


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

        private async void OnCheckinTapped(object sender, EventArgs e)
        {
            Image obj = sender as Image;
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
                if (_company.HasLocation)
                {
                    await App.AppCurrent.NavigationService.NavigateModalAsync(new MultiLocationRequestCheckin(_company), null, true);
                }
                else if (!_company.CheckedIn)
                {
                    await App.AppCurrent.NavigationService.NavigateModalAsync(new SingleLocationRequestCheckinPage(_company), null, true);
                }
                else
                {

                    CompanyService service = new CompanyService();
                    string msg = AppResource.alertWantCloseLocationBill;
                    string title = AppResource.textCheckout;

                    bool answer = await DisplayAlert(title, msg, AppResource.textOk, AppResource.alertCancel);

                    if (!answer)
                    {
                        return;
                    }

                    Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);

                    try
                    {
                        string result = await service.RequestCheckoutFromClient(_company.Id, 0);
                        Acr.UserDialogs.UserDialogs.Instance.Toast(AppResource.alertRequestSucess);
                        App.AppCurrent.NavigationService.ModalGoBack();

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

        private async void OnShareTapped(object sender, EventArgs e)
        {
            Image obj = sender as Image;
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
                Text = string.Format(AppResource.textHiCharlieShare, _company.Title)
            });



        }

        private async void OnBackTapped(object sender, EventArgs e)
        {
            await App.AppCurrent.NavigationService.ModalGoBack();
        }


        private async void OnInfoTapped(object sender, EventArgs e)
        {
            await App.AppCurrent.NavigationService.NavigateModalAsync(new CompanyInfoPage(_company), null, true);
        }

        private async void OnFavoriteTapped(object sender, EventArgs e)
        {
            Image obj = sender as Image;
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


                string result = await _service.AddRemoveCompanyFavorite(_company.Id);
                _company.IsFavorited = JsonConvert.DeserializeObject<bool>(result);
                imgHeart.Source = _company.IsFavorited ? "ic_heart_detail" : "ic_heart_detail_off";


            }
            catch (Exception)
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
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);
                ShowLoading(true);
                lblAddress.Text = _company.Address;
                lblPhone.Text = _company.Cellphone;

                //pegar o subCheckin
                string result = await _service.GetCheckinSubByLocationId(_location.Id);
                CheckinSub subCheckin = JsonConvert.DeserializeObject<CheckinSub>(result);

                if (subCheckin != null)
                {
                    _checkinSub = subCheckin;
                }

                string resultCategories = await _service.GetAllProductCategoryByLocationId(_location.Id, false);
                List<Category> categories = JsonConvert.DeserializeObject<List<Category>>(resultCategories);

                _categories = categories;
                if (categories != null && categories.Count > 0)
                {
                    LoadCategories(categories.Count());
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            finally
            {
                ShowLoading(false);
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
        }

        public void LoadCategories(int totalImages)
        {
            stkCategory.Children.Clear();
            Label lblFirst = new Label();

            for (int i = 0; i < totalImages; i++)
            {

                float categoryFontSize = 16;
                if (Device.Idiom == TargetIdiom.Tablet)
                {
                    categoryFontSize = 22;
                }

                Label lblName = new Label
                {
                    VerticalTextAlignment = TextAlignment.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Text = _categories[i].Name,
                    StyleId = _categories[i].Id,
                    Margin = new Thickness(6, 0, 6, 0),
                    FontSize = categoryFontSize
                };

                TapGestureRecognizer detailsRecognizer = new TapGestureRecognizer();
                detailsRecognizer.Tapped += DetailsRecognizer_Tapped;
                lblName.GestureRecognizers.Add(detailsRecognizer);

                if (i == 0 || _categoryId == _categories[i].Id)
                {
                    lblFirst = lblName;
                }

                stkCategory.Children.Add(lblName);
            }

            if (_categories.Count > 0)
            {
                if (!string.IsNullOrEmpty(_categoryId))
                {
                    DetailsRecognizer_Tapped(lblFirst, null);
                }
                else
                {
                    _categoryId = _categories[0].Id;
                    DetailsRecognizer_Tapped(lblFirst, null);
                }
            }
        }


        private async void DetailsRecognizer_Tapped(object sender, EventArgs e)
        {
            Label obj = sender as Label;

            StackLayout stkFather = obj.Parent as StackLayout;

            foreach (Label item in stkFather.Children)
            {
                item.FontAttributes = FontAttributes.None;
                item.TextColor = Color.Black;
            }


            obj.TextColor = Color.FromHex("#7DA4DC");
            obj.FontAttributes = FontAttributes.Bold;


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


            _categoryId = obj.StyleId;
            LoadItens();


        }

        public async void LoadItens()
        {

            CompanyService CompanyService = new CompanyService();
            try
            {
                stkItens.Children.Clear();
                ShowLoading(true);



                string result = await CompanyService.GetGroupsAndProductsByCategoryId(_categoryId);

                _listCategoryGroup = JsonConvert.DeserializeObject<List<CategoryGroup>>(result);

                if (_listCategoryGroup.Count > 0)
                {
                    LoadProducts();
                    _listProduct = _listCategoryGroup.SelectMany(m => m.Products).ToList();

                }


            }
            catch (Exception)
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
            foreach (CategoryGroup itemGroup in _listCategoryGroup)
            {

                float groupFontSize = 18;
                if (Device.Idiom == TargetIdiom.Tablet)
                {
                    groupFontSize = 24;
                }

                Label lblgroup = new Label
                {
                    FontSize = groupFontSize,
                    FontAttributes = FontAttributes.Bold,
                    Margin = new Thickness(0, 5, 0, 15),
                    TextColor = Color.FromHex("#212121"),
                    HorizontalTextAlignment = TextAlignment.Start,
                    Text = itemGroup.Name
                };

                stkItens.Children.Add(lblgroup);

                foreach (Product item in itemGroup.Products)
                {
                    Grid grid = new Grid
                    {
                        ColumnDefinitions =
                            {
                                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) }
                            }
                    };


                    StackLayout stk = new StackLayout
                    {
                        Margin = new Thickness(0, 0, 0, 10),
                        Spacing = 0
                    };

                    TapGestureRecognizer detailsRecognizer = new TapGestureRecognizer();
                    detailsRecognizer.Tapped += ProductGesture_Tapped;
                    grid.GestureRecognizers.Add(detailsRecognizer);
                    grid.StyleId = item.Id;

                    StackLayout stkImages = new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        HorizontalOptions = LayoutOptions.Center,
                        Margin = new Thickness(0, 0, 0, 10),
                        Spacing = 0
                    };

                    StackLayout stkprice = new StackLayout
                    {
                        Margin = new Thickness(0, 0, 0, 10),
                        Spacing = 0
                    };

                    ImageCircle.Forms.Plugin.Abstractions.CircleImage image = new ImageCircle.Forms.Plugin.Abstractions.CircleImage
                    {
                        HeightRequest = 50,
                        WidthRequest = 50,
                        FillColor = Color.FromHex("#ccc"),
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        Aspect = Aspect.AspectFill,
                        Source = item.ImageUri,
                        StyleId = item.Id
                    };


                    float nameFontSize = 12;
                    float commentFontSize = 10;
                    float priceFontSize = 12;

                    if (Device.Idiom == TargetIdiom.Tablet)
                    {
                        nameFontSize = 18;
                        commentFontSize = 16;
                        priceFontSize = 18;
                    }

                    Label lblName = new Label
                    {
                        FontSize = nameFontSize,
                        TextColor = Color.FromHex("#212121"),
                        HorizontalTextAlignment = TextAlignment.Start,
                        Text = item.Name
                    };

                    Label lblComentario = new Label
                    {
                        FontSize = commentFontSize,
                        TextColor = Color.Silver,
                        HorizontalTextAlignment = TextAlignment.Start,
                        Text = item.Description
                    };

                    Label lblPrice = new Label
                    {
                        FontSize = priceFontSize,
                        TextColor = Color.Green,
                        HorizontalTextAlignment = TextAlignment.Start
                    };
                    //lblPrice.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", item.Price);

                    if (item.ShowElementBySegment)
                    {
                        CultureInfo cult = _company.CurrencyType.ToCultureInfo();
                        lblPrice.Text = string.Format(cult, "{0:C}", item.Price);
                        lblPrice.IsVisible = true;
                    }
                    else
                    {
                        lblPrice.IsVisible = false;
                    }

                    Image imgStar = new Image
                    {
                        WidthRequest = 16,
                        Source = item.ProductStarImage
                    };

                    Image imgFavorite = new Image
                    {
                        WidthRequest = 16,
                        Source = "ic_heart_list"
                    };

                    BoxView box = new BoxView
                    {
                        HeightRequest = 1,
                        WidthRequest = 500,
                        Color = Color.FromHex("#ccc")
                    };


                    if (item.IsFavorited)
                    {
                        stkImages.Children.Add(imgFavorite);
                    }

                    //stkImages.Children.Add(imgStar);
                    stkprice.Children.Add(lblPrice);
                    stkprice.Children.Add(stkImages);

                    stk.Children.Add(lblName);
                    stk.Children.Add(lblComentario);

                    if (!string.IsNullOrEmpty(item.ImageUri))
                    {
                        grid.Children.Add(image, 0, 0);
                    }

                    grid.Children.Add(stk, 1, 0);
                    grid.Children.Add(stkprice, 2, 0);

                    stkItens.Children.Add(grid);
                    stkItens.Children.Add(box);
                }
            }

            ShowLoading(false);
        }

        private async void ProductGesture_Tapped(object sender, EventArgs e)
        {
            Grid obj = sender as Grid;
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

            if (_location.MenuType == Enums.EMenuType.Informative)
            {
                Product menu = _listProduct.Where(m => m.Id == obj.StyleId).FirstOrDefault();

                await App.AppCurrent.NavigationService.NavigateModalAsync(new CompanyInformativeMenuPage(_company, menu.InformativeMenuId, _location), null, true);
            }
            else
            {
                await App.AppCurrent.NavigationService.NavigateModalAsync(new CompanyOrderPage(_company, _listProduct.Where(m => m.Id == obj.StyleId).FirstOrDefault()), null, true);
            }
        }

        private async void Location_Tapped(object sender, EventArgs e)
        {
            StackLayout obj = sender as StackLayout;
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

            await App.AppCurrent.NavigationService.NavigateModalAsync(new CompanyProductList(obj.StyleId, _company), null, true);
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

        private async void OnChatTapped(object sender, EventArgs e)
        {
            Image obj = sender as Image;
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


            await App.AppCurrent.NavigationService.NavigateModalAsync(new ChatPage(_company.Id, _company.Title, _company.ImageUri), null, true);
        }
    }
}
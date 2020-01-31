using MocoApp.Extensions;
using MocoApp.Models;
using MocoApp.Resources;
using MocoApp.Services;
using MocoApp.Views.CartFlow;
using MocoApp.Views.ClientCheckinFlow;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MocoApp.Views.Empresa
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CompanyOrderPage : ContentPage
    {
        private string _entregaTapped = "";
        private readonly Company _company;
        private readonly Product _product;
        private List<CompanyDelivery> _listCompanyDelivery = new List<CompanyDelivery>();

        public CompanyOrderPage(Company company, Product product)
        {
            InitializeComponent();

            if (Device.RuntimePlatform == "Android")
            {
                frmXaml.CornerRadius = 30;
            }

            NavigationPage.SetHasNavigationBar(this, false);

            _company = company;
            _product = product;

            if (!product.ShowPrice)
            {
                stkPriceTotal.IsVisible = false;
                stkAddMore.IsVisible = false;
            }

            imgHeart.Source = product.IsFavorited ? "ic_heart_detail" : "ic_heart_detail_off";
            imgStar.Source = product.ProductStarImage;
            lblAvaliacoes.Text = "(" + product.TotalRating + " " + AppResource.textRatings;
            lblName.Text = product.Name;
            imgUser.Source = product.ImageUri;

            lblProductDesc.Text = product.Description;
            lblProductName.Text = product.Name;

            CultureInfo cult = company.CurrencyType.ToCultureInfo();

            lblPriceUnitario.Text = string.Format(cult, AppResource.lblOrderPriceUnid, product.Price);
            lblTotalPrice.Text = string.Format(cult, AppResource.lblOrderPrice, product.Price);

            if (CompanyCategoryListPage.GetInstance().InformativeWithoutPrice)
            {
                stkObservation.IsVisible = false;
                stkPriceTotal.IsVisible = false;
                grdAdd.IsVisible = false;
                grdConfirm.IsVisible = false;
            }

            ColorPage();
        }

        protected override void OnAppearing()
        {
            //LoadInfo();
            base.OnAppearing();

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


        private async void OnSolicitarTapped(object sender, EventArgs e)
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

            //if (Helpers.Settings.DisplayUserRole == Enums.UserRole.Guest.ToString())
            //{
            //    App.AppCurrent.AlertToGuest();
            //    return;
            //}

            ShowLoading(true);

            try
            {
                CreateOrder createOrder = new CreateOrder();

                //LoadInfo();
                bool has = await LoadInfoBool();
                if (!has)
                {
                    if (_company.HasCheckinInAnotherCompany)
                    {
                        throw new Exception(string.Format(AppResource.YouNeedToCloseCheckinBeforeOrdering, _company.CompanyName, _company.Title));
                    }
                    else
                    {
                        if (_company.HasLocation)
                        {
                            createOrder.ProductQuantity = Convert.ToInt32(lblQtd.Text);
                            createOrder.ProductId = _product.Id;
                            createOrder.CompanyId = _product.CompanyId;
                            createOrder.CreatedAt = DateTime.Now;
                            createOrder.CreatedAtStr = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                            createOrder.ClientId = Helpers.Settings.DisplayUserId;
                            createOrder.Observation = edtObs.Text;
                            createOrder.Company = _company;
                            createOrder.Product = _product;
                            createOrder.LocationId = _product.LocationId;




                            if (_listCompanyDelivery.Count > 0)
                            {
                                createOrder.Observation = edtObs.Text += "\n - Entregar: " + _entregaTapped;
                            }

                            _company.CheckedIn = false;
                            await App.AppCurrent.NavigationService.NavigateModalAsync(new RequestCheckinByLocationPage(_company, _product.LocationId, "", "", null, _product.Location, createOrder), null, false);

                        }
                        //await App.AppCurrent.NavigationService.NavigateModalAsync(new MultiLocationRequestCheckin(Company, Product.LocationId), null, true);
                        else if (!_company.CheckedIn)
                        {
                            await App.AppCurrent.NavigationService.NavigateModalAsync(new SingleLocationRequestCheckinPage(_company), null, true);
                        }
                    }

                    return;
                }

                if (_company.HasLocation)
                {
                    bool hasInThisLocation = await LoadInfoLocation(_product.Id);
                    if (!hasInThisLocation)
                    {
                        //if(Product.Location == null)
                        //{
                        //    var service = new LocationService();
                        //    var location = await service.GetLocationById(pro)
                        //}
                        createOrder.ProductQuantity = Convert.ToInt32(lblQtd.Text);
                        createOrder.ProductId = _product.Id;
                        createOrder.CompanyId = _product.CompanyId;
                        createOrder.CreatedAt = DateTime.Now;
                        createOrder.CreatedAtStr = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                        createOrder.ClientId = Helpers.Settings.DisplayUserId;
                        createOrder.Observation = edtObs.Text;
                        createOrder.Company = _company;
                        createOrder.Product = _product;
                        createOrder.LocationId = _product.LocationId;

                        Checkin currentCheckin = await LoadInfoCheckin();
                        //await App.AppCurrent.NavigationService.NavigateModalAsync(new MultiLocationRequestCheckin(Company, Product.LocationId), null, true);
                        await App.AppCurrent.NavigationService.NavigateModalAsync(new RequestCheckinByLocationPage(_company, _product.LocationId, "", "", currentCheckin, _product.Location, createOrder), null, false);

                        return;
                    }
                }


                ShowLoading(true);

                //var answer = await DisplayAlert(AppResource.alertConfirmActionTitle, AppResource.alertReallyWantMakeOrder, AppResource.textOk, AppResource.alertCancel);

                //if (!answer)
                //    return;


                createOrder.ProductQuantity = Convert.ToInt32(lblQtd.Text);
                createOrder.ProductId = _product.Id;
                createOrder.CompanyId = _product.CompanyId;
                createOrder.CreatedAt = DateTime.Now;
                createOrder.CreatedAtStr = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                createOrder.ClientId = Helpers.Settings.DisplayUserId;
                createOrder.Observation = edtObs.Text;
                createOrder.Company = _company;
                createOrder.Product = _product;
                createOrder.LocationId = _product.LocationId;




                if (_listCompanyDelivery.Count > 0)
                {
                    createOrder.Observation = edtObs.Text += "\n - Entregar: " + _entregaTapped;
                }

                OrderService orderService = new OrderService();

                if (_product.Location == null)
                {
                    _product.Location = await LoadLocationById(_product.LocationId);
                }

                bool result = await App.AppCurrent.Cart.AddOrder(createOrder, _company, _product.Location);

                if (result)
                {
                    await App.AppCurrent.NavigationService.ModalGoBack();
                }
            }
            catch (Exception ex)
            {
                DisplayAlert(MocoApp.Resources.AppResource.alertAlert, ex.Message, AppResource.textOk);
            }
            finally
            {
                ShowLoading(false);
            }
        }


        public async void GetCompanyDelivery()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);


                CompanyService cs = new CompanyService();
                string result = await cs.GetCompanyDelivery(_company.Id);

                _listCompanyDelivery = JsonConvert.DeserializeObject<List<CompanyDelivery>>(result);

                if (_listCompanyDelivery.Count > 0)
                {
                    stkEntrega.IsVisible = true;
                    foreach (CompanyDelivery item in _listCompanyDelivery)
                    {
                        pckDeliveries.Items.Add(item.Name);
                    }
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

        private void PckDeliveries_SelectedIndexChanged(object sender, EventArgs e)
        {
            _entregaTapped = "";

            try
            {
                Picker item = sender as Picker;

                if (item.SelectedIndex < 0)
                {
                    return;
                }

                _entregaTapped = _listCompanyDelivery.Where(m => m.Name == item.Items[item.SelectedIndex]).FirstOrDefault().Name;
            }
            catch (Exception)
            {


            }

        }

        public void ColorPage()
        {

            switch (App.CompanyTypeSelected)
            {
                case Models.Enums.CompanyType.Hotel:
                    BackgroundColor = (Color)App.Current.Resources["HotelColor"];
                    imgBack.Source = "ic_voltar_hotel";
                    break;
                case Models.Enums.CompanyType.Restaurante:
                    BackgroundColor = (Color)App.Current.Resources["RestauranteColor"];
                    imgBack.Source = "ic_voltar_restaurante";
                    break;
                case Models.Enums.CompanyType.Praia:
                    BackgroundImage = "bg_praia";
                    BackgroundColor = Color.Transparent;
                    imgBack.Source = "ic_voltar_praia";
                    break;
                case Enums.CompanyType.EsporteEvento:
                    BackgroundColor = (Color)App.Current.Resources["EsportesColor"];
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

            await App.AppCurrent.NavigationService.NavigateModalAsync(new ProductRatePage(_product), null, true);
        }

        private async void OnBackTapped(object sender, EventArgs e)
        {
            await App.AppCurrent.NavigationService.ModalGoBack();
        }

        private async void OnInfoTapped(object sender, EventArgs e)
        {
            //await App.AppCurrent.NavigationService.NavigateModalAsync(new InfoBarraPage(), null, true);
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

            if (Helpers.Settings.DisplayUserRole == Enums.UserRole.Guest.ToString())
            {
                App.AppCurrent.AlertToGuest();
                return;
            }

            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);
                ApiService apiService = new ApiService();

                string result = await apiService.GetAsync("product/includeClientProductFavorite?productId=" + _product.Id + "&clientId=" + Helpers.Settings.DisplayUserId);
                _product.IsFavorited = JsonConvert.DeserializeObject<bool>(result);
                imgHeart.Source = _product.IsFavorited ? "ic_heart_detail" : "ic_heart_detail_off";


            }
            catch (Exception)
            {


            }
            finally
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }

        }

        public async void LoadInfo()
        {
            CompanyService companyService = new CompanyService();
            bool checkin = await companyService.IsCheckedIn(_company.Id, Helpers.Settings.DisplayUserToken);
            if (checkin)
            {
                _company.HasCheckinInAnotherCompany = false;
                _company.CheckedIn = true;
                _company.CompanyName = "";
            }
        }

        public async Task<Location> LoadLocationById(string id)
        {
            LocationService companyService = new LocationService();
            Location location = await companyService.GetLocationById(id);

            return location;
        }
        public async Task<bool> LoadInfoBool()
        {
            CompanyService companyService = new CompanyService();
            bool checkin = await companyService.IsCheckedIn(_company.Id, Helpers.Settings.DisplayUserToken);

            return checkin;
        }
        public async Task<Checkin> LoadInfoCheckin()
        {
            CompanyService companyService = new CompanyService();
            Checkin checkin = await companyService.IsCheckedInWithCheckin(_company.Id, Helpers.Settings.DisplayUserToken);

            return checkin;
        }
        public async Task<bool> LoadInfoLocation(string productId)
        {
            CompanyService companyService = new CompanyService();
            bool checkin = await companyService.IsCheckedInSub(productId, Helpers.Settings.DisplayUserToken);

            return checkin;
        }
        private async void OnPraiaTapped(object sender, EventArgs e)
        {
            if (_entregaTapped != AppResource.lblBeachs)
            {
                //_entregaTapped = AppResource.lblBeach;
                //imgPraia.Source = "ic_check";
                //imgChurrasqueira.Source = "ic_uncheck";
                //imgQuarto.Source = "ic_uncheck";
            }

        }



        private async void OnAddMoreTapped(object sender, EventArgs e)
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

            lblQtd.Text = (Convert.ToInt32(lblQtd.Text) + 1).ToString();

            decimal total = _product.Price * (Convert.ToInt32(lblQtd.Text));
            lblTotalPrice.Text = string.Format(AppResource.lblOrderPrice, string.Format(App.AppCurrent.CompanyCulture, "{0:C}", total));
        }

        private async void OnAddLessTapped(object sender, EventArgs e)
        {
            if (lblQtd.Text == "1")
            {
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

            lblQtd.Text = (Convert.ToInt32(lblQtd.Text) - 1).ToString();

            decimal total = _product.Price * (Convert.ToInt32(lblQtd.Text));
            lblTotalPrice.Text = string.Format(AppResource.lblOrderPrice, string.Format(App.AppCurrent.CompanyCulture, "{0:C}", total));
        }

        private async void OnChurrasqueiraTapped(object sender, EventArgs e)
        {
            if (_entregaTapped != "Churrasqueira")
            {
                //_entregaTapped = "Churrasqueira";
                //imgChurrasqueira.Source = "ic_check";
                //imgPraia.Source = "ic_uncheck";
                //imgQuarto.Source = "ic_uncheck";
            }

        }

        private async void OnQuartoTapped(object sender, EventArgs e)
        {
            if (_entregaTapped != "Quarto")
            {
                //_entregaTapped = "Quarto";
                //imgQuarto.Source = "ic_check";
                //imgChurrasqueira.Source = "ic_uncheck";
                //imgPraia.Source = "ic_uncheck";
            }

        }

        private void ShowLoading(bool show)
        {
            stkAct.IsVisible = show;
            actPiece.IsRunning = show;
            actPiece.IsVisible = show;
            actPiece.IsEnabled = show;

            if (show)
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);
            }
            else
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
        }

        private async void OnCart_Tapped(object sender, EventArgs e)
        {
            await App.AppCurrent.NavigationService.NavigateModalAsync(new ProductsCart(), null, true);

        }

    }
}
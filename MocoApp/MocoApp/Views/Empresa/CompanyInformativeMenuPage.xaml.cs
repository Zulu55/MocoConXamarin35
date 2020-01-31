using MocoApp.Models;
using System.Collections.Generic;
using MocoApp.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MocoApp.Resources;
using System;
using System.Threading.Tasks;
using MocoApp.Views.Cliente;
using MocoApp.DTO;
using Newtonsoft.Json;
using Plugin.Share;
using MocoApp.Views.Chat;
using System.Linq;


namespace MocoApp.Views.Empresa
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CompanyInformativeMenuPage : ContentPage
    {
        Company Company;
        CompanyService service = new CompanyService();
        string _categoryId;
        string seletedCategory;
        string _menuId = "";
        InformativeMenu _menu;
        Location _location;
        public CompanyInformativeMenuPage(Company company, string menuId, Location location = null)
        {
            InitializeComponent();

            if (Device.RuntimePlatform == "Android")
                frmXaml.CornerRadius = 30;

            seletedCategory = "";
            _menuId = menuId;
            Company = company;
            _location = location;
            if (Company.HasMessageNotRead)
                imgChat.Source = "ic_chat_msg";

            lblName.Text = company.Title;
            lblAddress.Text = company.Address;
            lblPhone.Text = company.Cellphone;
            //imgUser.Source = company.ImageUri;
           
            lblInfoDistance.Text = company.DistanceString;
            lblAvaliacoes.Text = "(" + Company.TotalRating + AppResource.textRatings;

            ColorPage();
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();

            LoadCompany();
        }

        public async void LoadCompany()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);
                ShowLoading(true);
                lblAddress.Text = Company.Address;
                lblPhone.Text = Company.Cellphone;

                var resultMenus = await service.GetInformativeMenuById(_menuId);
                var menus = JsonConvert.DeserializeObject<InformativeMenu>(resultMenus);

                _menu = menus;
                imgUser.Source = _menu.ImageUri;

                LoadMenus();



            }
            catch (Exception ex)
            {


            }
            finally
            {
                ShowLoading(false);
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
        }

        public void LoadMenus()
        {
            LoadItens();
            //stkCategory.Children.Clear();
            //Label lblFirst = new Label();


            ////float categoryFontSize = 16;
            ////if (Device.Idiom == TargetIdiom.Tablet)
            ////    categoryFontSize = 22;

            ////Label lblName = new Label();
            ////lblName.VerticalTextAlignment = TextAlignment.Center;
            ////lblName.VerticalOptions = LayoutOptions.Center;
            ////lblName.Text = _menu.Name;
            ////lblName.StyleId = _menu.Id;
            ////lblName.Margin = new Thickness(6, 0, 6, 0);
            ////lblName.FontSize = categoryFontSize;

            ////var detailsRecognizer = new TapGestureRecognizer();
            ////detailsRecognizer.Tapped += DetailsRecognizer_Tapped;
            ////lblName.GestureRecognizers.Add(detailsRecognizer);

            //lblFirst = lblName;

            //stkCategory.Children.Add(lblName);

            //DetailsRecognizer_Tapped(lblFirst, null);
        }


        private async void DetailsRecognizer_Tapped(object sender, EventArgs e)
        {
            var obj = sender as Label;

            var stkFather = obj.Parent as StackLayout;

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
            stkItens.Children.Clear();

            var item = _menu;

            float groupFontSize = 16;
            if (Device.Idiom == TargetIdiom.Tablet)
                groupFontSize = 24;

            if (item != null)
            {
                lblName.Text = item.Name;
                imgUser.Source = item.ImageUri;
            }

            Label lblgroup = new Label();
            lblgroup.FontSize = groupFontSize + 2;
            lblgroup.FontAttributes = FontAttributes.Bold;
            lblgroup.Margin = new Thickness(0, 0, 0, 0);
            lblgroup.TextColor = Color.Black;
            lblgroup.HorizontalTextAlignment = TextAlignment.Start;
            lblgroup.Text = item.Title;


            Label lblBody = new Label();
            lblBody.FontSize = groupFontSize - 2;
            lblBody.Margin = new Thickness(0, 2, 0, 0);
            lblBody.TextColor = Color.Black;
            lblBody.HorizontalTextAlignment = TextAlignment.Start;
            lblBody.Text = item.Body;


            stkItens.Children.Add(lblgroup);
            stkItens.Children.Add(lblBody);

            if (!string.IsNullOrEmpty(item.Url))
            {
                Label lblUrl = new Label();
                lblUrl.FontSize = groupFontSize - 4;
                lblUrl.Margin = new Thickness(0, 4, 0, 0);
                lblUrl.TextColor = Color.Blue;
                lblUrl.HorizontalTextAlignment = TextAlignment.Start;
                var url = item.Url.Contains("http") ? item.Url : "http://" + item.Url;
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += (s, e) =>
                {
                    Device.OpenUri(new Uri(url));
                };
                lblUrl.GestureRecognizers.Add(tapGestureRecognizer);

                lblUrl.Text = item.Url;
                stkItens.Children.Add(lblUrl);
            }

            ShowLoading(false);
        }


        private void ShowLoading(bool show)
        {
            actPiece.IsRunning = show;
            actPiece.IsVisible = show;
            actPiece.IsEnabled = show;
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
                    if (Company.HasLocation)
                        await App.AppCurrent.NavigationService.NavigateModalAsync(new ListLocationBillsPage(resultMyOrders.Checkin, Company, true), null, false);
                    else
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
                var locationId = "";
                //if (location != null)
                //    locationId = location.Id;
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);
                var result = await service.SolicitarAtendimento(Company.Id, locationId);

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

        private async void OnBackTapped(object sender, EventArgs e)
        {
            await App.AppCurrent.NavigationService.ModalGoBack();
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
    }
}
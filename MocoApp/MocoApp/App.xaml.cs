using Com.OneSignal;
using Com.OneSignal.Abstractions;
using MocoApp.Interfaces;
using MocoApp.Models;
using MocoApp.Resources;
using MocoApp.Services;
using MocoApp.ViewModels;
using MocoApp.Views;
using MocoApp.Views.ManagerCheckinFlow;
using MocoApp.Views.Menu;
using Plugin.LocalNotifications;
using Rg.Plugins.Popup.Services;
using Rg.Plugins.Popup.Extensions;
using Naylah.Xamarin.Common;
using Newtonsoft.Json;
using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using static MocoApp.Models.Enums;

namespace MocoApp
{
    public partial class App : BootStrapper
    {
        
        
        public static App AppCurrent { get; set; }
        public static FilterType FilterType { get; set; }
        public string Latitude { get; set; }

        public User User { get; set; }

        public bool FirstLogin { get; set; }
        public bool GuestLogin { get; set; }

        public bool IsToRefresh { get; set; }
        public string Longitude { get; set; }

        public string LocationId { get; set; }
        public string LocationName { get; set; }
        public CultureInfo CompanyCulture { get; set; }
        public CultureInfo Culture { get; set; }
        //propriedade para saber qual tipo foi selecionada
        public static CompanyType CompanyTypeSelected { get; set; }

        public static string AppDeviceToken { get; set; }


        //carinho de pedidos
        public CartModel Cart { get; set; }

        public static string AppDeviceId { get; set; }
        public App()
        {
            InitializeComponent();



            CompanyTypeSelected = CompanyType.Praia;
            FilterType = FilterType.Proximidade;
            AppCurrent = this;
            Cart = new CartModel();
            ConfigurePrelaunchPhase();

            OneSignal.Current.StartInit("0f3334c6-d6e1-46ab-9dc5-99d96b0e8c29")
                .HandleNotificationReceived(HandleNotificationReceived)
                .InFocusDisplaying(OSInFocusDisplayOption.Notification)
                .HandleNotificationOpened(HandleNotificationOpened)
                  .EndInit();

            string idiom = Helpers.Settings.DisplayUserIdiom;
            Culture = new CultureInfo(idiom);
            AppResource.Culture = Culture;

            LoadPage();



        }

        private static void HandleNotificationReceived(OSNotification notification)
        {
            Acr.UserDialogs.UserDialogs.Instance.Toast(AppResource.AlertNewNotification, TimeSpan.FromSeconds(1));
            //CrossLocalNotifications.Current.Show("title", "body");

            //App.AppCurrent.NavigationService.NavigateAsync(new NotificationPopupPage());
            //PopupNavigation.Instance.PushAsync(new NotificationPopupPage(notification));


        }

        private static void HandleNotificationOpened(OSNotificationOpenedResult notification)
        {
            //atualiza a tela dos admin
            try
            {
                MessagingCenter.Send<object>(App.AppCurrent.NavigationService.CurrentPage, "Push");

            }
            catch (Exception)
            {

               
            }
           
        }

        public void IdsAvailable(string userID, string pushToken)
        {
            AppDeviceToken = pushToken;
            AppDeviceId = userID;
            SavePushUserId(pushToken, userID);
        }

        public async void SavePushUserId(string token, string id)
        {
            try
            {
                AppDeviceToken = token;
                AppDeviceId = id;

                if(!string.IsNullOrEmpty(Helpers.Settings.DisplayUserId))
                {
                    ApiService service = new ApiService();
                    var result = service.UpdateAcess(token, id, "", "");
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
        }

        public void LoadPage()
        {
            OneSignal.Current.IdsAvailable(IdsAvailable);
        }

        public void ConfigurePrelaunchPhase()
        {
            try
            {
                var a = new NavigationPage(new SplashPage());
                NavigationServiceFactory(a);
            }
            catch (Exception)
            {

            }

        }

        public static Action HideLoginView
        {
            get
            {
                return new Action(() => App.Current.MainPage.Navigation.PopModalAsync());
            }
        }

        public static async Task NavigateToProfile(string accessToken)
        {
            try
            {
                if (accessToken != AppResource.alertAlert)
                {
                    ApiService service = new ApiService();
                    var result = await service.GetAsync("user/facebook/getUser?acessToken=" + accessToken);

                    FacebookUserCommand user = JsonConvert.DeserializeObject<FacebookUserCommand>(result);

                    //não tem cadastro
                    if (!user.HasRegister)
                    {
                        App.AppCurrent.User = user.User;
                        //await App.AppCurrent.NavigationService.ModalGoBack();
                        await App.AppCurrent.NavigationService.NavigateModalAsync(new RegisterPage(), null, false);
                        return;
                    }
                    else
                    {
                        //tem cadastro
                        App.AppCurrent.StoreUser(user.User.Id, user.User.Name, user.User.Photo, user.User.UserRole, user.User.CompanyId);
                        await App.AppCurrent.ConfigureAppPhase();
                    }
                    await App.AppCurrent.NavigationService.ModalGoBack();
                }
                else
                {
                    await App.AppCurrent.NavigationService.ModalGoBack();
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                await AppCurrent.NavigationService.ModalGoBack();
            }
        }

        public override async Task LoadApp()
        {
            var nav = NavigationService.CurrentPage.BindingContext as SplashViewModel;
            await nav.LoadData();

        }

        public async Task ConfigureAppPhase()
        {
            try
            {
                var shellPage = new ShellPage();

                NavigationServiceFactory(shellPage);

                NavigationService.Navigating += shellPage.NavigationService_Navigating;
                NavigationService.Navigated += shellPage.NavigationService_Navigated;

                //cliente
                if (Helpers.Settings.DisplayUserRole == UserRole.Client.ToString() || Helpers.Settings.DisplayUserRole == UserRole.Guest.ToString())
                {
                    await shellPage.Vm.NavigateToSelectedMenuItem(MenuListData.Home);
                }

                //gerente
                if (Helpers.Settings.DisplayUserRole == UserRole.Manager.ToString())
                {
                    if (string.IsNullOrEmpty(Helpers.Settings.DisplayUserCompany))
                    {
                        await shellPage.Vm.NavigateToSelectedMenuItem(MenuListData.Empresas);
                    }
                    else
                    {
                        await shellPage.Vm.NavigateToSelectedMenuItem(MenuListData.ClientList);
                    }
                }

                if (Helpers.Settings.DisplayUserRole == UserRole.Employee.ToString())
                {
                    await shellPage.Vm.NavigateToSelectedMenuItem(MenuListData.OrdersOpen);
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        public async void AlertToGuest()
        {
            var msg = AppResource.alertGuestRegister;
            var answer = await AppCurrent.MainPage.DisplayAlert(AppResource.alertAlert, msg, AppResource.lblRegister, AppResource.alertCancel);

            if(answer)
            {
                Logout();
            }
        }

        public override async Task InitializeApp()
        {
            await base.InitializeApp();
            await LoadApp();
        }
        public async Task GetUserLocation()
        {
            try
            {
                var locator = CrossGeolocator.Current;

                if (!locator.IsGeolocationEnabled)
                    throw new Exception();

                if (locator.IsGeolocationAvailable && locator.IsGeolocationEnabled)
                {
                    locator.DesiredAccuracy = 50;
                    var position = await locator.GetPositionAsync(100000);

                    Latitude = position.Latitude.ToString().Replace(',', '.');
                    Longitude = position.Longitude.ToString().Replace(',', '.');

                }
                else
                {
                    //throw new Exception("Você precisa ativar seu GPS para dar continuidade. Verifique nas configurações.");

                }
            }
            catch (Exception ex)
            {
                Latitude = "";
                Longitude = "";
            }
        }

        public void StoreUser(string id, string name, string photo, UserRole role, string companyId = "", bool hasLocation = false, decimal offset = 0, string idiom = "en-us", string token = "")
        {
            Helpers.Settings.DisplayUserId = id;
            Helpers.Settings.DisplayUserName = name;
            Helpers.Settings.DisplayUserRole = role.ToString();
            Helpers.Settings.DisplayUserPhoto = photo;
            Helpers.Settings.DisplayUserCompany = companyId;
            Helpers.Settings.DisplayHasLocation = hasLocation;
            Helpers.Settings.DisplayMyOffset= offset;

            if(!string.IsNullOrEmpty(token))
                Helpers.Settings.DisplayUserToken = token;


            if (idiom == "1")
                Helpers.Settings.DisplayUserIdiom = "pt-br";
            else if (idiom == "2")
                Helpers.Settings.DisplayUserIdiom = "en-us";
            else if (idiom == "3")
                Helpers.Settings.DisplayUserIdiom = "es-es";
            else if(idiom == "0")
                Helpers.Settings.DisplayUserIdiom = "en-us";
            else if (idiom == "4")
                Helpers.Settings.DisplayUserIdiom = "it-it";
            else
                Helpers.Settings.DisplayUserIdiom = idiom;

            //20190104 - XND - Alterei aqui pra so mudar a cultura do resources inves da cultura do app todo 
            var culture = new CultureInfo(Helpers.Settings.DisplayUserIdiom);
            AppResource.Culture = culture;
            App.AppCurrent.Culture = culture;
            MenuListData.Reload();

            //Culture = CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(Helpers.Settings.DisplayUserIdiom);
            //AppResource.Culture = Culture;
        }

        public void UpdateLanguage(string idiom)
        {
            if (idiom == "1")
                Helpers.Settings.DisplayUserIdiom = "pt-br";
            else if (idiom == "2")
                Helpers.Settings.DisplayUserIdiom = "en-us";
            else if (idiom == "3")
                Helpers.Settings.DisplayUserIdiom = "es-es";
            else if (idiom == "0")
                Helpers.Settings.DisplayUserIdiom = "en-us";
            else if (idiom == "4")
                Helpers.Settings.DisplayUserIdiom = "it-it";
            else
                Helpers.Settings.DisplayUserIdiom = idiom;

            //20190104 - XND - Alterei aqui pra so mudar a cultura do resources inves da cultura do app todo 
            var culture = new CultureInfo(Helpers.Settings.DisplayUserIdiom);
            AppResource.Culture = culture;
            App.AppCurrent.Culture = culture;

        }

        public async void Logout()
        {

            ApiService service = new ApiService();
            service.GetAsync("user/logout?id=" + Helpers.Settings.DisplayUserId);

            Helpers.Settings.DisplayUserId = "";
            Helpers.Settings.DisplayUserName = "";
            Helpers.Settings.DisplayUserToken = "";
            Helpers.Settings.DisplayUserRole = "";
            Helpers.Settings.DisplayUserCompany = "";
            Helpers.Settings.DisplayHasLocation = false;
            App.AppCurrent.FirstLogin = false;

            LocationId = "";  
            

            ConfigurePrelaunchPhase();

            await LoadApp();

        }


        protected override void OnStart()
        {
            //// Handle when your app starts
            //AppCenter.Start("ios=99884d9d-7d3a-4b75-8c44-01164cd90c96;",
            //      typeof(Analytics), typeof(Crashes));
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }



        public void MessagingCenterSubs()
        {
            MessagingCenter.Subscribe<object, string>(this, "Push", (s, e) =>
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Teste push");
            });
        }


    }
}

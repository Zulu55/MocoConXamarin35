using MocoApp.Resources;
using MocoApp.Views.Cliente;
using MocoApp.Views.CompanyFluxo;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using static MocoApp.Models.Enums;

namespace MocoApp.Views.Menu
{
    public class MenuListData
    {

        public static List<NavMenuItem> TopItems { get; set; }
        public static List<NavMenuItem> BottomItems { get; set; }

        #region Original 
        //public static NavMenuItem Home { get; }
        //public static NavMenuItem Checkins { get; }
        //public static NavMenuItem EditProfile { get; }
        //public static NavMenuItem Sair { get; }

        //public static NavMenuItem Empresas { get; }

        //public static NavMenuItem OrdersOpen { get; }
        //public static NavMenuItem OrdersAccepted { get; }

        //public static NavMenuItem CardapioCategory { get; }

        //public static NavMenuItem CardapioLocation { get; }
        //public static NavMenuItem ClientList { get; }


        //public static NavMenuItem Reports { get; }



        //public static NavMenuItem Settings { get; }

        //public static NavMenuItem EditPassword { get; }

        //public static NavMenuItem EditEmployeeProfile { get; }

        //public static NavMenuItem CompanyList { get; }

        //public static NavMenuItem EditCompanyPage { get; }

        //public static NavMenuItem ContactUs { get; }

        //public static NavMenuItem ContactUsAdmin { get; }

        //public static NavMenuItem ClientNotifications { get; }

        //public static NavMenuItem CompanyNotificatins { get; }
        #endregion

        #region doxnd errado
        public static NavMenuItem Home { get; set; }
        public static NavMenuItem Checkins { get; set; }
        public static NavMenuItem EditProfile { get; set; }
        public static NavMenuItem Sair { get; set; }

        public static NavMenuItem Empresas { get; set; }

        public static NavMenuItem OrdersOpen { get; set; }
        public static NavMenuItem OrdersAccepted { get; set; }

        public static NavMenuItem CardapioCategory { get; set; }

        public static NavMenuItem CardapioLocation { get; set; }
        public static NavMenuItem ClientList { get; set; }


        public static NavMenuItem Reports { get; set; }



        public static NavMenuItem Settings { get; set; }

        public static NavMenuItem EditPassword { get; set; }

        public static NavMenuItem EditEmployeeProfile { get; set; }

        public static NavMenuItem CompanyList { get; set; }

        public static NavMenuItem EditCompanyPage { get; set; }

        public static NavMenuItem ContactUs { get; set; }

        public static NavMenuItem ContactUsAdmin { get; set; }

        public static NavMenuItem ClientNotifications { get; set; }

        public static NavMenuItem CompanyNotificatins { get; set; }
        public static NavMenuItem Chat { get; set; }
        public static NavMenuItem CompanyChat { get; set; }
        #endregion
        static MenuListData()
        {

            ContactUs = new NavMenuItem() { Title = AppResource.lblTalkWithUs, Icon = "ic_contato", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(ContactUsPage) };


            Home = new NavMenuItem() { Title = AppResource.lblCompanies, Icon = "ic_lista", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(HomePage) };
            Checkins = new NavMenuItem() { Title = AppResource.lblOrderDone, Icon = "ic_pedidos", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(ListCheckinsPage) };
            EditProfile = new NavMenuItem() { Title = AppResource.lblEditProfile, Icon = "ic_usuario", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(EditClientProfilePage) };
            ClientNotifications = new NavMenuItem() { Title = AppResource.lblNotifications, Icon = "ic_notification", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(ListNotificationClientPage) };
            Sair = new NavMenuItem() { Title = AppResource.lblLoginLogout, Icon = "ic_sair", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(HomePage) };

            //gerente
            Empresas = new NavMenuItem() { Title = AppResource.lblCompanies, Icon = "ic_lista", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(CompanyFluxo.ListCompanyPage) };
            //ReportProduct = new NavMenuItem() { Title = "Relatório Produtos", Icon = "", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(CompanyFluxo.ReportProductPage) };
            //ReportCompany = new NavMenuItem() { Title = "Relatório Estabelecimento", Icon = "", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(CompanyFluxo.ReportCompanyPage) };
            //OrderHistory = new NavMenuItem() { Title = "Histórico Pedidos", Icon = "", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(CompanyFluxo.OrderHistoryPage) };
            Reports = new NavMenuItem() { Title = AppResource.lblReports, Icon = "", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(CompanyFluxo.ReportPage) };
            CompanyList = new NavMenuItem() { Title = AppResource.lblCompanies, Icon = "", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(CompanyFluxo.ListCompanyPage) };
            Settings = new NavMenuItem() { Title = AppResource.lblSettings, Icon = "", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(CompanyFluxo.SettingsPage) };
            EditCompanyPage = new NavMenuItem() { Title = AppResource.lblEditCompany, Icon = "", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(CompanyFluxo.EditCompanyPage) };

            //empresa
            OrdersOpen = new NavMenuItem() { Title = AppResource.lblOrdersOpen, Icon = "", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(CompanyFluxo.OrderOpenListPage) };
            OrdersAccepted = new NavMenuItem() { Title = AppResource.lblOrdersAccepted, Icon = "", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(CompanyFluxo.OrderAcceptedListPage) };            
            ClientList = new NavMenuItem() { Title = AppResource.lblCheckinsActive, Icon = "", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(ManagerCheckinFlow.ClientListPage) };
            EditPassword = new NavMenuItem() { Title = AppResource.lblChangePassword, Icon = "", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(ChangePasswordPage) };
            EditEmployeeProfile = new NavMenuItem() { Title = AppResource.lblEditProfile, Icon = "", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(CompanyFluxo.EditProfilePage) };
            ContactUsAdmin = new NavMenuItem() { Title = AppResource.lblTalkWithUs, Icon = "", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(ContactUsAsAdminPage) };
            CompanyNotificatins = new NavMenuItem() { Title = AppResource.lblNotifications, Icon = "", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(NotificationListCompanyPage) };


            CardapioLocation = new NavMenuItem() { Title = AppResource.lblCart, Icon = "", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(CompanyFluxo.LocationListPage) };
            CardapioCategory = new NavMenuItem() { Title = AppResource.lblCart, Icon = "", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(CompanyFluxo.CategoryListPage) };


            Chat = new NavMenuItem() { Title = AppResource.lblChat, Icon = "ic_chat_menu", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(Chat.ClientChatList) };
            CompanyChat = new NavMenuItem() { Title = AppResource.lblChat, Icon = "", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(Chat.CompanyChatList) };



            LoadItems();
        }

        public static void Reload()
        {
            ContactUs = new NavMenuItem() { Title = AppResource.lblTalkWithUs, Icon = "ic_contato", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(ContactUsPage) };


            Home = new NavMenuItem() { Title = AppResource.lblCompanies, Icon = "ic_lista", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(HomePage) };
            Checkins = new NavMenuItem() { Title = AppResource.lblOrderDone, Icon = "ic_pedidos", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(ListCheckinsPage) };
            EditProfile = new NavMenuItem() { Title = AppResource.lblEditProfile, Icon = "ic_usuario", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(EditClientProfilePage) };
            ClientNotifications = new NavMenuItem() { Title = AppResource.lblNotifications, Icon = "ic_notification", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(ListNotificationClientPage) };
            Chat = new NavMenuItem() { Title = AppResource.lblChat, Icon = "ic_chat_menu", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(Chat.ClientChatList) };
            Sair = new NavMenuItem() { Title = AppResource.lblLoginLogout, Icon = "ic_sair", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(HomePage) };

            //gerente
            Empresas = new NavMenuItem() { Title = AppResource.lblCompanies, Icon = "ic_lista", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(CompanyFluxo.ListCompanyPage) };
            //ReportProduct = new NavMenuItem() { Title = "Relatório Produtos", Icon = "", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(CompanyFluxo.ReportProductPage) };
            //ReportCompany = new NavMenuItem() { Title = "Relatório Estabelecimento", Icon = "", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(CompanyFluxo.ReportCompanyPage) };
            //OrderHistory = new NavMenuItem() { Title = "Histórico Pedidos", Icon = "", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(CompanyFluxo.OrderHistoryPage) };
            Reports = new NavMenuItem() { Title = AppResource.lblReports, Icon = "", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(CompanyFluxo.ReportPage) };
            CompanyList = new NavMenuItem() { Title = AppResource.lblCompanies, Icon = "", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(CompanyFluxo.ListCompanyPage) };
            Settings = new NavMenuItem() { Title = AppResource.lblSettings, Icon = "", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(CompanyFluxo.SettingsPage) };
            EditCompanyPage = new NavMenuItem() { Title = AppResource.lblEditCompany, Icon = "", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(CompanyFluxo.EditCompanyPage) };

            //empresa
            OrdersOpen = new NavMenuItem() { Title = AppResource.lblOrdersOpen, Icon = "", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(CompanyFluxo.OrderOpenListPage) };
            OrdersAccepted = new NavMenuItem() { Title = AppResource.lblOrdersAccepted, Icon = "", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(CompanyFluxo.OrderAcceptedListPage) };
            ClientList = new NavMenuItem() { Title = AppResource.lblCheckinsActive, Icon = "", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(ManagerCheckinFlow.ClientListPage) };
            EditPassword = new NavMenuItem() { Title = AppResource.lblChangePassword, Icon = "", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(ChangePasswordPage) };
            EditEmployeeProfile = new NavMenuItem() { Title = AppResource.lblEditProfile, Icon = "", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(CompanyFluxo.EditProfilePage) };
            ContactUsAdmin = new NavMenuItem() { Title = AppResource.lblTalkWithUs, Icon = "", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(ContactUsAsAdminPage) };
            CompanyNotificatins = new NavMenuItem() { Title = AppResource.lblNotifications, Icon = "", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(NotificationListCompanyPage) };


            CardapioLocation = new NavMenuItem() { Title = AppResource.lblCart, Icon = "", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(CompanyFluxo.LocationListPage) };
            CardapioCategory = new NavMenuItem() { Title = AppResource.lblCart, Icon = "", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(CompanyFluxo.CategoryListPage) };




            CompanyChat = new NavMenuItem() { Title = AppResource.lblChat, Icon = "", TextColor = Xamarin.Forms.Color.FromHex("#4b4b4d"), TargetType = typeof(Chat.CompanyChatList) };


            LoadItems();
        }

        public static void LoadItems()
        {
            if (Helpers.Settings.DisplayUserRole == UserRole.Client.ToString())
            {
                TopItems = new List<NavMenuItem>()
                 {
                    Home,
                    Checkins,
                    ClientNotifications,
                    Chat,
                    ContactUs,
                    EditProfile                
                 };
            }

            if (Helpers.Settings.DisplayUserRole == UserRole.Guest.ToString())
            {
                TopItems = new List<NavMenuItem>()
                 {
                    Home,
                    ContactUs
                 };
            }

            if (Helpers.Settings.DisplayUserRole == UserRole.Manager.ToString())
            {
                TopItems = new List<NavMenuItem>()
                 {
                    OrdersOpen,
                    OrdersAccepted,
                    ClientList,   
                    EditCompanyPage,
                    CompanyNotificatins,
                    CompanyChat,
                    CompanyList,
                    ContactUsAdmin,
                    Reports,
                    Settings                  
                 };

                if (Helpers.Settings.DisplayHasLocation)
                    TopItems.Insert(5, CardapioLocation);
                else
                    TopItems.Insert(5, CardapioCategory);
                

            }

            if (Helpers.Settings.DisplayUserRole == UserRole.Employee.ToString())
            {
                TopItems = new List<NavMenuItem>()
                 {
                    OrdersOpen,
                    OrdersAccepted,
                    CompanyChat,
                    ClientList,
                    CompanyNotificatins,
                    EditPassword,
                    EditEmployeeProfile,
                    ContactUsAdmin
                 };
            }

            

            BottomItems = new List<NavMenuItem>()
            {
                Sair
            };

        }

    }

    public class NavMenuItem : MenuItem
    {

        public string Title { get; set; }

        public Xamarin.Forms.Color TextColor { get; set; }

        public Type TargetType { get; set; }

    }
}

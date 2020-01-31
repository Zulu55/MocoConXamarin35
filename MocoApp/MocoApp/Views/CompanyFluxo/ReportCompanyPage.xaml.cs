using MocoApp.DTO;
using MocoApp.Models;
using MocoApp.Resources;
using MocoApp.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MocoApp.Views.CompanyFluxo
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReportCompanyPage : ContentPage
    {
        List<User> ListEmployee;
        User User;
        Location Location;
        List<Location> ListLocation;
        public ReportCompanyPage()
        {
            InitializeComponent();

            dtInit.Date = DateTime.Now.AddDays(-7);
            dtEnd.Date = DateTime.Now;

            endTimerPicker.Time = new TimeSpan(0, 23, 59, 00);

            User = new User();
            Location = new Location();
            LoadManagerAndEmployeesFromCompany();
            LoadLocations();
        }

        public async void LoadManagerAndEmployeesFromCompany()
        {
            CompanyService companyService = new CompanyService();

            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);


                var result = await companyService.GetEmployeesAndManagers();
                var list = JsonConvert.DeserializeObject<List<User>>(result);

                ListEmployee = list;
                pckAtendentes.Items.Add(AppResource.txtAll);
                foreach (var item in list)
                {
                    pckAtendentes.Items.Add(item.Name);
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


        public async void LoadLocations()
        {
            CompanyService companyService = new CompanyService();

            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);


                var result = await companyService.GetAllLocationsByCompanyId();
                var list = JsonConvert.DeserializeObject<List<Location>>(result);

                ListLocation = list;
                pckLocations.Items.Add(AppResource.txtAll);
                foreach (var item in list)
                {
                    pckLocations.Items.Add(item.Name);
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

        private void pckAtendentes_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = sender as Picker;

            if (item.SelectedIndex < 0)
                return;

            var selected = item.Items[item.SelectedIndex];
            if (selected == AppResource.txtAll)
                User.Id = null;
            else
                User.Id = ListEmployee.Where(m => m.Name == selected).FirstOrDefault().Id;
        }

        private void pckLocations_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = sender as Picker;

            if (item.SelectedIndex < 0)
                return;

            var selected = item.Items[item.SelectedIndex];
            if (selected == AppResource.txtAll)
                Location.Id = null;
            else
                Location.Id = ListLocation.Where(m => m.Name == selected).FirstOrDefault().Id ;
        }

        private async void btnFiltrar_Clicked(object sender, EventArgs e)
        {
            CompanyService companyService = new CompanyService();
            stkHeader.IsVisible = false;

            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);
                listView.ItemsSource = null;

                var hourInit = initTimerPicker.Time;
                var hourEnd = endTimerPicker.Time;

                var result = await companyService.GetCompanyReport(dtInit.Date, dtEnd.Date, hourInit, hourEnd, User.Id, Location.Id);

                var companyReport = JsonConvert.DeserializeObject<CompanyReportDTO>(result);

                lblRendimento.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", companyReport.TotalGross);
                lblTotalClientes.Text = companyReport.TotalClients.ToString();
                lblTotalPedidos.Text = companyReport.TotalOrders.ToString();
                lblCancelados.Text = companyReport.TotalOrdersCanceled.ToString();
                lblTotalInCard.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", companyReport.PaidInCardHiCharlie);
                lblTotalInCash.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", companyReport.TotalInCash);
                lblCompanyCard.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", companyReport.TotalInCard);

                lblTotalTips.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", companyReport.TotalTip);
                lblTotalTaxs.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", companyReport.TotalTax);
                lblTotalDiscount.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", companyReport.TotalDiscount);

                lblTotalPricePedidos.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", companyReport.OrdersPrice);
                lblPriceCancelados.Text = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", companyReport.OrdersPriceCanceled);

                lblTotalCLientAttended.Text = companyReport.TotalClientAttended.ToString();
                lblTotalCart.Text = companyReport.TotalCarts.ToString();

                if (companyReport.Clients.Count > 0)
                {
                    lblEmpty.IsVisible = false;
                    listView.ItemsSource = companyReport.Clients;
                }
                else
                {
                    lblEmpty.IsVisible = true;
                }

                stkHeader.IsVisible = true;

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

        private void dtInit_DateSelected(object sender, DateChangedEventArgs e)
        {

        }

        private void dtEnd_DateSelected(object sender, DateChangedEventArgs e)
        {

        }
    }

    class CompanyReport
    {
        public string TotalClients { get; set; }
        public string TotalOrders { get; set; }
        public double TotalPrice { get; set; }
        public int TotalCancelled { get; set; }
        public decimal TotalPriceCanceled { get; set; }
        public List<ClientDTO> Clients { get; set; }
    }

    public class ClientDTO
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Photo { get; set; }
        public int TotalOrders { get; set; }
        public string DateCheckin { get; set; }

        public string TotalOrdersStr { get { return string.Format("{0} {1}", TotalOrders, AppResource.alertOrders.ToLower()); } }
    }
}
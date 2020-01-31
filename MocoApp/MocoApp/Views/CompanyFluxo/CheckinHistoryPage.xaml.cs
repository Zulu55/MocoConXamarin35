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
    public partial class CheckinHistoryPage : ContentPage
    {

        List<Checkin> ListCheckins;
        List<Location> ListLocation;
        bool _firstLoad = true;
        string _locationId = "";
        DateTimeOffset? dateInit = null;
        DateTimeOffset? dateEnd = null;
        public CheckinHistoryPage()
        {
            InitializeComponent();


            dtInit.Date = DateTime.Now.AddDays(-7);
            dtEnd.Date = DateTime.Now;

            listView.ItemTapped += ListView_ItemTapped;
            LoadLocations();
        }

        public async void LoadCheckins()
        {
            OrderService orderService = new OrderService();

            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);

                string localDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

                if (dateInit == null)
                    dateInit = DateTime.Now.AddDays(-7);

                if (dateEnd == null)
                    dateEnd = DateTime.Now;

                var result = await orderService.GetCheckinsByCompanyId(_locationId, dateInit, dateEnd, null);

                ListCheckins = JsonConvert.DeserializeObject<List<Checkin>>(result);

                if (ListCheckins.Count > 0)
                {
                    listView.ItemsSource = ListCheckins;
                    lblEmpty.IsVisible = false;
                }
                else
                {
                    listView.ItemsSource = null;
                    lblEmpty.IsVisible = true;
                }

                _firstLoad = false;
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

        private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {

            if (listView.SelectedItem == null)
                return;

            var item = listView.SelectedItem as Checkin;
            await App.AppCurrent.NavigationService.NavigateAsync(new CheckinHistoryDetailPage(item), null, false);

            listView.SelectedItem = null;
        }

        public async void LoadLocations()
        {
            CompanyService companyService = new CompanyService();

            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);


                var result = await companyService.GetLocationsByCompanyId();
                ListLocation = JsonConvert.DeserializeObject<List<Location>>(result);

                if (ListLocation != null)
                {
                    stkLocation.IsVisible = true;
                    pckLocations.Items.Add(AppResource.txtAll);
                    foreach (var item in ListLocation)
                    {
                        pckLocations.Items.Add(item.Name);
                    }

                    pckLocations.SelectedIndexChanged += PckLocations_SelectedIndexChanged;
                }
                else
                    stkLocation.IsVisible = false;

                LoadCheckins();
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

        private void PckLocations_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = sender as Picker;

            if (item.SelectedIndex < 0)
                return;

            if (item.Items[item.SelectedIndex] == AppResource.txtAll)
            { }
            else
            {
                _locationId = ListLocation.Where(m => m.Name == item.Items[item.SelectedIndex]).FirstOrDefault().Id;
                listView.ItemsSource = null;
            }

            LoadCheckins();
        }

        private void dtInit_DateSelected(object sender, DateChangedEventArgs e)
        {
            dateInit =  dtInit.Date;
            if (!_firstLoad)
                LoadCheckins();
        }

        private void dtEnd_DateSelected(object sender, DateChangedEventArgs e)
        {
            dateEnd = dtEnd.Date;

            if (!_firstLoad)
                LoadCheckins();
        }
    }
}
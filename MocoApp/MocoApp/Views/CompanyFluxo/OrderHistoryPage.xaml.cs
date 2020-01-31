using MocoApp.Extensions;
using MocoApp.Models;
using MocoApp.Resources;
using MocoApp.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static MocoApp.Models.Enums;

namespace MocoApp.Views.CompanyFluxo
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OrderHistoryPage : ContentPage
    {
        Location Location;
        bool _firstLoad = true;
        List<Location> ListLocation;
        public OrderHistoryPage()
        {
            InitializeComponent();

            dtInit.Date = DateTime.Now.AddDays(-7);
            dtEnd.Date = DateTime.Now;
            Location = new Location();


            endTimerPicker.Time = new TimeSpan(0, 23, 59, 00);
            initTimerPicker.Unfocused += InitTimerPicker_Unfocused;
            endTimerPicker.Unfocused += EndTimerPicker_Unfocused;


            LoadLocations();
            LoadOrders();



            //if (Device.OS == TargetPlatform.iOS)
            //{
            //    // move layout under the status bar
            //    this.Padding = new Thickness(0, 0, 0, 0);

            //    var menu = new ToolbarItem("Menu", "", () =>
            //    {
            //        App app = Application.Current as App;
            //        Naylah.Xamarin.Controls.Pages.MasterDetailNavigationPage md = (Naylah.Xamarin.Controls.Pages.MasterDetailNavigationPage)app.MainPage;

            //        md.IsPresented = true;

            //    }, 0, 0);
            //    menu.Priority = 0;

            //    ToolbarItems.Add(menu);
            //}
        }

        public async void LoadOrders()
        {
            OrderService orderService = new OrderService();

            try
            {
                listView.ItemsSource = null;


                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);


                var hourInit = initTimerPicker.Time;
                var hourEnd = endTimerPicker.Time;

                var datetimeInit = dtInit.Date;
                var datetimeEnd = dtEnd.Date;

                datetimeInit = datetimeInit.Date + hourInit;
                datetimeEnd = datetimeEnd.Date + hourEnd;

                //var result = await orderService.GetOrderHistory(datetimeInit.ToString("dd/MM/yyyy HH:mm"), datetimeEnd.ToString("dd/MM/yyyy HH:mm"), Location.Id);
                var result = await orderService.GetOrderHistory(datetimeInit, datetimeEnd, Location.Id);
                var dto = JsonConvert.DeserializeObject<HistoryOrderReportDTO>(result);

                var totalHours = Helpers.Settings.DisplayMyOffset;
                TimeSpan offset = TimeSpan.FromHours(Convert.ToDouble(totalHours));

                if (dto != null && dto.Orders.Any())
                {
                    dto.Orders.ForEach(x => x.ConvertMe(offset));
                }



                if (dto.Orders.Count > 0)
                {
                    //stkTotals.IsVisible = true;
                    listView.ItemsSource = dto.Orders;
                    lblEmpty.IsVisible = false;
                    //lblOne.Text = string.Format("Total taxas: {0:C}", dto.TotalTax);
                    //lblTwo.Text = string.Format("Total Gorjetas: {0:C}", dto.TotalTip);
                    //lblThree.Text = string.Format(AppResource.lblOrderPrice, dto.TotalSpent);
                }
                else
                {
                    lblEmpty.IsVisible = true;
                    //stkTotals.IsVisible = false;
                }



            }
            catch (Exception ex)
            {
                this.DisplayAlert(MocoApp.Resources.AppResource.alertAlert, ex.Message, AppResource.textOk);
            }
            finally
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                _firstLoad = false;
            }
        }

        private void EndTimerPicker_Unfocused(object sender, FocusEventArgs e)
        {
            if (!_firstLoad)
                LoadOrders();
        }

        private void InitTimerPicker_Unfocused(object sender, FocusEventArgs e)
        {
            if (!_firstLoad)
                LoadOrders();
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

        private void pckLocations_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = sender as Picker;

            if (item.SelectedIndex < 0)
                return;

            var selected = item.Items[item.SelectedIndex];
            if (selected == AppResource.txtAll)
                Location = new Location();
            else
                Location = ListLocation.Where(m => m.Name == selected).FirstOrDefault();

            if (!_firstLoad)
                LoadOrders();
        }

        private void dtInit_DateSelected(object sender, DateChangedEventArgs e)
        {
            if (!_firstLoad)
                LoadOrders();
        }

        private void dtEnd_DateSelected(object sender, DateChangedEventArgs e)
        {
            if (!_firstLoad)
                LoadOrders();
        }

        public class HistoryOrderReportDTO
        {
            public List<HistoryOrderDetailReportDTO> Orders { get; set; }

            //Depois vai ter que adicionar as Tax por Order 
            public decimal TotalTax { get; set; }
            public decimal TotalTip { get; set; }
            public decimal TotalSpent { get; set; }
        }

        public class HistoryOrderDetailReportDTO
        {
            //Client Info
            public ClientHistoryOrderReportDTO Client { get; set; }


            //Product Info
            public string Name { get; set; }


            //QuantityPriceStr (APP)
            public int ProductQuantity { get; set; }
            public decimal ProductPrice { get; set; }

            public string ProductPriceStr { get { return String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", ProductPrice); } }
            public string QuantityPriceStr { get { return ProductQuantity + "x " + ProductPriceStr; } }


            //Checkin Info
            public string Occupation { get; set; }
            public string OccupationStr
            {
                get
                {
                    switch (CompanyType)
                    {
                        case CompanyType.Hotel:
                            return !string.IsNullOrEmpty(Occupation) ? AppResource.lblRoom + " " + Occupation : null;
                        case CompanyType.Restaurante:
                            return !string.IsNullOrEmpty(Occupation) ? AppResource.lblTable + " " + Occupation : null;
                        case CompanyType.Praia:
                            return !string.IsNullOrEmpty(Occupation) ? AppResource.lblTable + " " + Occupation : null;
                        default:
                            return !string.IsNullOrEmpty(Occupation) ? AppResource.lblLocation + " " + Occupation : null;
                    }
                }
            }
            public CompanyType CompanyType { get; set; }


            //Order Info
            public string Id { get; set; }
            public DateTimeOffset CreatedAt { get; set; }
            public string OrderCreatedAtStr { get { return CreatedAt.ToString("dd/MM/yyyy HH:mm"); } }
            public string OrderCreatedAtStrConverted { get; set; }

            //Location Info
            public LocationHistoryOrderReportDTO Location { get; set; }

            public void ConvertMe(TimeSpan span)
            {
                var converted = CreatedAt + span;
                OrderCreatedAtStrConverted = converted.ToString("dd/MM/yyyy HH:mm");

            }
        }


        public class ClientHistoryOrderReportDTO
        {
            public string Photo { get; set; }
            public string Name { get; set; }
            public string Cellphone { get; set; }
        }

        public class LocationHistoryOrderReportDTO
        {
            public string Name { get; set; }
            public LocationType LocationType { get; set; }
            public string Prefix { get; set; }
            public string ImageUri { get; set; }
            //Adicionar essential 2 tips
        }

        //private void teste(DateTimeOffset data, TimeSpan span)
        //{
        //    //var offset = conv.Offset;
        //    //var offset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);
        //    var rtn = data - span;
        //}
    }
}
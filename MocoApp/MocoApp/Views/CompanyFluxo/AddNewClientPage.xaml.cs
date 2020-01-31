using MocoApp.Models;
using MocoApp.Resources;
using MocoApp.Services;
using MocoApp.Views.ManagerCheckinFlow;
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
    public partial class AddNewClientPage : ContentPage
    {

        private List<Location> ListLocation;
        string _locationId = "";
        bool newBillWithAOldCheckin;
        Client _c;
        string _occ;
        private Checkin _checkin;
        public AddNewClientPage(Client c = null, string occ = null, Checkin checkin = null)
        {
            InitializeComponent();

            newBillWithAOldCheckin = false;

            _c = c;
            _occ = occ;
            _checkin = checkin;

            LoadPage();

        }

        private async Task LoadPage()
        {
            if (_c != null)
                SetClientValues();

            //20190403
            //if (!Helpers.Settings.DisplayHasLocation)
            //    lblOccupation.Text = AppResource.lblLocationTableChair + " " + AppResource.optional; 
            //else if (App.CompanyTypeSelected == Enums.CompanyType.Hotel)
            //    lblOccupation.Text = AppResource.lblRoom + " " + AppResource.optional; 
            //else
            //    lblOccupation.Text = AppResource.lblRoomToPoints + " " + AppResource.optional;
            lblName.Text += " " + AppResource.optional;

            CompanyService service = new CompanyService();
            var comp = await service.GetCompanyById(Helpers.Settings.DisplayUserCompany);
            var company = JsonConvert.DeserializeObject<Company>(comp);
            lblOccupation.Text = company.OccupationPrefix;

            if (!Helpers.Settings.DisplayHasLocation)
                lblOccupation.Text = company.OccupationPrefix + " " + AppResource.optional;
            else if (App.CompanyTypeSelected == Enums.CompanyType.Hotel)
                lblOccupation.Text = company.OccupationPrefix;
            else
                lblOccupation.Text = company.OccupationPrefix + " " + AppResource.optional;


            lblEmail.Text += " " + AppResource.optional;
          

            if (!Helpers.Settings.DisplayHasLocation)
            {
                stkLocation.IsVisible = false;
                stkPrefix.IsVisible = false;
            }
            else
            {
                LoadLocations();
                stkLocation.IsVisible = true;
                //stkPrefix.IsVisible = true;
            }

      
                
        }

        public void SetClientValues()
        {
            txtEmail.Text = _c.Email;
            txtName.Text = _c.Name;
            txtOcupation.Text = _occ;
            if (!string.IsNullOrEmpty(_occ))
            {
                txtOcupation.IsEnabled = false;
            }
            //txtOcupation.IsEnabled = false;
            txtEmail.IsEnabled = false;
            txtName.IsEnabled = false;
        }

        protected override void OnAppearing()
        {
            //if (!Helpers.Settings.DisplayHasLocation)
            //    lblOccupation.Text = AppResource.lblLocationTableChair;
            //else if (App.CompanyTypeSelected == Enums.CompanyType.Hotel)
            //    lblOccupation.Text = AppResource.lblRoom + " " + AppResource.optional;
            //else
            //    lblOccupation.Text = AppResource.lblRoomToPoints + " " + AppResource.optional;
            //CompanyService service = new CompanyService();

            //var comp = await service.GetCompanyById(Helpers.Settings.DisplayUserCompany);


            base.OnAppearing();
        }
        private async void btnAddClient_Clicked(object sender, EventArgs e)
        {
            try
            {
                //if (string.IsNullOrEmpty(txtEmail.Text))
                //    throw new Exception(AppResource.alertFillEmailValid);

                var prefixALert = AppResource.alertFillPrefix;

                if (string.IsNullOrEmpty(txtQtd.Text))
                    throw new Exception(AppResource.alertFillQuantity);
                if (string.IsNullOrEmpty(txtPrefix.Text) && Helpers.Settings.DisplayHasLocation && lblPrefix.Text == AppResource.lblPrefix && stkPrefix.IsVisible)
                    throw new Exception(string.Format(prefixALert, AppResource.lblLocation.ToLower()));
                else if (string.IsNullOrEmpty(txtPrefix.Text) && Helpers.Settings.DisplayHasLocation && stkPrefix.IsVisible)
                    throw new Exception(string.Format(prefixALert, lblPrefix.Text));
                else if(string.IsNullOrEmpty(txtOcupation.Text) && !Helpers.Settings.DisplayHasLocation)
                    throw new Exception(string.Format(prefixALert, lblOccupation.Text.ToLower()));

                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);

                ApiService service = new ApiService();

                CheckinClientCompany command = new CheckinClientCompany();
                if (txtEmail.Text == null)
                    txtEmail.Text = "";
                command.ClientEmail = txtEmail.Text.TrimEnd().TrimStart();
                command.Occupation = txtOcupation.Text;
                command.ClientQuantity = txtQtd.Text;
                command.CompanyId = Helpers.Settings.DisplayUserCompany;
                command.ClientName = txtName.Text;
                command.LocationId = _locationId;
                command.AllocationNumber = txtPrefix.Text;
                command.CheckinId = _checkin != null ? _checkin.Id : null;
                var json = JsonConvert.SerializeObject(command);

                await service.PostAsync(json, "check/checkinClientInCompany");



                this.DisplayAlert(AppResource.alertSucess, AppResource.alertCheckinRegisteredSucess, AppResource.textOk);

                await App.AppCurrent.NavigationService.NavigateSetRootAsync(new ClientListPage(), null, true);

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

        private async void LoadLocations()
        {
            CompanyService companyService = new CompanyService();

            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);

                var result = await companyService.GetAllLocationsByCompanyId();
                ListLocation = JsonConvert.DeserializeObject<List<Location>>(result);

                pckLocation.Items.Clear();
                pckLocation.Items.Add(AppResource.lblClickHereLocation);

                if (ListLocation != null)
                {
                    stkLocation.IsVisible = true;
                    stkOcuppation.IsVisible = true;
                    foreach (var item in ListLocation)
                    {
                        pckLocation.Items.Add(item.Name);
                    }

                    pckLocation.SelectedIndexChanged += PckLocation_SelectedIndexChanged;
                }
                pckLocation.SelectedItem = pckLocation.Items[0];

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

        private void PckLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = sender as Picker;

            if (item.SelectedIndex < 0)
                return;

            if(item.SelectedIndex != 0)
            {
                var loc = ListLocation.Where(m => m.Name == item.Items[item.SelectedIndex]).FirstOrDefault();

                _locationId = loc.Id;
                stkPrefix.IsVisible = true;

                lblPrefix.Text = loc.Prefix;

                if (loc.LocationType == Enums.LocationType.Room)
                    stkPrefix.IsVisible = false;
            }

        }

        
        class CheckinClientCompany
        {
            public string ClientQuantity { get; set; }
            public string CompanyId { get; set; }
            public string ClientEmail { get; set; }
            public string ClientName { get; set; }
            public string Occupation { get; set; }

            public string AllocationNumber { get; set; }
            public string LocationId { get; set; }
            public string CheckinId { get; set; }
        }
    }
}
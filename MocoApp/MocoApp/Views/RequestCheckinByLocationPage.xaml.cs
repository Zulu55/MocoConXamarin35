using MocoApp.Models;
using MocoApp.Resources;
using MocoApp.Services;
using MocoApp.Views.Empresa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MocoApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RequestCheckinByLocationPage : ContentPage
    {
        Company _company;
        string _locationId = "";
        string _locationName = "";
        string _ocuppation = "";
        Location _location;
        CreateOrder _createOrder;

        public RequestCheckinByLocationPage(Company company, string locationId, string locationName, string ocuppationNumber = "", Checkin ck = null, Location location = null, CreateOrder createOrder = null)
        {
            InitializeComponent();


            if (Device.RuntimePlatform == "Android")
                frmXaml.CornerRadius = 30;
            _company = company;

            _locationId = locationId;
            _locationName = locationName;
            _ocuppation = ocuppationNumber;
            txtOccupation.Text = _ocuppation;

            if (ck != null)
            {
                txtOccupation.Text = ck.Occupation;
                if (!string.IsNullOrEmpty(txtOccupation.Text))
                    txtOccupation.IsEnabled = false;
            }
            else
            {
                stkOccupation.IsVisible = true;
                txtOccupation.IsEnabled = true;
                txtAllocationNumber.IsVisible = true;
                stkQtd.IsVisible = true;
                stkAlocationNumber.IsVisible = true;
            }

            if (location != null)
                LoadTexts(location);

            _createOrder = createOrder;
            _location = location;
            ColorPage();

        }

        public void LoadTexts(Location location)
        {
            switch (location.LocationType)
            {
                case Enums.LocationType.Undefined:
                    break;
                case Enums.LocationType.Room:
                    stkOccupation.IsVisible = true;
                    stkQtd.IsVisible = false;
                    break;
                case Enums.LocationType.Bar:
                    stkOccupation.IsVisible = false;
                    stkAlocationNumber.IsVisible = true;
                    stkQtd.IsVisible = true;
                    lblAllocation.Text = location.Prefix;
                    break;
                case Enums.LocationType.Restaurant:
                    stkOccupation.IsVisible = false;
                    stkAlocationNumber.IsVisible = true;
                    stkQtd.IsVisible = true;
                    lblAllocation.Text = location.Prefix;
                    break;
                case Enums.LocationType.MeetingRoom:
                    stkOccupation.IsVisible = false;
                    stkAlocationNumber.IsVisible = true;
                    stkQtd.IsVisible = true;
                    lblAllocation.Text = location.Prefix;
                    break;
                case Enums.LocationType.Spa:
                    stkOccupation.IsVisible = false;
                    stkAlocationNumber.IsVisible = true;
                    stkQtd.IsVisible = true;
                    lblAllocation.Text = location.Prefix;
                    break;
                case Enums.LocationType.Valet:
                    stkAlocationNumber.IsVisible = true;
                    lblAllocation.Text = location.Prefix;
                    break;
                case Enums.LocationType.Concierge:
                    stkOccupation.IsVisible = true;
                    break;
                case Enums.LocationType.SwimmingPool:
                    lblAllocation.Text = location.Prefix;
                    stkAlocationNumber.IsVisible = true;
                    stkQtd.IsVisible = true;
                    break;
                default:
                    break;
            }

            //20190501 - deixei alwasy true pq a tela anteiror morreu
            //stkOccupation.IsVisible = true;
            if (_company.CompanyType == Enums.CompanyType.Hotel)
                stkOccupation.IsVisible = true;
            if (location.LocationType == Enums.LocationType.Room)
                stkAlocationNumber.IsVisible = false;

            if (string.IsNullOrEmpty(txtOccupation.Text) && _company.CompanyType == Enums.CompanyType.Hotel)
                stkOccupation.IsVisible = true;

            //mudei aqui por conta desse ticket https://trello.com/c/HryuRiDr/394-prefixo-missing-for-pool
            // mudei pool para aparecer prefixo... dia 05/06/2018 1:48
        }


        public void ColorPage()
        {
            switch (_company.CompanyType)
            {
                case Models.Enums.CompanyType.Hotel:
                    this.BackgroundColor = Color.FromHex("#6dadeb");
                    lblName.TextColor = Color.FromHex("#6dadeb");
                    imgBack.Source = "ic_voltar_hotel";
                    break;
                case Models.Enums.CompanyType.Restaurante:
                    this.BackgroundColor = Color.FromHex("#fdbf2c");
                    lblName.TextColor = Color.FromHex("#fdbf2c");
                    imgBack.Source = "ic_voltar_restaurante";
                    break;
                case Models.Enums.CompanyType.Praia:
                    //this.BackgroundColor = Color.FromHex("#009145");     
                    this.BackgroundImage = "bg_praia";
                    this.BackgroundColor = Color.Transparent;
                    lblName.TextColor = Color.FromHex("#009145");
                    imgBack.Source = "ic_voltar_praia";
                    break;
                default:
                    break;
            }
        }


        private async void OnBackTapped(object sender, EventArgs e)
        {
            await App.AppCurrent.NavigationService.ModalGoBack();
        }

        private async void OnOkTapped(object sender, EventArgs e)
        {
            //20190403 
            if (_location.LocationType == Enums.LocationType.Room)
            {
                if (string.IsNullOrEmpty(txtOccupation.Text))
                {
                    await this.DisplayAlert(MocoApp.Resources.AppResource.alertAlert, AppResource.alertFillRoomNumber, AppResource.textOk);
                    return;
                }

                //OnOkTapped(null, null);
            }

            //20190621 https://trello.com/c/8nkPZzgl/281-25-room-not-mandatory-for-other-locations-as-guest-restaurant-spa-pool-etc-all-except-room
            //if (string.IsNullOrEmpty(txtOccupation.Text) && stkOccupation.IsVisible)
            //{
            //    await this.DisplayAlert(MocoApp.Resources.AppResource.alertAlert, AppResource.alertFillRoomNumber, AppResource.textOk);
            //    return;
            //}


            CompanyService service = new CompanyService();

            try
            {
                if (string.IsNullOrEmpty(txtQtd.Text) && stkQtd.IsVisible)
                {
                    await this.DisplayAlert(MocoApp.Resources.AppResource.alertAlert, AppResource.alertInformNumberOfPeople, AppResource.textOk);
                    return;
                }
                else if (string.IsNullOrEmpty(txtAllocationNumber.Text) && stkAlocationNumber.IsVisible)
                {
                    await this.DisplayAlert(MocoApp.Resources.AppResource.alertAlert, string.Format(AppResource.alertNeedToFillAllocation, lblAllocation.Text.ToLower()), AppResource.textOk);
                    return;
                }

                RequestCheckin requestCheckin = new RequestCheckin();
                requestCheckin.CompanyId = _company.Id;
                requestCheckin.ClientQuantity = txtQtd.Text;
                requestCheckin.ClientId = Helpers.Settings.DisplayUserId;
                requestCheckin.Occupation = string.IsNullOrEmpty(_ocuppation) ? txtOccupation.Text : _ocuppation;
                requestCheckin.AllocationNumber = txtAllocationNumber.Text;
                requestCheckin.LocationId = _locationId;


                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);
                bool result = false;

                if (_company.CheckedIn && !string.IsNullOrEmpty(_locationId))
                {
                    var sub = new CheckinSub();
                    sub.LocationId = _locationId;
                    sub.AllocationNumber = txtAllocationNumber.Text;
                    sub.Occupation = string.IsNullOrEmpty(_ocuppation) ? txtOccupation.Text : _ocuppation;
                    sub.ClientQuantity = txtQtd.Text;
                    sub.ClientId = Helpers.Settings.DisplayUserId;

                    result = await service.RequestCheckinSub(sub);
                }
                else
                    result = await service.AddRemoveCheckinPost(requestCheckin);


                if (result)
                {
                    //await this.DisplayAlert(AppResource.textCheckin, AppResource.alertCheckinSucess, AppResource.textOk);

                    await App.AppCurrent.Cart.AddOrder(_createOrder, _company, _location);

                    App.AppCurrent.NavigationService.ModalGoBack();
                    //App.AppCurrent.NavigationService.ModalGoBack();

                }
                else
                    await this.DisplayAlert(AppResource.alertAlert, AppResource.alertSomethingWentWrong, AppResource.textOk);




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
}
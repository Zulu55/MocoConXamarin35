using MocoApp.Controls;
using MocoApp.Models;
using MocoApp.Resources;
using MocoApp.Services;
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
    public partial class AcceptDeclineCheckinPage : ContentPage
    {
        Checkin _checkin;
        CheckinSub _sub;
        public AcceptDeclineCheckinPage(Checkin checkin, CheckinSub sub = null)
        {
            InitializeComponent();

            _checkin = checkin;

            this.BindingContext = _checkin;

            if (Device.RuntimePlatform == "iOS")
            {
                btnAceitar.WidthRequest = 110;
                btnRejeitar.WidthRequest = 100;
            }
            _sub = sub;
            if (_checkin.Company.HasLocation && sub != null)
            {
                txtAllocation.Text = _sub.AllocationNumber;
                txtPeopleQtd.Text = _sub.ClientQuantity;
            }
 
            switch (_checkin.Company.CompanyType)
            {
                case Enums.CompanyType.Hotel:
                    lblAloc.Text = AppResource.lblRoom + " #";
                    break;
                case Enums.CompanyType.Restaurante:
                    lblAloc.Text = AppResource.lblTable + " #";
                    break;
                case Enums.CompanyType.Praia:
                    lblAloc.Text = AppResource.lblLocation + " #";
                    break;
            }

            if (string.IsNullOrEmpty(_checkin.Occupation))
                stkhaOccupation.IsVisible = false;
            else
                stkhaOccupation.IsVisible = true;




            if (_checkin.Company.HasLocation && _checkin.LocationType == Enums.LocationType.Room)
            {
                txtPeopleQtd.IsVisible = false;
                lblPeopleQtd.IsVisible = false;
                stkAllocation.IsVisible = false;
            }
            else
            {
                txtPeopleQtd.IsVisible = true;
                lblPeopleQtd.IsVisible = true;
                stkAllocation.IsVisible = true;
            }
        }

        private async void btnAddClient_Clicked(object sender, EventArgs e)
        {
            CompanyService service = new CompanyService();

            try
            {


                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);
                var result = await service.UpdateRequestedCheckin(_checkin.Id, txtAlocation.Text, txtPeopleQtd.Text, Enums.CheckinStatus.Checkin, txtAllocation.Text, _sub != null ? _sub.Id : "");

                _checkin.CheckinStatus = Enums.CheckinStatus.Checkin;
                await App.AppCurrent.NavigationService.GoBack();



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

        private void btnDecline_Clicked(object sender, EventArgs e)
        {

            string comment = "";
            var popup = new EntryPopup(AppResource.alertFillMotiveCancel, "", AppResource.alertSave, AppResource.alertCancel);
            popup.PopupClosed += async (o, closedArgs) =>
            {
                if (closedArgs.Button == AppResource.textOk || closedArgs.Button == AppResource.alertSave)
                {
                    comment = closedArgs.Text;

                    UpdateCheckin(comment);

                }

            };

            popup.Show();

        }

        public async void UpdateCheckin(string comment)
        {
            CompanyService service = new CompanyService();

            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);



                var result = await service.UpdateRequestedCheckin(_checkin.Id, txtAlocation.Text, txtPeopleQtd.Text, Enums.CheckinStatus.Denied, txtAllocation.Text, _sub?.Id, comment);


                _checkin.CheckinStatus = Enums.CheckinStatus.Denied;
                await App.AppCurrent.NavigationService.GoBack();


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
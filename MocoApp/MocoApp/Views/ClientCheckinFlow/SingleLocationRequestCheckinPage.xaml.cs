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

namespace MocoApp.Views.ClientCheckinFlow
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SingleLocationRequestCheckinPage : ContentPage
    {
        Checkin _checkin;
        Company _company;
        public SingleLocationRequestCheckinPage(Company company)
        {
            InitializeComponent();
            _company = company;

            if (Device.RuntimePlatform == "Android")
                frmXaml.CornerRadius = 30;

            LoadPage();
        }

        public void ColorPage()
        {
            //https://trello.com/c/7PGnZeNB/424-categoria-praia-hotel-ecc-except-hotel-should-not-ask-room-number-at-check-in
            //setei para false txtOccupation e lblOcuupation quando não é hotel

            switch (_company.CompanyType)
            {
                case Models.Enums.CompanyType.Hotel:
                    this.BackgroundColor = Color.FromHex("#6dadeb");
                    lblName.TextColor = Color.FromHex("#6dadeb");
                    imgBack.Source = "ic_voltar_hotel";
                    //txtOccupation.Placeholder = AppResource.textNotGuestHotel;
                    break;
                case Models.Enums.CompanyType.Restaurante:
                    this.BackgroundColor = Color.FromHex("#fdbf2c");
                    lblName.TextColor = Color.FromHex("#fdbf2c");
                    imgBack.Source = "ic_voltar_restaurante";
                    txtOccupation.IsVisible = false;
                    lblOcuupation.IsVisible = false;
                    //txtOccupation.Placeholder = AppResource.textNotHaveReservation;
                    break;
                case Models.Enums.CompanyType.Praia:
                    this.BackgroundImage = "bg_praia";
                    this.BackgroundColor = Color.Transparent;
                    lblName.TextColor = Color.FromHex("#009145");
                    imgBack.Source = "ic_voltar_praia";
                    txtOccupation.IsVisible = false;
                    lblOcuupation.IsVisible = false;
                    //txtOccupation.Placeholder = AppResource.textNotHaveReservation;
                    break;
                case Models.Enums.CompanyType.EsporteEvento:
                    this.BackgroundColor = (Color)App.Current.Resources["EsportesColor"];
                    lblName.TextColor = (Color)App.Current.Resources["EsportesColor"];
                    imgBack.Source = "ic_voltar_esportes";
                    txtOccupation.IsVisible = false;
                    lblOcuupation.IsVisible = false;
                    //txtOccupation.Placeholder = AppResource.textNotHaveReservation;
                    break;
            }
        }

        public async void LoadPage()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading, Acr.UserDialogs.MaskType.Black);
                ColorPage();
                OrderService orderService = new OrderService();
                var result = await orderService.GetCheckinByClienteAndCompanyId(_company.Id);
                var _resultDTO = JsonConvert.DeserializeObject<MultiLocationRequestCheckinDTO>(result);

                _checkin = _resultDTO.Checkin;
                _company = _resultDTO.Company;

                //lblOcuupation.Text = string.Format(AppResource.lblPleaseFill, _company.OccupationPrefix);
                lblOcuupation.Text = _company.OccupationPrefix;
                if (_checkin != null)
                {
                    stkCheckout.IsVisible = true;
                    lblCheckOut.Text = AppResource.textRequestCheckout + _checkin.Company.Title;
                }


                if (_checkin != null && !string.IsNullOrEmpty(_checkin.Occupation))
                    txtOccupation.IsEnabled = false;

                ColorPage();


                lblOcuupation.IsVisible = true;
                lblTextRoomNumber.IsVisible = true;
                txtOccupation.IsVisible = true;
                txtQtd.IsVisible = true;

            }
            catch (Exception ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast(AppResource.alertNoneCheckinFound);
                App.AppCurrent.NavigationService.ModalGoBack();
            }
            finally
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }

        }

        private async void OnCancelTapped(object sender, EventArgs e)
        {
            CompanyService service = new CompanyService();
            string msg = AppResource.alertWantcancelCheckout;
            string title = AppResource.textCheckout;

            var answer = await DisplayAlert(title, msg, AppResource.textOk, AppResource.alertCancel);

            if (!answer)
                return;

            Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);

            try
            {
                var result = await service.RequestCheckoutFromClient(_company.Id, 0);
                Acr.UserDialogs.UserDialogs.Instance.Toast(AppResource.alertRequestSucess);
                App.AppCurrent.NavigationService.ModalGoBack();

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

        private async void OnOkTapped(object sender, EventArgs e)
        {
            CompanyService service = new CompanyService();

            try
            {

                RequestCheckin requestCheckin = new RequestCheckin();

                requestCheckin.CompanyId = _company.Id;
                requestCheckin.ClientQuantity = txtQtd.Text;
                requestCheckin.ClientId = Helpers.Settings.DisplayUserId;
                requestCheckin.Occupation = txtOccupation.Text;
                requestCheckin.LocationId = "";


                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);
                bool result = false;

                if (string.IsNullOrEmpty(txtQtd.Text) && txtQtd.IsVisible)
                {
                    await this.DisplayAlert(MocoApp.Resources.AppResource.alertAlert, AppResource.alertInformQtdPeople, AppResource.textOk);
                    return;
                }
                else if (string.IsNullOrEmpty(txtOccupation.Text) && txtOccupation.IsVisible)
                {
                    await this.DisplayAlert(MocoApp.Resources.AppResource.alertAlert, string.Format(AppResource.alertNeedInformOccupation, txtOccupation.Text), AppResource.textOk);
                    return;
                }
                result = await service.AddRemoveCheckinPost(requestCheckin);





                if (result)
                {
                    await this.DisplayAlert(AppResource.textCheckin, AppResource.alertCheckinSucess, AppResource.textOk);
                    await App.AppCurrent.NavigationService.ModalGoBack();
                }
                else
                    await this.DisplayAlert(AppResource.alertAlert, AppResource.alertErrorOcured, AppResource.textOk);




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
using MocoApp.Resources;
using MocoApp.Services;
using MocoApp.Services.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MocoApp.Views.CartFlow
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddCardPage : ContentPage
    {
        bool _ret;
        public AddCardPage(bool ret = false)
        {
            InitializeComponent();
            if (Device.RuntimePlatform == "Android")
                frmXaml.CornerRadius = 30;

            _ret = ret;
        }

        private async void OnSave_Tapped(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtCardNumber.Text) || string.IsNullOrEmpty(txtCountry.Text) || string.IsNullOrEmpty(txtCardName.Text) || string.IsNullOrEmpty(txtcvv.Text) || string.IsNullOrEmpty(txtExp.Text))
                {
                    await DisplayAlert(AppResource.alertAlert, AppResource.alertAlert, AppResource.textOk);

                    return;
                }


                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);

                if (string.IsNullOrEmpty(lblBrand.Text) || lblBrand.Text == "Invalid Card Number")
                {
                    await DisplayAlert(AppResource.alertAlert, "Invalid Card Number", AppResource.textOk);

                    return;
                }


                var userService = new UserServicev2();

                var stripe = new StripeService();
                var expDate = txtExp.Text.Split('/');

                var token = stripe.Generate(txtCardNumber.Text, txtcvv.Text, Convert.ToInt32(expDate[0]), Convert.ToInt32(expDate[1]));

                var result = await userService.AddCardV2(token, "");
                //var result = await userService.AddCard(new Models.UserWallet()
                //{
                //    CardBrand = lblBrand.Text,
                //    CardName = txtCardName.Text,
                //    CVV = txtcvv.Text,
                //    UserId = Helpers.Settings.DisplayUserId,
                //    ExpirationDate = txtExp.Text,
                //    CardNumber = txtCardNumber.Text
                //}, Helpers.Settings.DisplayUserToken);

                if (result)
                {
                    App.AppCurrent.NavigationService.ModalGoBack();
                    return;

                    //if (_ret)
                    //    return;

                }
                else
                {
                    await DisplayAlert(AppResource.alertAlert, AppResource.alertAtention, AppResource.textOk);
                    return;
                }


                await App.AppCurrent.NavigationService.ModalGoBack();



            }
            catch (Exception ex)
            {

                await DisplayAlert(AppResource.alertAlert, ex.Message, AppResource.textOk);

            }
            finally
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }


            App.AppCurrent.NavigationService.ModalGoBack();
        }

        private void OnContOnBackinue_Tapped(object sender, EventArgs e)
        {
            App.AppCurrent.NavigationService.ModalGoBack();

        }

        private void TxtExp_TextChanged(object sender, TextChangedEventArgs e)
        {
            var str = sender as Entry;

            if (!string.IsNullOrEmpty(e.NewTextValue) && !string.IsNullOrEmpty(e.OldTextValue))
            {

                if (e.NewTextValue.Length > e.OldTextValue.Length)
                {
                    if (e.NewTextValue.Length == 2)
                    {
                        str.Text = str.Text + "/";
                    }
                }

            }

        }

        public string GetCreditCardType(string CreditCardNumber)
        {
            Regex regVisa = new Regex("^4[0-9]{12}(?:[0-9]{3})?$");
            Regex regMaster = new Regex("^5[1-5][0-9]{14}$");
            Regex regExpress = new Regex("^3[47][0-9]{13}$");
            Regex regDiners = new Regex("^3(?:0[0-5]|[68][0-9])[0-9]{11}$");
            Regex regDiscover = new Regex("^6(?:011|5[0-9]{2})[0-9]{12}$");
            Regex regJCB = new Regex("^(?:2131|1800|35\\d{3})\\d{11}$");

            lblBrand.TextColor = Color.Green;

            if (regVisa.IsMatch(CreditCardNumber))
                return "Visa";
            else if (regMaster.IsMatch(CreditCardNumber))
                return "MasterCard";
            else if (regExpress.IsMatch(CreditCardNumber))
                return "Amex";
            else if (regDiners.IsMatch(CreditCardNumber))
                return "Diners";
            else if (regDiscover.IsMatch(CreditCardNumber))
                return "Discovers";
            else if (regJCB.IsMatch(CreditCardNumber))
                return "Jcb";
            else
            {
                lblBrand.TextColor = Color.Red;
                return "Invalid Card Number";
            }
        }

        //test validation
        ////https://clevercreditcards.com
        //Validate("4169773331987017");//visa
        //Validate("4658958254583145");//visa
        //Validate("4771320594033780");//visa

        //Validate("5410710000901089");//mc
        //Validate("5289675573349651");//mc
        //Validate("5582128534772839");//mc

        //Validate("349101032764066");//ae
        //Validate("343042534582349");//ae
        //Validate("371305972529535");//ae

        //Validate("6011683204539909");//discover
        //Validate("6011488563514596");//discover
        //Validate("6011465836488204");//discover

        //Validate("3529908921371639");//jcb
        //Validate("3589295535870728");//jcb
        //Validate("3569239206830557");//jcb

        private void TxtCardNumber_Unfocused(object sender, FocusEventArgs e)
        {
            lblBrand.Text = GetCreditCardType(txtCardNumber.Text);


        }
    }
}
using MocoApp.Resources;
using MocoApp.Views;
using Naylah.Xamarin.Services.NavigationService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MocoApp.ViewModels
{
    public class SplashViewModel : BaseViewModel
    {
        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode)
        {
            //await LoadData();
        }

        public async Task LoadData()
        {
            var a = AppResource.LblEnterAsGuest;

            // verifica se esta logado ou não            

            try
            {
                if (string.IsNullOrEmpty(Helpers.Settings.DisplayUserId))
                {
                    await NavigationService.NavigateSetRootAsync(new WelcomePage(), null, false);
                }
                else
                {
                    App.AppCurrent.GuestLogin = true;
                    await App.AppCurrent.ConfigureAppPhase();
                }
            }
            catch (Exception e)
            {


            }

        }
    }
}

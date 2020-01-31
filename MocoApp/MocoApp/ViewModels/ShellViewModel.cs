using MocoApp.Resources;
using MocoApp.Views.Menu;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MocoApp.ViewModels
{
    public class ShellViewModel : BaseViewModel
    {
        public async Task NavigateToSelectedMenuItem(NavMenuItem navMenuItem)
        {
            if (navMenuItem == null)
            {
                return;
            }

            try
            {
                Page displayPage = (Page)Activator.CreateInstance(navMenuItem.TargetType);

                if (!string.IsNullOrEmpty(navMenuItem.Title))
                {
                    if (navMenuItem.Icon == "ic_sair")
                    {
                        App.AppCurrent.Logout();
                        return;
                    }

                    await NavigationService.NavigateSetRootAsync(displayPage, null, true);
                }
                else
                {
                    await NavigationService.ClearHistory();
                    await NavigationService.NavigateAsync(displayPage, null, true);
                }

                displayPage.Title = navMenuItem.Title;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(AppResource.alertAlert, "Erro " + ex.Message, AppResource.textOk);
            }
        }
    }
}

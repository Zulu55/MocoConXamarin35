using MocoApp.ViewModels;
using MocoApp.Views.Menu;
using Naylah.Xamarin.Controls.Pages;
using System;
using Xamarin.Forms;

namespace MocoApp.Views
{
    public class ShellPage : MasterDetailNavigationPage
    {
        public ShellViewModel Vm => this.BindingContext as ShellViewModel;

        MenuPageXaml MasterAsMenuPage { get { return Master as MenuPageXaml; } }

        public ShellPage()
        {

            ((NavigationPage)Detail).BarBackgroundColor = Color.FromHex("#ffdb14");
            ((NavigationPage)Detail).BarTextColor = Color.White;

            Master = new MenuPageXaml();
            IsGestureEnabled = true;

            BindingContext = new ShellViewModel();

            MasterAsMenuPage.MenusItemTapped += async (s, i) =>
            {
                await Vm.NavigateToSelectedMenuItem(i.Item as NavMenuItem);
            };

            IsPresentedChanged += ShellPage_IsPresentedChanged;
        }

        private void ShellPage_IsPresentedChanged(object sender, EventArgs e)
        {
        }

        public void NavigationService_Navigating(object sender, EventArgs e)
        {
            try
            {
                IsPresented = false;
            }
            catch (Exception)
            {
            }
        }

        public void NavigationService_Navigated(object sender, EventArgs e)
        {
            try
            {
                MasterAsMenuPage.SelectByPage(Vm.NavigationService.CurrentPage);
            }
            catch (Exception)
            {
            }
        }
    }
}

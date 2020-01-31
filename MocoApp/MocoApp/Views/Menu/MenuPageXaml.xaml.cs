using Plugin.DeviceInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MocoApp.Views.Menu
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPageXaml : ContentPage
    {
        public MenuPageXaml()
        {
            InitializeComponent();
            MenuListData.LoadItems();

            Title = "Menu";

            _menuListViewTop.ItemsSource = MenuListData.TopItems;
            _menuListViewBottom.ItemsSource = MenuListData.BottomItems;

            int MenuItemsHeight = 48;

            int TopMenuItemsHeightRequest = (MenuItemsHeight + 2) * MenuListData.TopItems.Count;
            int BottomMenuItemsHeightRequest = (MenuItemsHeight + 2) * MenuListData.BottomItems.Count;

            _menuListViewTop.HeightRequest = TopMenuItemsHeightRequest;
            _menuListViewBottom.HeightRequest = BottomMenuItemsHeightRequest;

            //_menuListViewTop.RowHeight = MenuItemsHeight;
            //_menuListViewBottom.RowHeight = MenuItemsHeight;

            _menuListViewTop.ItemTapped += (s, i) => { MenusItemTapped?.Invoke(s, i); };
            _menuListViewBottom.ItemTapped += (s, i) => { MenusItemTapped?.Invoke(s, i); };

            imgUser.Source = Helpers.Settings.DisplayUserPhoto;
            lblUserName.Text = Helpers.Settings.DisplayUserName;

            if(Device.RuntimePlatform == "iOS")
                lblVersion.Text = CrossDeviceInfo.Current.AppBuild;
            else
                lblVersion.Text = CrossDeviceInfo.Current.AppVersion;
        }

      
      

        public void SelectByPage(Page page)
        {
            imgUser.Source = Helpers.Settings.DisplayUserPhoto;
            _menuListViewTop.SelectedItem = null;
            _menuListViewBottom.SelectedItem = null;

            _menuListViewTop.SelectedItem = MenuListData.TopItems.Where(x => x.TargetType == page.GetType()).FirstOrDefault();
            _menuListViewBottom.SelectedItem = MenuListData.BottomItems.Where(x => x.TargetType == page.GetType()).FirstOrDefault();
        }

        public event EventHandler<ItemTappedEventArgs> MenusItemTapped;
        private void MenuListViewItemTapped(object sender, ItemTappedEventArgs e)
        {


            if (_menuListViewTop != null)
            {
                _menuListViewTop.SelectedItem = null;
            }

            if (_menuListViewBottom != null)
            {
                _menuListViewBottom.SelectedItem = null;
            }

        }

        private async void OnImageProfileTapped(object sender, EventArgs e)
        {
           // await App.AppCurrent.NavigationService.NavigateSetRootAsync(new Views.PerfilPage(), null, true);
        }
    }
}
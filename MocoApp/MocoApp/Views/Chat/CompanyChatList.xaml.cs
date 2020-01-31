using MocoApp.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MocoApp.Views.Chat
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CompanyChatList : ContentPage
    {

        public ApiService service = new ApiService();
        public CompanyChatList()
        {
            InitializeComponent();

            if (Device.OS == TargetPlatform.iOS)
            {
                // move layout under the status bar
                this.Padding = new Thickness(0, 0, 0, 0);

                var menu = new ToolbarItem("Menu", "", () =>
                {
                    App app = Application.Current as App;
                    Naylah.Xamarin.Controls.Pages.MasterDetailNavigationPage md = (Naylah.Xamarin.Controls.Pages.MasterDetailNavigationPage)app.MainPage;

                    md.IsPresented = true;

                }, 0, 0);
                menu.Priority = 0;

                ToolbarItems.Add(menu);
            }
        }

        public async void LoadChats()
        {
            try
            {
                var result = await service.GetAsync("v1/chat/getCompanyChats?companyId=" + Helpers.Settings.DisplayUserCompany);

                var data = JsonConvert.DeserializeObject<List<ChatList>>(result);
                chatList.ItemsSource = null;
                chatList.ItemsSource = data;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private async void ChatItem_Tapped(object sender, ItemTappedEventArgs e)
        {
            chatList.SelectedItem = null;
            var obj = e.Item as ChatList;
            await App.AppCurrent.NavigationService.NavigateAsync(new CompanyChatPage(obj.id, obj.clientName, obj.companyId), null, true);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadChats();
        }
    }
}
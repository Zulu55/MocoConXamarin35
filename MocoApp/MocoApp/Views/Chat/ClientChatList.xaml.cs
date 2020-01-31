using MocoApp.Models;
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
    public partial class ClientChatList : ContentPage
    {
        public ApiService service = new ApiService();

        public ClientChatList()
        {
            InitializeComponent();

            if (Device.RuntimePlatform == "Android")
                frmXaml.CornerRadius = 30;

            NavigationPage.SetHasNavigationBar(this, false);
      
        }

        public async void LoadChats()
        {
            try
            {
                var result = await service.GetAsync("/v1/chat/getUserChats");
                Debug.WriteLine(result);

                var data = JsonConvert.DeserializeObject<List<ChatList>>(result);
                chatList.ItemsSource = null;
                chatList.ItemsSource = data.OrderByDescending(m => m.lastMessageDate);

            } catch (Exception ex)
            {

            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadChats();
        }

        private async void OnMenuTapped(object sender, EventArgs e)
        {
            App app = Application.Current as App;
            Naylah.Xamarin.Controls.Pages.MasterDetailNavigationPage md = (Naylah.Xamarin.Controls.Pages.MasterDetailNavigationPage)app.MainPage;
            md.IsPresented = true;

        }


        private async void ChatItem_Tapped(object sender, ItemTappedEventArgs e)
        {
            chatList.SelectedItem = null;
            var obj = e.Item as ChatList;
            await App.AppCurrent.NavigationService.NavigateModalAsync(new ChatPage(obj.companyId, obj.title, obj.imageUri), null, true);
        }
    }

    public class ChatList
    {
        public string id { get; set; }
        public DateTime lastMessageDate { get; set; }
        public string lastMessageDateStr { get; set; }
        public string lastMessage { get; set; }
        public string companyId { get; set; }
        public string clientName { get; set; }
        public string title { get; set; }
        public string imageUri { get; set; }
        public string photo { get; set; }
        public int chatMessageStatus { get; set; }
        public bool isRead { get; set; }

        // 
        public bool HasNewMessages => isRead ? false : true;
    }
}
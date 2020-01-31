using MocoApp.Models;
using MocoApp.Resources;
using MocoApp.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MocoApp.Views.Chat
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CompanyChatPage : ContentPage
    {

        string loadedChatId = "";
        string loadedCompanyId = "";

        CancellationTokenSource source;
        bool reloadMessages = false;

        ApiService service = new ApiService();
        public CompanyChatPage(string chatId, string clientName, string companyId )
        {
            InitializeComponent();
            loadedChatId = chatId;
            loadedCompanyId = companyId;
            reloadMessages = true;
            source = new CancellationTokenSource();


            msgList.ItemTapped += (sender, args) => {
                msgList.SelectedItem = null;
            };

            Title = clientName;

            Device.BeginInvokeOnMainThread(async () =>
            {
                await ReloadMessages(source.Token);
            });

            
        }

        public async void SendMessage(string _message)
        {
            try
            {
                var pl = JsonConvert.SerializeObject(new ChatPayload() { ChatId = loadedChatId, CompanyId = loadedCompanyId, Message = _message });
                var msg = await service.PutAsync(pl, "/v1/chat/createMessage");
                //Debug.WriteLine(msg);

                LoadMessages(loadedChatId, true);
            }
            catch (Exception ex)
            {

            }
        }

        private async Task ReloadMessages(CancellationToken cts)
        {
            while (reloadMessages)
            {
                LoadMessages(loadedChatId, false);
                await Task.Delay(8000);
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            reloadMessages = false;
            source.Cancel();
        }

        public async void LoadMessages(string chatId, bool animateScroll)
        {
            try
            {
                var result = await service.GetAsync("v1/chat/getMessagesByChatId?chatId=" + chatId);
                var data = JsonConvert.DeserializeObject<List<ChatMessage>>(result);

                // todo: arrumar aq? pra vir na primeira chamada so o chat id
                loadedChatId = data[0].chatId;

                // propzinha humilde de trocar o lado
                foreach (var item in data)
                {
                    item.IsOnCompanyChatPage = true;
                }

                data.Reverse();                
                msgList.ItemsSource = data;
                msgList.ScrollTo(data.Last(), ScrollToPosition.Start, animateScroll);
            }

            catch (Exception ex)
            {

            }
        }

        private void Send_Tapped(object sender, EventArgs e)
        {
            SendMessage(msgEntry.Text);
            msgEntry.Text = "";
        }

        private void MsgEntry_Completed(object sender, EventArgs e)
        {
            SendMessage(msgEntry.Text);
            msgEntry.Text = "";
        }
    }

  
}
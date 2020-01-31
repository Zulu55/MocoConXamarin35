using MocoApp.Models;
using MocoApp.Resources;
using MocoApp.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MocoApp.Views.Chat
{
    public class ChatPayload
    {
        public string ChatId { get; set; }
        public string CompanyId { get; set; }
        public string Message { get; set; }
        public string ClientId { get; set; }
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatPage : ContentPage
    {
        private readonly string _loadedCompId = "";
        private string _loadedChatId = "";
        private readonly ApiService _service = new ApiService();
        private readonly CancellationTokenSource _source;
        private bool _reloadMessages = false;

        public ChatPage(string compId, string compName, string compImage)
        {
            InitializeComponent();
            CanSend = true;
            _source = new CancellationTokenSource();

            if (Device.RuntimePlatform == "Android")
            {
                frmXaml.CornerRadius = 30;
            }

            NavigationPage.SetHasNavigationBar(this, false);

            _loadedCompId = compId;
            _reloadMessages = true;

            msgList.ItemTapped += (sender, args) =>
            {
                msgList.SelectedItem = null;
            };

            lblTitle.Text = compName;
            profilePic.Source = compImage;

            Device.BeginInvokeOnMainThread(async () =>
            {
                await ReloadMessages(_source.Token);
            });

            CheckCheckIn();
        }

        public bool CanSend { get; set; }

        private async void CheckCheckIn()
        {
            var companyService = new CompanyService();
            var companyString = await companyService.GetCompanyById(_loadedCompId);
            var company = JsonConvert.DeserializeObject<Company>(companyString);

            if (company.AllowChatOnlyForCustomers)
            {
                var isChecked = await companyService.IsCheckedIn(company.Id, Helpers.Settings.DisplayUserToken);
                if (!isChecked)
                {
                    await DisplayAlert(AppResource.msgError, AppResource.msgNotCheckIn, AppResource.msgAccept);
                    OnBackButtonPressed();
                }
            }
        }

        private async Task ReloadMessages(CancellationToken cts)
        {
            while (_reloadMessages)
            {
                LoadMessages(_loadedCompId, false);
                await Task.Delay(8000);
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _reloadMessages = false;
            _source.Cancel();
        }

        private async void OnMenuTapped(object sender, EventArgs e)
        {
            await App.AppCurrent.NavigationService.ModalGoBack();
        }

        public async void SendMessage(string _message)
        {
            try
            {
                CanSend = false;
                string pl = JsonConvert.SerializeObject(new ChatPayload() 
                { 
                    ChatId = _loadedChatId, 
                    CompanyId = _loadedCompId, 
                    Message = _message, 
                    ClientId = Helpers.Settings.DisplayUserId 
                });
                
                string msg = await _service.PutAsync(pl, "/v1/chat/createMessage");

                await LoadMessages(_loadedCompId, true);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            finally
            {
                CanSend = true;
            }
        }

        public async Task LoadMessages(string compId, bool animateScroll)
        {
            List<ChatMessage> data = new List<ChatMessage>();
            try
            {
                if (!CanSend)
                {
                    return;
                }

                string result = await _service.GetAsync("v1/chat/getChatByCompanyId?companyId=" + compId);
                data = JsonConvert.DeserializeObject<List<ChatMessage>>(result);

                if (data == null || data.Count() < 1)
                {
                    data = new List<ChatMessage>();
                    data.Insert(0, new ChatMessage()
                    {
                        fromClient = false,
                        chatMessageStatus = 2,
                        message = string.Format(AppResource.textWelcomeToChat, Helpers.Settings.DisplayUserName)
                    });
                    msgList.ItemsSource = data;
                }
                else
                {
                    _loadedChatId = data.First().chatId;
                    Debug.WriteLine(result);
                    data.Reverse();
                    data.Insert(0, new ChatMessage()
                    {
                        fromClient = false,
                        chatMessageStatus = 2,
                        message = string.Format(AppResource.textWelcomeToChat, Helpers.Settings.DisplayUserName)
                    });
                    msgList.ItemsSource = data;
                    msgList.ScrollTo(data.Last(), ScrollToPosition.Start, animateScroll);

                    sendImage.IsVisible = true;
                    sendingIndicator.IsVisible = false;
                    sendingIndicator.IsRunning = false;
                }
            }

            catch (Exception)
            {

            }
        }

        private void Send_Tapped(object sender, EventArgs e)
        {
            SendMessage(msgEntry.Text);
            msgEntry.Text = "";
            sendImage.IsVisible = false;
            sendingIndicator.IsVisible = true;
            sendingIndicator.IsRunning = true;
        }

        private void MsgEntry_Completed(object sender, EventArgs e)
        {
            SendMessage(msgEntry.Text);
            msgEntry.Text = "";
            sendImage.IsVisible = false;
            sendingIndicator.IsVisible = true;
            sendingIndicator.IsRunning = true;
        }
    }

    // not used mais
    public class ChatTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ReceivedTemplate { get; set; }
        public DataTemplate SentTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            string s = item as string;

            if (s == null)
            {
                return ReceivedTemplate;
            }

            if (s == "enviada")
            {
                return SentTemplate;
            }
            else if (s == "recebida")
            {
                return ReceivedTemplate;
            }
            else
            {
                return SentTemplate;
            }
        }
    }
}
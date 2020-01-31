using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MocoApp.Models
{
    public class ChatMessage
    {
        public string chatId { get; set; }
        public string userId { get; set; }
        public string id { get; set; }
        public string message { get; set; }
        public bool fromClient { get; set; }
        public string imageUri { get; set; }
        public string name { get; set; }
        public DateTime? readAt { get; set; }
        public int? chatMessageStatus { get; set; }
        public DateTime? createdAt { get; set; }


        //[JsonIgnore] public LayoutOptions Alignment => fromClient ? LayoutOptions.EndAndExpand : LayoutOptions.StartAndExpand;
        //[JsonIgnore] public LayoutOptions ReverseAlignment => fromClient ? LayoutOptions.End: LayoutOptions.Start;
        [JsonIgnore] public string FormattedTime => createdAt.HasValue ? createdAt.Value.ToString("hh:mm") : "";
        [JsonIgnore] public bool IsOnCompanyChatPage { get; set; } = false;
        [JsonIgnore] public string StatusIcon
        {
            get
            {
                if (chatMessageStatus == 2)
                    return "ic_msg_read";
                else
                    return "ic_msg_unread";
            }
        }
    }
}

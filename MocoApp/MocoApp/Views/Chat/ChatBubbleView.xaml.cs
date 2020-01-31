using MocoApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MocoApp.Views.Chat
{


	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ChatBubbleView : ViewCell
	{

        public static ColumnDefinitionCollection GridAlignmentLeft = new ColumnDefinitionCollection()
            {
                new ColumnDefinition() { Width = new GridLength(8, GridUnitType.Star)},
                new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star)}
            };

        public static ColumnDefinitionCollection GridAlignmentRight =  new ColumnDefinitionCollection()
            {
                new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star)},
                new ColumnDefinition() { Width = new GridLength(8, GridUnitType.Star)}
            };


        public ChatBubbleView()
        {
            InitializeComponent();

            if (Device.RuntimePlatform == "iOS")
            {
                bubbleFrame.CornerRadius = 0;
                bubbleFrame.Margin = 3;
                bubbleFrame.Padding = 20;                
            }
            else
            {
                bubbleFrame.CornerRadius = 8;
            }
        }

        protected override void OnBindingContextChanged()
        {
            var s = BindingContext as ChatMessage;

            if (s != null)
            {
                if (s.IsOnCompanyChatPage)
                {
                    bubbleGrid.ColumnDefinitions = s.fromClient ? GridAlignmentLeft : GridAlignmentRight;
                    Grid.SetColumn(bubbleFrame, s.fromClient ? 0 : 1);

                }
                else
                {
                    bubbleGrid.ColumnDefinitions = s.fromClient ? GridAlignmentRight : GridAlignmentLeft;
                    Grid.SetColumn(bubbleFrame, s.fromClient ? 1 : 0);
                }
            }

            base.OnBindingContextChanged();
        }
    }

}
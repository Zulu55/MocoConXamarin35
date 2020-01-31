using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MocoApp.Droid.Renderers;
using MocoApp.Interfaces;
using System.Runtime.CompilerServices;
using MocoApp.Controls;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(EntryPopupLoader))]
namespace MocoApp.Droid.Renderers
{
    public class EntryPopupLoader : IEntryPopupLoader
    {
        public void ShowPopup(EntryPopup popup)
        {
            var alert = new AlertDialog.Builder(Forms.Context);

            var edit = new EditText(Forms.Context) { Text = popup.Text };
            alert.SetView(edit);

            alert.SetTitle(popup.Title);

            alert.SetPositiveButton("OK", (senderAlert, args) =>
            {
                popup.OnPopupClosed(new EntryPopupClosedArgs
                {
                    Button = "OK",
                    Text = edit.Text
                });
            });

            alert.SetNegativeButton("Cancel", (senderAlert, args) =>
            {
                popup.OnPopupClosed(new EntryPopupClosedArgs
                {
                    Button = "Cancel",
                    Text = edit.Text
                });
            });
            alert.Show();
        }
    }
}
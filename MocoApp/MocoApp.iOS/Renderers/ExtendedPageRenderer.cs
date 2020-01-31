using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using MocoApp.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.Reflection;

[assembly: ExportRenderer(typeof(ContentPage), typeof(ExtendedPageRenderer))]
namespace MocoApp.iOS.Renderers
{
    public class ExtendedPageRenderer : PageRenderer
    {

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);





            var contentPage = this.Element as ContentPage;
            if (contentPage == null || NavigationController == null)
            {
                return;
            }

            var itemsInfo = contentPage.ToolbarItems;


            var navigationItem = this.NavigationController.TopViewController.NavigationItem;
            var leftNativeButtons = (navigationItem.LeftBarButtonItems ?? new UIBarButtonItem[] { }).ToList();
            var rightNativeButtons = (navigationItem.RightBarButtonItems ?? new UIBarButtonItem[] { }).ToList();

            if (leftNativeButtons.Count() > 0)
                return;

            foreach (var nativeItem in rightNativeButtons)
            {
                var field = nativeItem.GetType().GetField("_item", BindingFlags.NonPublic | BindingFlags.Instance);
                if (field == null)
                {
                    return;
                }

                var info = field.GetValue(nativeItem) as ToolbarItem;
                if (info != null && info.Priority == 0)
                {

                    leftNativeButtons.Add(nativeItem);
                    rightNativeButtons.Remove(nativeItem);


                    break;
                }
            }

            navigationItem.RightBarButtonItems = rightNativeButtons.ToArray();
            navigationItem.LeftBarButtonItems = leftNativeButtons.ToArray();
        }


    }
}
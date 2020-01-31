using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using MocoApp.iOS.Renderers;
using CoreGraphics;
using System.Drawing;

[assembly: ExportRenderer(typeof(Entry), typeof(EntryCustomRenderer))]
namespace MocoApp.iOS.Renderers
{
    public class EntryCustomRenderer : EntryRenderer
    {
        private CoreAnimation.CALayer _line;
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            this.Control.InputAccessoryView = null;

            //this.Control.BorderStyle = UITextBorderStyle.None;

            //_line = null;

            //if (Control == null || e.NewElement == null)
            //    return;

            //Control.BorderStyle = UITextBorderStyle.None;

            //_line = new CoreAnimation.CALayer
            //{
            //    BorderColor = UIColor.FromRGB(174, 174, 174).CGColor,
            //    BackgroundColor = UIColor.FromRGB(174, 174, 174).CGColor,
            //    Frame = new CGRect(e.NewElement.Height, e.NewElement.Width, 1, 1)
            //};

            //Control.Layer.AddSublayer(_line);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Forms.Platform.iOS;
using CoreGraphics;
using MocoApp.iOS.Renderers;
using Xamarin.Forms;
using System.Drawing;

[assembly: ExportRenderer(typeof(Frame), typeof(MaterialFrameRenderer))]
namespace MocoApp.iOS.Renderers
{
    public class MaterialFrameRenderer : VisualElementRenderer<Frame>
    {
        private Frame _control;
        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null)
            {
                _control = e.NewElement as Frame;
                this.SetupLayer();
            }
        }

        private void SetupLayer()
        {

            Layer.CornerRadius = 50;
            
            //if (Element.BackgroundColor != Xamarin.Forms.Color.Default)
            //{
            //    Layer.BackgroundColor = Element.BackgroundColor.ToCGColor();
            //}
            //else
            //{
            //    Layer.BackgroundColor = UIColor.White.CGColor;
            //}
            ////if (!base.Element.HasShadow)
            ////{
            ////	this.get_Layer().set_ShadowOpacity(0f);
            ////}
            ////else
            ////{
            ////	this.get_Layer().set_ShadowRadius(5);
            ////	this.get_Layer().set_ShadowColor(UIColor.get_Black().get_CGColor());
            ////	this.get_Layer().set_ShadowOpacity(0.8f);
            ////	this.get_Layer().set_ShadowOffset(new SizeF());
            ////}
            //if (Element.OutlineColor != Xamarin.Forms.Color.Default)
            //{
            //    this.Layer.BackgroundColor = base.Element.OutlineColor.ToCGColor();
            //    this.Layer.BorderWidth = 15;
            //}
            //else
            //{
            //    this.Layer.BackgroundColor = UIColor.Clear.CGColor;
            //}
            //this.Layer.RasterizationScale = UIScreen.MainScreen.Scale;
            //this.Layer.ShouldRasterize = true;
        }

    }
}
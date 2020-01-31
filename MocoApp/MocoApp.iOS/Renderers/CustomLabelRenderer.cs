using System;
using System.Collections.Generic;
using System.Text;
using MocoApp.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Xamarin.Forms.Label), typeof(CustomLabelRenderer))]
namespace MocoApp.iOS.Renderers
{
    public class CustomLabelRenderer : LabelRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                e.NewElement.FontAttributes = FontAttributes.None;

                
                e.NewElement.FontFamily = "OpenSans-Regular.ttf";
            }

        }
    }
}
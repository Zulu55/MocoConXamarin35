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
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using MocoApp.Droid.Renderers;

//[assembly: ExportRenderer(typeof(Frame), typeof(CustomFrameRenderer))]
namespace MocoApp.Droid.Renderers
{
    public class CustomFrameRenderer : FrameRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);
            //if (e.NewElement != null)
            //    e.NewElement.CornerRadius = (float)50;


        }
    }
}
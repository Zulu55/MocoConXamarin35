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
using MocoApp.Interfaces;
using Android.Graphics;
using System.IO;
using MocoApp.Droid.Renderers;

[assembly: Xamarin.Forms.Dependency(typeof(ImageResizer))]
namespace MocoApp.Droid.Renderers
{
    public class ImageResizer : IImageResizer
    {

        public byte[] Resize(byte[] imageData, float width, float height)
        {

            return ResizeImageAndroid(imageData, width, height);
        }

        public static byte[] ResizeImageAndroid(byte[] imageData, float width, float height)
        {
            return imageData;
            // Load the bitmap
            Bitmap originalImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
            var imageHeight = originalImage.Height;
            var imageWidth = originalImage.Width;

            var ratioX = (double)width / originalImage.Width;
            var ratioY = (double)height / originalImage.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(originalImage.Width * ratio);
            var newHeight = (int)(originalImage.Height * ratio);

            if (newWidth < 1 || newHeight < 1)
                return null;



            Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)newWidth, (int)newHeight, false);

            using (MemoryStream ms = new MemoryStream())
            {
                resizedImage.Compress(Bitmap.CompressFormat.Png, 0, ms);
                return ms.ToArray();
            }


        }

    }
}  
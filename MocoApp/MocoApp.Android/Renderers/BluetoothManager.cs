using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Android.Speech.Tts;
using Xamarin.Forms;
using MocoApp.Interfaces;


[assembly: Dependency(typeof(MocoApp.Droid.Renderers.BluetoothManager))]
namespace MocoApp.Droid.Renderers
{
    public class BluetoothManager : Java.Lang.Object, IBluetoothManager, TextToSpeech.IOnInitListener
    {
        private const string UuidUniverseProfile = "00001101-0000-1000-8000-00805f9b34fb";

        private Stream mmOutStream = null;

        private BluetoothSocket socket = null;

        private BluetoothAdapter btAdapter = null;

        private BluetoothDevice printer = null;

        private Java.Util.UUID UUID = null;


        public void ImprimirBitmap(Bitmap bitmap)
        {
            try
            {

                byte[] GS_DOWNLOAD = new byte[] { 0x1D, 0x2A };
                int formatBytes = 8;
                int? density = 8;
                byte[] init = new byte[] { 27, 64 };
                byte[] FW_DENSITY = new byte[] { 0x1D, 0x45 };
                byte[] GS_PRINT_NORMAL = new byte[] { 0x1D, 0x2F, 0x00 };
                byte[] LF = new byte[] { 10 };


                mmOutStream.Write(init, 0, init.Length);
                if (density != null)
                {
                    byte[] byteDensity = { (byte)(int)density };
                    mmOutStream.Write(FW_DENSITY, 0, FW_DENSITY.Length);
                    mmOutStream.Write(byteDensity, 0, byteDensity.Length);
                }



                int wid = bitmap.Width;
                int hei = bitmap.Height;
                byte[] blen = new byte[2];

                int[] dots = new int[wid * hei];
                bitmap.GetPixels(dots, 0, wid, 0, 0, wid, hei);

                blen[0] = (byte)(wid / 8);
                blen[1] = (byte)formatBytes;

                int offset = 0;

                while (offset < hei)
                {
                    mmOutStream.Write(GS_DOWNLOAD, 0, GS_DOWNLOAD.Length);
                    mmOutStream.Write(blen, 0, blen.Length);

                    for (int x = 0; x < wid; ++x)
                    {
                        for (int k = 0; k < formatBytes; ++k)
                        {
                            byte slice = 0;

                            for (int b = 0; b < 8; ++b)
                            {
                                int y = (((offset / 8) + k) * 8) + b;

                                int i = (y * wid) + x;

                                bool v = false;
                                if (i < dots.Length)
                                {
                                    v = (dots[i] == Android.Graphics.Color.Black);
                                }

                                slice |= (byte)((v ? 1 : 0) << (7 - b));
                            }

                            mmOutStream.WriteByte(slice);
                        }
                    }
                    offset += (formatBytes * 8);

                    mmOutStream.Write(GS_PRINT_NORMAL, 0, GS_PRINT_NORMAL.Length);

                }

                mmOutStream.Write(LF, 0, LF.Length);
                mmOutStream.Flush();



            }
            catch (Java.IO.IOException e)
            {
                throw;
            }
        }


        #region Interface
        public async Task PrintText(byte[] text)
        {
            if (socket.IsConnected)
            {
                mmOutStream.Write(text, 0, text.Length);
            }
        }

        public async Task PrintImage(byte[] image)
        {
            if (socket.IsConnected)
            {
                var imageSize = (image?.Length ?? 0) / 1000;
                Bitmap bitmap = null;
                if (imageSize > 2)
                    bitmap = BitmapFactory.DecodeByteArray(image, 0, image.Length, new BitmapFactory.Options { InSampleSize = 2 });
                else
                    bitmap = BitmapFactory.DecodeByteArray(image, 0, image.Length);

                ImprimirBitmap(bitmap);

                await Task.Delay(1500);

            }
        }

        public async Task OpenOutputStream()
        {
            try
            {
                if (btAdapter == null)
                {
                    btAdapter = BluetoothAdapter.DefaultAdapter;

                    var devices = btAdapter.BondedDevices;

                    if (devices != null && devices.Count > 0)
                    {
                        printer = devices.FirstOrDefault();

                        UUID = Java.Util.UUID.FromString(UuidUniverseProfile);
                        //aguarda 1s
                        Thread.Sleep(2300);

                        //aguarda 1s
                        //Thread.Sleep(1000);

                        socket = printer.CreateRfcommSocketToServiceRecord(UUID);

                        socket.Connect();

                        if (socket.IsConnected)
                        {
                            mmOutStream = socket.OutputStream;
                        }
                    }


                }
                else
                {
                    if (!btAdapter.IsEnabled)
                    {
                        btAdapter.Enable();
                    }
                }





            }
            catch (System.Exception ex)
            {
                Toast.MakeText(Android.App.Application.Context, "Não foi possível se conectar na impressora {0}\n\nVerifique as configurações de impressão e tente novamente", ToastLength.Long).Show();

                return;
            }
        }

        public async Task CloseOutputStream()
        {
            Thread.Sleep(1000);

            await socket.OutputStream.FlushAsync();

            //fecha
            socket.OutputStream.Close();

            socket.Close();

            socket.Dispose();

            socket = null;

            btAdapter.Dispose();


            //aguarda 2s
            Thread.Sleep(1000);

        }
        #endregion

        private byte[] bitmap2PrinterBytes(Bitmap bitmap, int left = 0)
        {
            if (left % 8 != 0)
            {
                left = left / 8 * 8;
            }

            int width = bitmap.Width;
            int height = bitmap.Height;

            byte[] imgbuf = new byte[(width / 8 + left / 8 + 4) * height];

            byte[] bitbuf = new byte[width / 8];
            int[] p = new int[8];
            int s = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width / 8; x++)
                {
                    for (int m = 0; m < 8; m++)
                    {
                        if (bitmap.GetPixel(x * 8 + m, y) == -1)
                        {
                            p[m] = 0;
                        }
                        else
                        {
                            p[m] = 1;
                        }
                    }

                    int value = p[0] * 128 + p[1] * 64 + p[2] * 32 + p[3] * 16 +
                                p[4] * 8 + p[5] * 4 + p[6] * 2 + p[7];
                    bitbuf[x] = ((byte)value);
                }

                if (y != 0)
                {
                    imgbuf[(++s)] = 22;
                }
                else
                {
                    imgbuf[s] = 22;
                }
                imgbuf[(++s)] = ((byte)(width / 8 + left / 8));

                for (int j = 0; j < left / 8; j++)
                {
                    imgbuf[(++s)] = 0;
                }

                for (int n = 0; n < width / 8; n++)
                {
                    imgbuf[(++s)] = bitbuf[n];
                }
                imgbuf[(++s)] = 21;
                imgbuf[(++s)] = 1;
            }

            return imgbuf;
        }

        public void OnInit(OperationResult status)
        {
            if (status.Equals(OperationResult.Success))
            {
                var p = new Dictionary<string, string>();

            }
        }
    }
}
using MocoApp.Helpers;
using MocoApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MocoApp.Services
{
    public class PrinterService
    {
        IBluetoothManager bluetoothManager;
        public PrinterService()
        {
            bluetoothManager = DependencyService.Get<IBluetoothManager>(DependencyFetchTarget.GlobalInstance);
        }
        public bool Connect()
        {
            return true;
        }

        public bool Disconnect()
        {
            return true;
        }

        public async Task PrintImage(byte[] data)
        {
            await bluetoothManager.PrintImage(data);
        }

        public async Task PrintText(string text)
        {
           
            await bluetoothManager.PrintText(Convert(text));
        }

        public byte[] Convert(string text)
        {
            StringBuilder stringBuilder = new StringBuilder(text);
            return System.Text.Encoding.UTF8.GetBytes(stringBuilder.ToString());

        }

      
    }
}

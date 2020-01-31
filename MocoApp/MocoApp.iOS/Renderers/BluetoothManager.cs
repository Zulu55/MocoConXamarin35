using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Foundation;
using UIKit;
using MocoApp.iOS.Renderers;
using MocoApp.Interfaces;
using System.Threading.Tasks;
using ExternalAccessory;

[assembly: Xamarin.Forms.Dependency(typeof(MocoApp.iOS.Renderers.BluetoothManager))]
namespace MocoApp.iOS.Renderers
{
    public class BluetoothManager : IBluetoothManager
    {
        EAAccessoryManager _eAcessory;
        public Task CloseOutputStream()
        {
            throw new NotImplementedException();
        }

        public async Task OpenOutputStream()
        {
            try
            {
                
                EAAccessoryManager _manager = EAAccessoryManager.SharedAccessoryManager;
                var _accessories = _manager.ConnectedAccessories;

                foreach (var _accessory in _accessories)
                {
                    Acr.UserDialogs.UserDialogs.Instance.Toast(_accessory.Connected.ToString());
                }

            }
            catch (System.Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task PrintImage(byte[] image)
        {
            throw new NotImplementedException();
        }

        public Task PrintText(byte[] text)
        {
            throw new NotImplementedException();
        }
    }
}
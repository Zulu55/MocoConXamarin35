using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MocoApp.Interfaces
{
    public interface IBluetoothManager
    {
        Task PrintText(byte[] text);
        Task PrintImage(byte[] image);
        Task OpenOutputStream();
        Task CloseOutputStream();

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MocoApp.Interfaces
{
    public interface ILocalNotification
    {
        void CreateNotification(string title, string message);
    }
}

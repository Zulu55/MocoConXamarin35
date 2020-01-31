using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MocoApp.Interfaces
{
    public interface ILocalize
    {        
        void SetLocale(CultureInfo ci);
    }
}

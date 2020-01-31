using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MocoApp.Interfaces
{
    public interface IShare
    {
        Task Show(string title, string message, string filePath);
    }
}

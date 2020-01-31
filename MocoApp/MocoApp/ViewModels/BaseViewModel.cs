using Naylah.Xamarin.Services.NavigationService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MocoApp.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged, INavigable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public INavigationService NavigationService { get { return App.AppCurrent.NavigationService; } }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                Notify("IsLoading");
            }
        }


        protected void Notify(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed(this, new PropertyChangedEventArgs(propertyName));
        }
        public virtual async Task OnNavigatedToAsync(object parameter, NavigationMode mode)
        {

        }

        public virtual async Task OnNavigatedFromAsync()
        {

        }

        public virtual async Task OnNavigatingToAsync(object parameter, NavigationMode mode)
        {

        }
    }
}

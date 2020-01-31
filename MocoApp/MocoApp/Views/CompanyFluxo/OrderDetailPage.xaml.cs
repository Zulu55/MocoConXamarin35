using MocoApp.Controls;
using MocoApp.Helpers;
using MocoApp.Interfaces;
using MocoApp.Models;
using MocoApp.Resources;
using MocoApp.Services;
using MocoApp.Views.ManagerCheckinFlow;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static MocoApp.Models.Enums;

namespace MocoApp.Views.CompanyFluxo
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OrderDetailPage : ContentPage
    {
        private Order _order;

        public OrderDetailPage(Order order, Checkin checkin = null)
        {
            InitializeComponent();

            _order = order;
            if(_order.Checkin == null)
            {
                _order.Checkin = checkin;
            }

            this.BindingContext = _order;

            if (!string.IsNullOrEmpty(_order.ReasonDenied))
            {
                stkCanceledReason.IsVisible = true;
            }

            var printer = new ToolbarItem(AppResource.lblPrint, "", () =>
            {
                PrintNow();
            }, 0, 0);

            printer.Priority = 1;
            ToolbarItems.Add(printer);
        }

        private void btnUpdateStatatus_Clicked(object sender, EventArgs e)
        {
            var btn = sender as Button;

            if(btn.StyleId == "4")
            {
                string comment = "";
                var popup = new EntryPopup(AppResource.alertFillMotiveCancel, "",AppResource.alertSave, AppResource.alertCancel);
                popup.PopupClosed += async (o, closedArgs) =>
                {
                    if (closedArgs.Button == AppResource.textOk || closedArgs.Button ==AppResource.alertSave)
                    {
                        try
                        {
                            comment = closedArgs.Text;

                            var status = Convert.ToInt32(btn.StyleId);

                            UpdateOrder(status, comment);
                        }
                        catch (Exception ex)
                        {
                            ex.ToString();
                        }
                    }
                };

                popup.Show();
            }
            else
            {
                UpdateOrder(Convert.ToInt32(btn.StyleId));
            }
        }

        public async void UpdateOrder(int status, string comment = "")
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);
                var orderService = new OrderService();

                if (ckbSameAnswer.IsToggled)
                {
                    var up = new UpdateOrder
                    {
                        EmployeeId = Settings.DisplayUserId,
                        OrderId = _order.Id,
                        OrderStatus = status,
                        ReasonDenied = comment
                    };

                    _order.OrderStatus = (Enums.OrderStatus)status;
                    _order.ReasonDenied = comment;
                    await orderService.UpdateAllOrderStatus(up);
                }
                else
                {
                    var up = new UpdateOrder
                    {
                        EmployeeId = Settings.DisplayUserId,
                        OrderId = _order.Id,
                        OrderStatus = status,
                        ReasonDenied = comment
                    };

                    _order.OrderStatus = (Enums.OrderStatus)status;
                    _order.ReasonDenied = comment;
                    await orderService.UpdateOrderStatus(up);
                }

                Acr.UserDialogs.UserDialogs.Instance.Toast(AppResource.lblItemUpdatedSucess);
                await App.AppCurrent.NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                ex.ToString();
                await DisplayAlert(AppResource.alertAlert, ex.Message, AppResource.textOk);
            }
            finally
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
        }

        public async void PrintNow()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertPrinting, Acr.UserDialogs.MaskType.Black);
                await Task.Delay(500);
                await DependencyService.Get<IBluetoothManager>().OpenOutputStream();
                PrinterService _printer = new PrinterService();

                await _printer.PrintText(PrintText.PrintOrder(_order.Checkin, _order));
            }
            catch (Exception ex)
            {
                await DisplayAlert(AppResource.alertAlert, ex.Message, AppResource.textOk);
            }
            finally
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
        }
    }
}
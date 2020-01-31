using Acr.UserDialogs;
using MocoApp.Extensions;
using MocoApp.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MocoApp.Models
{
    public class CartModel : INotifyPropertyChanged
    {

        public CartModel()
        {
            Orders = new ObservableCollection<CreateOrder>();
        }

        ObservableCollection<CreateOrder> _orders;
        public ObservableCollection<CreateOrder> Orders
        {
            get => _orders;
            private set
            {
                _orders = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Orders"));
            }
        }

        public string CompanyId { get; set; }

        public Company Company { get; private set; }

        public Location Location { get; private set; }
        public string LocationId { get; set; }

        public string CheckinId { get; set; }

        public Checkin Checkin { get; set; }



        public bool IsToDeliverNow { get; set; }



        public decimal PriceTaxPaid { get; set; }
        public decimal SubTotal { get; set; }
        public decimal PriceTipPaid { get; set; }

        public EPaymentOption PaymentOption { get; set; }
        public string PaymentMethod { get; set; }
        public ETimeToOrder TimeToOrder { get; set; }

        public string CardId { get; set; }
        public DateTimeOffset DateTimeToOrder { get; set; }

        public string StripeId { get; set; }

        public int? TotalOrdersInCart { get { return Orders?.Count(); } }

        string _totalPrice;
        public string TotalPrice
        {
            get => _totalPrice;
            private set
            {
                _totalPrice = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TotalPrice"));
            }
        }

        string _totalBill;
        public string TotalBill
        {
            get => _totalBill;
            set
            {
                _totalBill = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TotalBill"));
            }
        }

        string _proceTipPaidStr;
        public string PriceTipPaidStr
        {
            get => _proceTipPaidStr;
            set
            {
                _proceTipPaidStr = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PriceTipPaidStr"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void UpdateTotalPrice()
        {
            var total = Orders.Select(m => m.TotalPriceDecimal).Sum();

            CultureInfo culture;
            if (Company != null)
                culture = Company.CurrencyType.ToCultureInfo();
            else
                culture = App.AppCurrent.CompanyCulture;
            //var culture = Company.CurrencyType.ToCultureInfo();

            var str = total.ToString("C", culture);

            SubTotal = total;
            TotalPrice = str;
            UpdateTotalBill(culture);
        }

        public void UpdateTotalBill(CultureInfo culture)
        {
            var total = SubTotal + PriceTipPaid + PriceTaxPaid;

            TotalBill = total.ToString("C", culture);

        }

        public async Task<bool> AddOrder(CreateOrder order, Company company, Location location)
        {
            if(Orders != null && Orders.Count() > 0)
            {
                //ja colocou item no carrinho, verificar se ele ta adicionando o novo pedido da mesma empresa

                if (Company != null && Company.Id != company.Id)
                {
                    await App.AppCurrent.MainPage.DisplayAlert(AppResource.alertAlert, AppResource.alertAddOrderDifferentCompany + Company.CompanyName, AppResource.textOk);
                }else
                {


                    //nao pode ser de localizacoes diferentes
                    if (LocationId != location.Id)
                    {
                        await App.AppCurrent.MainPage.DisplayAlert(AppResource.alertAlert, string.Format(AppResource.alertAddOrderDifferenteLocation, Location.Name), AppResource.textOk);
                    }
                    else
                    {
                        //ja existe? Entao adiciona mais invés de adicionar
                        if (Orders.Where(m => m.ProductId == order.ProductId).Count() > 0)
                        {
                            var thisOrder = Orders.Where(m => m.ProductId == order.ProductId).FirstOrDefault();
                            thisOrder.ProductQuantity += order.ProductQuantity;
                            UpdateTotalPrice();

                            return true;
                        }
                        else
                        {
                            Orders.Add(order);
                            return true;
                        }

                  
                    }
                }

            }
            else
            {
                AddCompany(new Company() { Id = company.Id, Title = company.Title, CompanyType = company.CompanyType, CreditCardAllowed = company.CreditCardAllowed, CurrencyType = company.CurrencyType });
                AddLocation(new Location() { Id = location.Id, Name = location.Name, ImageUri = location.ImageUri });

                if(order != null)
                {
                    order.Company = null;
                    order.Product.Company = null;
                    order.Product.Location = null;

                    Orders.Add(order);
                }
                // pra n ficar com o objeto mt grande, limpei ele
               
                return true;
            }



            return false;
        }

        public void RemoveOrder(CreateOrder order)
        {
            CreateOrder tempOrder = order;

            Orders.Remove(order);

            ToastAction action = new ToastAction()
            {
                Text = AppResource.textUndo,
                Action = () =>
                {
                    Orders.Add(tempOrder);
                }   
            };

            ToastConfig config = new ToastConfig(AppResource.textOrderRemoved)
            {
                Action = action
            };

            UserDialogs.Instance.Toast(config);

        }

        public void ClearOrders()
        {
            LocationId = "";
            CompanyId = "";
            PriceTaxPaid = 0;
            PriceTipPaid = 0;
            Orders.Clear();
            UpdateTotalPrice();
            Company = null;
            Location = null;
        }

        public void AddCompany(Company c)
        {
            Company = c;
            CompanyId = c.Id;
        }

        public void AddLocation(Location l)
        {
            Location = l;
            LocationId = l.Id;
        }
    }


    public enum ETimeToOrder
    {
        None = 0,
        Asap = 1,
        SpecifiedTime = 2
    }

    public enum EPaymentOption
    {
        None = 0,
        KeepBillOpen = 1,
        CloseBill = 2
    }

    public enum EPaymentMethod
    {
        None = 0,
        Cash = 1,
        BillToRoom = 2,
        Card = 3
    }
}

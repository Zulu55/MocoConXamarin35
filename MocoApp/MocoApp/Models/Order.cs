using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;
using static MocoApp.Models.Enums;
using MocoApp.Resources;
using MocoApp.Extensions;

namespace MocoApp.Models
{
    public class Order : BaseModel
    {
        public virtual string ClientId { get; set; }
        public virtual Client Client { get; set; }

        public virtual string EmployeeId { get; set; }
        //public virtual Employee Employee { get; set; }

        public virtual string CompanyId { get; set; }
        public virtual Company Company { get; set; }

        public virtual User User { get; set; }

        public virtual string CheckinId { get; set; }
        public virtual Checkin Checkin { get; set; }

        public CheckinSub CheckinSub { get; set; }

        public OrderStatus OrderStatus { get; set; }
        public string Observation { get; set; }


        /// <summary>
        /// Quando um pedido eh cancelado
        /// </summary>
        public string ReasonDenied { get; set; }

        public bool ShowReasonDenied { get { return string.IsNullOrEmpty(ReasonDenied) ? false : true; } }
        public int ProductQuantity { get; set; }

        public Product Product { get; set; }

        public decimal TotalPrice { get; set; }

        public decimal ProductPrice { get; set; }

        public string ProductPriceStr { get { return String.Format(App.AppCurrent.CompanyCulture, "{0:C}", ProductPrice); } }

        public string TotalPriceStr { get { return String.Format(AppResource.lblOrderPrice, String.Format(App.AppCurrent.CompanyCulture, "{0:C}", TotalPrice)); } }
        public string TotalPriceStrMoney { get { return String.Format(App.AppCurrent.CompanyCulture, "{0:C}", TotalPrice); } }

        public string OrderCreatedAtStr { get { return CreatedAt.ToString("dd/MM/yyyy HH:mm"); } }

        public string QuantityPriceStr { get { return ProductQuantity + "x " + ProductPriceStr; } }

        public string OrderStatusStr
        {
            get
            {
                return OrderStatus.ToResource();
                //switch (OrderStatus)
                //{
                //    case OrderStatus.Pending:
                //        return "Pendente";
                //    case OrderStatus.Accepted:
                //        return "Aceito";
                //    case OrderStatus.OnGoing:
                //        return "Andamento";
                //    case OrderStatus.Canceled:
                //        return "Cancelado";
                //    case OrderStatus.Done:
                //        return "Entregue";
                //    case OrderStatus.Closed:
                //        return "Entregue";
                //    default:
                //        return "";

                //}
            }
        }

        public string OrderStatusColor
        {
            get
            {
                switch (OrderStatus)
                {
                    case OrderStatus.Pending:
                        return "#FBB03B";
                    case OrderStatus.Accepted:
                        return "#5762F3";
                    case OrderStatus.OnGoing:
                        return "#FD5900";
                    case OrderStatus.Canceled:
                        return "#fe0501";
                    case OrderStatus.Done:
                        return "#0D47A1";
                    case OrderStatus.Closed:
                        return "#57B447";
                    default:
                        return "#000";

                }
            }
        }

        public string OrderStatusIcon
        {
            get
            {
                switch (OrderStatus)
                {
                    case OrderStatus.Pending:
                        return "ic_pendente";
                    case OrderStatus.Accepted:
                        return "ic_aceito";
                    case OrderStatus.OnGoing:
                        return "ic_andamento";
                    case OrderStatus.Canceled:
                        return "ic_cancelado";
                    case OrderStatus.Done:
                        return "ic_entregue";
                    case OrderStatus.Closed:
                        return "ic_entregue";
                    default:
                        return "ic_pendente";

                }
            }
        }

        public ETimeToOrder TimeToOrder { get; set; }
        public DateTimeOffset? DateTimeToOrder { get; set; }

        public string ShowTimeOrderValue
        {
            get {
                string ret = "-";

                if (TimeToOrder == ETimeToOrder.SpecifiedTime)
                    ret = DateTimeToOrder.Value.ToString("dd/MM/yyyy HH:mm");
                else if (TimeToOrder == ETimeToOrder.Asap)
                    ret = AppResource.textCartAsap;


                return ret;

            }
        }


        public bool IsToShowTimeOrderValue { get { return TimeToOrder == ETimeToOrder.None ? false : true; } }
    }

    public class UpdateOrder
    {
        public string EmployeeId { get; set; }
        public string OrderId { get; set; }
        public int OrderStatus { get; set; }

        public string ReasonDenied { get; set; }
        public bool All { get; set; }

    }
}

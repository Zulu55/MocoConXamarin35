using MocoApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MocoApp.Helpers
{
    public static class PrintText
    {
        public static string CreateOrderPrint()
        {

        

            StringBuilder sb = new StringBuilder();
            sb.Append(String.Concat(Asci.ESC, Asci.ARROBA)); //INICIA A IMPRESSÃO ESC @
            sb.Append(String.Concat(Asci.ESC, Asci.EXCLAMACAO, Asci.NULO));   //ZERA FONTE
            sb.Append(String.Concat(Asci.ESC, Asci.a, Asci.UM)); //CENTRALIZADO
            sb.Append(Asci.LF);
            sb.Append(String.Concat(Asci.ESC, Asci.E, Asci.UM)); //NEGRITO
            //sb.Append(String.Concat(Asci.ESC, Asci.M, Asci.ZERO)); //SMALL FONT SIZE
            sb.Append("HI CHARLIEAPP");
            sb.Append(Asci.LF);
            sb.Append(Asci.LF);
            sb.Append(Asci.LF);



            sb.Append(Asci.LF);
            sb.Append(Asci.LF);
            sb.Append(Asci.LF);
            sb.Append(Asci.LF);
            sb.Append(Asci.LF);



            return sb.ToString();
        }

        public static string PrintCheckin(Checkin checkin, List<CheckinSub> listLocation)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append(String.Concat(Asci.ESC, Asci.ARROBA)); //INICIA A IMPRESSÃO ESC @


            //HEADER
            sb.Append(PrintHeader(checkin.Company, null).ToString());



            sb.Append(String.Concat(Asci.ESC, Asci.E, Asci.UM)); //NEGRITO
            sb.Append(String.Concat(Asci.ESC, Asci.M, Asci.ZERO));
            sb.Append(String.Concat(Asci.ESC, Asci.a, Asci.ZERO));              //CENTRALIZDO                  
            sb.Append(String.Concat(Asci.ESC, Asci.EXCLAMACAO, Asci.NULO));   //ZERA FONTE

            
            sb.Append(Asci.LF);            
            sb.Append("Checkin: " + checkin.CheckInNumber);
            sb.Append(Asci.LF);
            sb.Append(checkin.OccupationStr);
            sb.Append(Asci.LF);
            sb.Append(checkin.Client.Name);
            sb.Append(Asci.LF);  
            
            if(checkin.Company.HasLocation)
            {
                if (listLocation != null)
                {
                    sb.Append(PrintLocationBills(listLocation));
                }
            }else
            {
                if(checkin.Orders != null)
                {
                    sb.Append(PrintOrders(checkin.Orders));
                }
            }
            


            sb.Append(Asci.LF);            
            sb.Append(String.Concat(Asci.ESC, Asci.EXCLAMACAO, Asci.NULO));   //ZERA FONTE
            sb.Append(Asci.LF);
            sb.Append(String.Concat(Asci.ESC, Asci.a, Asci.DOIS));              //CENTRALIZDO
            sb.Append("Total Gasto: " + String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", checkin.TotalSpent));
            sb.Append(Asci.LF);
            sb.Append("Desconto: " + String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", checkin.PriceDiscount));
            sb.Append(Asci.LF);
            sb.Append("Total Faltante: " + String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", checkin.TotalToBePaid));
            sb.Append(Asci.LF);
            sb.Append("Total Pago: " + String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", checkin.TotalPaid));
            sb.Append(Asci.LF);


            //FOOTER
            sb.Append(PrintFooter().ToString());         



            return sb.ToString();
        }

        public static string PrintLocationBill(Checkin checkin, CheckinSubOrders subCheckin)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append(String.Concat(Asci.ESC, Asci.ARROBA)); //INICIA A IMPRESSÃO ESC @


            //HEADER
            sb.Append(PrintHeader(null, subCheckin.Location).ToString());



            sb.Append(String.Concat(Asci.ESC, Asci.E, Asci.UM)); //NEGRITO
            sb.Append(String.Concat(Asci.ESC, Asci.M, Asci.ZERO));
            sb.Append(String.Concat(Asci.ESC, Asci.a, Asci.ZERO));              //CENTRALIZDO                  
            sb.Append(String.Concat(Asci.ESC, Asci.EXCLAMACAO, Asci.NULO));   //ZERA FONTE


            sb.Append(Asci.LF);
            sb.Append(checkin.Client.Name);            
            sb.Append(Asci.LF);
            sb.Append("Checkin: " + checkin.CheckInNumber);
            sb.Append(Asci.LF);
            sb.Append(checkin.OccupationStr);
            sb.Append(Asci.LF);
            if(checkin.Company != null)
            {
                sb.Append(checkin.Company.Title);
                sb.Append(Asci.LF);
            }

            if(subCheckin.Orders != null)
            sb.Append(PrintOrders(subCheckin.Orders).ToString());

            sb.Append(String.Concat(Asci.ESC, Asci.E, Asci.UM)); //NEGRITO
            sb.Append(String.Concat(Asci.ESC, Asci.M, Asci.ZERO)); //Large FONT SIZE            
            sb.Append("DETALHES DA CONTA");

            sb.Append(Asci.LF);                        
            sb.Append(String.Concat(Asci.ESC, Asci.EXCLAMACAO, Asci.NULO));   //ZERA FONTE
            sb.Append(Asci.LF);
            sb.Append(String.Concat(Asci.ESC, Asci.a, Asci.DOIS));              //CENTRALIZDO
            sb.Append("Sub Total: " + String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", subCheckin.SubTotal));
            sb.Append(Asci.LF);
            sb.Append("Taxas: " + String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", subCheckin.PriceTaxPaid));
            sb.Append(Asci.LF);
            sb.Append("Gorjeta: " + String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", subCheckin.PriceTipPaid));
            sb.Append(Asci.LF);
            sb.Append("Desconto: " + String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", subCheckin.PriceDiscount));
            sb.Append(Asci.LF);
            sb.Append(Asci.LF);
            sb.Append(String.Concat(Asci.ESC, Asci.M, Asci.ZERO)); //Large FONT SIZE            
            sb.Append("TOTAL: " + String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", subCheckin.TotalPaid));
            sb.Append(Asci.LF);


            //FOOTER
            sb.Append(PrintFooter().ToString());



            return sb.ToString();
        }

        public static StringBuilder PrintHeader(Company company, Location location)
        {
            StringBuilder sb = new StringBuilder();
            if (company == null && location == null)
            {
                
                sb.Append(String.Concat(Asci.ESC, Asci.EXCLAMACAO, Asci.NULO));   //ZERA FONTE
                sb.Append(String.Concat(Asci.ESC, Asci.a, Asci.UM)); //CENTRALIZADO
                sb.Append(Asci.LF);
                sb.Append(String.Concat(Asci.ESC, Asci.E, Asci.UM)); //NEGRITO
                sb.Append(String.Concat(Asci.ESC, Asci.M, Asci.ZERO)); //Large FONT SIZE
                sb.Append(Asci.LF);
                sb.Append(Asci.LF);
                sb.Append("HICHARLIEAPP");
                sb.Append(Asci.LF);
                sb.Append(Asci.LF);
            }
            else if(location == null)
            {
                
                sb.Append(String.Concat(Asci.ESC, Asci.EXCLAMACAO, Asci.NULO));   //ZERA FONTE
                sb.Append(String.Concat(Asci.ESC, Asci.a, Asci.UM)); //CENTRALIZADO
                sb.Append(Asci.LF);
                sb.Append(String.Concat(Asci.ESC, Asci.E, Asci.UM)); //NEGRITO
                sb.Append(String.Concat(Asci.ESC, Asci.M, Asci.ZERO)); //Large FONT SIZE
                sb.Append(company.Title);
                sb.Append(Asci.LF);
                sb.Append(String.Concat(Asci.ESC, Asci.EXCLAMACAO, Asci.NULO));   //ZERA FONTE
                sb.Append(company.Address);
                sb.Append(Asci.LF);
                sb.Append(company.Cellphone);
                sb.Append(Asci.LF);
                sb.Append(Asci.LF);
                sb.Append(Asci.LF);

            }
            else
            {
                sb.Append(String.Concat(Asci.ESC, Asci.EXCLAMACAO, Asci.NULO));   //ZERA FONTE
                sb.Append(String.Concat(Asci.ESC, Asci.a, Asci.UM)); //CENTRALIZADO
                sb.Append(Asci.LF);
                sb.Append(String.Concat(Asci.ESC, Asci.E, Asci.UM)); //NEGRITO
                sb.Append(String.Concat(Asci.ESC, Asci.M, Asci.ZERO)); //Large FONT SIZE
                sb.Append(location.Name);
                sb.Append(Asci.LF);
                sb.Append(String.Concat(Asci.ESC, Asci.EXCLAMACAO, Asci.NULO));   //ZERA FONTE                                
                sb.Append(Asci.LF);                

            }

            return sb;
        }


        public static string PrintOrder(Checkin checkin, Order oder)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append(String.Concat(Asci.ESC, Asci.ARROBA)); //INICIA A IMPRESSÃO ESC @


            //HEADER
            sb.Append(PrintHeader(checkin.Company, null).ToString());



            sb.Append(String.Concat(Asci.ESC, Asci.E, Asci.UM)); //NEGRITO
            sb.Append(String.Concat(Asci.ESC, Asci.M, Asci.ZERO));
            sb.Append(String.Concat(Asci.ESC, Asci.a, Asci.ZERO));              //CENTRALIZDO                  
            sb.Append(String.Concat(Asci.ESC, Asci.EXCLAMACAO, Asci.NULO));   //ZERA FONTE


            if(checkin != null)
            {
                sb.Append(Asci.LF);
                sb.Append("Checkin: " + checkin.CheckInNumber);
                sb.Append(Asci.LF);
                sb.Append(checkin.OccupationStr);
                sb.Append(Asci.LF);
                sb.Append(checkin.Client.Name);
                sb.Append(Asci.LF);
            }

            sb.Append(String.Concat(Asci.ESC, Asci.E, Asci.UM)); //NEGRITO
            sb.Append(String.Concat(Asci.ESC, Asci.M, Asci.ZERO)); //Large FONT SIZE
            sb.Append(String.Concat(Asci.ESC, Asci.a, Asci.UM)); //CENTRALIZADO     

            sb.Append("PEDIDO");
            sb.Append(Asci.LF);

            sb.Append(String.Concat(Asci.ESC, Asci.E, Asci.ZERO)); //LIMPA NEGRITO
            sb.Append(Asci.LF);
            sb.Append(string.Format("{0} {1} {2}", oder.ProductQuantity, oder.Product.Name, oder.TotalPriceStrMoney));
            sb.Append(Asci.LF);
            sb.Append(Asci.LF);
            sb.Append(String.Concat(Asci.ESC, Asci.a, Asci.ZERO));              //CENTRALIZDO  
            sb.Append("OBSERVACAO: ");
            sb.Append(Asci.LF);
            sb.Append(Asci.LF);
            sb.Append(string.Format("{0}", oder.Observation));
            sb.Append(Asci.LF);
            sb.Append(Asci.LF);

            //FOOTER
            sb.Append(PrintFooter().ToString());



            return sb.ToString();
        }
        public static StringBuilder PrintOrders(List<Order> list)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(String.Concat(Asci.ESC, Asci.E, Asci.UM)); //NEGRITO
            sb.Append(String.Concat(Asci.ESC, Asci.M, Asci.ZERO)); //Large FONT SIZE
            sb.Append(String.Concat(Asci.ESC, Asci.a, Asci.UM)); //CENTRALIZADO     

            sb.Append("PEDIDOS");
            sb.Append(Asci.LF);

            sb.Append(String.Concat(Asci.ESC, Asci.E, Asci.ZERO)); //LIMPA NEGRITO
                   

            foreach (var item in list)
            {                
                sb.Append(Asci.LF);
                sb.Append(string.Format("{0} {1} {2}", item.ProductQuantity, item.Product.Name, item.TotalPriceStrMoney));
                sb.Append(Asci.LF);
                if(!string.IsNullOrEmpty(item.Observation))
                {
                    sb.Append(string.Format("{0}", item.Observation));
                    sb.Append(Asci.LF);
                }
                
            }
            sb.Append(Asci.LF);




            return sb;
        }

        public static StringBuilder PrintLocationBills(List<CheckinSub> list)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(String.Concat(Asci.ESC, Asci.E, Asci.UM)); //NEGRITO
            sb.Append(String.Concat(Asci.ESC, Asci.M, Asci.ZERO)); //Large FONT SIZE
            sb.Append(String.Concat(Asci.ESC, Asci.a, Asci.UM)); //CENTRALIZADO     

            sb.Append("SubCheckins");
            sb.Append(Asci.LF);

            sb.Append(String.Concat(Asci.ESC, Asci.E, Asci.ZERO)); //LIMPA NEGRITO
            sb.Append(String.Concat(Asci.ESC, Asci.a, Asci.ZERO));   

            foreach (var item in list.OrderByDescending(m => m.CreatedAt).ToList())
            {
           
                if (item.CheckinSubStatus == Enums.CheckinSubStatus.Closed)
                {  
                    sb.Append(string.Format("{0} - {1} {2}", item.Location.Name, item.CheckinSubStatusStr, String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", item.TotalPaid)));
                }
                else
                {
                    sb.Append(string.Format("{0} - {1}", item.Location.Name, item.CheckinSubStatusStr));
                }

                //if (!item.Paid)
                //{

                //    sb.Append("- Pagar Depois: " + String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", item.TotalPaid));
                //}
                //else
                //{

                //    sb.Append("- Pendente Pagamento: " + String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", item.TotalPaid));
                //}

                //if (item.CheckinSubStatus == Enums.CheckinSubStatus.RequestedCheckout)
                //{
                //    if (item.PaidInCard > 0)
                //    {
                //        sb.Append("- (CARTAO)");

                //    }
                //    else if (item.PaidInCash > 0)
                //    {

                //        sb.Append("- (DINHEIRO)");
                //    }
                //}

                sb.Append(Asci.LF);
                sb.Append(Asci.LF);
            }

    


            return sb;
        }
        public static StringBuilder PrintFooter()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(String.Concat(Asci.ESC, Asci.EXCLAMACAO, Asci.NULO));   //ZERA FONTE
            sb.Append(String.Concat(Asci.ESC, Asci.a, Asci.UM)); //CENTRALIZADO
            sb.Append(String.Concat(Asci.ESC, Asci.E, Asci.UM)); //NEGRITO
            sb.Append(Asci.LF);
            sb.Append(Asci.LF);
            sb.Append("HICHARLIEAPP");
            sb.Append(Asci.LF);
            sb.Append("HiCharlie app faster order for better living");
            sb.Append(Asci.LF);
            sb.Append(DateTime.Now);
            sb.Append(Asci.LF);
            sb.Append(Asci.LF);
            sb.Append(Asci.LF);


            return sb;
        }

        public static StringBuilder PrintLines()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(String.Concat(Asci.ESC, Asci.a, Asci.ZERO));      
            sb.Append("- - - - - ");
     


            return sb;
        }
    }
}

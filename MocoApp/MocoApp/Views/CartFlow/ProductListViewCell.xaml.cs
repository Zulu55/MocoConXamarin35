using MocoApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MocoApp.Views.CartFlow
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProductListViewCell : ViewCell
	{
        CreateOrder p;

		public ProductListViewCell ()
		{
			InitializeComponent ();
		}

        protected override void OnBindingContextChanged()
        {
            p = BindingContext as CreateOrder;
            App.AppCurrent.Cart.UpdateTotalPrice();

            base.OnBindingContextChanged();
        }

        private void Sub_Tapped(object sender, EventArgs e)
        {
            if (p.ProductQuantity > 1)
            {
                p.ProductQuantity -= 1;
                // atualizar aqui pra n fazer uns binding complicadao desnecessario
                UpdateLabels();

            }
            else
            {
                App.AppCurrent.Cart.RemoveOrder(p);
            }


            if (App.AppCurrent.Cart.Orders.Count < 1)
                App.AppCurrent.Cart.ClearOrders();
            else
                App.AppCurrent.Cart.UpdateTotalPrice();

        }

        private void Add_Tapped(object sender, EventArgs e)
        {
            p.ProductQuantity += 1;

            // atualizar aqui pra n fazer uns binding complicadao desnecessario
            UpdateLabels();

            App.AppCurrent.Cart.UpdateTotalPrice();

        }

        void UpdateLabels()
        {
            entryQuantity.Text = p.ProductQuantity.ToString();
            //totalPrice.Text = p.TotalPrice;
        }

        private void entryQuantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            //int n = 0;
            //var s = int.TryParse(e.NewTextValue, out n);

            //if (n != 0)
            //{
            //    App.AppCurrent.Cart.UpdateTotalPrice();
            //}
        }
    }
}
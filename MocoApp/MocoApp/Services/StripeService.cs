using MocoApp.Interfaces;
using Xamarin.Forms;

namespace MocoApp.Services
{
    public class StripeService
    {
        public string Generate(string number, string cvc, int month, int year)
        {
            var result = DependencyService.Get<IStripeService>().GenerateToken(Constants.Constantes.PublishKey, number, cvc, month, year);

            return result;
        }
    }
}

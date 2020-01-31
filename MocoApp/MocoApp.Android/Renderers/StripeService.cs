using MocoApp.Droid.Renderers;
using MocoApp.Interfaces;
using Stripe;

[assembly: Xamarin.Forms.Dependency(typeof(StripeService))]
namespace MocoApp.Droid.Renderers
{
    public class StripeService : IStripeService
    {
        public string GenerateToken(string key, string number, string cvc, int month, int year)
        {
            StripeConfiguration.SetApiKey(key);

            var tokenOptions = new TokenCreateOptions()
            {
                Card = new CreditCardOptions()
                {
                    Number = number,
                    Cvc = cvc,
                    ExpYear = year,
                    ExpMonth = month
                }
            };

            var tokenService = new TokenService();
            Token stripeToken = tokenService.Create(tokenOptions);

            return stripeToken?.Id; // This is the token
        }
    }
}
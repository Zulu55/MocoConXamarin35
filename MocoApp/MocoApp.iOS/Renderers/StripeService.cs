using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using MocoApp.Interfaces;
using MocoApp.iOS.Renderers;
using Stripe;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(StripeService))]
namespace MocoApp.iOS.Renderers
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
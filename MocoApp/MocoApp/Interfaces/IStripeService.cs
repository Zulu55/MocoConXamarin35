namespace MocoApp.Interfaces
{
    public interface IStripeService
    {
        string GenerateToken(string key, string number, string cvc, int month, int year);
    }
}

using System.Collections.Generic;
using Xamarin.Forms;
using static MocoApp.Models.Enums;

namespace MocoApp.Models
{
    public class Company : BaseModel
    {
        public Company()
        {
            Payments = new List<Payment>();
        }

        public string Title { get; set; }
        
        public string Description { get; set; }
        
        public string Cellphone { get; set; }

        public string Address { get; set; }
        
        public string Latitude { get; set; }
        
        public string Longitude { get; set; }
        
        public string ImageUri { get; set; }

        public decimal Rating { get; set; }
        
        public int TotalRating { get; set; }

        public CompanyType CompanyType { get; set; }
        
        public ECurrencyType CurrencyType { get; set; }

        public virtual ICollection<Manager> Managers { get; set; }

        public virtual ICollection<Client> Clients { get; set; }

        public virtual ICollection<Differential> Differentials { get; set; }
        
        public int CheckInSum { get; set; }

        public bool IsFavorited { get; set; }

        public string Worktime { get; set; }

        public bool HasLocation { get; set; }

        public string OccupationPrefix { get; set; }

        public string DistanceString { get; set; }

        public bool HasCheckinInAnotherCompany { get; set; }

        public bool CreditCardAllowed { get; set; }

        public bool AllowChatOnlyForCustomers { get; set; }

        public string CompanyName { get; set; }

        public Color CompanyColor
        {
            get
            {
                if (CompanyType == CompanyType.Praia)
                {
                    return (Color)App.Current.Resources["BarracaColor"];
                }

                if (CompanyType == CompanyType.Hotel)
                {
                    return (Color)App.Current.Resources["HotelColor"];
                }

                if (CompanyType == CompanyType.Restaurante)
                {
                    return (Color)App.Current.Resources["RestauranteColor"];
                }

                if (CompanyType == CompanyType.EsporteEvento)
                {
                    return (Color)App.Current.Resources["EsportesColor"];
                }

                return Color.Gray;
            }
        }

        public string CompanyStarImage
        {
            get
            {                
                if (Rating < 1)
                {
                    return "ic_star";
                }
                
                if (Rating < 2)
                {
                    return "ic_1star_list";
                }

                if (Rating < 3)
                {
                    return "ic_2star_list";
                }

                if (Rating < 4)
                {
                    return "ic_3star_list";

                }

                if (Rating <= (decimal)4.9)
                {
                    return "ic_4star_list";
                }

                if (Rating == 5)
                {
                    return "ic_5star_list";
                }

                return "ic_5star_list";
            }
        }

        public string CompanyStarImageDetail
        {
            get
            {
                if (Rating < 1)
                {
                    return "ic_star";
                }

                if (Rating < 2)
                {
                    return "ic_1star_detail";
                }

                if (Rating < 3)
                {
                    return "ic_2star_detail";
                }

                if (Rating < 4)
                {
                    return "ic_3star_detail";
                }

                if (Rating <= (decimal)4.9)
                {
                    return "ic_4star_detail";
                }

                if (Rating == 5)
                {
                    return "ic_5star_detail";
                }

                return "ic_5star_detail";
            }
        }

        public bool CheckedIn { get; set; }

        public  List<Payment> Payments { get; set; }

        public List<Category> Categories { get; set; }

        public List<Location> Locations { get; set; }

        public List<InformativeMenu> InformativeMenus { get; set; }

        public decimal TaxPercentage { get; set; }

        public decimal RecommendedTipPercentage { get; set; }

        public bool HasMessageNotRead { get; set; }
    }

    public class Differential : BaseModel
    {
        public string Name { get; set; }

        public string ImageUri { get; set; }
    }

    public class Payment : BaseModel
    {
        public string Name { get; set; }

        public string ImageUri { get; set; }
    }
}
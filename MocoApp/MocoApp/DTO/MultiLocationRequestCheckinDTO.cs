using MocoApp.Models;
using System.Collections.Generic;

namespace MocoApp.DTO
{
    public class MultiLocationRequestCheckinDTO
    {
        public Checkin Checkin { get; set; }
        public Company Company { get; set; }
        public List<Location> Locations { get; set; }
    }

    public enum CheckinAction
    {
        EnableLocationCheckin = 0,
        CheckoutLocationCheckin = 1,
        HasAnotherLocationPending = 2
    }
}

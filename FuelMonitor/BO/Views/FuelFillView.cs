using FuelMonitor.BO.Models;
using System;

namespace FuelMonitor.BO.Views
{
    public class FuelFillView : FuelFill
    {
        public decimal DistanceTraveled { get; set; }
        public decimal AVGKMPL { get; set; }
        public DateTime? LastFillDate { get; set; }
    }
}
using FuelMonitor.BO.Models;

namespace FuelMonitor.BO.Views
{
    public class FuelFillView : FuelFill
    {
        public decimal DistanceTraveled { get; set; }
        public decimal AVGKMPL { get; set; }
    }
}
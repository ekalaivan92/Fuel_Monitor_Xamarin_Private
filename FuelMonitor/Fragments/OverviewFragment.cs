using Android.App;
using Android.OS;
using Android.Views;
using FuelMonitor.BO.DAO;
using FuelMonitor.BO.Views;
using Microcharts;
using Microcharts.Droid;
using System;
using System.Collections.Generic;
using System.Linq;
using AndroidWidget = Android.Widget;

namespace FuelMonitor.Fragments
{
    public class OverviewFragment : Fragment
    {
        private ChartView kmlplChartView;
        private ChartView priceChartView;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.OverviewFragment, container, false);

            kmlplChartView = view.FindViewById<ChartView>(Resource.Id.kmplChartView);
            priceChartView = view.FindViewById<ChartView>(Resource.Id.priceChartView);
            var fuelEntries = FuelFillDAO.GetAllView();

            var validEntriesCount = fuelEntries
                .Where(x => x.AVGKMPL > 0 && x.LastFillDate.HasValue)
                .Count();
            if (validEntriesCount > 0)
            {
                ShowCharts(fuelEntries);
            }
            else
            {
                var toast = AndroidWidget.Toast.MakeText(
                    Context,
                    "No valid entries for graphical analyze",
                    AndroidWidget.ToastLength.Long);

                toast.Show();
            }

            return view;
        }

        private void ShowCharts(IEnumerable<FuelFillView> fuelEntries)
        {
            var lineColor = SkiaSharp.SKColor.Parse("#3f51b5");
            var labelColor = SkiaSharp.SKColor.Parse("#607D8B");
            var barColor = SkiaSharp.SKColor.Parse("#a0e2ff");

            var entries = fuelEntries
                .Where(x => x.AVGKMPL > 0)
                .OrderBy(X => X.Date)
                .Select(x =>
                    new ChartEntry((float)x.AVGKMPL)
                    {
                        Label = x.Date.ToString("MMMyy"),
                        ValueLabel = x.AVGKMPL.ToString("#0 kmpl"),
                        Color = lineColor,
                        TextColor = labelColor,
                        ValueLabelColor = labelColor
                    });

            kmlplChartView.Chart = new LineChart
            {
                Entries = entries,
                LabelOrientation = Orientation.Horizontal,
                ValueLabelOrientation = Orientation.Horizontal,
                LabelColor = labelColor,
                LineMode = LineMode.Spline,
                PointMode = PointMode.Circle,
                EnableYFadeOutGradient = true
            };

            entries = fuelEntries
                .Where(x => x.LastFillDate.HasValue)
                .OrderBy(X => X.Date)
                .Select(x =>
                {
                    var runningDays = (x.Date - x.LastFillDate)?.Days ?? 0;
                    var pricePerDay = 0m;

                    if (x.FuelCost.HasValue && runningDays > 0)
                    {
                        pricePerDay = Math.Round(x.FuelCost.Value / runningDays, 2);
                    }

                    return new ChartEntry((float)pricePerDay)
                    {
                        Label = x.Date.ToString("MMMyy"),
                        ValueLabel = $"Rs.{pricePerDay} / Day",
                        Color = barColor,
                        TextColor = labelColor,
                        ValueLabelColor = labelColor
                    };
                });

            priceChartView.Chart = new BarChart
            {
                Entries = entries,
                LabelOrientation = Orientation.Horizontal,
                ValueLabelOrientation = Orientation.Horizontal,
                LabelColor = labelColor,
                PointMode = PointMode.Circle
            };
        }
    }
}
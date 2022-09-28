using Android.App;
using Android.OS;
using Android.Views;
using FuelMonitor.BO.DAO;
using Microcharts;
using Microcharts.Droid;
using System.Linq;

namespace FuelMonitor.Fragments
{
    public class OverviewFragment : Fragment
    {
        private ChartView chartView;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.OverviewFragment, container, false);

            chartView = view.FindViewById<ChartView>(Resource.Id.chartView);

            var color = SkiaSharp.SKColor.Parse("#03a9f4");

            var entries = FuelFillDAO
                .GetAllView()
                .Where(x => x.AVGKMPL > 0)
                .OrderBy(X => X.Date)
                .Select(x =>
                    new ChartEntry((float)x.AVGKMPL)
                    {
                        Label = x.Date.ToString("MMMyy"),
                        ValueLabel = x.AVGKMPL.ToString("#0 kmpl"),
                        Color = color,
                        TextColor = color,
                        ValueLabelColor = color
                    });

            chartView.Chart = new LineChart
            {
                Entries = entries,
                LabelOrientation = Orientation.Horizontal,
                ValueLabelOrientation = Orientation.Horizontal,
                LabelColor = color,
                LineMode = LineMode.Spline,
                PointMode = PointMode.Circle,
                EnableYFadeOutGradient = true
            };

            return view;
        }
    }
}
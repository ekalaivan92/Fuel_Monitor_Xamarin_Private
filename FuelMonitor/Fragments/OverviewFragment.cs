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

            var entries = FuelFillDAO
                .GetAllView()
                .Where(x => x.AVGKMPL > 0)
                .OrderBy(X => X.Date)
                .Select(x =>
                    new ChartEntry((float)x.AVGKMPL)
                    {
                        Label = x.Date.ToString("MMMyy"),
                        ValueLabel = x.ODOValue.ToString(),
                        Color = SkiaSharp.SKColor.Parse("#2c3e50"),
                        TextColor = SkiaSharp.SKColor.Parse("#2c3e50"),
                        ValueLabelColor = SkiaSharp.SKColor.Parse("#2c3e50")
                    });

            chartView.Chart = new LineChart
            {
                Entries = entries,
                LabelOrientation = Orientation.Horizontal,
                ValueLabelOrientation = Orientation.Horizontal,
                LineMode = LineMode.Spline,
                PointMode = PointMode.Circle,
                EnableYFadeOutGradient = true
            };

            return view;
        }
    }
}
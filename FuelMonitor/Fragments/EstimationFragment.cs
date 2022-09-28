using Android.App;
using Android.OS;
using Android.Preferences;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using FuelMonitor.BO.DAO;
using System;

namespace FuelMonitor.Fragments
{
    public class EstimationFragment : Fragment
    {
        private TextInputEditText lastODOValueTextInput;
        private TextInputEditText avgMilageTextInput;
        private TextInputEditText currentODOValueTextInput;
        private Button calculateButton;
        private TextView expectedTravelDistanceValue;
        private TextView expectedFuelAvailabilityValue;
        private TextInputEditText tankCapacityTextInput;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.EstimationFragment, container, false);

            lastODOValueTextInput = view.FindViewById<TextInputEditText>(Resource.Id.lastODOValueTextInput);
            avgMilageTextInput = view.FindViewById<TextInputEditText>(Resource.Id.avgMilageTextInput);
            currentODOValueTextInput = view.FindViewById<TextInputEditText>(Resource.Id.currentODOValueTextInput);
            calculateButton = view.FindViewById<Button>(Resource.Id.calculateButton);
            expectedTravelDistanceValue = view.FindViewById<TextView>(Resource.Id.expectedTravelDistanceValue);
            expectedFuelAvailabilityValue = view.FindViewById<TextView>(Resource.Id.expectedFuelAvailabilityValue);
            tankCapacityTextInput = view.FindViewById<TextInputEditText>(Resource.Id.tankCapacitInLitTextInput);

            calculateButton.Click += Caculate;

            lastODOValueTextInput.RequestFocus();

            LoadInitialValues();

            return view;
        }

        private void Caculate(object sender, EventArgs e)
        {
            var isOdoValid = decimal.TryParse(lastODOValueTextInput.Text, out decimal odoValue);
            var isAvgMilageValid = decimal.TryParse(avgMilageTextInput.Text, out decimal avgMilage);
            var isCurrentOdoValid = decimal.TryParse(currentODOValueTextInput.Text, out decimal currentODO);
            var isTankCapacityValid = decimal.TryParse(tankCapacityTextInput.Text, out decimal tankCapacity);

            isOdoValid &= (odoValue >= 0);
            isAvgMilageValid &= (avgMilage > 0);
            isCurrentOdoValid &= (currentODO >= 0);
            isTankCapacityValid &= (tankCapacity > 0);
            var isODODiffValid = currentODO >= odoValue;

            var isValid = (isOdoValid && isAvgMilageValid && isCurrentOdoValid && isODODiffValid && isTankCapacityValid);

            expectedTravelDistanceValue.Text = "0.00 km";
            expectedFuelAvailabilityValue.Text = "0.00 lt";

            if (!isValid)
            {
                string message;
                if (!isOdoValid)
                {
                    message = "Invalid ODO value on last filling";
                }
                else if (!isAvgMilageValid)
                {
                    message = "Invalid AVG millage";
                }
                else if (!isTankCapacityValid)
                {
                    message = "Invalid Tank capacity";
                }
                else if (!isCurrentOdoValid)
                {
                    message = "Invalid Current ODO value";
                }
                else
                {
                    message = "Current ODO Value should above last ODO value";
                }

                var toast = Toast.MakeText(Context, message, ToastLength.Long);
                toast.Show();

                return;
            }

            if (isValid)
            {
                var fullExpectedKM = tankCapacity * avgMilage;
                var expectedDistanceInKM = Math.Round(fullExpectedKM - (currentODO - odoValue));
                var expectedFuelInLt = Math.Round(expectedDistanceInKM / avgMilage, 2);

                if (expectedFuelInLt < 0 || expectedDistanceInKM < 0)
                {
                    var message = "Unexpected availability, Check input before calculate";
                    var toast = Toast.MakeText(Context, message, ToastLength.Long);
                    toast.Show();
                }
                else
                {
                    expectedTravelDistanceValue.Text = expectedDistanceInKM.ToString("~ #0.00 km");
                    expectedFuelAvailabilityValue.Text = expectedFuelInLt.ToString("~ #0.00 lt");
                }
            }
        }

        private void LoadInitialValues()
        {
            var sharedPreference = PreferenceManager.GetDefaultSharedPreferences(Context);
            var tankCapacity = (decimal)sharedPreference.GetFloat("TankCapacity", 10.00f);

            tankCapacityTextInput.Text = tankCapacity.ToString("#0.00");

            if (FuelFillDAO.HasInfoToEstimste())
            {
                var lastFuelFilling = FuelFillDAO.GetLatestFillingSummary();

                lastODOValueTextInput.Text = lastFuelFilling.ODOValue?.ToString("#0.00");
                avgMilageTextInput.Text = lastFuelFilling.AVGKMPL.ToString("#0.00");

                currentODOValueTextInput.RequestFocus();
            }
        }
    }
}
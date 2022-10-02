using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using System;

namespace FuelMonitor.Fragments
{
    public class SettingsFragment : Fragment
    {
        private TextInputEditText tankCapacityTextInput;
        private Button saveButton;
        private ISharedPreferences _sharedPreference;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.SettingsFragment, container, false);

            tankCapacityTextInput = view.FindViewById<TextInputEditText>(Resource.Id.tankCapacitInLitTextInput);
            saveButton = view.FindViewById<Button>(Resource.Id.saveButton);

            saveButton.Click += saveButton_click;

            _sharedPreference = PreferenceManager.GetDefaultSharedPreferences(Context);
            var tankCapacity = (decimal)_sharedPreference.GetFloat("TankCapacity", 10.00f);

            tankCapacityTextInput.Text = tankCapacity.ToString("#0.00");

            return view;
        }

        private void saveButton_click(object sender, EventArgs e)
        {
            var isTankCapacityValid = float.TryParse(tankCapacityTextInput.Text, out float tankCapacity);

            if (!isTankCapacityValid)
            {
                var toast = Toast.MakeText(Context, "Invalid tank capacity input", ToastLength.Long);
                toast.Show();
                return;
            }

            var editor = _sharedPreference.Edit();
            editor.PutFloat("TankCapacity", tankCapacity);
            editor.Commit();

            var cToast = Toast.MakeText(Context, "Settings updated", ToastLength.Long);
            cToast.Show();
        }
    }
}
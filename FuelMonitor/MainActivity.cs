using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using FuelMonitor.Models;
using System;
using System.Globalization;

namespace FuelMonitor
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            var button = FindViewById(Resource.Id.saveButton);
            button.Click += SaveButton_Click;

            ClearInputs();
            LoadEntries();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            var dataStored = SaveFuelEntry();

            if (dataStored)
            {
                ClearInputs();
                LoadEntries();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void ClearInputs()
        {
            var dateInput = FindViewById<TextInputEditText>(Resource.Id.dateTextInput);
            var odoValueInput = FindViewById<TextInputEditText>(Resource.Id.odoValueTextInput);
            var fuelFilled = FindViewById<TextInputEditText>(Resource.Id.currentFilledFuleTextInput);
            var fuelCost = FindViewById<TextInputEditText>(Resource.Id.fuelCostTextInput);
            dateInput.Text = DateTime.Now.ToString("dd-MM-yyyy HH:mm");
            odoValueInput.Text = string.Empty;
            fuelFilled.Text = string.Empty;
            fuelCost.Text = string.Empty;

            odoValueInput.RequestFocus();
        }

        private bool SaveFuelEntry()
        {
            var dateInputText = FindViewById<TextInputEditText>(Resource.Id.dateTextInput);
            var odoValueInputText = FindViewById<TextInputEditText>(Resource.Id.odoValueTextInput);
            var fuelFilledText = FindViewById<TextInputEditText>(Resource.Id.currentFilledFuleTextInput);
            var fuelCostText = FindViewById<TextInputEditText>(Resource.Id.fuelCostTextInput);

            var dateValid = DateTime.TryParseExact(dateInputText.Text, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date);
            var odoValid = long.TryParse(odoValueInputText.Text, out long odoValue);
            var fuelFilledValid = decimal.TryParse(fuelFilledText.Text, out decimal fuelFilled);
            var fuelCostValid = decimal.TryParse(fuelCostText.Text, out decimal fuelCost);

            var isValid = dateValid && odoValid && fuelFilledValid && fuelCostValid;

            if (isValid)
            {
                var entry = new FuelFill
                {
                    Date = date,
                    ODOValue = odoValue,
                    FuelFilled = fuelFilled,
                    FuelCost = fuelCost
                };

                AppUtil.Connection.BeginTransaction();
                AppUtil.Connection.Insert(entry);
                AppUtil.Connection.Commit();

                return true;
            }
            else
            {
                var message = string.Empty;

                if (!dateValid)
                {
                    message = "Date Invalid, Format: 'date-Month-Year Time:Minute'";
                }
                else if (!odoValid)
                {
                    message = "ODO Meter value Invalid";
                }
                else if (!fuelFilledValid)
                {
                    message = "Filled Fuel value Invalid";
                }
                else if (!fuelCostValid)
                {
                    message = "Filled Fuel Cost Invalid";
                }


                var toast = Toast.MakeText(ApplicationContext, message, ToastLength.Long);
                toast.Show();
            }

            return false;
        }

        private void LoadEntries()
        {
            var rows = AppUtil.Connection.Query<FuelFillView>(@"
                        select 
                            *,
                            odovalue - lastodovalue as distancetraveled,
                            round((odovalue - lastodovalue) / fuelfilled, 2) as avgkmpl
                        from 
                            (select 
                                *, 
                                (lag(ODOValue, 1) over (order by date, _id)) as lastodovalue 
                            from fuelfills 
                            order by date desc, _id desc)x");

            var layout = FindViewById<TableLayout>(Resource.Id.entriesListTable);
            layout.RemoveAllViews();
            layout.SetBackgroundColor(Color.WhiteSmoke);

            var headerDateTextView = GetColumnTextView("Date", TextAlignment.Center);
            var headerFuelFilledTextView = GetColumnTextView("Fuel (l)", TextAlignment.Center);
            var headerFuelCostTextView = GetColumnTextView("Cost", TextAlignment.Center);
            var headerODOTextView = GetColumnTextView("ODO", TextAlignment.Center);
            var distanceTraveledTextView = GetColumnTextView("Dist. Traveled", TextAlignment.TextEnd);
            var avgTraveledTextView = GetColumnTextView("Avg KM/L", TextAlignment.TextEnd);

            var th = GetTableRow(headerDateTextView, headerFuelFilledTextView, headerFuelCostTextView, headerODOTextView, distanceTraveledTextView, avgTraveledTextView);
            layout.AddView(th);


            foreach (var row in rows)
            {
                var dateTextView = GetColumnTextView(row.Date.ToString("dd-MM-yyyy"));
                var fuelFilledTextView = GetColumnTextView(row.FuelFilled.ToString("#.00"), TextAlignment.TextEnd);
                var fuelCostTextView = GetColumnTextView(row.FuelCost.ToString("#.00"), TextAlignment.TextEnd);
                var odoTextView = GetColumnTextView(row.ODOValue.ToString("#.00"), TextAlignment.TextEnd);
                var distanceTextView = GetColumnTextView(row.DistanceTraveled.ToString("#"), TextAlignment.TextEnd);
                var avgTextView = GetColumnTextView(row.AVGKMPL.ToString("#"), TextAlignment.TextEnd);

                var tr = GetTableRow(dateTextView, fuelFilledTextView, fuelCostTextView, odoTextView, distanceTextView, avgTextView);

                layout.AddView(tr);
            }

            var ftr = GetColumnTextView($"Total Entries: {rows.Count}", TextAlignment.TextStart);
            layout.AddView(ftr);

        }

        private TextView GetColumnTextView(string text, TextAlignment textAlignment = TextAlignment.TextStart)
        {
            var textView = new TextView(ApplicationContext)
            {
                LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.MatchParent, TableRow.LayoutParams.WrapContent),
                Text = $"{text}",
                TextAlignment = textAlignment
            };

            textView.SetPadding(10, 10, 10, 10);
            return textView;
        }

        private TableRow GetTableRow(params TextView[] views)
        {
            var tr = new TableRow(ApplicationContext)
            {
                LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.MatchParent, TableRow.LayoutParams.WrapContent)
            };

            foreach (var param in views)
            {
                tr.AddView(param);
            }

            return tr;
        }
    }
}


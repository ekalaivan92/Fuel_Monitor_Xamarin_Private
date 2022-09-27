using Android.App;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using FuelMonitor.BO.DAO;
using FuelMonitor.BO.Models;
using FuelMonitor.Utils;
using System;
using System.Globalization;
using System.IO;

namespace FuelMonitor.Fragments
{
    public class FuelCaptureFragment : Fragment
    {
        private long _editingId;

        private Button saveButton;
        private Button cancelButton;
        private TextInputEditText dateInputText;
        private TextInputEditText odoValueInputText;
        private TextInputEditText fuelFilledText;
        private TextInputEditText fuelCostText;
        private TextView sectionTooltipTextView;
        private ImageButton imageCaptureButton;
        private ImageButton imageUploadButton;
        private ImageView imageView;
        private TableLayout entriesListTableLayout;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.FuelCaptureFragment, container, false);

            saveButton = view.FindViewById<Button>(Resource.Id.saveButton);
            cancelButton = view.FindViewById<Button>(Resource.Id.cancelButton);
            imageCaptureButton = view.FindViewById<ImageButton>(Resource.Id.imageCaptureButton);
            imageUploadButton = view.FindViewById<ImageButton>(Resource.Id.imageUploadButton);
            imageView = view.FindViewById<ImageView>(Resource.Id.fillingImageView);
            dateInputText = view.FindViewById<TextInputEditText>(Resource.Id.dateTextInput);
            odoValueInputText = view.FindViewById<TextInputEditText>(Resource.Id.odoValueTextInput);
            fuelFilledText = view.FindViewById<TextInputEditText>(Resource.Id.currentFilledFuleTextInput);
            fuelCostText = view.FindViewById<TextInputEditText>(Resource.Id.fuelCostTextInput);
            sectionTooltipTextView = view.FindViewById<TextView>(Resource.Id.sectionToolTip);
            entriesListTableLayout = view.FindViewById<TableLayout>(Resource.Id.entriesListTable);

            saveButton.Click += SaveButton_Click;
            cancelButton.Click += CancelButton_Click;
            imageCaptureButton.Click += CaptureImageButton_Click;
            imageUploadButton.Click += UploadImageButton_Click;

            ClearInputs();
            LoadEntries();

            return view;
        }

        private async void CaptureImageButton_Click(object sender, EventArgs e)
        {
            var result = await MediaPickerUtils.CaptureImageWithCamera<Bitmap>("");
            if (result.Success)
            {
                imageView.SetImageBitmap(result.Result);
                ShowCaptureSectionToolTip(ViewStates.Visible);
            }
            else
            {
                var toast = Toast.MakeText(Context, result.ErrorMessage, ToastLength.Long);
                toast.Show();
            }
        }

        private async void UploadImageButton_Click(object sender, EventArgs e)
        {
            var result = await MediaPickerUtils.PickImageFromFile<Bitmap>("");
            if (result.Success)
            {
                imageView.SetImageBitmap(result.Result);
                ShowCaptureSectionToolTip(ViewStates.Visible);
            }
            else
            {
                var toast = Toast.MakeText(Context, result.ErrorMessage, ToastLength.Long);
                toast.Show();
            }
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

        private void CancelButton_Click(object sender, EventArgs e)
        {
            ClearInputs();
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            var editingId = Convert.ToInt64(((View)sender).ContentDescription);
            ShowFuelEntry(editingId);
        }

        private void ClearInputs()
        {
            _editingId = 0;

            var image = Resources.GetDrawable(Resource.Drawable.abc_ab_share_pack_mtrl_alpha, Context.Theme);

            dateInputText.Text = DateTime.Now.ToString("dd-MM-yyyy HH:mm");
            odoValueInputText.Text = string.Empty;
            fuelFilledText.Text = string.Empty;
            fuelCostText.Text = string.Empty;
            imageView.SetImageDrawable(image);

            ShowCaptureSectionToolTip(ViewStates.Gone);

            odoValueInputText.RequestFocus();
        }

        private bool SaveFuelEntry()
        {
            var dateValid = DateTime.TryParseExact(
                dateInputText.Text,
                "dd-MM-yyyy HH:mm",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime date);

            var odoValid = long.TryParse(odoValueInputText.Text, out long odoValue);
            var fuelFilledValid = decimal.TryParse(fuelFilledText.Text, out decimal fuelFilled);
            var fuelCostValid = decimal.TryParse(fuelCostText.Text, out decimal fuelCost);
            var isValidPhoto = TryGetImage(imageView, out byte[] image);

            var isValid = dateValid &&
                ((odoValid && fuelFilledValid && fuelCostValid) || isValidPhoto);

            if (isValid)
            {
                var entry = new FuelFill
                {
                    Date = date,
                    PhotoCapute = image
                };

                if (odoValue != 0)
                {
                    entry.ODOValue = odoValue;
                }
                if (fuelFilled != 0)
                {
                    entry.FuelFilled = fuelFilled;
                }
                if (fuelCost != 0)
                {
                    entry.FuelCost = fuelCost;
                }

                AppUtil.Connection.BeginTransaction();

                if (_editingId > 0)
                {
                    entry.ID = _editingId;
                    FuelFillDAO.Update(entry);
                }
                else
                {
                    FuelFillDAO.Create(entry);
                }
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

                var toast = Toast.MakeText(Context, message, ToastLength.Long);
                toast.Show();
            }

            return false;
        }

        private void ShowFuelEntry(long editingId)
        {
            ClearInputs();

            _editingId = editingId;

            var row = FuelFillDAO.Get(_editingId);

            dateInputText.Text = row.Date.ToString("dd-MM-yyyy HH:mm");
            odoValueInputText.Text = row.ODOValue?.ToString("#");
            fuelFilledText.Text = row.FuelFilled?.ToString("#.00");
            fuelCostText.Text = row.FuelCost?.ToString("#.00");

            TrySetImage(imageView, row.PhotoCapute);
        }

        private void ShowCaptureSectionToolTip(ViewStates viewState)
        {
            sectionTooltipTextView.Visibility = viewState;
        }

        private bool TryGetImage(ImageView imageView, out byte[] bitmapData)
        {
            bitmapData = null;

            try
            {
                var bitmapDrawable = ((BitmapDrawable)imageView.Drawable);
                Bitmap bitmap;
                if (bitmapDrawable == null)
                {
                    imageView.BuildDrawingCache();
                    bitmap = imageView.GetDrawingCache(false);
                    imageView.DestroyDrawingCache();
                }
                else
                {
                    bitmap = bitmapDrawable.Bitmap;
                }

                if (bitmap.AllocationByteCount == 0)
                {
                    return false;
                }

                using (var stream = new MemoryStream())
                {
                    bitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);
                    bitmapData = stream.ToArray();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void TrySetImage(ImageView view, byte[] data)
        {
            if (data != null && data.Length > 0)
            {
                var bitmap = BitmapFactory.DecodeByteArray(data, 0, data.Length);
                view.SetImageBitmap(bitmap);
            }
        }

        private void LoadEntries()
        {
            var rows = FuelFillDAO.GetAllView();

            entriesListTableLayout.RemoveAllViews();
            entriesListTableLayout.SetBackgroundColor(Color.WhiteSmoke);

            var headerDateTextView = GetColumnTextView("Date", TextAlignment.Center);
            var headerFuelFilledTextView = GetColumnTextView("Fuel (l)", TextAlignment.Center);
            var headerFuelCostTextView = GetColumnTextView("Cost", TextAlignment.Center);
            var headerODOTextView = GetColumnTextView("ODO", TextAlignment.Center);
            var distanceTraveledTextView = GetColumnTextView("Dist. Traveled", TextAlignment.TextEnd);
            var avgTraveledTextView = GetColumnTextView("Avg KM/L", TextAlignment.TextEnd);
            var editButtonCol = GetColumnTextView("Edit", TextAlignment.TextStart);

            var th = GetTableRow(editButtonCol, headerDateTextView, headerFuelFilledTextView, headerFuelCostTextView, headerODOTextView, distanceTraveledTextView, avgTraveledTextView);
            entriesListTableLayout.AddView(th);

            foreach (var row in rows)
            {
                var dateTextView = GetColumnTextView(row.Date.ToString("dd-MM-yyyy"));
                var fuelFilledTextView = GetColumnTextView(row.FuelFilled?.ToString("#.00"), TextAlignment.TextEnd);
                var fuelCostTextView = GetColumnTextView(row.FuelCost?.ToString("#.00"), TextAlignment.TextEnd);
                var odoTextView = GetColumnTextView(row.ODOValue?.ToString("#"), TextAlignment.TextEnd);
                var distanceTextView = GetColumnTextView(row.DistanceTraveled.ToString("#"), TextAlignment.TextEnd);
                var avgTextView = GetColumnTextView(row.AVGKMPL.ToString("#"), TextAlignment.TextEnd);
                var editButton = GetColumnButtonView(row.ID.ToString(), "E", TextAlignment.TextEnd);

                var tr = GetTableRow(editButton, dateTextView, fuelFilledTextView, fuelCostTextView, odoTextView, distanceTextView, avgTextView);

                entriesListTableLayout.AddView(tr);
            }

            var ftr = GetColumnTextView($"Total Entries: {rows.Count}", TextAlignment.TextStart);

            entriesListTableLayout.AddView(ftr);
        }

        private TextView GetColumnTextView(string text, TextAlignment textAlignment = TextAlignment.TextStart)
        {
            var textView = new TextView(Context)
            {
                LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.MatchParent, TableRow.LayoutParams.WrapContent),
                Text = $"{text}",
                TextAlignment = textAlignment
            };

            textView.SetPadding(10, 10, 10, 10);

            return textView;
        }

        private ImageButton GetColumnButtonView(string val, string text, TextAlignment textAlignment = TextAlignment.TextStart)
        {
            var button = new ImageButton(Context)
            {
                LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.WrapContent, TableRow.LayoutParams.WrapContent),
                ContentDescription = val,
                TextAlignment = textAlignment,
            };

            button.SetAdjustViewBounds(true);
            button.SetBackgroundColor(Color.WhiteSmoke);
            button.SetMaxWidth(5);
            button.SetImageResource(Android.Resource.Drawable.IcMenuEdit);
            button.Click += EditButton_Click;
            button.SetPadding(5, 5, 5, 5);

            return button;
        }

        private TableRow GetTableRow(params View[] views)
        {
            var tr = new TableRow(Context)
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
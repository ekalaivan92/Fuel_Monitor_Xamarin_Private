using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Widget;
using V4Widget = Android.Support.V4.Widget;
using V7Widget = Android.Support.V7.Widget;
using EssentialPlatform = Xamarin.Essentials.Platform;

namespace FuelMonitor.Activities
{
    public partial class MainActivity
    {
        private V7Widget.Toolbar toolbar;
        private V4Widget.DrawerLayout drawerLayout;
        private Button saveButton;
        private Button cancelButton;
        private ImageButton imageCaptureButton;
        private ImageButton imageUploadButton;
        private ActionBarDrawerToggle drawerToggle;
        private TextInputEditText dateInputText;
        private TextInputEditText odoValueInputText;
        private TextInputEditText fuelFilledText;
        private TextInputEditText fuelCostText;
        private ImageView imageView;
        private TableLayout entriesListTableLayout;

        private void Initialize(Android.OS.Bundle savedInstanceState)
        {
            EssentialPlatform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            toolbar = FindViewById<V7Widget.Toolbar>(Resource.Id.toolbar);
            saveButton = FindViewById<Button>(Resource.Id.saveButton);
            cancelButton = FindViewById<Button>(Resource.Id.cancelButton);
            imageCaptureButton = FindViewById<ImageButton>(Resource.Id.imageCaptureButton);
            imageUploadButton = FindViewById<ImageButton>(Resource.Id.imageUploadButton);
            drawerLayout = FindViewById<V4Widget.DrawerLayout>(Resource.Id.drawer_Layout);
            dateInputText = FindViewById<TextInputEditText>(Resource.Id.dateTextInput);
            odoValueInputText = FindViewById<TextInputEditText>(Resource.Id.odoValueTextInput);
            fuelFilledText = FindViewById<TextInputEditText>(Resource.Id.currentFilledFuleTextInput);
            fuelCostText = FindViewById<TextInputEditText>(Resource.Id.fuelCostTextInput);
            imageView = (ImageView)FindViewById(Resource.Id.fillingImageView);
            entriesListTableLayout = FindViewById<TableLayout>(Resource.Id.entriesListTable);

            SetSupportActionBar(toolbar);
            drawerToggle = new ActionBarDrawerToggle(this, drawerLayout, toolbar, Resource.String.open_drawer, Resource.String.close_drawer);
        }
    }
}
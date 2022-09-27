using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using FuelMonitor.Fragments;
using static Android.Support.Design.Widget.NavigationView;
using EssentialPlatform = Xamarin.Essentials.Platform;

namespace FuelMonitor.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public partial class MainActivity : AppCompatActivity, IOnNavigationItemSelectedListener
    {
        private NavigationView _navView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Initialize(savedInstanceState);

            drawerLayout.AddDrawerListener(drawerToggle);
            drawerToggle.SyncState();

            _navView = FindViewById<NavigationView>(Resource.Id.nav_view);
            _navView.SetNavigationItemSelectedListener(this);

            SwitchFragment(Resource.Id.action_fuel_Capture);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            EssentialPlatform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public bool OnNavigationItemSelected(IMenuItem menuItem)
        {
            return SwitchFragment(menuItem.ItemId);
        }

        private bool SwitchFragment(int menuItemItemId)
        {
            Fragment newFragment = null;

            switch (menuItemItemId)
            {
                case Resource.Id.action_fuel_Capture:
                    newFragment = new FuelCaptureFragment();
                    break;

                case Resource.Id.action_estimate:
                    newFragment = new EstimationFragment();
                    break;

                default:
                    var toast = Toast.MakeText(ApplicationContext, "Invalid Option", ToastLength.Long);
                    toast.Show();
                    return true;
            }

            var fragmentTransaction = FragmentManager.BeginTransaction();
            fragmentTransaction.Replace(Resource.Id.lr, newFragment);
            fragmentTransaction.Commit();

            drawerLayout.CloseDrawer(GravityCompat.Start);
            return true;
        }
    }
}
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
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
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            EssentialPlatform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public bool OnNavigationItemSelected(IMenuItem menuItem)
        {
            Fragment newFragment = null;
            var fragment = FragmentManager.FindFragmentById<Fragment>(Resource.Id.fuelCaptureFragment);

            switch (menuItem.ItemId)
            {
                case Resource.Id.action_fuel_Capture:
                    newFragment = new FuelCaptureFragment();
                    break;

                default:
                    var toast = Toast.MakeText(ApplicationContext, "Invalid Option", ToastLength.Long);
                    toast.Show();
                    return true;
            }

            var fragmentTransaction = FragmentManager.BeginTransaction();
            fragmentTransaction.Detach(fragment);
            fragmentTransaction.Attach(newFragment);
            fragmentTransaction.Commit();

            drawerLayout.CloseDrawer(GravityCompat.Start);
            return true;
        }
    }
}
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using EssentialPlatform = Xamarin.Essentials.Platform;

namespace FuelMonitor.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public partial class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Initialize(savedInstanceState);

            drawerLayout.AddDrawerListener(drawerToggle);
            drawerToggle.SyncState();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            EssentialPlatform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
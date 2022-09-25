using Android.Support.V7.App;
using EssentialPlatform = Xamarin.Essentials.Platform;
using V4Widget = Android.Support.V4.Widget;
using V7Widget = Android.Support.V7.Widget;

namespace FuelMonitor.Activities
{
    public partial class MainActivity
    {
        private V7Widget.Toolbar toolbar;
        private V4Widget.DrawerLayout drawerLayout;
        private ActionBarDrawerToggle drawerToggle;

        private void Initialize(Android.OS.Bundle savedInstanceState)
        {
            EssentialPlatform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            toolbar = FindViewById<V7Widget.Toolbar>(Resource.Id.toolbar);
            drawerLayout = FindViewById<V4Widget.DrawerLayout>(Resource.Id.drawer_Layout);

            SetSupportActionBar(toolbar);
            drawerToggle = new ActionBarDrawerToggle(this, drawerLayout, toolbar, Resource.String.open_drawer, Resource.String.close_drawer);
        }
    }
}
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Content.PM;


using AndroidX.AppCompat.App;

using Google.Android.Material.Snackbar;

using Com.Sirl.Core;
using Com.Sirl.Core.Listeners;
using Com.Sirl.Core.Location;
using Com.Sirl.Core.Recording;
using Com.Sirl.Mapping;
using Com.Sirl.Retail.UI;
using Com.Sirl.Mapping.Routeutils;
using Google.Android.Material.Tabs;

namespace SIRLDemo
{
    [Activity(Label = "Retailer's Shopping List", Theme = "@style/AppTheme.NoActionBar")]
    public class ShoppingListActivity : AppCompatActivity, ILocationUpdateListener
    {
        private const string TAG = "SearchActivity";

        private SirlPipsManager mSirlManager;
        private TripStateListener mTripStateListener;
        private ExternalLogger mExternalLogger;

        private SirlMapFragment mSirlMapFragment;
        private TabLayout tabLayout;

        private FrameLayout listFrame;
        private FrameLayout mapFrame;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_shopping_list);

            AndroidX.AppCompat.Widget.Toolbar toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            mExternalLogger = ExternalLogger.Instance;
            mTripStateListener = new TutorialTripStateListener(this, mExternalLogger);

            mSirlMapFragment = (SirlMapFragment)SupportFragmentManager.FindFragmentById(Resource.Id.map);
            listFrame = FindViewById<FrameLayout>(Resource.Id.list_frame);
            mapFrame = FindViewById<FrameLayout>(Resource.Id.map_frame);

            tabLayout = FindViewById<TabLayout>(Resource.Id.tabs);
            tabLayout.AddOnTabSelectedListener(new TabSelectionListener(this));
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

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (mSirlManager != null)
            {
                mSirlManager.DeregisterTripStateListener(mTripStateListener);
                mSirlManager.DeregisterLocationListener(this);
                mSirlManager.StopLocationUpdates();
            }
        }

        public void OnLocationUpdate(Location location)
        {
            //
            // Quick Start - Toast Location Updates
            //
            //string locationString =
            //     string.Format(
            //         "Your position is x:{0:F2}, y:{1:F2}",
            //         location.GetX(),
            //         location.GetY()
            //     );
            //Toast.MakeText(this, locationString, ToastLength.Short).Show();
        }

        private class TutorialRouteStatusListener : Java.Lang.Object, IRouteStatusListener
        {
            public void OnRouteStart()
            {
                Log.Debug("ShopperPortal", "Route start!");
            }

            public void OnRouteComplete()
            {
                Log.Debug("ShopperPortal", "Route complete!");
            }

            public void OnRouteFail(RouteError routeError)
            {
                Log.Error("ShopperPortal", "Route error: " + routeError.Message);
            }
        }

        private class TabSelectionListener : Java.Lang.Object, TabLayout.IOnTabSelectedListener
        {
            private static int POSITION_LIST = 0;
            private static int POSITION_MAP = 1;

            ShoppingListActivity activity = null;

            public TabSelectionListener(ShoppingListActivity activity)
            {
                this.activity = activity;
            }

            public void OnTabReselected(TabLayout.Tab tab)
            {
            }

            public void OnTabSelected(TabLayout.Tab tab)
            {
                activity.listFrame.Visibility = ViewStates.Gone;
                activity.mapFrame.Visibility = ViewStates.Gone;

                if (tab.Position == POSITION_LIST)
                {
                    activity.listFrame.Visibility = ViewStates.Visible;
                }
                else if (tab.Position == POSITION_MAP)
                {
                    activity.mapFrame.Visibility = ViewStates.Visible;
                }
            }

            public void OnTabUnselected(TabLayout.Tab tab)
            {
            }
        }

        private class TutorialTripStateListener : TripStateListener
        {
            private Context context;
            private ExternalLogger externalLogger;

            public TutorialTripStateListener(Context context, ExternalLogger logger)
            {
                this.context = context;
                this.externalLogger = logger;
            }

            public override void OnTripStart()
            {
                //Signifies that the trip has begun. One or more of the
                //calls for before the trip should be used in this hook.
                Log.Debug(TAG, "Trip Started");
                Toast.MakeText(
                      context,
                      "Trip Started.",
                      ToastLength.Short
                ).Show();
            }

            public override void OnTripStop()
            {
                //Signifies that the trip has begun. One or more of the
                //calls for after the trip should be used in this hook.
                Log.Debug(TAG, "Trip Stopped");
                Toast.MakeText(
                      context,
                      "Trip Ended.",
                      ToastLength.Short
                ).Show();
            }
        }
    }
}

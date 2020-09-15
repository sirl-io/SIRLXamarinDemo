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
using System.Collections.Generic;

namespace SIRLDemo
{
    [Activity(Label = "Search Demo", Theme = "@style/AppTheme.NoActionBar")]
    public class SearchActivity : AppCompatActivity, ILocationUpdateListener
    {
        private const string TAG = "SearchActivity";

        private SirlPipsManager mSirlManager;
        private TripStateListener mTripStateListener;
        private ExternalLogger mExternalLogger;
        private SirlMapFragment mSirlMapFragment;
        private SearchFragment mSirlSearchFragment;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_search);

            AndroidX.AppCompat.Widget.Toolbar toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            mExternalLogger = ExternalLogger.Instance;
            mTripStateListener = new TutorialTripStateListener(this, mExternalLogger);

            mSirlMapFragment = (SirlMapFragment)SupportFragmentManager.FindFragmentById(Resource.Id.map);
            mSirlSearchFragment = (SearchFragment)SupportFragmentManager.FindFragmentById(Resource.Id.search_bar);

            mSirlSearchFragment.AttachMapFragment(mSirlMapFragment);
            mSirlSearchFragment.RegisterRouteStatusListener(new TutorialRouteStatusListener());
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
            public void OnRouteStart(IList<IRoutedObject> p0)
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

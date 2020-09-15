using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Content.PM;

using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;

using Google.Android.Material.Tabs;

using Com.Sirl.Core;
using Com.Sirl.Core.Listeners;
using Com.Sirl.Core.Location;
using Com.Sirl.Core.Recording;
using Com.Sirl.Mapping;
using Com.Sirl.Retail;
using Com.Sirl.Retail.Routeutils;
using Com.Sirl.Retail.Models;
using Com.Sirl.Mapping.Routeutils;


using SIRLDemo.Retail.ShoppingList;

using System.Collections.Generic;

using static Android.Views.View;

namespace SIRLDemo
{
    [Activity(Label = "@string/sirl_demo_retailers_list", Theme = "@style/AppTheme.NoActionBar")]
    public class ShoppingListActivity : AppCompatActivity, ILocationUpdateListener, IOnClickListener, IRetailRouteStatusListener, IRetailRouteEventListener
    {
        private const string TAG = "SearchActivity";

        private const int TAB_POSITION_LIST = 0;
        private const int TAB_POSITION_MAP = 1;

        private SirlPipsManager mSirlManager;
        private TripStateListener mTripStateListener;
        private ExternalLogger mExternalLogger;

        private SirlMapFragment mSirlMapFragment;
        private TabLayout tabLayout;

        private FrameLayout listFrame;
        private FrameLayout mapFrame;

        private RecyclerView listView;
        private RecyclerView missingListView;
        private TextView missingListLabel;
        private List<ShoppingListItem> shoppingList = new List<ShoppingListItem>();
        private List<ShoppingListItem> missingList = new List<ShoppingListItem>();
        private ShoppingListAdapter adapter;
        private ShoppingListAdapter missingAdapter;

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

            SetupTabs();
            SetupShoppingList();
            SetupShoppingListViews();
        }

        private void SetupTabs()
        {
            listFrame = FindViewById<FrameLayout>(Resource.Id.list_frame);
            mapFrame = FindViewById<FrameLayout>(Resource.Id.map_frame);

            tabLayout = FindViewById<TabLayout>(Resource.Id.tabs);
            tabLayout.AddOnTabSelectedListener(new TabSelectionListener(this));
        }

        private void SetupShoppingList()
        {
            shoppingList.Add(new ShoppingListItem("Nature's Way Sambucus Zinc with Elderberry and Vitamin C Lozenges, Honey Lemon, 24 Ct", "033674120897", 1));
            shoppingList.Add(new ShoppingListItem("Golden - Zucchini Pancakes 10.60 oz", "041641201074", 1));
            shoppingList.Add(new ShoppingListItem("Vermont Creamery Fresh Goat Cheese", "011826100126", 2));
            shoppingList.Add(new ShoppingListItem("Heinz Tomato Ketchup, 64 oz", "013000001212", 1));
            shoppingList.Add(new ShoppingListItem("Hood 2% - Gallon", "hood2", 3));
            shoppingList.Add(new ShoppingListItem("Reese's Puffs Cereal - 11.5oz - General Mills", "016000122222", 1));
        }

        private void SetupShoppingListViews()
        {
            listView = FindViewById<RecyclerView>(Resource.Id.list);
            listView.SetLayoutManager(new LinearLayoutManager(this));

            adapter = new ShoppingListAdapter(shoppingList);
            listView.SetAdapter(adapter);

            missingListLabel = FindViewById<TextView>(Resource.Id.missing_lbl);

            missingListView = FindViewById<RecyclerView>(Resource.Id.missing_list);
            missingListView.SetLayoutManager(new LinearLayoutManager(this));

            missingAdapter = new ShoppingListAdapter(missingList);
            missingListView.SetAdapter(missingAdapter);

            Button route = FindViewById<Button>(Resource.Id.route_list_btn);
            route.SetOnClickListener(this);
        }

        public void OnClick(View v)
        {
            if(v.Id == Resource.Id.route_list_btn)
            {
                tabLayout.GetTabAt(TAB_POSITION_MAP).Select();
                SirlRetailWrapper.GetInstance(Android.App.Application.Context)
                    .ShowRoute(
                        mSirlMapFragment,
                        GetSkus(),
                        this,
                        this);
            }
        }

        private IList<string> GetSkus()
        {
            List<string> strings = new List<string>();
            foreach(var item in shoppingList)
            {
                strings.Add(item.GetSku());
            }
            return strings;
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

        private void SwitchTab(int tab_position)
        {
            listFrame.Visibility = ViewStates.Gone;
            mapFrame.Visibility = ViewStates.Gone;

            if (tab_position == TAB_POSITION_LIST)
            {
                listFrame.Visibility = ViewStates.Visible;
            }
            else if (tab_position == TAB_POSITION_MAP)
            {
                mapFrame.Visibility = ViewStates.Visible;
            }
        }

        public void OnRouteEvent(RetailRouteEvent p0)
        {
            //This is where designers can hook in pop-ups
            Log.Info(TAG, "Received: " + p0.ToString());
        }

        public void OnProductLookup(IList<StoreProduct> products, IList<string> missing)
        {
            // This is where SIRL provides information on what products are
            // available (known locations) in the store and those that are
            // missing.
            foreach(var sku in missing)
            {
                var index = getSkuIndex(sku);
                if(index >= 0)
                {
                    missingList.Add(shoppingList[index]);
                    shoppingList.RemoveAt(index);
                }
            }

            updateListView();
        }

        private int getSkuIndex(string sku)
        {
            int index = 0;
            foreach(var p in shoppingList)
            {
                if(p.GetSku() == sku)
                {
                    return index;
                }
                index++;
            }

            return -1;
        }

        private void updateListView()
        {
            if(missingList.Count > 0)
            {
                missingListLabel.Visibility = ViewStates.Visible;
                missingListView.Visibility = ViewStates.Visible;
            }
            else
            {
                missingListLabel.Visibility = ViewStates.Gone;
                missingListView.Visibility = ViewStates.Gone;
            }

            adapter.NotifyDataSetChanged();
            missingAdapter.NotifyDataSetChanged();
        }

        public void OnRouteComplete()
        {
            Log.Info(TAG, "Completed Route");
        }

        public void OnRouteFail(RetailRouteError p0)
        {
            Log.Error(TAG, "Route Error: " + p0.ToString());
        }

        public void OnRouteStart(IList<RoutedStoreProduct> ordered_list)
        {
            updateListOrder(ordered_list);
            updateListView();
        }

        private void updateListOrder(IList<RoutedStoreProduct> ordered_list)
        {
            List<ShoppingListItem> ordered_items = new List<ShoppingListItem>(shoppingList.Count);
            foreach(var product in ordered_list)
            {
                int index = getSkuIndex(product.Product.Id);
                if(index >= 0)
                {
                    ordered_items.Add(shoppingList[index]);
                    shoppingList.RemoveAt(index);
                }
            }

            if(shoppingList.Count > 0)
            {
                Log.Error(TAG, "Routed list does not match expected shopping list");
                missingList.AddRange(shoppingList);
                shoppingList.Clear();
            }

            shoppingList.AddRange(ordered_items);
        }

        private class TabSelectionListener : Java.Lang.Object, TabLayout.IOnTabSelectedListener
        {
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
                activity.SwitchTab(tab.Position);
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

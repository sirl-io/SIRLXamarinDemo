using System;

using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Content;

using AndroidX.AppCompat.App;
using AndroidX.CardView.Widget;
using AndroidX.Core.App;
using AndroidX.Core.Content;

using Google.Android.Material.Snackbar;

using Com.Sirl.Core;
using Com.Sirl.Core.Location;
using Com.Sirl.Core.Recording;
using Com.Sirl.Core.Location.Filters;
using Com.Sirl.Core.Models;

namespace SIRLDemo
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, View.IOnClickListener
    {
        private const String TAG = "MainActivity";

        private ExternalLogger mExternalLogger;
        private SirlPipsManager mSirlManager;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            var id = Resource.Layout.activity_main;
            SetContentView(Resource.Layout.activity_main);

            AndroidX.AppCompat.Widget.Toolbar toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            mExternalLogger = ExternalLogger.Instance;

            CardView search_card = (CardView)FindViewById(Resource.Id.search_card);
            CardView list_card = (CardView)FindViewById(Resource.Id.list_card);
            CardView next_item_card = (CardView)FindViewById(Resource.Id.next_item_card);

            search_card.SetOnClickListener(this);
            list_card.SetOnClickListener(this);
            next_item_card.SetOnClickListener(this);

            setupSirl();
        }

        public void OnClick(Android.Views.View? v)
        {
            Intent activityIntent = null;

            if (v == null)
            {
                return;
            }

            if (v.Id == Resource.Id.search_card)
            {
                activityIntent = new Intent(this, typeof(SearchActivity));
            }
            else if(v.Id == Resource.Id.list_card)
            {
                activityIntent = new Intent(this, typeof(ShoppingListActivity));
            }
            else if (v.Id == Resource.Id.next_item_card)
            {
                activityIntent = new Intent(this, typeof(ShoppingListActivity));
            }
                

            if (activityIntent != null)
            {
                StartActivity(activityIntent);
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


            if (grantResults.Length > 0 && requestCode == 1)
            {
                if (grantResults[0] == Permission.Granted)
                {
                    Log.Debug("permission", "location permissions granted");
                    initializeSirl();
                }
                else
                {
                    //Close the Activity & App if Location Permissions not granted
                    Finish();
                }
            }

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void FabOnClick(object sender, System.EventArgs eventArgs)
        {
            View view = (View)sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }

        protected override void OnDestroy()
        {
        }

        private void setupSirl()
        {
            if (hasPermissions())
            {
                initializeSirl();
            }
            else
            {
                requestPermissions();
            }
        }

        private Boolean hasPermissions()
        {
            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.M)
            {
                return !((ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation)
                            != (int)Permission.Granted)
                        || (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation)
                            != (int)Permission.Granted)
                        || (ContextCompat.CheckSelfPermission(this, Manifest.Permission.Internet)
                            != (int)Permission.Granted));
            }
            return true;
        }

        public void initializeSirl()
        {
            LocationProviderConfig locationProviderConfig =
                new LocationProviderConfig.Builder(Android.App.Application.Context)
                    //We disable background mode here to allow manual control, but
                    //this can be enabled to allow the system to automatically handle
                    //location update state.
                    .EnableAutoTripStartStop(false)
                    .UseMappedLocationResolver(new TutorialMappedLocationResolver(Android.App.Application.Context))
                    .UseLocationEngine(new TutorialEngine())
                    .UseLocationFilters(createTestLocationFilters())
                    .Build();

            SirlPipsManager.Config config = new SirlPipsManager.Config(Android.App.Application.Context);
            config.SetAPIKey("9mkYplpPuo2PCes7dqhIU74yg5dgO16M9D5hcqJS");
            config.SetLocationConfig(locationProviderConfig);

            mSirlManager = SirlPipsManager.GetInstance(config);
            mSirlManager.StartLocationUpdates();
        }


        private class TutorialEngine : SirlLocationEngine
        {
            public Handler mHandler = new Handler();

            public override void InitializeForMappedLocation(MappedLocation mappedLocation)
            {
                //For this test engine, this hook is not necessary. For any custom location engine,
                //this hook is meant to initialize the component with any relevant configurations 
                //for the current location.
            }

            public override string EngineVersion
            {
                // Metadata.xml XPath method reference: path="/api/package[@name='com.sirl.core.location']/class[@name='SirlLocationEngine']/method[@name='getEngineVersion' and count(parameter)=0]"
                [Register("getEngineVersion", "()Ljava/lang/String;", "GetGetEngineVersionHandler")]
                get
                {
                    return "Tutorial Test Engine";
                }
            }

            public override void StartPolling()
            {
                _ = mHandler.Post(new RandomPositionRunnable(this));
            }

            public override void StopPolling()
            {
                mHandler.RemoveCallbacksAndMessages(null);
            }

            private class RandomPositionRunnable : Java.Lang.Object, Java.Lang.IRunnable
            {
                TutorialEngine mEngine = null;
                public RandomPositionRunnable(TutorialEngine engine)
                {
                    mEngine = engine;
                }

                public void Run()
                {
                    mEngine.mHandler.PostDelayed(this, 1000);

                    Random rand = new Random();

                    Location randomLocation = new Location(
                            11.5 + rand.NextDouble() - 0.5,
                            4.5 + rand.NextDouble() - 0.5,
                            1.5
                    );

                    mEngine.Update(randomLocation);
                }
            }
        }

        private class TutorialMappedLocationResolver : MappedLocationResolver
        {
            public TutorialMappedLocationResolver(Context context)
                    : base(context) { }

            public override void DetermineMappedLocation(IMappedLocationResolveCallback cb)
            {
                cb.EnteredLocation(new MappedLocation(10));
            }
        }

        private LocationFilters createTestLocationFilters()
        {
            return new LocationFilters.Builder()
                    .ShouldLockToRegion(true)
                    .ShouldPredict(false)
                    .Build();
        }

        private void requestPermissions()
        {
            ActivityCompat.RequestPermissions(this, new String[]
                    {Manifest.Permission.AccessFineLocation,
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.Internet}, 1);
        }
    }
}

using System;

using Android.App;

namespace SIRLDemo
{
    #if DEBUG
    [Application(Debuggable=true)]
    #else
    [Application(Debuggable=false)]
    #endif
    public class MainApplication : Application
    {
        public MainApplication(IntPtr javaReference, Android.Runtime.JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace CombatManagerDroid
{

    [Service]
    public class WebService : Service
    {
        static readonly string TAG = typeof(WebService).FullName;


        bool isStarted;
        Handler handler;
        Action runnable;

        public override void OnCreate()
        {
            base.OnCreate();
            // This Action is only for demonstration purposes.
             runnable = new Action(() =>
            {

            });
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            if (isStarted)
            {

            }
            else
            {

                isStarted = true;
            }

            // This tells Android not to restart the service if it is killed to reclaim resources.
            return StartCommandResult.NotSticky;
        }


        public override IBinder OnBind(Intent intent)
        {
            return null;
        }


        public override void OnDestroy()
        {

            // Stop the handler.
            handler.RemoveCallbacks(runnable);

            isStarted = false;
            base.OnDestroy();
        }


    }
}
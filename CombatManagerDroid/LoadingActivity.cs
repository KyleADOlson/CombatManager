
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using CombatManager;

namespace CombatManagerDroid
{
    [Activity (Label = "Combat Manager", MainLauncher = true, Theme="@android:style/Theme.Black.NoTitleBar")]			
    public class LoadingActivity : Activity
    {
        protected override void OnCreate (Bundle bundle)
        {
            
            CoreContext.Context = Application.Context;

            base.OnCreate (bundle);

            SetContentView (Resource.Layout.Loading);

            Thread t = new Thread(new ThreadStart(delegate {
                Monster m = Monster.Monsters[0];
                RunOnUiThread(delegate {
                    LoadComplete();
                });
            }));
            t.Start();

        }

        private void LoadComplete()
        {
            Intent intent = new Intent(this.BaseContext, (Java.Lang.Class) new HomeActivity().Class); 
            intent.AddFlags(ActivityFlags.NewTask); 
            StartActivity(intent);

            Finish();
        }

    }
}


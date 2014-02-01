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
using CombatManager;

namespace CombatManagerDroid
{
    class SettingsDialog : Dialog

    {
        public SettingsDialog(Context context) : base (context)
        {
           
            SetContentView(Resource.Layout.SettingsDialog);
            SetCanceledOnTouchOutside(true);
            SetTitle("Settings");

            ISharedPreferences p = context.GetCMPrefs();

            CheckBox c = FindViewById<CheckBox>(Resource.Id.confirmBox);
            c.Checked = p.GetConfirmInitiative();
            c.Click += (object sender, EventArgs e) => 
            {
                ISharedPreferences sp = context.GetCMPrefs();
                var ed = sp.Edit();
                ed.PutBoolean(CMPreferences.SettingConfirmInitiative, c.Checked);
                ed.Commit();
            };


            CheckBox d = FindViewById<CheckBox>(Resource.Id.roll3d6Box);
            d.Checked = p.GetBoolean(CMPreferences.SettingRoll3d6, false);
            d.Click += (object sender, EventArgs e) => 
            {
                ISharedPreferences sp = context.GetCMPrefs();
                var ed = sp.Edit();
                ed.PutBoolean(CMPreferences.SettingRoll3d6, d.Checked);
                CombatState.use3d6 = d.Checked;
                ed.Commit();
            };

            CheckBox r = FindViewById<CheckBox>(Resource.Id.rollMonsterHPBox);
            r.Checked = p.GetBoolean(CMPreferences.SettingRollHP, false);
            r.Click += (object sender, EventArgs e) => 
            {
                ISharedPreferences sp = context.GetCMPrefs();
                var ed = sp.Edit();
                ed.PutBoolean(CMPreferences.SettingRollHP, r.Checked);
               
                ed.Commit();
            };


            View b = FindViewById<View>(Resource.Id.closeButton);
            b.Click += (object sender, EventArgs e) => 
            {
                Dismiss();
            };
        }
    }
}


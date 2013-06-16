
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
    [Activity (Label = "Monster Editor", Theme = "@android:style/Theme.Light.NoTitleBar")]   
    class MonsterEditorStatsActivity : MonsterEditorActivity
    {
        protected override int LayoutID
        {
            get
            {
                return Resource.Layout.MonsterEditorStats;
            }
        }

        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);

            ArrayAdapter<String> availableAdapter = new ArrayAdapter<string>
                (this, Android.Resource.Layout.SelectDialogItem);

            foreach (SkillValue v in EditMonster.SkillValueList)
            {
                availableAdapter.Add(v.FullName);
            }

            FindViewById<ListView>(Resource.Id.skillsListView).Adapter = availableAdapter;

        }
    }
}


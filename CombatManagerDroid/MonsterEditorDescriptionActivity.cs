
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
    [Activity (Label = "Monster Editor - Description", Theme = "@android:style/Theme.Light.NoTitleBar")]   
    class MonsterEditorDescriptionActivity : MonsterEditorActivity
    {
        protected override int LayoutID
        {
            get
            {
                return Resource.Layout.MonsterEditorDescription;
            }
        }

        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);

            AttachEditTextString(EditMonster, Resource.Id.environmentText, "Environment");
            AttachEditTextString(EditMonster, Resource.Id.organizationText, "Organization");
            AttachEditTextString(EditMonster, Resource.Id.treasureText, "Treasure");
            AttachEditTextString(EditMonster, Resource.Id.beforeCombatText, "BeforeCombat");
            AttachEditTextString(EditMonster, Resource.Id.duringCombatText, "DuringCombat");
            AttachEditTextString(EditMonster, Resource.Id.moraleText, "Morale");
            AttachEditTextString(EditMonster, Resource.Id.visualDescriptionText, "Description_Visual");
            AttachEditTextString(EditMonster, Resource.Id.descriptionText, "Description");

        }
    }
}


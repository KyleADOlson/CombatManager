
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
    [Activity (Label = "Monster Editor", Theme = "@android:style/Theme.Light.NoTitleBar")]   
    class MonsterEditorOffenseActivity : MonsterEditorActivity
    {
        protected override int LayoutID
        {
            get
            {
                return Resource.Layout.MonsterEditorOffense;
            }
        }

        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);

            //movement
            AttachEditTextInt (EditMonster.Adjuster, Resource.Id.speedText, "LandSpeed");
            AttachEditTextIntNull (EditMonster.Adjuster, Resource.Id.flyText, "FlySpeed");

            List<Tuple<int, string>> flyList = new List<Tuple<int, string>>();
            for (int i=0; i< CombatManager.Monster.FlyQualityList.Count; i++)
            {
                flyList.Add(new Tuple<int, string>(i, 
                                                   CombatManager.Monster.FlyQualityList[i]));
            }

            AttachButtonIntStringList(EditMonster.Adjuster, Resource.Id.flyQualityButton, "FlyQuality", flyList);


            
            AttachEditTextIntNull (EditMonster.Adjuster, Resource.Id.climbText, "ClimbSpeed");
            AttachEditTextIntNull (EditMonster.Adjuster, Resource.Id.burrowText, "BurrowSpeed");
            AttachEditTextIntNull (EditMonster.Adjuster, Resource.Id.swimText, "SwimSpeed");

            AttachEditTextInt (EditMonster.Adjuster, Resource.Id.spaceText, "Space");
            AttachEditTextInt (EditMonster.Adjuster, Resource.Id.reachText, "Reach");

            //attacks
            Button b = FindViewById<Button>(Resource.Id.meleeAttacksButton);
            b.Text = EditMonster.Melee;

            b = FindViewById<Button>(Resource.Id.rangedAttacksButton);
            b.Text = EditMonster.Ranged;

            //other offense text
            AttachEditTextString(EditMonster, Resource.Id.specialAttacksText, "SpecialAttacks");
            AttachEditTextString(EditMonster, Resource.Id.spellLikeAbilitiesText, "SpellLikeAbilities");
            AttachEditTextString(EditMonster, Resource.Id.spellsKnownText, "SpellsKnown");
            AttachEditTextString(EditMonster, Resource.Id.spellsPreparedText, "SpellsPrepared");

        }
    }
}


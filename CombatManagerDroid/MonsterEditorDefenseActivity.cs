
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
    class MonsterEditorDefenseActivity : MonsterEditorActivity
    {
        protected override int LayoutID
        {
            get
            {
                return Resource.Layout.MonsterEditorDefense;
            }
        }

        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);


            //hp section
            //hd button
            Button b = FindViewById<Button>(Resource.Id.hdButton);
            b.Text = EditMonster.Adjuster.HD.Text;
            b.Click += (object sender, EventArgs e) => 
            {
                HDDialog dialog = new HDDialog(this, EditMonster.Adjuster.HD.Text);
                dialog.OkClicked += (object s, EventArgs ea) => 
                {
                    EditMonster.Adjuster.HD = dialog.DieRoll;
                    b.Text = EditMonster.Adjuster.HD.Text;
                };
                dialog.Show();
            };

            AttachEditTextInt(EditMonster, Resource.Id.hpText, "HP");
            AttachEditTextString(EditMonster, Resource.Id.hpModsText, "HP_Mods");

            //ac section
            AttachEditTextInt (EditMonster, Resource.Id.acText, "FullAC");
            AttachEditTextInt (EditMonster, Resource.Id.touchText, "TouchAC");
            AttachEditTextInt (EditMonster, Resource.Id.flatFootedText, "FlatFootedAC");

            //ac mods section
            AttachEditTextInt (EditMonster.Adjuster, Resource.Id.armorText, "Armor");
            AttachEditTextInt (EditMonster.Adjuster, Resource.Id.dodgeText, "Dodge");
            AttachEditTextInt (EditMonster.Adjuster, Resource.Id.naturalText, "NaturalArmor");
            AttachEditTextInt (EditMonster.Adjuster, Resource.Id.deflectionText, "Deflection");
            AttachEditTextInt (EditMonster.Adjuster, Resource.Id.shieldText, "Shield");

            //save section
            AttachEditTextInt (EditMonster, Resource.Id.fortText, "Fort");
            AttachEditTextInt (EditMonster, Resource.Id.refText, "Ref");
            AttachEditTextInt (EditMonster, Resource.Id.willText, "Will");

            //text section
            AttachEditTextString(EditMonster, Resource.Id.defAbilitiesText, "DefensiveAbilities");
            AttachEditTextString(EditMonster, Resource.Id.drText, "DR");
            AttachEditTextString(EditMonster, Resource.Id.immuneText, "Immune");
            AttachEditTextString(EditMonster, Resource.Id.srText, "SR");
            AttachEditTextString(EditMonster, Resource.Id.resistText, "Resist");
            AttachEditTextString(EditMonster, Resource.Id.weaknessText, "Weaknesses");


        }
    }
}


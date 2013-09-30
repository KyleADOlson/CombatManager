
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

            CreateSkillsList();

            FindViewById<Button>(Resource.Id.addButton).Click += HandleAddClick;

            AttachEditTextInt(EditMonster.Adjuster, Resource.Id.baseAttackText, "BaseAtk");
            AttachEditTextInt(EditMonster, Resource.Id.cmbText, "CMB_Numeric");
            AttachEditTextInt(EditMonster, Resource.Id.cmdText, "CMD_Numeric");

            AttachEditTextString(EditMonster, Resource.Id.racialModsText, "RacialMods");
            AttachEditTextString(EditMonster, Resource.Id.auraText, "Aura");
            
            AttachEditTextString(EditMonster, Resource.Id.languagesText, "Languages");
            AttachEditTextString(EditMonster, Resource.Id.sqText, "SQ");
            AttachEditTextString(EditMonster, Resource.Id.gearText, "Gear");


        }


        void HandleAddClick (object sender, EventArgs e)
        {
            SkillPickerDialog dl = new SkillPickerDialog(this, EditMonster);
            dl.Show ();
            dl.DismissEvent += (object s, EventArgs ex) => 
                {
                CreateSkillsList();
            };
        }

        void CreateSkillsList()
        {
            LinearLayout l = FindViewById<LinearLayout>(Resource.Id.skillsLinearLayout);
            l.RemoveAllViews();

            foreach (SkillValue v in EditMonster.SkillValueList)
            {
                LinearLayout row = new LinearLayout(this);
                row.Orientation = Orientation.Horizontal;
                row.SetBackgroundColor(new Android.Graphics.Color(0xff, 0xff, 0xff));
                row.SetPadding(1, 2, 1, 2);

                TextView t = new TextView(this);
                t.Text = v.Name;
                t.SetTextSize(Android.Util.ComplexUnitType.Dip, 18);
                //t.SetBackgroundColor(new Android.Graphics.Color(0xff, 0x33, 0x33));
                t.SetMinimumWidth(130);

                
                row.AddView(t);
                if (Monster.SkillsDetails[v.Name].Subtypes != null)
                {
                    EditText et = new EditText(this);
                    et.Text = v.Subtype;
                    et.LayoutParameters = new LinearLayout.LayoutParams(
                        ViewGroup.LayoutParams.FillParent,
                        ViewGroup.LayoutParams.MatchParent, 1f);
                    row.AddView(et);
                }
                else
                {
                    t.LayoutParameters = new LinearLayout.LayoutParams(
                        ViewGroup.LayoutParams.FillParent,
                        ViewGroup.LayoutParams.MatchParent, 1f);

                }
                
                SkillValue s = v;


                EditText mod = new EditText(this);
                mod.MakeNumber();
                mod.Text = v.Mod.ToString();
                row.AddView(mod);
                mod.SetMinimumWidth(60);
                mod.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) => 

                {
                    HandleModChanged((EditText)sender, s);
                };


                ImageButton b = new ImageButton(this);
                b.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.redx));
               
                row.AddView(b);
                b.Tag = v.FullName;
                b.Click += (object sender, EventArgs e) => 
                {

                    AlertDialog.Builder bui = new AlertDialog.Builder(this);
                    bui.SetMessage("Remove skill?");
                    bui.SetPositiveButton("Yes", (a, x) => {
                        EditMonster.SkillValueDictionary.Remove(s.FullName);
                        EditMonster.UpdateSkillValueList();
                        CreateSkillsList ();
                    });
                    bui.SetNegativeButton("No", (a, x) => {});
                    bui.Show();
                };


                l.AddView(row);
            }


        
        }

        void HandleModChanged(EditText field, SkillValue val)
        {
            int value;
            if (int.TryParse(field.Text, out value))
            {
                EditMonster.ChangeSkill(val.Name, value - val.Mod);
            }

        }
    }
}


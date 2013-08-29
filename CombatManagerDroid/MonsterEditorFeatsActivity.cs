
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
using System.Text.RegularExpressions;
using Android.Text;

namespace CombatManagerDroid
{
    [Activity (Label = "Monster Editor", Theme = "@android:style/Theme.Light.NoTitleBar")]   
    class MonsterEditorFeatsActivity : MonsterEditorActivity
    {
        protected override int LayoutID
        {
            get
            {
                return Resource.Layout.MonsterEditorFeats;
            }
        }

        
        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);

            Button b = FindViewById<Button>(Resource.Id.addButton);
            b.Click += (object sender, EventArgs e) => 
            {
                FeatPickerDialog dl = new FeatPickerDialog(this, EditMonster);
                dl.Show ();
                dl.DismissEvent += (object s, EventArgs ex) => 
                {
                    CreateFeatsList();
                };
            };

            CreateFeatsList();

        }

        private void CreateFeatsList()
        {
            
            LinearLayout featsList = FindViewById<LinearLayout>(Resource.Id.featsLayout);

            featsList.RemoveAllViews();

            foreach (String s in EditMonster.FeatsList)
            {
                LinearLayout row = new LinearLayout(this);
                row.Orientation = Orientation.Horizontal;
                row.SetBackgroundColor(new Android.Graphics.Color(0xff, 0xff, 0xff));
                row.SetPadding(5, 2, 5, 2);
                var v = SplitFeat(s);
                TextView t = new TextView(this);
                t.SetTextSizeDip(18);
                t.LayoutParameters = new LinearLayout.LayoutParams(
                    ViewGroup.LayoutParams.FillParent,
                    ViewGroup.LayoutParams.MatchParent, 1f);

                
                if (v.Item2 == null)
                {
                    t.Text = v.Item1;
                }
                else
                {
                    t.TextFormatted = Html.FromHtml(v.Item1 + "<i>(" + v.Item2 + ")</i>");

                }
                
                row.AddView(t);

                ImageButton b = new ImageButton(this);
                b.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.redx));
              
                row.AddView(b);
                String feat = s;
                b.Click += (object sender, EventArgs e) => 
                {
                    EditMonster.RemoveFeat(feat);
                    CreateFeatsList();
                };


                featsList.AddView(row);
            }
        }

        public Tuple<string, string> SplitFeat(string feat)
        {
            Regex x = new Regex("(?<feat>[^\\()]+)(\\((?<sub>.+?)\\))?");
            Match m = x.Match(feat);
            string name = m.Groups["feat"].Value.Trim();
            string sub = null;
            if (m.Groups["sub"].Success)
            {
                sub = m.Groups["sub"].Value;
            }
            return new Tuple<string, string>(name, sub);
        }

       
    }
}


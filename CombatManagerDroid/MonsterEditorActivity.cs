
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
using System.Reflection;

namespace CombatManagerDroid
{		
    public abstract class MonsterEditorActivity : Activity
    {
        private static Monster _SourceMonster;
        private static Monster _EditMonster;

        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);

            OverridePendingTransition(Resource.Animation.fadein, Resource.Animation.fadeout);

            SetContentView (LayoutID);

            
            Window.SetSoftInputMode(SoftInput.StateHidden);

            FindViewById<Button>(Resource.Id.cancelButton).Click += delegate
            {
                CancelClicked();
            };
            FindViewById<Button>(Resource.Id.okButton).Click += delegate
            {
                OKClicked();
            };

            foreach (int x in new int [] {Resource.Id.mainTab,
                                Resource.Id.defenseTab,
                                Resource.Id.offenseTab,
                                Resource.Id.statsTab,
                                Resource.Id.featsTab,
                                Resource.Id.specialTab,
                Resource.Id.descriptionTab})
            {
                
                int a = x;
                Button b = FindViewById<Button>(x);
                b.Click += delegate
                {
                    TabClicked(a);
                };

                if (LayoutID == LayoutFromTab(x))
                {
                    b.Selected = true;
                    b.SetBackgroundDrawable(Resources.GetDrawable(Resource.Drawable.init_button));
                }
                else
                  
                {
                    b.SetBackgroundDrawable(Resources.GetDrawable(Resource.Drawable.init_button));
                }
            }
        }

        protected abstract int LayoutID
        {
            get;
        }

        void OKClicked()
        {
            _SourceMonster.CopyFrom(EditMonster);
            GoHome();
        }
        void CancelClicked()
        {
            GoHome();
        }

        void TabClicked(int tab)
        {
            if (LayoutID != LayoutFromTab(tab))
            {
 
                Finish();
                Intent intent = new Intent(this.BaseContext, (Java.Lang.Class) ActivityFromTab(tab).Class); 
                intent.AddFlags(ActivityFlags.NewTask); 
                StartActivity(intent);
            }
        }

        int LayoutFromTab(int tab)
        {
            switch (tab)
            {
            case Resource.Id.mainTab:
                return Resource.Layout.MonsterEditor;
                break;
            case Resource.Id.defenseTab:
                return Resource.Layout.MonsterEditorDefense;
                break;
            case Resource.Id.offenseTab:
                return Resource.Layout.MonsterEditorOffense;
                break;
            case Resource.Id.statsTab:
                return Resource.Layout.MonsterEditorStats;
                break;
            case Resource.Id.featsTab:
                return Resource.Layout.MonsterEditorFeats;
                break;
            case Resource.Id.specialTab:
                return Resource.Layout.MonsterEditorSpecial;
                break;
            case Resource.Id.descriptionTab:
                return Resource.Layout.MonsterEditorDescription;
                break;
            }
            return 0;
        }

        private MonsterEditorActivity ActivityFromTab(int tab)
        {
            switch (tab)
            {
            case Resource.Id.mainTab:
                return new MonsterEditorMainActivity();
                break;
            case Resource.Id.defenseTab:
                return new MonsterEditorDefenseActivity();
                break;
            case Resource.Id.offenseTab:
                return new MonsterEditorOffenseActivity();
                break;
            case Resource.Id.statsTab:
                return new MonsterEditorStatsActivity();
                break;
            case Resource.Id.featsTab:
                return new MonsterEditorFeatsActivity();
                break;
            case Resource.Id.specialTab:
                return new MonsterEditorSpecialActivity();
                break;
            case Resource.Id.descriptionTab:
                return new MonsterEditorDescriptionActivity();
                break;
            }
            return null;
        }


        void GoHome()
        {
            Finish();
        }

        public static Monster SourceMonster
        {
            get
            {
                return _SourceMonster;
            }
            set
            {
                _SourceMonster = value;
                _EditMonster = (Monster)_SourceMonster.Clone();
            }
        }
        protected static Monster EditMonster
        {
            get
            {
                return _EditMonster;
            }
        }

        
        protected void AttachEditTextString(object ob, int id, String property)
        {
            PropertyInfo pi = ob.GetType().GetProperty(property);
            
            EditText t = FindViewById<EditText>(id);
            t.Text = (string)pi.GetGetMethod().Invoke(ob, new object[]{});
            
            t.TextChanged += (sender, e) => {pi.GetSetMethod().Invoke(ob, new object[]{t.Text});};
        }

        protected void AttachButtonStringList(object ob, int id, String property, List<String> options)
        {
            AttachButtonStringList(ob, id, property, options, "{0}");
        }

        protected void AttachButtonStringList(object ob, int id, String property, List<String> options, string format)
        {
            
            PropertyInfo pi = ob.GetType().GetProperty(property);

            Button t = FindViewById<Button>(id);
            String text = (string)pi.GetGetMethod().Invoke(ob, new object[]{});
            t.Text = String.Format(format, text);
            t.Click += (object sender, EventArgs e) => {


                AlertDialog.Builder builderSingle = new AlertDialog.Builder(this);

                builderSingle.SetTitle(property);
                ArrayAdapter<String> arrayAdapter = new ArrayAdapter<String>(
                    this,
                    Android.Resource.Layout.SelectDialogItem);
                arrayAdapter.AddAll(options);

               
                builderSingle.SetAdapter (arrayAdapter, (se, ev)=> {
                    string val = arrayAdapter.GetItem(ev.Which);
                    t.Text = String.Format(format, val);
                    pi.GetSetMethod().Invoke(ob, new object[]{val});

                });
               
                builderSingle.Show();
            };
        }

        protected void AttachButtonIntStringList(object ob, int id, String property, List<Tuple<int, string>> options)
        {

            PropertyInfo pi = ob.GetType().GetProperty(property);
            
            Button t = FindViewById<Button>(id);
            int val = (int)pi.GetGetMethod().Invoke(ob, new object[]{});
            t.Text = options.Find(a => a.Item1 == val).Item2;
            t.Click += (object sender, EventArgs e) => {

                AlertDialog.Builder builderSingle = new AlertDialog.Builder(this);
                
                builderSingle.SetTitle(property);
                ArrayAdapter<String> arrayAdapter = new ArrayAdapter<String>(
                    this,
                    Android.Resource.Layout.SelectDialogItem);
                arrayAdapter.AddAll(new List<string>(from x in options select x.Item2));
                

                builderSingle.SetAdapter (arrayAdapter, (se, ev)=> {
                    string item = arrayAdapter.GetItem(ev.Which);
                    t.Text = item;
                    pi.GetSetMethod().Invoke(ob, new object[]{options.Find (a=> a.Item2 == item).Item1});
                    
                });
                
                builderSingle.Show();
            };
        }

        protected void AttachEditTextIntNull(object ob, int id, String property)
        {
            PropertyInfo pi = ob.GetType().GetProperty(property);
            
            EditText t = FindViewById<EditText>(id);
            int? inputVal = (int?)pi.GetGetMethod().Invoke(ob, new object[]{});
            if (inputVal == null)
            {
                t.Text = "";
            }
            else
            {
                t.Text = inputVal.ToString();
            }

            t.TextChanged += (sender, e) => {

                int? val = null;
                int parseVal = 0;
                if (int.TryParse(t.Text, out parseVal))
                {
                    val = parseVal;
                }

                pi.GetSetMethod().Invoke(ob, new object[]{val});
            
                
            };
        }

        
        protected void AttachEditTextInt(object ob, int id, String property)
        {
            PropertyInfo pi = ob.GetType().GetProperty(property);
            
            EditText t = FindViewById<EditText>(id);
            int inputVal = (int)pi.GetGetMethod().Invoke(ob, new object[]{});

            t.Text = inputVal.ToString();

            
            t.TextChanged += (sender, e) => {

                int parseVal = 0;
                if (int.TryParse(t.Text, out parseVal))
                {
                   
                    pi.GetSetMethod().Invoke(ob, new object[]{parseVal});
                }
            };
        }

    }
}


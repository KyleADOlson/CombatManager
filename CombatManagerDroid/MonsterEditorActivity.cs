
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
using Android.Support.V4.Content;

namespace CombatManagerDroid
{		
    public abstract class MonsterEditorActivity : Activity
    {
        private static Monster _SourceMonster;
        private static Monster _EditMonster;
        private static bool _DBMonster;

        int[] buttonResources = new int[] {Resource.Id.mainTab,
                                Resource.Id.defenseTab,
                                Resource.Id.offenseTab,
                                Resource.Id.statsTab,
                                Resource.Id.featsTab,
                                Resource.Id.specialTab,
                Resource.Id.descriptionTab };
        int[] imageButtonResources = new int[] {Resource.Id.mainImageTab,
                                Resource.Id.defenseImageTab,
                                Resource.Id.offenseImageTab,
                                Resource.Id.statsImageTab,
                                Resource.Id.featsImageTab,
                                Resource.Id.specialImageTab,
                                Resource.Id.descriptionImageTab };


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


            foreach (int x in GetCurrentButtons())
            {
                
                int a = x;
                View b = FindViewById<View>(x);
                b.Click += delegate
                {
                    TabClicked(a);
                };

                if (LayoutID == LayoutFromTab(x))
                {
                    b.Selected = true;
                    b.Background = (ContextCompat.GetDrawable(this, Resource.Drawable.init_button));
                }
                else
                  
                {
                    b.Background = (ContextCompat.GetDrawable(this, Resource.Drawable.init_button));
                }
            }
        }

        private int[] GetCurrentButtons()
        {

            View v = FindViewById<View>(Resource.Id.mainTab);
            

            if (v != null)
            {
                return buttonResources;
            }
            else
            {
                return imageButtonResources;
            }
        }

        protected abstract int LayoutID
        {
            get;
        }

        void OKClicked()
        {
            _SourceMonster.CopyFrom(EditMonster);

            if (DBMonster)
            {
                MonsterDB.DB.UpdateMonster(_SourceMonster);
            };

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
                case Resource.Id.mainImageTab:
                    return Resource.Layout.MonsterEditor;
                case Resource.Id.defenseTab:
                case Resource.Id.defenseImageTab:
                    return Resource.Layout.MonsterEditorDefense;
                case Resource.Id.offenseTab:
                case Resource.Id.offenseImageTab:
                    return Resource.Layout.MonsterEditorOffense;
                case Resource.Id.statsTab:
                case Resource.Id.statsImageTab:
                    return Resource.Layout.MonsterEditorStats;
                case Resource.Id.featsTab:
                case Resource.Id.featsImageTab:
                    return Resource.Layout.MonsterEditorFeats;
                case Resource.Id.specialTab:
                case Resource.Id.specialImageTab:
                    return Resource.Layout.MonsterEditorSpecial;
                case Resource.Id.descriptionTab:
                case Resource.Id.descriptionImageTab:
                    return Resource.Layout.MonsterEditorDescription;
            }
            return 0;
        }

        private MonsterEditorActivity ActivityFromTab(int tab)
        {
            switch (tab)
            {
                case Resource.Id.mainTab:
                case Resource.Id.mainImageTab:
                    return new MonsterEditorMainActivity();
                case Resource.Id.defenseTab:
                case Resource.Id.defenseImageTab:
                    return new MonsterEditorDefenseActivity();
                case Resource.Id.offenseTab:
                case Resource.Id.offenseImageTab:
                    return new MonsterEditorOffenseActivity();
                case Resource.Id.statsTab:
                case Resource.Id.statsImageTab:
                    return new MonsterEditorStatsActivity();
                case Resource.Id.featsTab:
                case Resource.Id.featsImageTab:
                    return new MonsterEditorFeatsActivity();
                case Resource.Id.specialTab:
                case Resource.Id.specialImageTab:
                    return new MonsterEditorSpecialActivity();
                case Resource.Id.descriptionTab:
                case Resource.Id.descriptionImageTab:
                    return new MonsterEditorDescriptionActivity();
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

        public static bool DBMonster
        {
            get => _DBMonster;
            set => _DBMonster = value;
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



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
    class SkillPickerDialog : Dialog
    {
        Monster _Monster;

        public SkillPickerDialog(Context context, Monster m) : base (context)
        {
            _Monster = m;

            RequestWindowFeature((int)WindowFeatures.NoTitle);
            
            SetContentView(Resource.Layout.SkillPickerDialog);
            SetCanceledOnTouchOutside(true);

            ((Button)FindViewById(Resource.Id.cancelButton)).Click += 
            (object sender, EventArgs e) => {Dismiss();};
            
            ((Button)FindViewById(Resource.Id.addButton)).Click += 
            (object sender, EventArgs e) => {AddClicked ();};

            ((ListView)FindViewById(Resource.Id.skillListView)).ItemSelected += ItemSelected;

            BuildAdapter();

        }

        void BuildAdapter()
        {


            var list = new List<Tuple<String, Object>>();
           
            foreach (var v in Monster.SkillsDetails.Values)
            {
                if (v.Subtypes != null || !_Monster.SkillValueDictionary.ContainsKey(v.Name))
                {
                    list.Add (new Tuple<string, object>(v.Name, v));
                }
            }

            TextSelectionAdapater ad  = new TextSelectionAdapater(

                Context, list);

            ((ListView)FindViewById(Resource.Id.skillListView)).Adapter = ad;

        }

        void AddClicked()
        {
            TextSelectionAdapater ad  = (TextSelectionAdapater)((ListView)FindViewById(Resource.Id.skillListView)).Adapter;
            CombatManager.Monster.SkillInfo info = (CombatManager.Monster.SkillInfo)ad.SelectedObject;

            if ( info != null)
            {
                if (info.Subtypes == null)
                {
                    _Monster.AddOrChangeSkill(info.Name, 0);
                    Dismiss();
                }
                else
                {
                    EditText st = (EditText)FindViewById(Resource.Id.subtypeText);
                    if (st.Text.Trim().Length > 0)
                    {
                        
                        _Monster.AddOrChangeSkill(info.Name, st.Text.Trim(), 0);
                        Dismiss();
                    }
                }
            }

        }

        void ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            //if (Monster.SkillsDetails[
        }

        EditText SubtypeText
        {
            get
            {
                return((EditText)(FindViewById(Resource.Id.subtypeText)));
            }
        }

    }
}


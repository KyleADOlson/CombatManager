
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
    class FeatPickerDialog : Dialog
    {
        Monster _Monster;

        public FeatPickerDialog(Context context, Monster m) : base (context)
        {
            _Monster = m;

            RequestWindowFeature((int)WindowFeatures.NoTitle);
            
            SetContentView(Resource.Layout.FeatPickerDialog);
            SetCanceledOnTouchOutside(true);
            
            ((Button)FindViewById(Resource.Id.cancelButton)).Click += 
            (object sender, EventArgs e) => {Dismiss();};
            
            ((Button)FindViewById(Resource.Id.addButton)).Click += 
            (object sender, EventArgs e) => {AddClicked ();};


            ((EditText)FindViewById(Resource.Id.filterText)).AfterTextChanged += 
                (object sender, Android.Text.AfterTextChangedEventArgs e) => 
            {
                BuildAdapter();
            };

             BuildAdapter();
        }

        void AddClicked()
        {
            _Monster.AddFeat(CurrentFeat);
            Dismiss();
        }

        void BuildAdapter()
        {
            var list = new List<Tuple<String, Object>>();

            String filterText = ((EditText)FindViewById(Resource.Id.filterText)).Text.Trim().ToUpper();
            
            foreach (var v in Feat.Feats)
            {
                if (filterText.Length == 0 || v.Name.ToUpper().Contains(filterText))
                {
                    list.Add (new Tuple<string, object>(v.Name, v));
                }
            }
            
            TextSelectionAdapater ad  = new TextSelectionAdapater(
                
                Context, list);
            
            ((ListView)FindViewById(Resource.Id.featsList)).Adapter = ad;
            
        }

        string CurrentFeat
        {
            get
            {
                TextSelectionAdapater ad  = (TextSelectionAdapater)((ListView)FindViewById(Resource.Id.featsList)).Adapter;
                Feat ft = (Feat)ad.SelectedObject;
                String sub = ((EditText)FindViewById(Resource.Id.subtypeText)).Text;

                String feat = MakeFeat(ft.Name, sub);

                return feat;

            }
        }
        
        
        static String MakeFeat(string feat, string sub)
        {
            if (sub == null || sub.Trim().Length == 0)
            {
                return feat.Trim();
            }
            else
            {
                return feat.Trim() + " (" + sub.Trim() + ")";
            }
        }



    }
}


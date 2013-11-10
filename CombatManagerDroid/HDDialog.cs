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
    class HDDialog : Dialog
    {
        DieRoll _DieRoll;

        public event EventHandler OkClicked;

        public HDDialog(Context context, string HD) : base (context)
        {
            _DieRoll = DieRoll.FromString(HD);

            RequestWindowFeature((int)WindowFeatures.NoTitle);

            SetContentView(Resource.Layout.HDDialog);
            SetCanceledOnTouchOutside(true);

            Window.SetSoftInputMode(SoftInput.StateHidden);

            EditText et = FindViewById<EditText>(Resource.Id.modText);
            et.Text = _DieRoll.mod.ToString();
            et.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) => 
            {
                int val;
                if (int.TryParse(et.Text, out val))
                {
                    _DieRoll.mod = val;
                }
            };

            ((Button)FindViewById(Resource.Id.cancelButton)).Click += 
                (object sender, EventArgs e) => {Dismiss();};

            ((Button)FindViewById(Resource.Id.okButton)).Click += 
            (object sender, EventArgs e) => {Dismiss(); FireOk ();};

            SetupDice();

        }

        void SetupDice()
        {
            
            SetDieText();

            List<int> addButtons = new List<int>() {Resource.Id.add4Button,
                Resource.Id.add6Button,
                Resource.Id.add8Button,
                Resource.Id.add10Button,
                Resource.Id.add12Button,
                Resource.Id.add20Button
            };

            foreach (int r in addButtons)
            {
                Button b = FindViewById<Button>(r);
                b.Click += (object sender, EventArgs e) => 
                {
                    int val = int.Parse((String)b.Tag);
                    _DieRoll.AddDie(val);
                    SetDieText();
                };
            }

            
            List<int> subtractButtons = new List<int>() {Resource.Id.subtract4Button,
                Resource.Id.subtract6Button,
                Resource.Id.subtract8Button,
                Resource.Id.subtract10Button,
                Resource.Id.subtract12Button,
                Resource.Id.subtract20Button
            };
            
            foreach (int r in subtractButtons)
            {
                Button b = FindViewById<Button>(r);
                b.Click += (object sender, EventArgs e) => 
                {
                    int val = int.Parse((String)b.Tag);
                    _DieRoll.RemoveDie(new DieStep(1, val));
                    SetDieText();
                };
            }

        }

        public DieRoll DieRoll
        {
            get
            {
                return _DieRoll;
            }
        }


        void SetDieText()
        {
            TextView t;
            t = FindViewById<TextView>(Resource.Id.d4Text);
            t.Text = _DieRoll.DieCount(4) + "d4";
            t = FindViewById<TextView>(Resource.Id.d6Text);
            t.Text = _DieRoll.DieCount(6) + "d6";
            t = FindViewById<TextView>(Resource.Id.d8Text);
            t.Text = _DieRoll.DieCount(8) + "d8";
            t = FindViewById<TextView>(Resource.Id.d10Text);
            t.Text = _DieRoll.DieCount(10) + "d10";
            t = FindViewById<TextView>(Resource.Id.d12Text);
            t.Text = _DieRoll.DieCount(12) + "d12";
            t = FindViewById<TextView>(Resource.Id.d20Text);
            t.Text = _DieRoll.DieCount(20) + "d20";
        }

        void FireOk()
        {
            if (OkClicked != null)
            {
                OkClicked(this, new EventArgs());
            }
        }
    }
}


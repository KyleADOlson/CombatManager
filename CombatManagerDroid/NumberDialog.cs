
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
    public class NumberDialog : Dialog
    {
        int _Value;
        public NumberDialog(String property, Object ob, Context context) : this(property, property, ob, context)
        {

        }

        public NumberDialog(String property, String title, Object ob, Context context) : base (context)
        {

            SetContentView(Resource.Layout.NumberDialog);
            SetTitle(title);
            SetCanceledOnTouchOutside(true);

            var prop = ob.GetType().GetProperty(property);
            _Value = (int)prop.GetGetMethod().Invoke(ob, new object[]{}); 

            
            SetDialogNumber();
            
            ((Button)FindViewById(Resource.Id.okButton)).Click += 
            delegate {
                
                prop.GetSetMethod().Invoke(ob, new object[] {
                    _Value});
                Dismiss();
            };
            
            
            ((Button)FindViewById(Resource.Id.cancelButton)).Click += 
            delegate {Dismiss();  };

            foreach (int x in new int[] {Resource.Id.column1, Resource.Id.column2, 
                Resource.Id.column3, Resource.Id.column4})
            {
                View v = FindViewById(x);
                
                foreach (int y in new int[] {Resource.Id.button1, Resource.Id.button2, 
                    Resource.Id.button3, Resource.Id.button4})
                {
                    Button b = v.FindViewById<Button>(y);
                    
                    b.Click += (object sender, EventArgs e) => 
                    {
                        int diff = int.Parse(b.Tag.ToString());
                        _Value += diff;
                        SetDialogNumber();
                    };
                }
                
            }


        }
        
        private void SetDialogNumber()
        {
            int kVal = ((_Value / 1000) % 10);
            string kText = kVal.ToString();
            int cVal = ((Math.Abs(_Value) / 100) % 10);
            string cText = cVal.ToString();
            int dVal = ((Math.Abs(_Value)/10)%10);
            string dText = dVal.ToString();
            int iVal = (Math.Abs(_Value) % 10);
            string iText = iVal.ToString();
                
            string dashText = "";
            if (_Value < 0)
            {
                dashText = "-";
            }

            int absVal = Math.Abs(_Value);
            if (absVal < 10)
            {
                dText = "";
                iText = dashText + iText;
                dashText = "";
            }
            if (absVal < 100)
            {
                cText = "";
                dText = dashText + dText;
                dashText = "";
            }
            if (absVal < 1000)
            {
                kText = "";
                cText = dashText + cText;
                dashText = "";
            }



            ((TextView)FindViewById(Resource.Id.textView1)).Text = kText;
            ((TextView)FindViewById(Resource.Id.textView2)).Text = cText;
            ((TextView)FindViewById(Resource.Id.textView3)).Text = dText;
            ((TextView)FindViewById(Resource.Id.textView4)).Text = iText;
            
        }
    }
}


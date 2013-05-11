
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

        public NumberDialog(String property, Object ob, Context context) : base (context)
        {
            SetContentView(Resource.Layout.NumberDialog);
            SetTitle(property);

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
            String kText = "";
            if ((_Value < 0) && (_Value > -1000))
            {
                kText += "-";
            }
            kText += ((_Value/1000)%10).ToString();
            ((TextView)FindViewById(Resource.Id.textView1)).Text = kText;
            ((TextView)FindViewById(Resource.Id.textView2)).Text = ((Math.Abs(_Value)/100)%10).ToString();
            ((TextView)FindViewById(Resource.Id.textView3)).Text = ((Math.Abs(_Value)/10)%10).ToString();
            ((TextView)FindViewById(Resource.Id.textView4)).Text = (Math.Abs(_Value)%10).ToString();
            
        }
    }
}


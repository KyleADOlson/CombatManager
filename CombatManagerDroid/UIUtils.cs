
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
    static class UIUtils
    {
        public static void ShowTextDialog(String property, Object ob, Context context)
        {


            Dialog d = new Dialog(context);
            d.SetContentView(Resource.Layout.TextDialog);
            d.SetTitle(property);

            var prop = ob.GetType().GetProperty(property);
            String val = (string)prop.GetGetMethod().Invoke(ob, new object[]{}); 

            ((EditText)d.FindViewById(Resource.Id.textField)).Text = val;

            ((Button)d.FindViewById(Resource.Id.okButton)).Click += 
                delegate {
                    
                        prop.GetSetMethod().Invoke(ob, new object[] {
                    ((EditText)d.FindViewById(Resource.Id.textField)).Text});
                        d.Dismiss();
                    };
            
            ((Button)d.FindViewById(Resource.Id.cancelButton)).Click += 
            delegate {d.Dismiss();  };

            


            d.Show();
        }

        public static View GetItemViewAt(this ListView view, int index)
        {
            View v = view.GetChildAt(index - 
              view.FirstVisiblePosition);

            return v;
        }



    }
}


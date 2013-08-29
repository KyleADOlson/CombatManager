
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

        public static Button GetButton(this Activity x, int resource)
        {
            return (Button)x.FindViewById (resource);
        }
        public static Button GetButton(this Dialog x, int resource)
        {
            return (Button)x.FindViewById (resource);
        }
        public static Button GetButton(this View x, int resource)
        {
            return (Button)x.FindViewById (resource);
        }
        
        public static EditText GetEditText(this Activity x, int resource)
        {
            return (EditText)x.FindViewById (resource);
        }
        public static EditText GetEditText(this Dialog x, int resource)
        {
            return (EditText)x.FindViewById (resource);
        }
        public static EditText GetEditText(this View x, int resource)
        {
            return (EditText)x.FindViewById (resource);
        }

        public static void MakeNumber(this EditText et)
        {
            et.InputType = Android.Text.InputTypes.ClassNumber | Android.Text.InputTypes.NumberFlagSigned;

        }

        public static void SetTextSizeDip(this TextView t, float size)
        {

           t.SetTextSize(Android.Util.ComplexUnitType.Dip, size);
        }

    }
}


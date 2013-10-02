
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
using System.Reflection;

namespace CombatManagerDroid
{
    static class UIUtils
    {

        public static void ShowTextDialog(String property, Object ob, Context context)
        {
            ShowTextDialog(property, ob, context, false);

        }
        public static void ShowTextDialog(String property, Object ob, Context context, bool multiline)
        {


            Dialog d = new Dialog(context);
            d.SetContentView(multiline?Resource.Layout.MultilineTextDialog:Resource.Layout.TextDialog);
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

        public static void AttachButtonStringList(View v, object ob, int id, String property, List<String> options)
        {
            AttachButtonStringList(v, ob, id, property, options, "{0}");
        }
        
        public static void AttachButtonStringList(View v, object ob, int id, String property, List<String> options, string format)
        {

            PropertyInfo pi = ob.GetType().GetProperty(property);
            
            Button t = v.FindViewById<Button>(id);
            String text = (string)pi.GetGetMethod().Invoke(ob, new object[]{});
            t.Text = String.Format(format, text);
            t.Click += (object sender, EventArgs e) => {
                
                
                AlertDialog.Builder builderSingle = new AlertDialog.Builder(v.Context);
                
                builderSingle.SetTitle(property);
                ArrayAdapter<String> arrayAdapter = new ArrayAdapter<String>(
                    v.Context,
                    Android.Resource.Layout.SelectDialogSingleChoice);
                arrayAdapter.AddAll(options);
                
                
                builderSingle.SetAdapter (arrayAdapter, (se, ev)=> {
                    string val = arrayAdapter.GetItem(ev.Which);
                    t.Text = String.Format(format, val);
                    pi.GetSetMethod().Invoke(ob, new object[]{val});
                    
                });
                
                builderSingle.Show();
            };
        }

    }
}


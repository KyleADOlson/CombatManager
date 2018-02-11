
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
using Android.Support.V4.Content;
using Android.Content.Res;
using System.ComponentModel;

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
            d.SetCanceledOnTouchOutside(true);
            d.SetContentView(multiline ? Resource.Layout.MultilineTextDialog : Resource.Layout.TextDialog);
            d.SetTitle(property);

            var prop = ob.GetType().GetProperty(property);
            String val = (string)prop.GetGetMethod().Invoke(ob, new object[] { });

            ((EditText)d.FindViewById(Resource.Id.textField)).Text = val;

            ((Button)d.FindViewById(Resource.Id.okButton)).Click +=
                delegate
                {

                    prop.GetSetMethod().Invoke(ob, new object[] {
                    ((EditText)d.FindViewById(Resource.Id.textField)).Text});
                    d.Dismiss();
                };

            ((Button)d.FindViewById(Resource.Id.cancelButton)).Click +=
            delegate { d.Dismiss(); };




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
            return (Button)x.FindViewById(resource);
        }
        public static Button GetButton(this Dialog x, int resource)
        {
            return (Button)x.FindViewById(resource);
        }
        public static Button GetButton(this View x, int resource)
        {
            return (Button)x.FindViewById(resource);
        }

        public static EditText GetEditText(this Activity x, int resource)
        {
            return (EditText)x.FindViewById(resource);
        }
        public static EditText GetEditText(this Dialog x, int resource)
        {
            return (EditText)x.FindViewById(resource);
        }
        public static EditText GetEditText(this View x, int resource)
        {
            return (EditText)x.FindViewById(resource);
        }

        public static void MakeNumber(this EditText et)
        {
            et.InputType = Android.Text.InputTypes.ClassNumber | Android.Text.InputTypes.NumberFlagSigned;

        }

        public static void SetTextSizeDip(this TextView t, float size)
        {

            t.SetTextSize(Android.Util.ComplexUnitType.Dip, size);
        }

        public static void AttachButtonStringList(View v, object ob, int id, String property, List<String> options, string format = "{0}")
        {

            Button t = v.FindViewById<Button>(id);
            t.AttachButtonStringList(ob, property, options, format);
        }

        public static void AttachButtonStringList(this Button t, object ob, String property, IEnumerable<String> options, string format = "{0}")
        {

            PropertyInfo pi = ob.GetType().GetProperty(property);

            String text = (string)pi.GetGetMethod().Invoke(ob, new object[] { });
            t.Text = String.Format(format, text);
            t.Click += (object sender, EventArgs e) =>
            {


                AlertDialog.Builder builderSingle = new AlertDialog.Builder(t.Context);

                builderSingle.SetTitle(property);
                ArrayAdapter<String> arrayAdapter = new ArrayAdapter<String>(
                    t.Context,
                    Android.Resource.Layout.SelectDialogSingleChoice);
                arrayAdapter.AddAll(new List<string>(options));


                builderSingle.SetAdapter(arrayAdapter, (se, ev) =>
                {
                    string val = arrayAdapter.GetItem(ev.Which);
                    t.Text = String.Format(format, val);
                    pi.GetSetMethod().Invoke(ob, new object[] { val });

                });

                builderSingle.Show();
            };
        }

        public delegate void ListPopoverResponseHandler(int item);

        public static void ShowListPopover(View v, String title, List<String> options, ListPopoverResponseHandler handler)
        {

            AlertDialog.Builder builderSingle = new AlertDialog.Builder(v.Context);

            builderSingle.SetTitle(title);
            ArrayAdapter<String> arrayAdapter = new ArrayAdapter<String>(
                v.Context,
                Android.Resource.Layout.SelectDialogItem);
            arrayAdapter.AddAll(options);


            builderSingle.SetAdapter(arrayAdapter, (se, ev) =>
            {
                string val = arrayAdapter.GetItem(ev.Which);
                handler?.Invoke(ev.Which);

            });

            builderSingle.Show();
        }


        public static void ShowOKCancelDialog(this Context context, String message, Action okAction, Action cancelAction = null)
        {
            AlertDialog.Builder bui = new AlertDialog.Builder(context);
            bui.SetMessage(message);
            bui.SetPositiveButton("OK", (a, x) =>
            {
                okAction?.Invoke();
            });
            bui.SetNegativeButton("Cancel", (a, x) => { cancelAction?.Invoke(); });
            bui.Show();

        }

        public static void ShowOKDialog(this Context context, String message, Action okAction = null)
        {
            AlertDialog.Builder bui = new AlertDialog.Builder(context);
            bui.SetMessage(message);
            bui.SetPositiveButton("OK", (a, x) =>
            {

                okAction?.Invoke();
            });

        }

        public static void SetLeftDrawableResource(this Button b, int resource)
        {

            b.SetCompoundDrawablesWithIntrinsicBounds(ContextCompat.GetDrawable(b.Context, resource), null, null, null);
        }

        public static bool IsTablet(this Context context)
        {
            ScreenLayout screenSize = context.Resources.Configuration.ScreenLayout &
                    ScreenLayout.SizeMask;

            return screenSize == ScreenLayout.SizeLarge || screenSize == ScreenLayout.SizeXlarge;
        }

        public static bool IsOrientationPortrait(this Context context)
        {
            var ori = context.Resources.Configuration.Orientation;

            return (ori == Android.Content.Res.Orientation.Portrait);

        }

        public static bool IsOrientationLandscape(this Context context)
        {
            var ori = context.Resources.Configuration.Orientation;

            return (ori == Android.Content.Res.Orientation.Landscape);

        }

        public static void AttachEditTextString(this EditText t, object ob, String property)
        {
            PropertyInfo pi = ob.GetType().GetProperty(property);
            
            t.Text = (string)pi.GetGetMethod().Invoke(ob, new object[] { });

            t.TextChanged += (sender, e) => { pi.GetSetMethod().Invoke(ob, new object[] { t.Text }); };
        }

        public static void AttachEditTextIntNull(this EditText t, object ob, String property)
        {
            PropertyInfo pi = ob.GetType().GetProperty(property);
            
            int? inputVal = (int?)pi.GetGetMethod().Invoke(ob, new object[] { });
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

                pi.GetSetMethod().Invoke(ob, new object[] { val });


            };
        }


        public static void AttachEditTextInt(this EditText t, object ob, String property)
        {
            PropertyInfo pi = ob.GetType().GetProperty(property);
            
            int inputVal = (int)pi.GetGetMethod().Invoke(ob, new object[] { });

            t.Text = inputVal.ToString();


            t.TextChanged += (sender, e) => {

                int parseVal = 0;
                if (int.TryParse(t.Text, out parseVal))
                {

                    pi.GetSetMethod().Invoke(ob, new object[] { parseVal });
                }
            };
        }

        public static bool StringEmptyOrNull(this String s)
        {
            return s == null || s.Length == 0;
        }

        public static void AttachBool(this CheckBox t, object ob, String property)
        {
            PropertyInfo pi = ob.GetType().GetProperty(property);

            bool inputVal = (bool)pi.GetGetMethod().Invoke(ob, new object[] { });

            t.Checked = inputVal;
            
            t.CheckedChange += (sender, e) => 
            {
                pi.GetSetMethod().Invoke(ob, new object[] { t.Checked });
            };
        }

        public static void AttachTextCombo(this Button b, object ob, String property, IEnumerable<String> options, String title = null)
        {
            b.Text = GetProperty<string>(ob, property);
            b.Click += (sender, e) =>
            {
                ShowTextComboDialog(property, ob, options, b.Context, title);
            };
            if (ob is INotifyPropertyChanged)
            {
                INotifyPropertyChanged pc = ob as INotifyPropertyChanged;
                pc.PropertyChanged += (sender, e) =>
                {
                    b.Text = GetProperty<string>(ob, property);
                };
            }

        }


        public static void ShowTextComboDialog(String property, Object ob, IEnumerable<String> options, Context context, String title = null)
        {


            Dialog d = new Dialog(context);
            d.SetCanceledOnTouchOutside(true);
            d.SetContentView(Resource.Layout.TextComboDialog);
            d.SetTitle(title ?? property);

            String val = GetProperty<String>(ob, property);

            EditText edit = ((EditText)d.FindViewById(Resource.Id.textField));
            edit.Text = val;
            edit.RequestFocus();

            ((Button)d.FindViewById(Resource.Id.okButton)).Click +=
                delegate {

                    SetProperty<String>(ob, property,
                    ((EditText)d.FindViewById(Resource.Id.textField)).Text?.Trim());
                    d.Dismiss();
                };

            ((Button)d.FindViewById(Resource.Id.cancelButton)).Click +=
            delegate { d.Dismiss(); };


            ListView view = ((ListView)d.FindViewById(Resource.Id.itemListView));
            List<String> optionsList = new List<string>(options);
            view.Adapter = new ArrayAdapter<String>(context, Android.Resource.Layout.SimpleListItem1, new List<string>(options));
            view.ItemClick += (sender, e) =>
            {
                if (e.Position >= 0 && e.Position < optionsList.Count)
                    edit.Text = optionsList[e.Position];

            };


            d.Window.SetSoftInputMode(SoftInput.StateVisible);
            d.Show();
        }

        public static T GetProperty<T>(this Object ob, String property)
        {

            var prop = ob.GetType().GetProperty(property);
            return (T)prop.GetGetMethod().Invoke(ob, new object[] { });
        }
        public static void SetProperty<T>(this Object ob, String property, T value)
        {

            var prop = ob.GetType().GetProperty(property);
            prop.GetSetMethod().Invoke(ob, new object[] { value });
        }

    }
}


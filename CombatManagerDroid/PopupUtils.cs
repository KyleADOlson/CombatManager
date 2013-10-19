using System;using System;
using System.Reflection;
using Android.Widget;
using Android.App;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Graphics.Drawables;

namespace CombatManagerDroid
{

    
    public delegate  void ActionClickedDelegate(View b, int index, string value);

    public static class PopupUtils
    {

        public static void AttachButtonStringPopover(String title, View t, List<String> options, int startIndex, ActionClickedDelegate callback)
        {
            AttachButtonStringPopover(title, t, options, "{0}", startIndex, callback);
        }

        public static void AttachButtonStringPopover(String title, View t,  List<String> options, string format, int startIndex, ActionClickedDelegate callback)
        {
            String text = "";
            if (startIndex >= 0)
            {

                t.Tag = options[startIndex];
            }
            if (t.Tag != null)
            {
                text = t.Tag.ToString();
            }

            if (t is TextView)
            {
                TextView tv = (TextView)t;
                tv.Text = String.Format(format, text);
            }
            t.Click += (object sender, EventArgs e) => {


                AlertDialog.Builder builderSingle = new AlertDialog.Builder(t.Context);

                builderSingle.SetTitle(title);
                ArrayAdapter<String> arrayAdapter = new ArrayAdapter<String>(t.Context,
                    Android.Resource.Layout.SelectDialogItem);
                arrayAdapter.AddAll(options);


                builderSingle.SetAdapter (arrayAdapter, (se, ev)=> {
                    string val = arrayAdapter.GetItem(ev.Which);
                    if (t is TextView)
                    {
                        TextView tv = (TextView)t;
                        tv.Text = String.Format(format, val);
                    
                    }
                    t.Tag = val;

                    
                    if ( callback != null)
                    {
                        callback(t, ev.Which, val);
                    }

                });


                builderSingle.Show();
            };
        }

        public static void AttachSimpleStringPopover(String title, View t,  List<String> options, ActionClickedDelegate callback)
        {


            t.Click += (object sender, EventArgs e) => {


                AlertDialog.Builder builderSingle = new AlertDialog.Builder(t.Context);

                builderSingle.SetTitle(title);
                ArrayAdapter<String> arrayAdapter = new ArrayAdapter<String>(t.Context,
                                                                             Android.Resource.Layout.SelectDialogItem);
                arrayAdapter.AddAll(options);


                builderSingle.SetAdapter (arrayAdapter, (se, ev)=> {
                    string val = arrayAdapter.GetItem(ev.Which);
                    t.Tag = val;


                    if ( callback != null)
                    {
                        callback(t, ev.Which, val);
                    }

                });


                builderSingle.Show();
            };
        }

        public static Drawable NamedImage(Context con, string name)
        {
            if (name == null || name == "")
            {
                return null;
            }
            int resID = con.Resources.GetIdentifier(name.Replace("-", "").ToLower() + "16", "drawable", con.PackageName);
            if (resID == 0)
            {
                return null;
            }
            Drawable d = con.Resources.GetDrawable(resID);
            return d;
        }

    }
}




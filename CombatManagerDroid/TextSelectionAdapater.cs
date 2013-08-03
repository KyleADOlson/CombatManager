
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
using Android.Graphics;

namespace CombatManagerDroid
{
    public class TextSelectionAdapater : BaseAdapter 
    {
        List<Tuple<String, Object>> _Items;
        Context _Context;

        int selection = -1;
        
        public TextSelectionAdapater(Context context, List<Tuple<String, Object>> items)
        {
            _Context = context;
            _Items = items;
        }
        
        public override int Count
        {
            get
            {
                return _Items.Count;
            }
        }
        
        public List<Tuple<String, Object>> Items
        {
            get
            {
                return _Items;
            }
        }

        public string SelectedText
        {
            get
            {
                string text = null;
                if (selection > 0 || selection < _Items.Count)
                {
                    text = _Items[selection].Item1;
                }
                return text;
            }
        }

        public object SelectedObject
        {
            get
            {
                object ob = null;
                if (selection > 0 || selection < _Items.Count)
                {
                    ob = _Items[selection].Item2;
                }
                return ob;
            }
        }
        
        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            Tuple<string, object> item = _Items[position];
            TextView t = (TextView)convertView;
            if (t == null)
            {
                t = new TextView(_Context);
            }
            t.Text = item.Item1;
            t.SetTextColor(new Color(0xee, 0xee, 0xee));
            t.SetTextSize(Android.Util.ComplexUnitType.Dip, 20f);
            t.SetBackgroundColor(new Color(0, 0, 0));
            if (position == selection)
            {
                t.SetBackgroundColor(new Color(0xee, 0xee, 0xee));
                t.SetTextColor(new Color(0x11, 0x11, 011));
            }
            t.Click += (object sender, EventArgs e) => 
            {
                selection = position;
                NotifyDataSetChanged();

            };

            return t;
        }

       
    }
}



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

using CombatManager;
using Android.Webkit;

namespace CombatManagerDroid
{
    public abstract class LookupFragment<T> : Fragment
    {

        protected abstract List<T> GetItems();
        
        protected abstract String ItemName(T item);
        
        protected abstract String ItemHtml(T item);

        View _v;


        static T _SelectedItem;

        List<T> items;
        List<T> filteredItems;

        public override void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate (savedInstanceState);

            // Create your fragment here


        }

        public override Android.Views.View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View v = inflater.Inflate(Resource.Layout.Lookup, container, false);

            _v = v;

            items = GetItems();
            items.Sort((a, b) => ItemName(a).CompareTo(ItemName(b)));
            filteredItems = new List<T>(items);

            ItemList.SetAdapter(new LookupAdapter(this));
            ItemList.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) => 
            {
                _SelectedItem = filteredItems[e.Position];
                ShowItem(filteredItems[e.Position]);
            };
            ((TextView)_v.FindViewById(Resource.Id.filterText)).TextChanged += 
                (object sender, Android.Text.TextChangedEventArgs e) => 
            {
                FilterItems();
                ((BaseAdapter)ItemList.Adapter).NotifyDataSetChanged();
                ShowItem(_SelectedItem);
            };
            FilterItems();
            ShowItem(_SelectedItem);

            return v;
        }

        protected virtual bool FilterItem(string filtertext, T item)
        {
            return ItemName(item).ToUpper().Contains(filtertext.ToUpper());
        }

        private void FilterItems()
        {
            string filtertext = ((TextView)_v.FindViewById(Resource.Id.filterText)).Text;

            if (filtertext == null || filtertext.Length == 0)
            {
                filteredItems = items;
            }
            else
            {

                filteredItems = new List<T>();
                foreach (T item in items)
                {
                    if (FilterItem (filtertext, item))
                    {
                        filteredItems.Add(item);
                    }
                }
            }

            if (!filteredItems.Contains(_SelectedItem))
            {
                if (filteredItems.Count == 0)
                {
                    _SelectedItem = default(T);
                }
                else
                {
                    _SelectedItem = filteredItems[0];
                }
            }

        }

        public ListView ItemList
        {
            get
            {
                return (ListView)_v.FindViewById(Resource.Id.itemList);
            }
        }

        private void ShowItem(T item)
        {

            WebView wv = _v.FindViewById<WebView>(Resource.Id.itemView);
            wv.LoadUrl("about:blank");
            if (!item.Equals(default(T)))
            {
                wv.LoadData(ItemHtml(item), "text/html", null);
            }
        }
       

        class LookupAdapter : BaseAdapter
        {
            LookupFragment<T> _lf;
            Context _Context;

            
            public LookupAdapter(LookupFragment<T> lf)
            {
                _lf = lf;
                _Context = lf._v.Context;
            }
            public override int Count
            {
                get
                {
                    return _lf.filteredItems.Count;
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
                
                TextView t = (TextView)convertView;
                if (t == null)
                {
                    t = new TextView(_Context);
                }
                t.Text = _lf.ItemName(_lf.filteredItems[position]);
                return t;
            }
        }
    }
}


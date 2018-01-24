
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
using System.Threading;
using Android.Support.V4.Content;

namespace CombatManagerDroid
{
    public abstract class LookupFragment<T> : Fragment where T: class
    {

        protected abstract List<T> GetItems();
        
        protected abstract String ItemName(T item);
        
        protected abstract String ItemHtml(T item);

        public View _v;


        static T _SelectedItem;

        List<T> items;
        List<T> _FilteredItems;

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
            _FilteredItems = new List<T>(items);

            ItemList.Adapter = (new LookupAdapter(this));
            ItemList.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) => 
            {
                _SelectedItem = _FilteredItems[e.Position];
                ShowItem(_FilteredItems[e.Position]);
            };
            ((TextView)_v.FindViewById(Resource.Id.filterText)).TextChanged += 
                (object sender, Android.Text.TextChangedEventArgs e) => 
            {
                UpdateFilter();
            };
            BuildFilters();
            FilterItems();
            BuildAdditionalLayouts();
            RefreshItem();

            return v;
        }

        public void UpdateFilter()
        {
            Thread t = new Thread(() =>
            {
                FilterItems();
                Activity ac = Activity;
                if (ac != null)
                {
                    ac.RunOnUiThread(() =>
                    {
                        ((BaseAdapter)ItemList.Adapter).NotifyDataSetChanged();
                        ShowItem(_SelectedItem);
                    });
                }
            });
            t.Start();

        }

        protected virtual bool FilterItem(string filtertext, T item)
        {
            return ItemName(item).ToUpper().Contains(filtertext.ToUpper());
        }

        protected void FilterItems()
        {
            lock (this)
            {
                string filtertext = ((TextView)_v.FindViewById(Resource.Id.filterText)).Text;

                List<T> outputItems;

                if (filtertext == null || filtertext.Length == 0)
                {
                    outputItems = new List<T>();

                    foreach (T item in items)
                    {
                        if (CustomFilterItem(item))
                        {
                            outputItems.Add(item);
                        }
                    }
                }
                else
                {

                    outputItems = new List<T>();
                    foreach (T item in items)
                    {
                        if (FilterItem(filtertext, item) && CustomFilterItem(item))
                        {
                            outputItems.Add(item);
                        }
                    }
                }

                if (!outputItems.Contains(_SelectedItem))
                {
                    if (outputItems.Count == 0)
                    {
                        _SelectedItem = default(T);
                    }
                    else
                    {
                        _SelectedItem = outputItems[0];
                    }
                }

                _FilteredItems = outputItems;
            }

        }

        public ListView ItemList
        {
            get
            {
                return (ListView)_v.FindViewById(Resource.Id.itemList);
            }
        }

        protected void RefreshItem()
        {
            ShowItem(_SelectedItem);
        }

        protected virtual void ShowItem(T item)
        {
            if (!SkipShowItem)
            {
                WebView wv = _v.FindViewById<WebView>(Resource.Id.itemView);
                if (item != null)
                {
                    wv.LoadDataWithBaseURL(null, ItemHtml(item), "text/html", "utf-8", null);
                }
                else
                {
                    
                    wv.LoadUrl("about:blank");
                }
            }
        }
        
        protected Apmem.FlowLayout FilterLayout
        {
            get
            {

                return  _v.FindViewById<Apmem.FlowLayout>(Resource.Id.filterLayout);

            }
        }



        protected virtual void BuildFilters()
        {

        }

        protected virtual void BuildAdditionalLayouts()
        {
        }

        protected virtual bool SkipShowItem
        {
            get
            {
                return false;
            }
        }

        List<LinearLayout> filterLevels = new List<LinearLayout>();

        protected void CreateMultipleLevelLayout(int levels)
        {
            for (int i = 0; i < levels; i++)
            {
                LinearLayout l = new LinearLayout(_v.Context);
                filterLevels.Add(l);
                FilterLayout.AddView(l);
            }

        }

        protected Button BuildFilterButton(String text, int size, int? level = null)
        {
            Button b = new Button(_v.Context);
            b.Text = text;
            b.SetMinimumWidth(size);
            if (level != null)
            {
                filterLevels[level.Value].AddView(b);
            }
            else
            {
                FilterLayout.AddView(b);
            }
            b.SetCompoundDrawablesWithIntrinsicBounds(ContextCompat.GetDrawable(_v.Context, Resource.Drawable.down16), null, null, null);

            return b;
            
        }
        
        protected LinearLayout BottomLayout
        {
            get
            {
                return _v.FindViewById<LinearLayout>(Resource.Id.bottomLayout);
            }
        }

        protected LinearLayout LeftLayout
        {
            get
            {
                return _v.FindViewById<LinearLayout>(Resource.Id.leftLayout);
            }
        }

        protected LinearLayout SearchLayout
        {
            get
            {
                return _v.FindViewById<LinearLayout>(Resource.Id.searchLayout);
            }
        }
        protected LinearLayout SearchReplacementLayout
        {
            get
            {
                return _v.FindViewById<LinearLayout>(Resource.Id.searchReplacementLayout);
            }
        }

        protected virtual bool CustomFilterItem(T item)
        {
            return true;
        }

        public T SelectedItem
        {
            get
            {
                return _SelectedItem;
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
                    return _lf._FilteredItems.Count;
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
                t.Text = _lf.ItemName(_lf._FilteredItems[position]);
                return t;
            }
        }
    }
}


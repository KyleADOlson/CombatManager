using System;
using CombatManager;
using System.Collections.Generic;
using Android.Widget;

namespace CombatManagerDroid
{
    public class FeatFragment : LookupFragment<Feat>
    {
        string _Type = "All";

        protected override List<Feat> GetItems ()
        {
            return new List<Feat>(Feat.Feats);
        }

        protected override string ItemHtml (Feat item)
        {
            return FeatHtmlCreator.CreateHtml(item);
        }

        protected override string ItemName (Feat item)
        {
            return item.Name;
        }

        protected override bool IsCustom(Feat item)
        {
            return item != null && item.IsCustom;
        }

        protected override bool CustomFilterItem(Feat item)
        {
            return (_Type == "All") || (item.Type.Contains(_Type));
        }

        protected override void BuildFilters()
        {
            Button b;

            b = BuildFilterButton("type", 180);

            List<String> fts = new List<string>(Feat.FeatTypes);
            fts.Insert(0, "All");
            PopupUtils.AttachButtonStringPopover("Type", b, 
                                                 fts, 
                                                 0, (r1, index, val)=>
                                                 {
                _Type = val;
                UpdateFilter();

            });

            b = new Button(_v.Context);
            b.Text = "New";
            b.Click += (sender, e) =>
            {
                NewItem();
            };
            FilterLayout.AddView(b);
            NewButton = b;

            b = new Button(_v.Context);
            b.Text = "Customize";
            b.Click += (sender, e) =>
            {
                CustomizeItem();
            };
            FilterLayout.AddView(b);
            CustomizeButton = b;

            b = new Button(_v.Context);
            b.Text = "Edit";
            b.Click += (sender, e) =>
            {
                EditItem();
            };
            FilterLayout.AddView(b);
            EditButton = b;
        }

        void NewItem()
        {
            FeatDialog fd = new FeatDialog(_v.Context, new Feat());
            fd.FeatComplete += (sender, e) =>
            {
                Feat.AddCustomFeat(e);
                RefreshPage();
            };

            fd.Show();
        }
        void CustomizeItem()
        {
            if (SelectedItem != null)
            {
                FeatDialog fd = new FeatDialog(_v.Context, SelectedItem);
                fd.FeatComplete += (sender, e) =>
                {
                    Feat.AddCustomFeat(e);
                    RefreshPage();
                };
                fd.Show();
            }
        }
        void EditItem()
        {
            if (SelectedItem != null)
            {
                FeatDialog fd = new FeatDialog(_v.Context, SelectedItem);
                fd.FeatComplete += (sender, e) =>
                {
                    Feat.UpdateCustomFeat(e);
                    RefreshPage();
                };
                fd.Show();
            }
        }

        protected override void DeleteItem(Feat item)
        {
            Feat.RemoveCustomFeat(item);
            RefreshPage();
        }
    }
}


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
        }
    }
}


using System;
using CombatManager;
using System.Collections.Generic;

namespace CombatManagerDroid
{
    public class FeatFragment : LookupFragment<Feat>
    {
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
    }
}


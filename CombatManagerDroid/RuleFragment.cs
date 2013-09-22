using System;
using CombatManager;
using System.Collections.Generic;

namespace CombatManagerDroid
{
    public class RuleFragment: LookupFragment<Rule>
    {
        protected override List<Rule> GetItems ()
        {
            return new List<Rule>(Rule.Rules);
        }

        protected override string ItemHtml (Rule item)
        {
            return RuleHtmlCreator.CreateHtml(item);
        }

        protected override string ItemName (Rule item)
        {
            return item.Name;
        }
    }
}


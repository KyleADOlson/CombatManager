using System;
using CombatManager;
using System.Collections.Generic;

namespace CombatManagerDroid
{
    public class SpellFragment: LookupFragment<Spell>
    {
        protected override List<Spell> GetItems ()
        {
            return new List<Spell>(Spell.Spells);
        }

        protected override string ItemHtml (Spell item)
        {
            return SpellHtmlCreator.CreateHtml(item);
        }

        protected override string ItemName (Spell item)
        {
            return item.Name;
        }
    }
}


/*
 *  SpellHtmlCreator.cs
 *
 *  Copyright (C) 2010-2012 Kyle Olson, kyle@kyleolson.com
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU General Public License
 *  as published by the Free Software Foundation; either version 2
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 * 
 *  You should have received a copy of the GNU General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 *
 */

using System;
using System.Text;
using CombatManager;
using System.Collections.Generic;

namespace CombatManager
{
	public class SpellHtmlCreator
	{
		public SpellHtmlCreator ()
		{
		}

        public static string CreateHtml(Spell spell)
        {
            return CreateHtml(spell, false, true);
        }

        public static string CreateHtml(Spell spell, bool shortForm, bool showTitle)
        {
            StringBuilder blocks = new StringBuilder();
			blocks.CreateHtmlHeader();


            if (!shortForm)
            {
                if (showTitle)
                {
                    blocks.CreateHeader(spell.name);
                }

               	
                blocks.CreateItemIfNotNull("School ", true, spell.school, null, false);


                blocks.CreateItemIfNotNull(" (", false, spell.subschool, ")", false);
                blocks.CreateItemIfNotNull(" [", false, spell.descriptor, "]", false);

                blocks.AppendLineBreak();
				
				blocks.CreateItemIfNotNull("Level ", spell.spell_level);
                blocks.CreateItemIfNotNull("Preparation Time ", spell.preparation_time);
                blocks.CreateItemIfNotNull("Casting Time ", spell.casting_time);
                blocks.CreateItemIfNotNull("Components ", spell.components);
                blocks.CreateItemIfNotNull("Range ", spell.range);
                blocks.CreateItemIfNotNull("Area ", spell.area);
                blocks.CreateItemIfNotNull("Targets ", spell.targets);
                blocks.CreateItemIfNotNull("Effects ", spell.effect);
				
                blocks.CreateItemIfNotNull("Duration ", spell.duration);
                blocks.CreateItemIfNotNull("Saving Throw ", spell.saving_throw);

                blocks.CreateItemIfNotNull("Spell Resistance ", spell.spell_resistence);

                if (spell.source != "PFRPG Core")
                {
                    blocks.CreateItemIfNotNull("Source ", SourceInfo.GetSource(spell.source));
                }

                if (spell.description_formated != null && spell.description_formated.Length > 0)
                {
                    blocks.Append(spell.description_formated);
                }
                else if (spell.description != null && spell.description.Length > 0)
                {
                    blocks.AppendHtml(spell.description);
                }
            }
            else
            {
                if (showTitle)
                {
                   blocks.AppendEscapedTag("p", "bolded", spell.name);
                }

                if (spell.description_formated != null && spell.description_formated.Length > 0)
                {
                    blocks.Append(spell.description_formated);
                }
                else if (spell.description != null && spell.description.Length > 0)
                {
                    blocks.AppendHtml(spell.description);
                }


            }
			
			blocks.CreateHtmlFooter();


            return blocks.ToString();

        }

    }
}


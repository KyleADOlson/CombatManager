using System;
using System.Text;
using CombatManager;
using System.Collections.Generic;

namespace CombatManagerMono
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


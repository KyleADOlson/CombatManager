using System;
using System.Text;
using CombatManager;
using System.Collections.Generic;

namespace CombatManagerMono
{
	public class RuleHtmlCreator
	{
		public RuleHtmlCreator ()
		{
		}
		
		
		public static string CreateHtml(Rule rule)
        {
            return CreateHtml(rule, true);
        }

        public static string CreateHtml(Rule rule, bool showTitle)
        {

            StringBuilder blocks = new StringBuilder();

			blocks.CreateHtmlHeader();
			
            string name = rule.Name;

            string extraText = "";
            if (rule.AbilityType != null && rule.AbilityType.Length > 0)
            {
                extraText += "(" + rule.AbilityType + ")";
            }

            if (rule.Type == "Skills")
            {
                extraText += "(" + rule.Ability + (rule.Untrained ? "" : "; Trained Only") + ")";
            }

            if (extraText.Length > 0)
            {
                name = name + " " + extraText;
            }

            if (showTitle)
            {
                if (rule.Subtype.NotNullString())
                {
                    blocks.CreateHeader(name, rule.Subtype);
                }
                else
                {
                    blocks.CreateHeader(name);
                }
            }
 


            blocks.Append(rule.Details);


            blocks.AppendOpenTag("p");

            if (SourceInfo.GetSourceType(rule.Source) != SourceType.Core)
            {
                blocks.CreateItemIfNotNull("Source: ", SourceInfo.GetSource(rule.Source));
            }

            blocks.CreateItemIfNotNull("Format: ", true, rule.Format, "; ", false);
            blocks.CreateItemIfNotNull("Location: ", true, rule.Location, ".", true);
            blocks.CreateItemIfNotNull("Format: ", true, rule.Format2, "; ", false);
            blocks.CreateItemIfNotNull("Location: ", true, rule.Location2, ".", true);

			blocks.AppendCloseTag("p");

            blocks.CreateHtmlFooter();

            return blocks.ToString();

        }
    }
}


using System;
using System.Text;
using CombatManager;
using System.Collections.Generic;

namespace CombatManagerMono
{
	public class FeatHtmlCreator
	{
		public FeatHtmlCreator ()
		{
		}
		
		public static String CreateHtml(Feat feat)
        {
            return CreateHtml(feat, true);
        }

        public static String CreateHtml(Feat feat, bool showTitle)
        {
            StringBuilder blocks = new StringBuilder();
			blocks.CreateHtmlHeader();

            string featString = null;
            if (feat.Types != null)
            {
                featString = "";
                bool first = true;
                foreach (String featType in feat.Types)
                {
                    if (feat.Type != "General")
                    {
                        if (first)
                        {
                            first = false;
                        }
                        else
                        {
                            featString += ", ";
                        }

                        featString += featType;
                    }
                }
            }

            if (showTitle)
            {

                string text = feat.Name;
                

                if (featString != null && featString.Length > 0)
                {

                    text += " (" + featString + ")";
                }
                


                blocks.CreateHeader(text);
            }
            else
            {
                if (featString != null && featString.Length > 0)
                {
					blocks.AppendEscapedTag("p", "altheader", featString);
                }
            }

			blocks.AppendOpenTag("p");

            blocks.CreateItemIfNotNull(null, feat.Summary);
            if (feat.Prerequistites != null)
            {
                blocks.CreateItemIfNotNull("Prerequisitites: ", feat.Prerequistites.Trim(new char[] { '-' }));
            }
            blocks.CreateItemIfNotNull("Benefit: ", feat.Benefit);
            blocks.CreateItemIfNotNull("Normal: ", feat.Normal);
            blocks.CreateItemIfNotNull("Special: ", feat.Special);

            if (feat.Source != "Pathfinder Core Rulebook")
            {
                blocks.CreateItemIfNotNull("Source: ", SourceInfo.GetSource(feat.Source));
            }


            
			blocks.AppendCloseTag("p");
			blocks.CreateHtmlFooter();


            return blocks.ToString();

        }
	}
}


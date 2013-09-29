using System;
using System.Text;

using CombatManager;

namespace CombatManager
{
    public class MagicItemHtmlCreator
    {
        public MagicItemHtmlCreator ()
        {
        }
        
        public static String CreateHtml(MagicItem item)
        {
            return CreateHtml(item, true);
        }

        public static String CreateHtml(MagicItem item, bool showTitle)
        {

            StringBuilder blocks = new StringBuilder();
            blocks.CreateHtmlHeader();

            if (showTitle)
            {
                blocks.CreateHeader(item.Name, item.Group);
            }


            /*Paragraph details = new Paragraph();
            details.Margin = new Thickness(0, 2, 0, 0);*/
            
            blocks.AppendOpenTag("p");
            blocks.CreateItemIfNotNull("Aura ", true, item.Aura, " ", false);
            blocks.CreateItemIfNotNull("[", false, item.AuraStrength, "]; ", false);
            blocks.CreateItemIfNotNull("CL ", true, item.CL.PastTense(), "", true);
            blocks.CreateItemIfNotNull("Slot ", true, item.Slot, "; ", false);
            blocks.CreateItemIfNotNull("Price ", true, item.Price, "; ", false);
            blocks.CreateItemIfNotNull("Weight ", true, item.Weight, "", true);
            blocks.AppendCloseTag("p");



            if (!String.IsNullOrEmpty(item.DescHTML) || !String.IsNullOrEmpty(item.Description))
            {
                blocks.CreateSectionHeader("DESCRIPTION");

                if (!String.IsNullOrEmpty(item.DescHTML) && item.DescHTML != "NULL")
                {

                    blocks.AppendOpenTagWithClass("p", "description");
                    blocks.Append(item.DescHTML);
                    blocks.AppendCloseTag("p");
                }
                else if (item.Description != "NULL")
                {


                    blocks.AppendOpenTagWithClass("p", "description");

                    blocks.CreateItemIfNotNull( null, true, item.Description, null, true);


                    blocks.AppendCloseTag("p");
                }

            }

            if (item.Requirements != null && item.Requirements.Length > 0 &&
                item.Cost != null && item.Cost.Length > 0)
            {
                blocks.CreateSectionHeader("CONSTRUCTION");

                
                blocks.AppendOpenTag("p");
                blocks.CreateItemIfNotNull("Requirements ", true, item.Requirements, "; ", false);
                blocks.CreateItemIfNotNull("Cost ", true, item.Cost, "", true);
                blocks.AppendCloseTag("p");

            }

            if (!String.IsNullOrEmpty(item.Destruction) && item.Destruction != "NULL")
            {
                blocks.CreateSectionHeader("DESTRUCTION");


                blocks.AppendOpenTag("p");

                blocks.CreateItemIfNotNull(null, false, item.Destruction, null, true);

                blocks.AppendCloseTag("p");
            }

            if (SourceInfo.GetSourceType(item.Source) != SourceType.Core)
            {
                blocks.CreateItemIfNotNull("Source: ", SourceInfo.GetSource(item.Source));

            }

            blocks.CreateHtmlFooter();

            return blocks.ToString();
        }
    }
}


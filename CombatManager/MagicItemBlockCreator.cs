using System;
using System.Data;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace CombatManager
{
    class MagicItemBlockCreator : BlockCreator
    {
        public MagicItemBlockCreator (FlowDocument document)
            : base(document)
        {
        }

        public List<Block> CreateBlocks(MagicItem item)
        {
            return CreateBlocks(item, true);
        }

        public List<Block> CreateBlocks(MagicItem item, bool showTitle)
        {

            List<Block> blocks = new List<Block>();

            if (showTitle)
            {
                blocks.Add(CreateHeaderParagraph(item.Name, item.Group));
            }


            Paragraph details = new Paragraph();
            details.Margin = new Thickness(0, 2, 0, 0);

            CreateItemIfNotNull(details.Inlines, "Aura ", true, item.Aura, " ", false);
            CreateItemIfNotNull(details.Inlines, "[", false, item.AuraStrength, "]; ", false);
            CreateItemIfNotNull(details.Inlines, "CL ", true, PastTenseNumber(item.CL), "", true);
            CreateItemIfNotNull(details.Inlines, "Slot ", true, item.Slot, "; ", false);
            CreateItemIfNotNull(details.Inlines, "Price ", true, item.Price, "; ", false);
            CreateItemIfNotNull(details.Inlines, "Weight ", true, item.Weight, "", true);


            blocks.Add(details);

            if (NotNullString(item.DescHTML) || NotNullString(item.Description))
            {
                blocks.AddRange(CreateSectionHeader("DESCRIPTION"));

                if (NotNullString(item.DescHTML))
                {
                    blocks.AddRange(CreateFlowFromDescription(item.DescHTML));
                }
                else
                {


                    Paragraph description = new Paragraph();
                    description.Margin = new Thickness(0, 2, 0, 0);

                    string text = FixBodyString(item.Description);

                    CreateItemIfNotNull(description.Inlines, null, true, text, null, true);


                    blocks.Add(description);
                }

            }

            if (item.Requirements != null && item.Requirements.Length > 0 &&
                item.Cost != null && item.Cost.Length > 0)
            {
                blocks.AddRange(CreateSectionHeader("CONSTRUCTION"));

                Paragraph construction = new Paragraph();
                construction.Margin = new Thickness(0, 2, 0, 0);

                CreateItemIfNotNull(construction.Inlines, "Requirements ", true, item.Requirements, "; ", false);
                CreateItemIfNotNull(construction.Inlines, "Cost ", true, item.Cost, "", true);

                blocks.Add(construction);
            }

            if (NotNullString(item.Destruction))
            {
                blocks.AddRange(CreateSectionHeader("DESTRUCTION"));

                Paragraph desctruction = new Paragraph();
                desctruction.Margin = new Thickness(0, 2, 0, 0);


                CreateItemIfNotNull(desctruction.Inlines, null, false, FixBodyString(item.Destruction), null, true);


                blocks.Add(desctruction);
            }

            if (SourceInfo.GetSourceType(item.Source) != SourceType.Core)
            {
                Paragraph source = new Paragraph();
                source.Margin = new Thickness(0, 2, 0, 0);
                CreateItemIfNotNull(source.Inlines, "Source: ", SourceInfo.GetSource(item.Source));
                blocks.Add(source);
            }


            return blocks;
        }
    }
}

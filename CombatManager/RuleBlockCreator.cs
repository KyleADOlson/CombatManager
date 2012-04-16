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
    class RuleBlockCreator : BlockCreator
    {
        public RuleBlockCreator(FlowDocument document)
            : base(document)
        {
        }

        public List<Block> CreateBlocks(Rule rule)
        {
            return CreateBlocks(rule, true);
        }

        public List<Block> CreateBlocks(Rule rule, bool showTitle)
        {

            List<Block> blocks = new List<Block>();


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
                if (NotNullString(rule.Subtype))
                {
                    blocks.Add(CreateHeaderParagraph(name, rule.Subtype));
                }
                else
                {
                    blocks.Add(CreateHeaderParagraph(name));
                }
            }
 


            string detailsText = rule.Details;

            if (detailsText == null)
            {
                detailsText = "";
            }

            detailsText = Regex.Replace(detailsText, "&mdash;|&ndash;", "­–");
            detailsText = Regex.Replace(detailsText, "\t", "");
            detailsText = Regex.Replace(detailsText, "&hellip;", "...");


            if (detailsText.Length <= 2 || detailsText.Substring(0, 2) != "<p")
            {
                detailsText = "<p>" + detailsText + "</p>";
            }
            else
            {
                detailsText = detailsText.Replace("\n", "");
                detailsText = detailsText.Replace("\r", "");
            }


            blocks.AddRange(CreateFlowFromDescription(detailsText));


            Paragraph details = new Paragraph();
            if (showTitle)
            {
                details.Margin = new Thickness(0, 2, 0, 0);
            }
            else
            {
                if (rule.Subtype != null)
                {
                    extraText = rule.Subtype + " " + extraText;
                }

                details.Inlines.Add(new Italic(new Run(extraText)));
                details.Inlines.Add(new LineBreak());
            }

            if (SourceInfo.GetSourceType(rule.Source) != SourceType.Core)
            {
                CreateItemIfNotNull(details.Inlines, "Source: ", SourceInfo.GetSource(rule.Source));
            }

            CreateItemIfNotNull(details.Inlines, "Format: ", true, rule.Format, "; ", false);
            CreateItemIfNotNull(details.Inlines, "Location: ", true, rule.Location, ".", true);
            CreateItemIfNotNull(details.Inlines, "Format: ", true, rule.Format2, "; ", false);
            CreateItemIfNotNull(details.Inlines, "Location: ", true, rule.Location2, ".", true);


            blocks.Add(details);

            return blocks;

        }
    }
}

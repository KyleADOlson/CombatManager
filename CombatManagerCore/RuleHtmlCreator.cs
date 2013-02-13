/*
 *  RuleHtmlCreator.cs
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


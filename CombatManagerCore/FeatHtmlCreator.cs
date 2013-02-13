/*
 *  FeatHtmlCreator.cs
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


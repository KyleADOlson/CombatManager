/*
 *  BlockCreator.cs
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

﻿using System;
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


    public delegate void DocumentLinkHander(object sender, DocumentLinkEventArgs e);

    public class DocumentLinkEventArgs : EventArgs
    {
        string _Name;
        string _Type;

        public DocumentLinkEventArgs(string name, string type)
        {
            _Name = name;
            _Type = type;
        }



        public string Type
        {
            get
            {
                return _Type;
            }
            set
            {
                _Type = value;
            }
        }

        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }
    }

    public abstract class BlockCreator
    {
        private static Dictionary<string, string> sourceMap;

        static BlockCreator()
        {
            sourceMap = new Dictionary<string, string>();

            sourceMap.Add("Pathfinder Core Rulebook", "Pathfinder Core Rulebook");
            sourceMap.Add("PFRPG CORE", "Pathfinder Core Rulebook");
            sourceMap.Add("PFRPG Bestiary", "Pathfinder Bestiary");
            sourceMap.Add("APG", "Advanced Player's Guide");

            sourceMap.Add("PCo:DoG", "Dwarves of Golarion");
            sourceMap.Add("DwarvesOfGolarion", sourceMap["PCo:DoG"]);
            sourceMap.Add("Orcs of Golarion", "Orcs of Golarion");
            sourceMap.Add("Gnomes", "Gnomes of Golarion");

            sourceMap.Add("Faction Guide", "Faction Guide");
            sourceMap.Add("Book of the Damned V1", "Book of the Damned - Volume 1: Princes of Darkness");
            sourceMap.Add("Book of the Dammed V1", sourceMap["Book of the Damned V1"]);


            sourceMap.Add("PCh:CTR", "Classic Treasures Revisited");
            sourceMap.Add("Classic Treasures", sourceMap["PCh:CTR"]);
            sourceMap.Add("CMR", "Classic Monsters Revisited");
            sourceMap.Add("PCh:CMR", sourceMap["CMR"]);
            sourceMap.Add("PCh:CHR", "Classic Horrors Revisited");
            sourceMap.Add("PCh:DR", "Dragons Revisited");
            sourceMap.Add("PCh:DDR", "Dungeon Denizens Revisited");

            sourceMap.Add("P:CTWE", "Character Traits Web Enhancement ");

            sourceMap.Add("PCh:CS", "Pathfinder Chronicles Campaign Setting");
            sourceMap.Add("PCo:QGttE", "Qadira, Gateway to the East");
            sourceMap.Add("Sargava", "Sargava, the Lost Colony");
            sourceMap.Add("Pathfinder Companion: Sargava, the Lost Colony", sourceMap["Sargava"]);
            sourceMap.Add("Andoran", "Andoran, Spirit of Liberty");
            sourceMap.Add("PCh:ASoL", sourceMap["Andoran"]);
            sourceMap.Add("PCo:CEoD", "Cheliax, Empire Of Devils");
            sourceMap.Add("Cheliax Empire Of Devils", sourceMap["PCo:CEoD"]);
            sourceMap.Add("PCo:OLoP", "Osirion, Land of Pharaohs");
            sourceMap.Add("PCo:TEoG", "Taldor, Echoes of Glory");
            sourceMap.Add("PCh:GtDV", "Guide to Darkmoon Vale");
            sourceMap.Add("PCh:DM", "Dark Markets - A Guide to Katapesh");
            sourceMap.Add("PCh:GttRK", "Guide to the River Kingdoms");

            sourceMap.Add("PCh:CoG", "Cities of Golarion");

            sourceMap.Add("PCh:SoS", "Seekers of Secrets");

            sourceMap.Add("PAP:RotRPG", "Rise of the Runelords Player's Guide");
            sourceMap.Add("PAP:CotCTPG", "Curse of the Crimson Throne Player's Guide");
            sourceMap.Add("PCo:SD", "Second Darkness Player's Guide");
            sourceMap.Add("PCo:LoFPG", "Legacy of Fire Player's Guide");
            
            sourceMap.Add("PAP1:RotR1", "Rise of the Runelords: Burnt Offerings");
            sourceMap.Add("PAP2:RotR2", "Rise of the Runelords: The Skinsaw Murders");
            sourceMap.Add("PAP3:RotR3", "Rise of the Runelords: Hook Mountain Massacre");
            sourceMap.Add("PAP4:RotR4", "Rise of the Runelords: Fortress of the Stone Giants");
            sourceMap.Add("PAP5:RotR5", "Rise of the Runelords: Sins of the Saviors");
            sourceMap.Add("PAP6:RotR6", "Rise of the Runelords: Spires of Xin-Shalast");
            sourceMap.Add("PAP7:CotCT1", "Curse of the Crimson Throne: Edge of Anarchy");
            sourceMap.Add("PAP8:CotCT2", "Curse of the Crimson Throne: Seven Days to the Grave");
            sourceMap.Add("PAP9:CotCT3", "Curse of the Crimson Throne: Escape from Old Korvosa");
            sourceMap.Add("PAP10:CotCT4", "Curse of the Crimson Throne: A History of Ashes");
            sourceMap.Add("PAP11:CotCT5", "Curse of the Crimson Throne: Skeletons of Scarwall");
            sourceMap.Add("PAP12:CotCT6", "Curse of the Crimson Throne: Crown of Fangs");
            sourceMap.Add("PAP13:SD1", "Second Darkness: Shadow in the Sky");
            sourceMap.Add("PAP14:SD2", "Second Darkness: Children of the Void");
            sourceMap.Add("PAP15:SD3", "Second Darkness: The Armageddon Echo");
            sourceMap.Add("PAP16:SD4", "Second Darkness: Endless Night");
            sourceMap.Add("PAP17:SD5", "Second Darkness: A Memory of Darkness");
            sourceMap.Add("PAP18:SD6", "Second Darkness: Descent into Midnight");
            sourceMap.Add("PAP19:LoF1", "Legacy of Fire: Howl of the Carrion King");
            sourceMap.Add("PAP20:LoF2", "Legacy of Fire: House of the Beast");
            sourceMap.Add("PAP21:LoF3", "Legacy of Fire: The Jackal's Price");
            sourceMap.Add("PAP22:LoF4", "Legacy of Fire: The End of Eternity");
            sourceMap.Add("PAP23:LoF5", "Legacy of Fire: The Impossible Eye");
            sourceMap.Add("PAP24:LoF6", "Legacy of Fire: The Final Wish");
            sourceMap.Add("PAP25:CoT1", "Council of Thieves: The Bastards of Erebus");
            sourceMap.Add("PAP26:CoT2", "Council of Thieves: The Sixfold Trial");
            sourceMap.Add("PAP27:CoT3", "Council of Thieves: What Lies in Dust");
            sourceMap.Add("PAP28:CoT4", "Council of Thieves: The Infernal Syndrome");
            sourceMap.Add("PAP29:CoT5", "Council of Thieves: Mother of Flies");
            sourceMap.Add("PAP30:CoT6", "Council of Thieves: The Twice-Damned Prince");
            sourceMap.Add("PAP31:K1", "Kingmaker: Stolen Land");
            sourceMap.Add("PAP32:K2", "Kingmaker: Rivers Run Red");
            sourceMap.Add("PAP33:K3", "Kingmaker: The Varnhold Vanishing");
            sourceMap.Add("PAP34:K4", "Kingmaker: Blood for Blood");
            sourceMap.Add("PAP35:K5", "Kingmaker: War of the River Kings");
            sourceMap.Add("PAP36:K6", "Kingmaker: Sound of a Thousand Screams");


            sourceMap.Add("AP 1", sourceMap["PAP1:RotR1"]);
            sourceMap.Add("AP 2", sourceMap["PAP2:RotR2"]);
            sourceMap.Add("AP 3", sourceMap["PAP3:RotR3"]);
            sourceMap.Add("AP 4", sourceMap["PAP4:RotR4"]);
            sourceMap.Add("AP 5", sourceMap["PAP5:RotR5"]);
            sourceMap.Add("AP 6", sourceMap["PAP6:RotR6"]);
            sourceMap.Add("AP 7", sourceMap["PAP7:CotCT1"]);
            sourceMap.Add("AP 8", sourceMap["PAP8:CotCT2"]);
            sourceMap.Add("AP 9", sourceMap["PAP9:CotCT3"]);
            sourceMap.Add("AP 10", sourceMap["PAP10:CotCT4"]);
            sourceMap.Add("AP 11", sourceMap["PAP11:CotCT5"]);
            sourceMap.Add("AP 12", sourceMap["PAP12:CotCT6"]);
            sourceMap.Add("AP 13", sourceMap["PAP13:SD1"]);
            sourceMap.Add("AP 14", sourceMap["PAP14:SD2"]);
            sourceMap.Add("AP 15", sourceMap["PAP15:SD3"]);
            sourceMap.Add("AP 16", sourceMap["PAP16:SD4"]);
            sourceMap.Add("AP 17", sourceMap["PAP17:SD5"]);
            sourceMap.Add("AP 18", sourceMap["PAP18:SD6"]);
            sourceMap.Add("AP 19", sourceMap["PAP19:LoF1"]);
            sourceMap.Add("AP 20", sourceMap["PAP20:LoF2"]);
            sourceMap.Add("AP 21", sourceMap["PAP21:LoF3"]);
            sourceMap.Add("AP 22", sourceMap["PAP22:LoF4"]);
            sourceMap.Add("AP 23", sourceMap["PAP23:LoF5"]);
            sourceMap.Add("AP 24", sourceMap["PAP24:LoF6"]);
            sourceMap.Add("AP 25", sourceMap["PAP25:CoT1"]);
            sourceMap.Add("AP 26", sourceMap["PAP26:CoT2"]);
            sourceMap.Add("AP 27", sourceMap["PAP27:CoT3"]);
            sourceMap.Add("AP 28", sourceMap["PAP28:CoT4"]);
            sourceMap.Add("AP 29", sourceMap["PAP29:CoT5"]);
            sourceMap.Add("AP 30", sourceMap["PAP30:CoT6"]);
            sourceMap.Add("AP 31", sourceMap["PAP31:K1"]);
            sourceMap.Add("AP 32", sourceMap["PAP32:K2"]);
            sourceMap.Add("AP 33", sourceMap["PAP33:K3"]);
            sourceMap.Add("AP 34", sourceMap["PAP34:K4"]);
            sourceMap.Add("AP 35", sourceMap["PAP35:K5"]);
            sourceMap.Add("AP 36", sourceMap["PAP36:K6"]);

            sourceMap.Add("PM:J2", "Module J2: Guardians of Dragonfall");
            sourceMap.Add("Golden Death", "City of Golden Death");
        }


        public BlockCreator(FlowDocument document)
        {
            this.document = document;
        }



        private FlowDocument document;

        public FlowDocument Document
        {
            get
            {
                return document;
            }
        }

        protected List<Inline> CreateMultiValueLine(List<TitleValuePair> values, string seperator)
        {
            List<Inline> inlines = new List<Inline>();

            bool itemAdded = false;

            foreach (TitleValuePair pair in values)
            {
                if (pair.Value != null && pair.Value.Length > 0)
                {
                    if (itemAdded && seperator != null)
                    {
                        inlines.Add(new Run(seperator));
                    }

                    inlines.AddRange(CreateItemIfNotNull(pair.Title, true, pair.Value, null, false));

                    itemAdded = true;
                }
            }

            if (itemAdded)
            {
                inlines.Add(new LineBreak());
            }

            return inlines;
        }


        protected List<Block> CreateSectionHeader(String title)
        {
            return CreateSectionHeader(title, false);
        }

        protected List<Block> CreateSectionHeader(String title, bool bold)
        {
            List<Block> blocks = new List<Block>();



            blocks.Add(CreateHeaderLine());



            Paragraph header = new Paragraph();

            if (bold)
            {
                header.Inlines.Add(new Bold(new Run(title)));
            }
            else
            {
                header.Inlines.Add(title);
            }
            header.Margin = new Thickness(0);

            blocks.Add(header);

            blocks.Add(CreateHeaderLine());

            return blocks;
        }

        protected Block CreateHeaderLine()
        {
            Grid grid = new Grid();

            Line line = new Line();
            line.Name = "Line";
            line.HorizontalAlignment = HorizontalAlignment.Stretch;
            line.VerticalAlignment = VerticalAlignment.Bottom;
            line.Height = 1;
            line.Width = double.NaN;
            line.Margin = new Thickness(0);
            line.X1 = 0;
            line.X2 = 1;
            line.Y1 = 0;
            line.Y2 = 0;
            line.Stretch = Stretch.Fill;
            line.Stroke = new SolidColorBrush(Colors.Black);

            grid.Children.Add(line);
            BlockUIContainer cont = new BlockUIContainer(grid);
            cont.Margin = new Thickness(0, 2, 0, 2);

            return cont;
        }

        protected void CreateItemIfNotNull(InlineCollection inlines, String title, String value)
        {
            List<Inline> list = CreateItemIfNotNull(title, value);

            if (list != null && list.Count > 0)
            {
                inlines.AddRange(list);
            }
        }

        protected void CreateItemIfNotNull(InlineCollection inlines, String title, bool boldTitle, String value, String end, bool linebreak)
        {
            List<Inline> list = CreateItemIfNotNull(title, boldTitle, value, end, linebreak);

            if (list != null && list.Count > 0)
            {
                inlines.AddRange(list);
            }
        }


        protected List<Inline> CreateItemIfNotNull(String title, String value)
        {
            return CreateItemIfNotNull(title, true, value, null, true);
        }

        protected List<Inline> CreateItemIfNotNull(String title, bool boldTitle, String value, String end, bool linebreak)
        {
            List<Inline> inlines = new List<Inline>();

            if (NotNullString(value))
            {
                if (title != null)
                {
                    if (boldTitle)
                    {
                        inlines.Add(new Bold(new Run(title)));
                    }
                    else
                    {
                        inlines.Add(new Run(title));
                    }
                }

                inlines.Add(new Run(value));

                if (end != null)
                {
                    inlines.Add(new Run(end));
                }
                if (linebreak)
                {
                    inlines.Add(new LineBreak());
                }

            }
            return inlines;
        }

        public static bool NotNullString(string value)
        {
            return value != null && value.Length > 0 && value != "NULL";
        }


        protected Block CreateHeaderParagraph(String text)
        {
            return CreateHeaderParagraph(text, null);
        }


        protected Block CreateHeaderParagraph(string text, string secondaryText)
        {
            

            Paragraph header = new Paragraph();
            header.Background = new SolidColorBrush((Color)document.FindResource("SecondaryColorBDarker"));
            header.Foreground = document.Background;
            header.FontSize = document.FontSize * 1.3;


            if (secondaryText != null)
            {
                Paragraph floaterParagraph = new Paragraph(new Run(secondaryText));
                floaterParagraph.Padding = new Thickness(0);
                floaterParagraph.Padding = new Thickness(0);
                Floater floater = new Floater(floaterParagraph);
                floater.Padding = new Thickness(4, 0, 4, 0);
                floater.Margin = new Thickness(0);


                floater.HorizontalAlignment = HorizontalAlignment.Right;
                header.Inlines.Add(floater);
            }

            Run titleRun = new Run(text);
            titleRun.FontWeight = FontWeights.Bold;
            //titleRun.FontSize = titleRun.FontSize * 1.8;
            titleRun.Background = new SolidColorBrush();
            header.Inlines.Add(titleRun);
            header.Padding = new Thickness(4, 2, 4, 2);


            header.Margin = new Thickness(0);



            return header;
        }

        protected List<Block> CreateFlowFromDescription(String description)
        {
            return CreateFlowFromDescription(description, false, false);
        }

        protected List<Block> CreateFlowFromDescription(String description, bool startBold, bool startItalic)
        {
            List<Block> blocks = new List<Block>();

            

            Regex regex = new Regex("<(?<tag>/?[\\p{L}0-9]+)[^<>]*?>", RegexOptions.IgnoreCase);
            Match match = (regex.Match(description));

            bool bold = startBold;
            bool italic = startItalic;
            Paragraph paragraph = null;

            
            int start = 0;


            while (match.Success)
            {
                bool noAdvanceStart = false;
                string lastText = description.Substring(start, match.Index - start);



                if (paragraph != null && lastText.Length > 0)
                {
                    Run run = new Run(lastText);
                    run.FontStyle = italic ? System.Windows.FontStyles.Italic : System.Windows.FontStyles.Normal;
                    run.FontWeight = bold ? System.Windows.FontWeights.Bold : System.Windows.FontWeights.Normal;



                    paragraph.Inlines.Add(new Bold(run));
                }

                string tag = match.Groups["tag"].Value.ToLower();

                if (tag == "p")
                {
                    paragraph = new Paragraph();
                }
                if (tag == "b")
                {
                    bold = true;
                }
                if (tag == "i")
                {
                    italic = true;
                }
                if (tag == "/b")
                {
                    bold = false;
                }
                if (tag == "/i")
                {
                    italic = false;
                }
                if (tag == "/p" && paragraph != null)
                {
                    if (paragraph.Inlines.Count > 0)
                    {
                        blocks.Add(paragraph);
                        paragraph = new Paragraph();
                    }


                }
                if (tag == "br" || tag == "/br" || tag == "br/")
                {
                    if (paragraph != null)
                    {
                        blocks.Add(paragraph);
                        paragraph = new Paragraph();
                    }
                }
                if (tag == "h1" || tag == "h2" || tag == "h3" || tag == "h4")
                {
                    int textEnd;
                    string bodytext = FindEndTag(description, tag, match.Index + match.Length, out textEnd);

                    if (bodytext == null)
                    {
                        textEnd = description.Length;
                    }
                    else
                    {                        
                        blocks.AddRange(CreateFlowFromDescription("<p>" + bodytext + "</p>", true, italic));
                    }
                    start = textEnd;
                    noAdvanceStart = true;


                }
                if (tag == "table" )
                {
                    int textEnd;
                    string bodytext = FindEndTag(description, "table", match.Index + match.Length, out textEnd);

                    if (bodytext == null)
                    {
                        textEnd = description.Length;
                    }
                    else
                    {

                        Block table = CreateTableBlock(bodytext);
                        blocks.Add(table);
                    }
                    start = textEnd;
                    noAdvanceStart = true;


                }
                if (tag == "ul")
                {

                    int textEnd;
                    string bodytext = FindEndTag(description, "ul", match.Index + match.Length, out textEnd);

                    if (bodytext == null)
                    {
                        textEnd = description.Length;
                    }
                    else
                    {

                        Block table = CreateUnorderedListBlock(bodytext);
                        blocks.Add(table);
                    }
                    start = textEnd;
                    noAdvanceStart = true;

                }

                if (tag == "sup" || tag == "sub")
                {

                    int textEnd;
                    string bodytext = FindEndTag(description, tag, match.Index + match.Length, out textEnd);

                    if (bodytext == null)
                    {
                        textEnd = description.Length;
                    }
                    else
                    {

                        if (paragraph != null)
                        {
                            Run run = new Run(bodytext );
                            //temp minor fix for script problem
                            run.FontSize = document.FontSize - 3;
                            if (tag == "sup")
                            {
                                run.BaselineAlignment = BaselineAlignment.Top;
                            }
                            else
                            {
                                run.BaselineAlignment = BaselineAlignment.Subscript;
                            }
                            paragraph.Inlines.Add(run);
                        }
                    }
                    start = textEnd;
                    noAdvanceStart = true;
                }

                if (!noAdvanceStart)
                {
                    start = match.Index + match.Length;
                }
                match = regex.Match(description, start);

            }

            if (paragraph != null)
            {
                if (paragraph.Inlines.Count > 0)
                {
                    blocks.Add(paragraph);
                    // Debugger.Break();
                }
            }

            return blocks;

        }

        protected Block CreateTableBlock(String tableText)
        {
            Table table = new Table();
            TableRowGroup group = new TableRowGroup();


            Regex regRow = new Regex("<tr.*?>(?<text>(.|\n|\r)+?)</tr>", RegexOptions.IgnoreCase);


            
            foreach (Match m in regRow.Matches(tableText))
            {
                TableRow row = CreateTableRow(m.Groups["text"].Value);
                group.Rows.Add(row);
            }

            table.RowGroups.Add(group);

            return table;
        }

        protected TableRow CreateTableRow(String tableText)
        {
            TableRow row = new TableRow();


            Regex regItem = new Regex("<th(?<headerparams>.*?)>(?<header>(.|/n)+?)</th>|<td(?<cellparams>.*?)>(?<cell>(.|/n)+?)</td>", RegexOptions.IgnoreCase);



            foreach (Match m in regItem.Matches(tableText))
            {
                string text;

                if (m.Groups["header"].Success)
                {

                    text = m.Groups["header"].Value;
                    TableCell c = new TableCell();

                    if (m.Groups["headerparams"].Success)
                    {

                        SetCellParams(c, m.Groups["headerparams"].Value);

                    }

                    c.Blocks.AddRange(CreateFlowFromDescription("<p>" + text + "</p>", true, false));
                    row.Cells.Add(c);
                }
                else
                {
                    Paragraph p = new Paragraph();
                    p.Foreground = Brushes.Black;
                    TableCell c = new TableCell();

                    
                    if (m.Groups["cellparams"].Success)
                    {
                        SetCellParams(c, m.Groups["cellparams"].Value);

                    }
                    c.Blocks.AddRange(CreateFlowFromDescription("<p>" + m.Groups["cell"].Value + "</p>"));
                    row.Cells.Add(c);
                }



            }


            return row;
        }


        private void SetCellParams(TableCell c, string paramText)
        {
            int colspan = GetColumnSpan(paramText);

            if (colspan > 0)
            {
                c.ColumnSpan = colspan;
            }

            int rowspan = GetRowSpan(paramText);

            if (rowspan > 0)
            {
                c.RowSpan = rowspan;
            }

        }

        private static int GetColumnSpan(string paramText)
        {
            int value = 0;      

            Match mp = Regex.Match(paramText, "colspan=\"(?<val>[0-9]+)\"");

            if (mp.Success)
            {
                int.TryParse(mp.Groups["val"].Value, out value);
              
            }

            return value;

        }

        private static int GetRowSpan(string paramText)
        {
            int value = 0;

            Match mp = Regex.Match(paramText, "rowspan=\"(?<val>[0-9]+)\"");

            if (mp.Success)
            {
                int.TryParse(mp.Groups["val"].Value, out value);

            }

            return value;

        }

        protected Block CreateUnorderedListBlock(String text)
        {

            List list = new List();




            Regex regItem = new Regex("<li.*?>", RegexOptions.IgnoreCase);

            int start = 0;
            Match m = regItem.Match(text, start);



            while (m.Success)
            {
                int end;
                int searchStart = m.Index + m.Length;
                string body = FindEndTag(text, "li", searchStart, out end);

                if (body == null)
                {
                    break;
                }

                else
                {
                    if (body.Length > 0 && body[0] != '<')
                    {
                        body = "<p>" + body + "</p>";
                    }

                    ListItem item = new ListItem();
                    item.Blocks.AddRange(CreateFlowFromDescription(body));
                    list.ListItems.Add(item);

                    start = end;
                    m = regItem.Match(text, start);
                }
            }

            return list;

        }


        public static string FixBodyString(string value)
        {
            value = value.Replace('\u00a0', ' ');
            value = value.Replace("\u0393\u00c7\u00a3", "");
            value = value.Replace("\u0393\u00c7\u00a5", "");
            value = value.Replace("\u0393\u00c7\u00ff", "'");
            value = ReplaceSingle(value, "\u0393\u00c7\u00f3", "");
            value = value.Replace("\u0393\u00c7\u00f3", "");
            value = value.Replace("\u0393\u00c7\u00aa", "");
            value = value.Replace("\u0393\u00c7\u00d6", "'");
            value = value.Replace("\u251c\u00bc", "&#236;");

            return value;
        }

        public static string ReplaceSingle(string source, string oldValue, string newValue)
        {
            int index = source.IndexOf(oldValue);

            if (index < 0)
            {
                return source;
            }

            StringBuilder ret = new StringBuilder();

            ret.Append(source.Substring(0, index));
            ret.Append(newValue);
            ret.Append(source.Substring(index + oldValue.Length));

            return ret.ToString();
        }


        public static string PastTenseNumber(string number)
        {
            string res = number;

            if (res != null && res.Length > 0)
            {
                int num;

                res = res.Trim();

                if (int.TryParse(res, out num))
                {
                    res = PastTenseNumber(num); 
                }
               
            }

            return res;
        }

        public static string PastTenseNumber(int num)
        {


            string res = num.ToString();


            int last = num % 10;

            switch (last)
            {
                case 1:
                    res += "st";
                    break;
                case 2:
                    res += "nd";
                    break;
                case 3:
                    res += "rd";
                    break;
                default:
                    res += "th";
                    break;
            }
                
            return res;
        
        }

        static string FindEndTag(string text, string tag, int start, out int tagEnd)
        {
            int tagCount = 0;
            int searchStart = start;
            bool found = false;
            bool failed = false;
            Regex regItem = new Regex("(?<text>(.|\n|\r)+?)((?<open><" + tag + ".*?>)|(?<close></" + tag +  ">))", RegexOptions.IgnoreCase);

            string returnText = null;
            tagEnd = -1;

            while (!found && !failed)
            {
                Match m = regItem.Match(text, searchStart);

                if (!m.Success)
                {
                    failed = true;
                }
                else
                {
                    if (m.Groups["open"].Success)
                    {
                        tagCount++;
                        searchStart = m.Groups["open"].Index + m.Groups["open"].Length;

                    }
                    else
                    {
                        if (tagCount == 0)
                        {
                            returnText = text.Substring(searchStart,  m.Groups["close"].Index - searchStart);
                            tagEnd = m.Groups["close"].Index + m.Groups["close"].Length;
                            found = true;
                        }
                        else
                        {
                            tagCount--;
                            searchStart = m.Groups["close"].Index + m.Groups["close"].Length;

                        }
                    }
                }
            }

            return returnText;
        }

    }

    
}

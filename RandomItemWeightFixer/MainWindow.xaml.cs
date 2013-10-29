/*
 *  MainWindow.xaml.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CombatManager;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Diagnostics;

namespace RandomItemWeightFixer
{


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {

        ListCollectionView spellView;
        SortedSet<String> sources;

        public MainWindow()
        {
            InitializeComponent();

            spellView = new ListCollectionView(Spell.Spells);
            sources = new SortedSet<string>();

            foreach (Spell spell in Spell.Spells)
            {
                sources.Add(SourceInfo.GetSource(spell.source));
            }

        }



        private void ClassComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (spellView != null)
            {
                spellView.Refresh();
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            foreach (Spell spell in Spell.Spells)
            {
                spell.PotionWeight = NullValue(spell.PotionWeight);
                spell.ArcaneScrollWeight = NullValue(spell.ArcaneScrollWeight);
                spell.DivineScrollWeight = NullValue(spell.DivineScrollWeight);
                spell.WandWeight = NullValue(spell.WandWeight);
            }

            List<Spell> list = new List<Spell>(Spell.Spells);
            XmlListLoader<Spell>.Save(list, "Spells.xml");
        }

        private string NullValue(string text)
        {
            if (text != null && text.Length == 0)
            {
                return null;
            }
            return text;
        }


        private void LevelComboBox_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {

        }

        private void LevelComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (spellView != null)
            {
                spellView.Refresh();
            }

        }

        private void SpellWeightRunButton_Click(object sender, RoutedEventArgs e)
        {
            List<WeightValue> weights = XmlListLoader<WeightValue>.Load("Weight.xml");
            foreach (WeightValue w in weights)
            {
                //find spell
                Spell spell = Spell.Spells.FirstOrDefault(a => String.Compare(a.name, w.Name, true) == 0);

                if (spell == null)
                {
                    System.Diagnostics.Debug.WriteLine(w.Name + " " + w.Type + " Level " + w.Level);
                }
                else
                {
                    if (w.Type == "Potion")
                    {
                        spell.PotionCost = w.Price;
                        spell.PotionLevel = w.Level;
                        spell.PotionWeight = w.Weight;
                    }
                    else if (w.Type == "Wand")
                    {
                        spell.WandCost = w.Price;
                        spell.WandLevel = w.Level;
                        spell.WandWeight = w.Weight;
                    }
                    else if (w.Type == "Divine")
                    {
                        spell.DivineScrollCost = w.Price;
                        spell.DivineScrollLevel = w.Level;
                        spell.DivineScrollWeight = w.Weight;
                    }
                    else if (w.Type == "Arcane")
                    {
                        spell.ArcaneScrollCost = w.Price;
                        spell.ArcaneScrollLevel = w.Level;
                        spell.ArcaneScrollWeight = w.Weight;
                    }
                }

            }

        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            List<SpecificItemChart> chartList = XmlListLoader<SpecificItemChart>.Load("APGItemChart.xml");
            foreach (SpecificItemChart chart in chartList)
            {
                chart.Major = FixChartNum(chart.Major, chart.Type);
                chart.Medium = FixChartNum(chart.Medium, chart.Type);
                chart.Minor = FixChartNum(chart.Minor, chart.Type);
                

                chart.Name = FixChartTextItem(chart.Name);
                chart.Cost = FixChartTextItem(chart.Cost);
            }
            XmlListLoader<SpecificItemChart>.Save(chartList, "APGItemChartOut.xml");

        }


        private string FixChartTextItem(string text)
        {
            if (text != null)
            {
                text = text.Trim();
                text = text.Trim('\t');
                if (text.Length == 0 || text == "—")
                {
                    text = null;
                }

            }
            return text;
        }

        private string FixChartItem(string text)
        {
            if (text != null)
            {
                text = text.Trim();
                text = text.Trim('\t');
                if (text.Length == 0 || text == "—")
                {
                    text = null;
                }
                
            }
            if (text != null)
            {
                Regex regNum = new Regex("(?<num1>[0-9]+)(\\—(?<num2>[0-9]+))?");

                text = regNum.Replace(text, delegate(Match m)
                {
                    if (m.Groups["num2"].Success)
                    {
                        int val1 = int.Parse(m.Groups["num1"].Value);
                        int val2 = int.Parse(m.Groups["num2"].Value);

                        int total = val2 - val1 + 1;

                        return total.ToString();
                    }
                    else
                    {
                        return "1";
                    }
                });

            }

            return text;
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            List<WeaponSpecialAbility> wsList = XmlListLoader<WeaponSpecialAbility>.Load("WeaponSpecialAbility.xml");

            foreach (WeaponSpecialAbility ws in wsList)
            {
                ws.Minor = FixChartNum(ws.Minor);
                ws.Medium = FixChartNum(ws.Medium);
                ws.Major = FixChartNum(ws.Major);
                ws.RangedMinor = FixChartNum(ws.RangedMinor);
                ws.RangedMedium = FixChartNum(ws.RangedMedium);
                ws.RangedMajor = FixChartNum(ws.RangedMajor);
            }

            XmlListLoader<WeaponSpecialAbility>.Save(wsList, "WeaponSpecialAbility2.xml");

        }

        string FixChartNum(String startVal, String type)
        {
            if (startVal == null)
            {
                return null;
            }
            if (type == "Wondrous Item")
            {
                if (startVal.Trim().Length > 0)
                {
                    return "1";
                }
                else
                {
                    return startVal;
                }
            }
            else
            {
                return FixChartNum(startVal);
            }
        }

        string FixChartNum(String startVal)
        {
            if (startVal == null)
            {
                return null;
            }
            Regex regNum = new Regex("(?<num1>[0-9]+)((—|-|–)(?<num2>[0-9]+))?");

            string text = regNum.Replace(startVal, delegate(Match m)
            {
                if (m.Groups["num2"].Success)
                {
                    int val1 = int.Parse(m.Groups["num1"].Value);
                    int val2 = int.Parse(m.Groups["num2"].Value);

                    int total = val2 - val1 + 1;

                    return total.ToString();
                }
                else
                {
                    return "1";
                }
            });

            return text.Trim();
        }


        private void GetSpellPropList(string filename, string prop, bool comboItem)
        {

            XDocument doc1 = XDocument.Load(filename);

            var vals = from c in doc1.Descendants()
                       where c.Element(prop) != null &&
                       (NoParenCheck.IsChecked == false || 
                        (Regex.Match( c.Element(prop).Value, "[()]").Success == false))

                       /*&& (
                        c.ElementValue("source") == "PFRPG Core" ||
                        c.ElementValue("Source") == "PFRPG Bestiary")*/
                       select new
                       {
                           Name = c.Element("name"),
                           Prop = c.Element(prop).Value
                       };

                                              
                        

            SortedSet<string> items = new SortedSet<string>();

            foreach (var c in vals)
            {
                items.Add(c.Prop.Trim().Trim(new char[] { ',' }));
            }

            string file = prop + ".txt";

            if (comboItem)
            {
                file = "combo." + file;
            }

            using (StreamWriter wr = new StreamWriter(file))
            {
                foreach (string s in items)
                {
                    string text = s;

                    if (comboItem)
                    {
                        text = "<ComboBoxItem Content=\"" + s + "\"/>";
                    }
                    wr.WriteLine(text);
                }
            }

            Process.Start(file);


        }


        private void spellFixButton_Click(object sender, RoutedEventArgs e)
        {


            List<string> source = new List<string>() { 
                    "AP 41", "AP 42", "AP 43", "AP 44", "AP 45", "AP 46", "Inner Sea World Guide", "Undead Revisited",
                    "Academy Of Secrets", "Tomb Of The Iron Medusa", "Cult Of The Ebon Destroyers", "Lost Cities Of Golarion"
                };

            XDocument doc1 = XDocument.Load("bestiary.xml");
            XDocument doc2 = XDocument.Load("monster_bestiary_full.xml");

            var vals = from c in doc2.Descendants("item")
                       select new
                       {
                           Name = c.Element("Name").Value,
                           SpellsKnown = c.ElementValue("SpellsKnown"),
                           SpellsPrepared = c.ElementValue("SpellsPrepared"),
                           SpellLikeAbilities = c.ElementValue("SpellLikeAbilities"),
                           Source = c.ElementValue("Source"),
                           Element = c
                       };

            int notfoundcount = 0;
            int namecommacount = 0;
            int namecommacount2 = 0;

            int foundparen = 0;
            int added = 0;

            foreach (var item in vals)
            {

                bool found = false;
                IEnumerable<XElement> val2 = from c in doc1.Descendants("Monster")
                          where String.Compare(c.Element("Name").Value,item.Name, true) == 0
                          select c;


                foreach (var node in val2)
                {
                    found = true;
                    if (item.SpellsKnown != null && item.SpellsKnown.Length > 0)
                    {
                        node.SetElementValue("SpellsKnown", item.SpellsKnown);
                    }

                    if (item.SpellsPrepared != null && item.SpellsPrepared.Length > 0)
                    {

                        node.SetElementValue("SpellsPrepared", item.SpellsPrepared);
                    }


                    if (item.SpellLikeAbilities != null && item.SpellLikeAbilities.Length > 0)
                    {

                        node.SetElementValue("SpellLikeAbilities", item.SpellLikeAbilities);
                    }

                }

                if (!found)
                {
                    string name = item.Name;
                    name = Regex.Replace(name, "\\(Giant\\)", "");
                    bool giant = name != item.Name;

                    name = Regex.Replace(name, " \\(.+\\)", "");
                    
                    string preswarm = name;
                    name = Regex.Replace(name, ", Swarm", "");
                    bool swarm =  (preswarm != name);


                    name = Regex.Replace(name, ", Common", "");


                    name = CMStringUtilities.DecommaText(name);
                    if (giant)
                    {
                        name = "Giant " + name.Trim();
                    }
                    if (swarm)
                    {
                        name = name.Trim() + " Swarm";
                    }

                    name = name.Trim();
                    

                    val2 = from c in doc1.Descendants("Monster")
                               where String.Compare(c.Element("Name").Value, name, true) == 0
                               select c;
                    
                     foreach (var node in val2)
                    {
                        found = true;
                         namecommacount++;

                         if (item.SpellsKnown != null && item.SpellsKnown.Length > 0)
                         {
                             node.SetElementValue("SpellsKnown", item.SpellsKnown);
                         }

                         if (item.SpellsPrepared != null && item.SpellsPrepared.Length > 0)
                         {

                             node.SetElementValue("SpellsPrepared", item.SpellsPrepared);
                         }


                         if (item.SpellLikeAbilities != null && item.SpellLikeAbilities.Length > 0)
                         {

                             node.SetElementValue("SpellLikeAbilities", item.SpellLikeAbilities);
                         }

                    }

                }


                if (!found)
                {
                                        string name = item.Name;
                    name = Regex.Replace(name, "\\(Giant\\)", "");
                    bool giant = name != item.Name;


                    name = Regex.Replace(name, "(?<start>.+?), (?<end>.+)", delegate(Match m) 
                    {
                        return CMStringUtilities.DecommaText(m.Groups["end"].Value);
                    });


                    if (giant)
                    {
                        name = "Giant " + name.Trim();
                    }


                    val2 = from c in doc1.Descendants("Monster")
                           where String.Compare(c.Element("Name").Value, name, true) == 0
                           select c;

                    foreach (var node in val2)
                    {
                        found = true;
                        namecommacount2++;

                        if (item.SpellsKnown != null && item.SpellsKnown.Length > 0)
                        {
                            node.SetElementValue("SpellsKnown", item.SpellsKnown);
                        }

                        if (item.SpellsPrepared != null && item.SpellsPrepared.Length > 0)
                        {

                            node.SetElementValue("SpellsPrepared", item.SpellsPrepared);
                        }


                        if (item.SpellLikeAbilities != null && item.SpellLikeAbilities.Length > 0)
                        {

                            node.SetElementValue("SpellLikeAbilities", item.SpellLikeAbilities);
                        }

                    }

                }


                if (!found)
                {
                    string name = item.Name;
                    name = Regex.Replace(name, ".+\\((?<name>.+)\\).*", delegate(Match m)
                    {
                        return m.Groups["name"].Value;
                    });
                    


                    val2 = from c in doc1.Descendants("Monster")
                           where String.Compare(c.Element("Name").Value, name, true) == 0
                           select c;

                    foreach (var node in val2)
                    {
                        found = true;
                        foundparen++;

                        if (item.SpellsKnown != null && item.SpellsKnown.Length > 0)
                        {
                            node.SetElementValue("SpellsKnown", item.SpellsKnown);
                        }

                        if (item.SpellsPrepared != null && item.SpellsPrepared.Length > 0)
                        {

                            node.SetElementValue("SpellsPrepared", item.SpellsPrepared);
                        }


                        if (item.SpellLikeAbilities != null && item.SpellLikeAbilities.Length > 0)
                        {

                            node.SetElementValue("SpellLikeAbilities", item.SpellLikeAbilities);
                        }

                    }

                }


                if (!found)
                {
                    if (item.Source == "d20pfsrd")
                    {
                        XElement x = new XElement(item.Element);

                        x.SetElementValue("Source", "PFRPG Bestiary");
                        x.Name = "Monster";

                        doc1.Root.Add(x);

                        added++;
                    }
                    else if (source.FindIndex(a => a == item.Source) != -1)
                    {

                        XElement x = new XElement(item.Element);

                        x.Name = "Monster";

                        doc1.Root.Add(x);

                        added++;
                    }

                    else
                    {
                        System.Console.WriteLine(item.Name + " - " + item.Source);
                        notfoundcount++;
                    }

                }
                

            }

            System.Console.WriteLine("Not Found: " + notfoundcount);
            System.Console.WriteLine("Found Decomma: " + namecommacount);
            System.Console.WriteLine("Found Remove Prefix: " + namecommacount2);
            System.Console.WriteLine("Found Paren: " + foundparen);
            System.Console.WriteLine("Added: " + added);

            doc1.Save("bestiary2.xml");

        }

        private void SpellPropButton_Click(object sender, RoutedEventArgs e)
        {
            if (SpellPropText.Text.Length > 0)
            {
                if (new FileInfo(FileNameText.Text).Exists)
                {

                    GetSpellPropList(FileNameText.Text, SpellPropText.Text, SpellPropCheckBox.IsChecked == true);
                }
            }
        }

        private void button2_Click_1(object sender, RoutedEventArgs e)
        {

            Feat.LoadFeats();

            StreamReader str = new StreamReader("NewFeat.txt");

            String bigText = str.ReadToEnd();

            List<Feat> feats = new List<Feat>();

            bigText = Regex.Replace(bigText, "(\\<a href[^\\>]+>)|(\\</a\\>)|\\</?i\\>", delegate(Match asd)
            {
                return "";
            });

            bigText = Regex.Replace(bigText, " ?\\<br/\\>", "\r\n");
            bigText = Regex.Replace(bigText, "&ndash;", "-");


            Regex x = new Regex("\\<h2 id\\=\"[-\\p{L}0-9()]+\"\\>(?<name>[-\\p{L} '0-9]+)( \\((?<type>[^)]+)\\))?\\</h2\\>[ \\t\\r\\n]*" +
                "\\<p\\>(?<short>([^\\<]|\\</?i\\>)+)\\</p\\>[ \\t\\r\\n]*" +
                "(\\<p\\>\\<b\\>Prerequisites?\\</b\\>: (?<prereq>[^\\<]+)\\</p\\>[ \\t\\r\\n]*)?" +
                "(\\<p\\>\\<b\\>Benefits?\\</b\\>: (?<benefit>[^\\<]+)\\</p\\>[ \\t\\r\\n]*)?" +
                "(?<beneextra>(\\<p\\>([^\\<]|\\</?i\\>)+\\</p\\>[ \\t\\r\\n]*)+)?" +
                "(\\<p\\>\\<b\\>Special\\</b\\>: (?<special>[^\\<]+)\\</p\\>[ \\t\\r\\n]*)?" +
                "(\\<p id=\"normal\"\\>\\<b\\>Normal\\</b\\>: (?<normal>[^\\<]+)\\</p\\>[ \\t\\r\\n]*)?" +
                "(\\<p id=\"note\"\\>\\<b\\>Note\\</b\\>: (?<note>[^\\<]+)\\</p\\>[ \\t\\r\\n]*)?" + 

                "");

            Match m = x.Match(bigText);

            int count = 0;
            int prereq = 0;
            int bene = 0;
            int beneextra = 0;
            int special = 0;
            int normal = 0;
            int note = 0;
            while (m.Success)
            {
                count++;
                if (m.Groups["prereq"].Success)
                {
                    prereq++;
                }
                if (m.Groups["benefit"].Success)
                {
                    bene++;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(m.Groups["name"]);
                }
                if (m.Groups["beneextra"].Success)
                {
                    beneextra++;
                }
                if (m.Groups["special"].Success)
                {
                    special++;

                }
                if (m.Groups["normal"].Success)
                {
                    normal++;
                }
                if (m.Groups["note"].Success)
                {
                    note++;
                }

                
                Feat f = new Feat();
                f.Name = m.Value("name");
                f.Summary = m.Value("short");

                string benefitText = m.Value("benefit").Trim();

                if (m.GroupSuccess("beneextra"))
                {
                    string bx = m.Value("beneextra");
                    bx = Regex.Replace(bx, "\\<p\\>", "\r\n");
                    bx = Regex.Replace(bx, "\t|\\</p\\>", "");
                    benefitText += bx;


                }
                if (m.GroupSuccess("note"))
                {
                    benefitText += "\r\nNote: " + m.Value("note");
                }

                benefitText = Regex.Replace(benefitText, "(            )|(\\t)", "");
                benefitText = benefitText.Trim(new char[] {'\r', '\n', ' ', '\t'});

                benefitText = Regex.Replace(benefitText, "(\\r\\n)*\\r\\n", "\r\n\r\n");

                f.Benefit = benefitText;
                f.Normal = m.Value("normal");
                f.Special = m.Value("special");
                f.Prerequistites = m.Value("prereq");

                if (m.GroupSuccess("type"))
                {
                    f.Type = m.Value("type");
                }
                else
                {
                    f.Type = "General";
                }

                f.Source = "APG";
                
                string name = m.Groups["name"].Value;
                if (!Feat.FeatMap.ContainsKey(name))
                {
                    System.Diagnostics.Debug.WriteLine(name);
                    feats.Add(f);
                }
                


                m = m.NextMatch();
            }

            

            XmlListLoader<Feat>.Save(feats, "NewFeats.xml");

            MessageBox.Show(count.ToString() + "\r\npre: " + prereq
                + "\r\nbene: " + bene

                + "\r\nextra: " + beneextra
                 + "\r\nspecial: " + special
                  + "\r\nnormal: " + normal
                   + "\r\nnote: " + note);


            


        }

        private void SpellCheck_Click(object sender, RoutedEventArgs e)
        {
            List<Spell> newsp = XmlListLoader<Spell>.Load("spells.xml");
            Dictionary<String, List<Spell>> spells = new Dictionary<string, List<Spell>>();



            foreach (Spell sp in newsp)
            {
                if (spells.ContainsKey(sp.Name))
                {
                }
                else
                {
                    spells[sp.Name] = new List<Spell>();
                }
                spells[sp.Name].Add(sp);
            }

            foreach (List<Spell> spl in from sl in spells.Values where sl.Count > 1 select sl)
            {
                System.Diagnostics.Debug.Write(spl[0].Name);
                foreach (Spell sp in spl)
                {
                    System.Diagnostics.Debug.Write(" / " + sp.source);
                }
                System.Diagnostics.Debug.WriteLine("");

            }
        }

    }


}

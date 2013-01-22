/*
 *  Program.cs
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
using System.Data.SQLite;
using System.IO;
using System.Xml.Linq;
using CombatManager;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace DetailsRipper
{
    class Program
    {
        static void Main(string[] args)
        {
            if (File.Exists("Details.db"))
            {
                File.Delete("Details.db");
            }

            SQLiteConnection cn = new SQLiteConnection("Data Source=Details.db");
            cn.Open();
            var v = cn.CreateCommand();
            v.CommandText = "Create Table Rules (ID integer primary key, details string)";
            v.ExecuteNonQuery();

            XDocument docRules = XDocument.Load("Rule.xml");

            var vals = docRules.Descendants("Rule");
            var t = cn.BeginTransaction();
           
            foreach (XElement x in vals)
            {
                var cm = cn.CreateCommand();
                cm.CommandText = "Insert into Rules (ID, details) values (?, ?)";
                var p1 = cm.CreateParameter();
                p1.Value = x.ElementValue("ID");
                cm.Parameters.Add(p1);
                var p2 = cm.CreateParameter();
                p2.Value = x.ElementValue("Details");
                cm.Parameters.Add(p2);
                cm.ExecuteNonQuery();
                x.Element("Details").Remove();
            }

            t.Commit();
           


            cn.Close();

            docRules.Save("RuleShort.xml");

            XDocument docMagic = XDocument.Load("MagicItems.xml");

            foreach (XElement x in docMagic.Descendants("MagicItem"))
            {
                if (x.Element("DescHTML") != null)
                {
                    x.Element("DescHTML").Remove();
                }
            }

            docMagic.Save("MagicItemsShort.xml");


            XDocument docSpells = XDocument.Load("Spells.xml");

            Dictionary<String, XElement> spellName = new Dictionary<string,XElement>();
            var eleme = docSpells.Descendants("Spell");
            List<XElement> spellsToRemove = new List<XElement>();
            foreach (XElement x in eleme)
            {
                if (x.Element("full_text") != null)
                {
                    x.Element("full_text").Remove();
                }

                String currentName = x.Element("name").Value;
                if (!spellName.ContainsKey(currentName))
                {
                    spellName[currentName] = x;
                }
                else
                {
                    XElement a = spellName[currentName];
                    XElement b = x;
                    if (a.Element("source") != null && b.Element("source") != null && a.Element("source").Value == b.Element("source").Value)
                    {
                        spellsToRemove.Add(x);
                        System.Diagnostics.Debug.WriteLine("Removed " + currentName);
                    }
                }
                   
            }

            foreach (XElement x in spellsToRemove)
            {
                x.Remove();
            }

            docSpells.Save("Spells.xml");


            Dictionary<string, XElement> monsterList = new Dictionary<string, XElement>();

            XDocument docMon = XDocument.Load("Bestiary.xml");

            List<XElement> removeAfter = new List<XElement>();

            foreach (XElement x in docMon.Descendants("Monster"))
            {
                if (x.Element("FullText") != null)
                {
                    x.Element("FullText").Remove();
                }

                FixNumbers(x);
                string name = x.Element("Name").Value;

                if (name == name.ToUpper())
                {
                    x.Element("Name").Value = name.ToLower().Capitalize();
                }

                XElement oldElement;
                string monName = x.Element("Name").Value;
                string source = x.Element("Source").Value;
                Debug.Assert(source != null);
                if (Regex.Match(source, "Tome of Horrors").Success)
                {
                    removeAfter.Add(x);
                    //x.Remove();
                }
                else if (monsterList.TryGetValue(monName, out oldElement))
                {
                    string oldSource = oldElement.ElementValue("Source");
                    string newSource = x.ElementValue("Source");

                    //System.Diagnostics.Debug.WriteLine(x.Element("Name").Value);
                    if (newSource == "Bestiary 3" || newSource == "PFRPG Bestiary 3")
                    {
                        oldElement.Remove();
                        //System.Diagnostics.Debug.WriteLine("Remove " + oldSource);
                        monsterList[monName] = x;
                    }
                    else if (oldSource == "Bestiary 3")
                    {
                        x.Remove();
                        //System.Diagnostics.Debug.WriteLine("Remove " + newSource);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("No replace: " + x.Element("Name").Value);
                    }

                }
                else
                {
                    monsterList[monName] = x;
                }
            }

            foreach (XElement x in removeAfter)
            {
                x.Remove();
            }


            docMon.Save("BestiaryShort.xml");

            File.Copy("BestiaryShort.xml", "..\\..\\..\\CombatManagerCore\\BestiaryShort.xml", true);

            docMon = XDocument.Load("NPC.xml");

            int dupcount = 0;
            List<string> dupNames = new List<string>();
            List<XElement> removeList = new List<XElement>();
            foreach (XElement x in docMon.Descendants("Monster"))
            {
                if (x.Element("FullText") != null)
                {
                    x.Element("FullText").Remove();
                }

                string name = x.Element("Name").Value;

                if (name == name.ToUpper())
                {
                    x.Element("Name").Value = name.ToLower().Capitalize();
                }

                FixNumbers(x);

                XElement oldElement;
                string monName = x.Element("Name").Value;
                if (monsterList.TryGetValue(monName, out oldElement))
                {
                    dupcount++;
                    dupNames.Add(monName);


                    string oldSource = oldElement.ElementValue("Source");
                    string newSource = x.ElementValue("Source");

                    if (oldSource == "Bestiary 3")
                    {
                        removeList.Add(x);
                        System.Diagnostics.Debug.WriteLine("Remove " + monName);
                    }
                }
                else
                {
                    monsterList[monName] = x;
                }
            }
            foreach (XElement x in removeList)
            {
                x.Remove();
            }
            dupNames.Sort();
            foreach (string dupName in dupNames)
            {

                System.Diagnostics.Debug.WriteLine("Dup NPC: " + dupName);
            }
            System.Diagnostics.Debug.WriteLine("Dup Count: " + dupcount);

            docMon.Save("NPCShort.xml");


            File.Copy("NPCShort.xml", "..\\..\\..\\CombatManagerCore\\NPCShort.xml", true);



        }


        public static void FixNumbers(XElement x)
        {
            x.Element("Will").FixNum();
            x.Element("Fort").FixNum();
            x.Element("Ref").FixNum();
            x.Element("Init").FixNum();
            x.Element("HP").FixNum();

        }

    }



    public static class XElementExt
    {
        public static void FixNum(this XElement x)
        {
            int outVal;
            if (!int.TryParse(x.Value, out outVal))
            {
                Regex reg = new Regex("-?[0-9]+");
                Match m = reg.Match(x.Value);

                x.Value = m.Value;
            }
        }

        public static string ElementValue(this XElement x, string name)
        {
            return x.Element(name) == null ? null : x.Element(name).Value;
        }
    }
}


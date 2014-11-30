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

        static SQLiteConnection cn;

        static void Main(string[] args)
        {
            if (File.Exists("Details.db"))
            {
                File.Delete("Details.db");
            }
            cn = new SQLiteConnection("Data Source=Details.db");
            cn.Open();
            CleanupRules();

            CleanupMagicItems();


            CleanupSpells();

            CleanupMonsters();



            cn.Close();

            CopyFileToCore("details.db");

        }

        static void CleanupRules()
        {
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


            SaveCopyFile(docRules,"RuleShort.xml");
        }

        static void CleanupMagicItems()
        {

            var v = cn.CreateCommand();
            CreateTable(v, "MagicItems", MagicItem.DetailsFields);

            XDocument docMagic = XDocument.Load("MagicItems.xml");

            var t = cn.BeginTransaction();

            string command = CreateCommand("MagicItems", MagicItem.DetailsFields);

            foreach (XElement x in docMagic.Descendants("MagicItem"))
            {
                if (x.Element("CL") != null)
                {
                    if (x.Element("CL").Value == "varies")
                    {
                        x.Element("CL").Value = "0";
                    }
                }


                TransferElementToDB(cn, command, x, MagicItem.DetailsFields);
            }

            t.Commit();

            docMagic.Save("MagicItemsShort.xml");
            CopyFileToCore("MagicItemsShort.xml");
        }

        static void TransferElementToDB(SQLiteConnection cn, string command, XElement x, List<String> fields)
        {
            var cm = cn.CreateCommand();
            cm.CommandText = command;
            var p1 = cm.CreateParameter();
            p1.Value = x.ElementValue("id");
            cm.Parameters.Add(p1);

            foreach (string s in fields)
            {

                var p2 = cm.CreateParameter();
                p2.Value = x.ElementValue(s);
                cm.Parameters.Add(p2);

                RemoveIfNotNull(x, s);
            }


            cm.ExecuteNonQuery();
        }

        static void CleanupMonsters()
        {

            var v = cn.CreateCommand();
            CreateTable(v, "Bestiary", Monster.MonsterDBFields);

            Dictionary<string, XElement> monsterList = new Dictionary<string, XElement>();

            XDocument docMon = XDocument.Load("Bestiary.xml");

            List<XElement> removeAfter = new List<XElement>();

            HashSet<String> names = new HashSet<string>();

            foreach (XElement x in docMon.Descendants("Monster"))
            {
                foreach (var va in x.Elements())
                {
                    names.Add(va.Name.LocalName);
                }

                CleanMonsterElements(x);

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

                    if (newSource == "Bestiary 3" || newSource == "PFRPG Bestiary 3")
                    {
                        oldElement.Remove();
                        monsterList[monName] = x;
                    }
                    else if (oldSource == "Bestiary 3")
                    {
                        x.Remove();
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

            foreach (String s in names)
            {
                System.Diagnostics.Debug.WriteLine(s);
            }

            foreach (XElement x in removeAfter)
            {
                x.Remove();
            }

            MoveMonsterFieldsToDB(docMon);
            XDocument doc2 = new XDocument(docMon);

            SplitDescendents(docMon, doc2, 1000);


            SaveCopyFile(docMon, "BestiaryShort.xml");
            SaveCopyFile(doc2, "BestiaryShort2.xml");

            docMon = XDocument.Load("NPC.xml");

            int dupcount = 0;
            List<string> dupNames = new List<string>();
            List<XElement> removeList = new List<XElement>();
            foreach (XElement x in docMon.Descendants("Monster"))
            {
                CleanMonsterElements(x);

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


            MoveMonsterFieldsToDB(docMon);
            doc2 = new XDocument(docMon);
            SplitDescendents(docMon, doc2, 1000);

            SaveCopyFile(docMon, "NPCShort.xml");
            SaveCopyFile(doc2, "NPCShort2.xml");
        }

        static void CreateTable(SQLiteCommand v, string name, IEnumerable<string> fields)
        {
            string command = "Create Table " + name + " (ID string primary key";
            foreach (string s in fields)
            {
                command += ", ";
                command += s;
            }
            command += ")";
            v.CommandText = command;
            v.ExecuteNonQuery();
        }

        static string CreateCommand(string table, IEnumerable<string> fields)
        {

            string command = "Insert into " + table + " (ID";

            string values = ") values (?";

            foreach (string s in fields)
            {
                command += ", " + s;
                values += ", ?";
            }
            command = command + values + ")";

            return command;
        }


        static void MoveMonsterFieldsToDB(XDocument doc)
        {
            var t = cn.BeginTransaction();

            string command = CreateCommand("Bestiary", Monster.MonsterDBFields);

            foreach (XElement x in doc.Descendants("Monster"))
            {

                TransferElementToDB(cn, command, x, Monster.MonsterDBFields);
            }

            t.Commit();
        }


        static void RemoveIfNotNull(XElement x, string name)
        {
            XElement xe = x.Element(name);
            if (xe != null)
            {
                xe.Remove();
            }
        }

        static void CleanMonsterElements(XElement x)
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

            string cr = x.Element("CR").Value;
            if (cr == "-")
            {
                x.Element("CR").Value = "0";
            }

            string cmb = x.Element("CMB").Value;
            if (cmb != null)
            {
                x.Element("CMB").Value = Monster.ChangeStartingModOrVal(cmb, 0);
            }

        }

        static List<String> _SpellAnnotationFields = new List<string>()
            {
                "name",
                "PotionWeight",
                "DivineScrollWeight",
                "ArcaneScrollWeight",
                "WandWeight",
                "PotionLevel",
                "PotionCost",
                "ArcaneScrollLevel",
                "ArcaneScrollCost",
                "DivineScrollLevel",
                "DivineScrollCost",
                "WandLevel",
                "WandCost",
                "Bonus"
            };

        static void CleanupSpells()
        {


            XDocument docSpells = XDocument.Load("Spells.xml");

            Dictionary<String, XElement> spellName = new Dictionary<string, XElement>();
            var eleme = docSpells.Descendants("Spell");
            List<XElement> spellsToRemove = new List<XElement>();
            foreach (XElement x in eleme)
            {
                if (x.Element("full_text") != null)
                {
                    x.Element("full_text").Remove();
                }

                string name = x.Element("name").Value;

                if (name == name.ToUpper())
                {
                    x.Element("name").Value = name.ToLower().Capitalize();
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
                    if (a.Element("source") != null && b.Element("source") != null && (
                            String.Compare(a.Element("source").Value, b.Element("source").Value, true) == 0) || a.Element("source").Value == "Inner Sea World Guide")
                    {
                        spellsToRemove.Add(b);
                        System.Diagnostics.Debug.WriteLine("Removed " + currentName);
                    }
                    else if (a.Element("source").Value.StartsWith("AP ") || b.Element("source").Value == "APG" || b.Element("source").Value == "Advanced Race Guide")
                    {
                        spellsToRemove.Add(a);
                    }
                    else
                    {

                        System.Diagnostics.Debug.WriteLine("Duplicate " + currentName + "/" + a.Element("source").Value + "/" + b.Element("source").Value + "/");
                    }
                }

            }

            foreach (XElement x in spellsToRemove)
            {
                x.Remove();
            }

            docSpells.Save("Spells.xml");





            var v = cn.CreateCommand();
            CreateTable(v, "Spells", Spell.DetailsFields);

            var t = cn.BeginTransaction();

            string command = CreateCommand("Spells", Spell.DetailsFields);

            foreach (XElement x in docSpells.Descendants("Spell"))
            {
                TransferElementToDB(cn, command, x, Spell.DetailsFields);
            }


            t.Commit();

            //SaveSpellAnnotations(docSpells);
            ApplySpellAnnotations(docSpells);
            

            SaveCopyFile(docSpells, "SpellsShort.xml");

        }

        private static void ApplySpellAnnotations(XDocument doc)
        {

            XDocument docAnnotations = XDocument.Load("SpellAnnotations.xml");

            Dictionary<string, XElement> dict = new Dictionary<string, XElement>();

            foreach (XElement anno in docAnnotations.Descendants("Spell"))
            {
                dict.Add(anno.Element("name").Value, anno);
            }

            foreach (XElement sp in doc.Descendants("Spell"))
            {
                if (dict.ContainsKey(sp.Element("name").Value))
                {
                    foreach (XElement val in dict[sp.Element("name").Value].Elements())
                    {
                        if (val.Name.LocalName != "name")
                        {
                            sp.Add(new XElement(val));
                        }
                    }
                }
            }
        }

        private static void SaveSpellAnnotations(XDocument docClone)
        {


            List<XElement> removeNodes = new List<XElement>();
            foreach (XElement sp in docClone.Descendants("Spell"))
            {
                List<XElement> remove = new List<XElement>();
                foreach (XElement x in sp.Elements())
                {
                    if (!_SpellAnnotationFields.Contains(x.Name.LocalName))
                    {
                        remove.Add(x);
                    }
                }
                foreach (XElement x in remove)
                {
                    x.Remove();
                }
                if (sp.Elements().Count() <= 1)
                {
                    removeNodes.Add(sp);
                }
            }
            foreach (XElement sp in removeNodes)
            {
                sp.Remove();
            }
            SaveCopyFile(docClone, "SpellAnnotations.xml");
        }

        private static void SaveCopyFile(XDocument doc, String filename)
        {

            doc.Save(filename);


            CopyFileToCore(filename);
        }

        private static void CopyFileToCore(string filename)
        {

            File.Copy(filename, "..\\..\\..\\CombatManagerCore\\" + filename, true);
        }

        private static void SplitDescendents(XDocument docMon, XDocument doc2, int splitValue)
        {

            int count = 0;
            List<XElement> removeList = new List<XElement>();
            foreach (XElement x in docMon.Descendants("Monster"))
            {
                if (count < splitValue)
                {
                    count++;
                }
                else
                {
                    removeList.Add(x);
                }
            }
            count = 0;
            foreach (XElement x in doc2.Descendants("Monster"))
            {
                if (count < splitValue)
                {
                    removeList.Add(x);
                    count++;
                }
            }
            foreach (XElement x in removeList)
            {
                x.Remove();
            }
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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.IO;
using System.Xml.Linq;
using CombatManager;

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

            foreach (XElement x in docSpells.Descendants("Spell"))
            {
                if (x.Element("full_text") != null)
                {
                    x.Element("full_text").Remove();
                }
            }

            docSpells.Save("Spells.xml");


            Dictionary<string, XElement> monsterList = new Dictionary<string, XElement>();

            XDocument docMon = XDocument.Load("Bestiary.xml");

            foreach (XElement x in docMon.Descendants("Monster"))
            {
                if (x.Element("FullText") != null)
                {
                    x.Element("FullText").Remove();
                }
                XElement oldElement;
                string monName = x.Element("Name").Value;
                if (monsterList.TryGetValue(monName, out oldElement))
                {
                    string oldSource = oldElement.ElementValue("Source");
                    string newSource = x.ElementValue("Source");

                    //System.Diagnostics.Debug.WriteLine(x.Element("Name").Value);
                    if (newSource == "Bestiary 3")
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

            docMon.Save("BestiaryShort.xml");

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




        }


    }
    public static class XElementExt
    {

        public static string ElementValue(this XElement x, string name)
        {
            return x.Element(name) == null ? null : x.Element(name).Value;
        }
    }
}


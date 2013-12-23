﻿using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
﻿using System.Diagnostics;
﻿using System.Linq;
﻿using System.Net.Sockets;
﻿using System.Runtime.Serialization.Formatters;
﻿using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Linq;
using Herolab;
using Ionic.Zip;
using System.Threading.Tasks;


namespace CombatManager
{
    public partial class Monster
    {
        public static List<Monster> FromFile(string filename)
        {
            List<Monster> returnMonsters = null;
            try
            {
                if (ZipFile.IsZipFile(filename))
                {
                    //returnMonsters = FromHeroLabZip(filename);
                    returnMonsters = FromHeroLabZipXml(filename);
                }
                else
                {

                    using (FileStream stream = new FileStream(filename, FileMode.Open))
                    {

                        XDocument doc = XDocument.Parse(new StreamReader(stream).ReadToEnd());


                        //look for herolab file
                        XElement it = doc.Root;

                        if (it.Name == "document")
                        {
                            string sig = it.Attribute("signature").Value;

                            if (sig == "Hero Lab Portfolio")
                            {
                                XElement prod = it.Element("product");

                                if (prod != null)
                                {

                                    int major = 0;
                                    int minor = 0;
                                    int patch = 0;


                                    major = GetAttributeIntValue(prod, "major");
                                    minor = GetAttributeIntValue(prod, "minor");
                                    patch = GetAttributeIntValue(prod, "patch");

                                    if (!CheckVersion(major, minor, patch, 3, 6, 7))
                                    {
                                        throw new MonsterParseException("Combat Manager requires files from a newer version of HeroLab." +
                                            "\r\nUpgrade HeroLab to the newest version, reload the file and save the file.");
                                    }

                                    returnMonsters = FromHeroLabFile(doc);
                                }
                            }
                        }
                        else
                        {
                            //look for PCGen file
                            //"/group-set/groups/group/combatants"
                            XElement el = doc.Root;
                            if (el.Name == "group-set")
                            {
                                el = el.Element("groups");
                                if (el != null)
                                {
                                    el = el.Element("group");
                                    if (el != null)
                                    {

                                        el = el.Element("combatants");
                                    }

                                    if (el != null)
                                    {
                                        returnMonsters = FromPCGenExportFile(doc);
                                    }
                                }
                            }
                        }





                        if (returnMonsters == null)
                        {
                            throw new MonsterParseException("Unrecognized file format");
                        }
                    }


                }
            }
            catch (IOException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                throw new MonsterParseException("CombatManager was not able to read the file.", ex);
            }
            catch (ArgumentException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                throw new MonsterParseException("CombatManger was not able to understand the file.", ex);
            }
            catch (FormatException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                throw new MonsterParseException("CombatManger was not able to understand the file.", ex);
            }
            catch (XmlException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                throw new MonsterParseException("CombatManger was not able to understand the file.", ex);
            }


            return returnMonsters;
        }

        private static bool CheckVersion(int major, int minor, int patch, int checkMajor, int checkMinor, int checkPatch)
        {
            if (major > checkMajor)
            {
                return true;
            }
            else if (major == checkMajor)
            {
                if (minor > checkMinor)
                {
                    return true;
                }
                else if (minor == checkMinor)
                {
                    if (patch >= checkPatch)
                    {
                        return true;
                    }
                }
            }
            return false;

        }

        private static List<Monster> FromHeroLabFile(XDocument doc)
        {
            List<Monster> monsters = new List<Monster>();


            //attempt to get the stats block
            foreach (XElement heroElement in doc.Root.Element("portfolio").Elements("hero"))
            {



                Monster monster = new Monster();

                monster.Name = heroElement.Attribute("heroname").Value;


                XElement statblock = heroElement.Element("statblock");

                if (statblock != null)
                {
                    string statsblock = statblock.Value;

                    ImportHeroLabBlock(statsblock, monster);

                    monsters.Add(monster);
                }




            }

            return monsters;

        }

        private static List<Monster> FromHeroLabZip(string filename)
        {

            List<Monster> monsters = new List<Monster>();

            ZipFile f = ZipFile.Read(filename);


            foreach (var en in from v in f.Entries where v.FileName.StartsWith("statblocks_text") && !v.IsDirectory select v)
            {
                MemoryStream m = new MemoryStream();

                StreamReader r = new StreamReader(en.OpenReader());
                String block = r.ReadToEnd();

                Monster monster = new Monster();
                ImportHeroLabBlock(block, monster, true);
                monsters.Add(monster);
            }

            return monsters;
        }


        private static List<Monster> FromPCGenExportFile(XDocument doc)
        {
            List<Monster> monsters = new List<Monster>();


            //attempt to get the stats block

            ///group-set/groups/group/combatants/combatant
            XElement combatant = doc.Root.Element("groups");
            combatant = combatant.Element("group");
            combatant = combatant.Element("combatants");
            combatant = combatant.Element("combatant");
            if (combatant != null)
            {

                Monster monster = new Monster();

                //get name
                monster.Name = combatant.Element("name").Value;

                Match m = null;

                //get type
                XElement it = combatant.Element("fullType");
                if (it != null)
                {
                    Regex regFull = new Regex("(?<align>" + AlignString + ")( )+(?<size>" + SizesString + ")( )*\r?\n?" +
                         "(?<type>" + TypesString + ")", RegexOptions.IgnoreCase);

                    m = regFull.Match(it.Value);

                    if (m.Success)
                    {
                        monster.Alignment = m.Groups["align"].Value;
                        if (monster.Alignment == "NN")
                        {
                            monster.Alignment = "N";
                        }
                        monster.Size = m.Groups["size"].Value;
                        monster.Type = m.Groups["type"].Value;
                    }
                }

                it = combatant;

                //get hp
                monster.HP = GetElementIntValue(it, "hitPoints");
                string hd = GetElementStringValue(it, "hitDice");

                hd = Regex.Replace(hd, "\\([0-9]+ hp\\)", "");
                hd = Regex.Replace(hd, "\\(|\\)", "");
                hd = "(" + hd.Trim() + ")";
                monster.HD = hd;

                //get stats
                string abilityScores = GetElementStringValue(it, "fullAbilities");

                string cha = GetElementStringValue(it, "cha");
                Regex regInt = new Regex("(?<val>[0-9]+) ");
                m = regInt.Match(cha);
                int chaInt;
                if (int.TryParse(m.Groups["val"].Value, out chaInt))
                {
                    abilityScores = Regex.Replace(abilityScores, "Cha (?<val>[0-9]+)",
                        delegate(Match ma)
                        {
                            return "Cha " + chaInt;
                        }
                    );
                }

                monster.abilitiyScores = abilityScores;




                monster.CR = GetElementStringValue(it, "challengeRating");

                monster.XP = GetCRValue(monster.CR).ToString();

                monster.Init = GetElementIntValue(it, "init-modifier");

                string ac = GetElementStringValue(it, "fullArmorClass");
                if (ac != null)
                {
                    ac = ac.Replace(":", "");
                    monster.AC = ac;
                }

                monster.ParseAC();


                monster.Fort = GetElementIntValue(it, "fortSave");
                monster.Ref = GetElementIntValue(it, "reflexSave");
                monster.Will = GetElementIntValue(it, "willSave");

                //load skills
                string skills = GetElementStringValue(it, "skills");
                skills = Regex.Replace(skills, ";( )*\r\n", ", ").Trim().Trim(new char[] { ',' });
                monster.skills = skills;


                //BAB, CMB, CMD
                SizeMods mods = SizeMods.GetMods(SizeMods.GetSize(monster.Size));
                monster.BaseAtk = GetElementIntValue(it, "baseAttack");


                monster.CMB = CMStringUtilities.PlusFormatNumber(monster.BaseAtk + AbilityBonus(monster.Strength) +
                    mods.Combat);

                monster.CMD = (monster.BaseAtk + mods.Combat + AbilityBonus(monster.Strength) + AbilityBonus(monster.Dexterity) + 10).ToString();


                string feats = GetElementStringValue(it, "feats");
                if (feats != null)
                {
                    feats = FixPCGenFeatList(feats);
                }
                monster.feats = feats;

                //load and fix speed
                string speed = GetElementStringValue(it, "speed");
                speed = Regex.Replace(speed, "Walk ", "");
                monster.Speed = speed.ToLower();



                //load senses
                string senses = GetElementStringValue(it, "senses").ToLower();

                //fix low light vision
                senses = Regex.Replace(senses, "low-light,", "low-light vision,");
                senses = Regex.Replace(senses, "^ *, perception", "Perception");

                //remove unneeded brackets
                senses = Regex.Replace(senses, "\\((?<val>[0-9]+) ft.\\)", delegate(Match ma)
                {
                    return ma.Groups["val"].Value + " ft.";
                }
                );

                //add perception
                Regex regSense = new Regex(", Listen (\\+|-)[0-9]+, Spot (\\+|-)[0-9]+", RegexOptions.IgnoreCase);
                int perception = 0;
                if (monster.SkillValueDictionary.ContainsKey("Perception"))
                {
                    perception = monster.SkillValueDictionary["Perception"].Mod;
                }
                senses = regSense.Replace(senses, "; Perception " + CMStringUtilities.PlusFormatNumber(perception));

                //set senses
                monster.Senses = senses;


                monster.SpecialAttacks = GetElementStringValue(it, "specialAttacks");

                string gear = GetElementStringValue(it, "possessions");
                if (gear != null)
                {
                    gear = Regex.Replace(gear, "\r\n", "");
                    gear = Regex.Replace(gear, "[\\];[]+ *", ", ");
                    monster.gear = gear;
                }


                string attacks = GetElementStringValue(it, "attack");
                attacks = Regex.Replace(attacks, "(\r?\n)|:|(\r)", "");

                List<String> meleeStrings = new List<string>();
                List<String> rangedStrings = new List<string>();

                Regex regAttack = new Regex("(?<weapon>[ ,\\p{L}0-9+]+)( \\((?<sub>[-+ \\p{L}0-9/]+)\\))? (?<bonus>(N/A|([-+/0-9]+))) (?<type>(melee|ranged)) (?<dmg>\\([0-9]+d[0-9]+(\\+[0-9]+)?(/[0-9]+-20)?(/x[0-9]+)?\\))");

                foreach (Match ma in regAttack.Matches(attacks))
                {
                    string bonus = ma.Groups["bonus"].Value;

                    if (bonus == "N/A")
                    {
                        bonus = "+0";
                    }

                    string weaponname = ma.Groups["weapon"].Value.Trim();

                    if (String.Compare(weaponname, "Longbow", true) == 0)
                    {
                        if (ma.Groups["sub"].Success)
                        {
                            string sub = ma.Groups["sub"].Value;

                            if (Regex.Match(sub, "composite", RegexOptions.IgnoreCase).Success)
                            {
                                weaponname = "composite longbow";
                            }
                        }
                    }

                    string attack = weaponname + " " + bonus +
                        " " + ma.Groups["dmg"].Value;

                    if (ma.Groups["type"].Value == "melee")
                    {
                        meleeStrings.Add(attack.Trim());
                    }
                    else
                    {
                        rangedStrings.Add(attack.Trim());
                    }
                }

                string melee = "";

                foreach (string attack in meleeStrings)
                {
                    if (melee.Length > 0)
                    {
                        melee += " or ";
                    }
                    melee += attack;
                }

                monster.Melee = melee;


                string ranged = "";

                foreach (string attack in rangedStrings)
                {
                    if (ranged.Length > 0)
                    {
                        ranged += " or ";
                    }
                    ranged += attack;
                }

                monster.Ranged = ranged;


                monsters.Add(monster);


            }
            return monsters;
        }
        private static List<Monster> FromHeroLabZipXml(string filename)
        {

            List<Monster> hLmonsters = new List<Monster>();
            

            using (ZipFile file = ZipFile.Read(filename))
            {

                foreach (
                    var entry in        
                        from zipedfile in file.Entries where zipedfile.FileName.StartsWith("statblocks_xml") && !zipedfile.IsDirectory select zipedfile)
                {
                    using (var ms = new MemoryStream())
                    {
                        
                        entry.Extract(ms); // extract uncompressed content into a memorystream 
                        ms.Seek(0, SeekOrigin.Begin);// rewind the pointer the top of the stream
                        XmlSerializer Ser = new XmlSerializer(typeof(document)); //get a serializer
                        document doc = (document)Ser.Deserialize(ms);// Create [document] from the Memory Stream [Don't forget the cast dumbass]
                        Monster monster = new Monster();
                        ImportHeroLabxml(doc, monster, true);
                        hLmonsters.Add(monster);

                    }
                }
            }

            return hLmonsters;
        }

        private static void ImportHeroLabxml(document doc, Monster monster, bool b)
        {

            foreach (var Character in doc.@public.character)
            {
                
                    monster.name = (Character.name ?? "No Name Defined");
                    monster.race = (Character.race.name ?? "No Race Defined");
                    monster.alignment = (Character.alignment.name ?? "No Alignment Defined");
                    monster.size = (Character.size.name ?? "No Size Defined");
                    monster.space = (Character.size.space.value ?? "No Space Defined");
                    monster.reach = (Character.size.reach.value ?? "No Space Defined");
                    monster.cr = (Character.challengerating != null ? Character.challengerating.value : "No CR Defined");
                    monster.xp = (Character.xpaward != null ? Character.xpaward.value : "No XP Defined");
                    monster.className = (Character.classes != null ? Character.classes.summary : "No Classes Defined");

                    if (Character.type != null)
                    {
                        foreach (var type in Character.types.type)
                        {
                            monster.type = type.name;
                        }
                    }
                    if (Character.type != null)
                    {
                        foreach (var subtype in Character.subtypes.subtype)
                        {
                            monster.subType += subtype.name;
                        }
                    }
                    if (Character.senses.special != null)
                    {
                        foreach (var Sense in Character.senses.special)
                        {
                            monster.senses += (string.IsNullOrEmpty(monster.senses) ? Sense.shortname : ", " + Sense.shortname);
                        }
                    }

                    monster.hd = (Character.health.hitdice ?? "No HD Defined");
                    if (Character.health.hitpoints != null)
                    {
                        int.TryParse(Character.health.hitpoints, out monster.hp);
                    
                    }
                    monster.gender = (Character.personal != null ?Character.personal.gender.ToString(): "No Gender Defined");
                    monster.description_visual = (Character.personal.description.Value ?? "No Visual Despription");
                    if (Character.languages != null)
                    {
                        foreach (var language in Character.languages.language)
                        {
                            monster.languages += (string.IsNullOrEmpty(monster.languages) ? language.name : ", " + language.name);
                        }
                    }
                
                    monster.statsParsed = true;
                    if (Character.attributes != null)
                    {
                    
                        foreach (var stat in Character.attributes.attribute)
	                        {
	                            switch (stat.name)
	                            {
	                                case "Strength":
	                                {
	                                    monster.strength = Int32.Parse(stat.attrvalue.modified);
                                        break;
	                                }
                                    case "Dexterity":
	                                {
	                                    monster.dexterity = Int32.Parse(stat.attrvalue.modified);
                                        break;
	                                }
                                    case "Constitution":
	                                {
	                                    monster.constitution = Int32.Parse(stat.attrvalue.modified);
                                        break;
	                                }
                                    case "Intelligence":
	                                {
                                        monster.intelligence = Int32.Parse(stat.attrvalue.modified);
                                        break;
	                                }
                                    case "Wisdom":
	                                {
                                        monster.wisdom = Int32.Parse(stat.attrvalue.modified);
                                        break;
	                                }
                                    case "Charisma":
	                                {
                                        monster.charisma = Int32.Parse(stat.attrvalue.modified);
                                        break;
	                                }
	                            }
                                
	                        }
                    }
             
                    if (Character.saves == null) continue;
                        foreach (var save in Character.saves.save)
                        {
                            switch (save.abbr)
                            {
                                case "Fort":
                                {
                                    int.TryParse(save.save, out monster.fort);
                                    break;
                                }
                                case "Ref":
                                {
                                    int.TryParse(save.save, out monster.reflex);
                                    break;
                                }
                                case "Will":
                                {
                                    int.TryParse(save.save, out monster.will);
                                    break;
                                }                              
                            }
                        }

                    if (Character.immunities.special != null)
                    {
                        foreach (var Immunity in Character.immunities.special)
                        {
                            monster.immune += (string.IsNullOrEmpty(monster.immune) ? Immunity.shortname : ", " + Immunity.shortname);
                        }
                    }
                    if (Character.resistances.special != null)
                    {
                        foreach (var Resistance in Character.resistances.special)
                        {
                            monster.resist += (string.IsNullOrEmpty(monster.resist) ? Resistance.shortname : ", " + Resistance.shortname);
                        }
                    }
                    if (Character.weaknesses.special != null)
                    {
                        foreach (var Weakness in Character.weaknesses.special)
                        {
                            monster.weaknesses += (string.IsNullOrEmpty(monster.weaknesses) ? Weakness.shortname : ", " + Weakness.shortname);
                        }
                    }
              
                    monster.acParsed = true;

                    if (Character.armorclass != null)
                    {
                        int.TryParse(Character.armorclass.ac, out monster.fullAC);

                        int.TryParse(Character.armorclass.touch, out monster.touchAC);

                        int.TryParse(Character.armorclass.flatfooted, out monster.flatFootedAC);

                        int.TryParse(Character.armorclass.fromarmor, out monster.armor);
  
                        int.TryParse(Character.armorclass.fromshield, out monster.shield);
   
                        int.TryParse(Character.armorclass.fromdeflect, out monster.deflection);
             
                        int.TryParse(Character.armorclass.fromdodge, out monster.dodge);
             
                        int.TryParse(Character.armorclass.fromnatural, out monster.naturalArmor);
                    }
                
                    monster.cmb = (Character.maneuvers.cmb ?? "10");
                    monster.cmd = (Character.maneuvers.cmd ?? "10");
                    if(Character.attack.baseattack != null)
                    {
                        int.TryParse(Character.attack.baseattack, out monster.baseAtk);
                    }
                    if (Character.initiative != null)
                    {
                        int.TryParse(Character.initiative.total, out monster.init);
                    }
                    if (Character.feats == null) continue;
                        foreach (var feat in Character.feats.feat)
                        {

                            if (feat.name.Contains("- All"))
                            {
                                string fixedfeat = feat.name.Replace(" - All", "");
                                monster.AddFeat(fixedfeat);

                            }
                            else
                            {
                                monster.AddFeat(feat.name);
                            }


                        }
                    //if (Character.skills.skill != null)
                    //{
                    //    foreach (var Skill in Character.skills.skill)
                    //    {
                    //        monster.AddOrChangeSkill(Skill.name, Int32.Parse(Skill.value));
                    //    }
                    //}
                        var sb = new StringBuilder();
                    foreach (var spellclass in Character.spellclasses.spellclass)
                    {
                    
                        //if (Character.spellsknown.spell != null)
                        //    {

                        //        var list = Character.spellsknown.spell.Select(Spellitem => Spellitem).ToList();

                        //        foreach (var spell in list)
                        //        {
                        //            sb.Append(spell.@class + ",");

                        //        }
                        //        monster.spellsKnown = sb.ToString();

                        //    }
                        if (spellclass.spells != "Spontaneous")
                                {

                                    var list = new List<Herolab.Spell>();
                                    list = Character.spellsmemorized.spell.ToList();
                                    //var x = from spell in list
                                    //   where spell.level == "1"
                                    //   orderby spell.name
                                    //   select spell;
                                
                                    var x = list.Where(spell => spell.@class == spellclass.name).OrderBy(spell => spell.level).ThenBy(spell => spell.name);
                                
                                    List<Class> acClasses = Character.classes.@class.ToList();
                                    var Concentration = from acClass in acClasses
                                        where acClass.name == spellclass.name
                                        select new {acClass.concentrationcheck, acClass.casterlevel};
                                    foreach (var VARIABLE in Concentration)
                                    {

                                        sb.Append(spellclass.name + " Spells Prepared " + "(CL "+ VARIABLE.casterlevel + "; concentration " + VARIABLE.concentrationcheck + ") ");
                                        string tracker = "";
                                        foreach (var spell in x)
                                        {
                                            if (tracker != spell.level)
                                            {

                                                sb.Append(spell.level + "-");
                                            }

                                            switch (spell.level)
                                            {
                                                case "0":
                                                    sb.Append(spell.range != "Personal" ? spell.name + " (DC " + spell.dc + "), " : "");
                                                    break;
                                                case "1":
                                                    sb.Append(spell.range != "Personal" ? spell.name + " (DC " + spell.dc + "), " : "");
                                                    break;
                                                case "2":
                                                    sb.Append(spell.range != "Personal" ? spell.name + " (DC " + spell.dc + "), " : "");
                                                    break;
                                                case "3":
                                                    sb.Append(spell.range != "Personal" ? spell.name + " (DC " + spell.dc + "), " : "");
                                                    break;
                                                case "4":
                                                    sb.Append(spell.range != "Personal" ? spell.name + " (DC " + spell.dc + "), " : "");
                                                    break;
                                                case "5":
                                                    sb.Append(spell.range != "Personal" ? spell.name + " (DC " + spell.dc + "), " : "");
                                                    break;
                                                case "6":
                                                    sb.Append(spell.range != "Personal" ? spell.name + " (DC " + spell.dc + "), " : "");
                                                    break;
                                                case "7":
                                                    sb.Append(spell.range != "Personal" ? spell.name + " (DC " + spell.dc + "), " : "");
                                                    break;
                                                case "8":
                                                    sb.Append(spell.range != "Personal" ? spell.name + " (DC " + spell.dc + "), " : "");
                                                    break;
                                                case "9":
                                                    sb.Append(spell.range != "Personal" ? spell.name + " (DC " + spell.dc + "), " : "");
                                                    break;
                                            
                                            }
                                        
                                            tracker = spell.level;
                                        }
                                    }
                                
                                }


                    }
                    monster.spellsPrepared = sb.ToString();
                    monster.ParseSpellsPrepared();
                }

            

        }
        private static string HeroLabStatRegexString(string stat)
        {
            return StringCapitalizer.Capitalize(stat) + " ([0-9]+/)?(?<" + stat.ToLower() + ">([0-9]+|-))";
        }
        private static void ImportHeroLabBlock(string statsblock, Monster monster, bool readNameBlock = false)
        {
            if (readNameBlock)
            {
                StringReader reader = new StringReader(statsblock);
                String name = reader.ReadLine();
                int loc = name.IndexOf('\t');
                if (loc != -1)
                {
                    name = name.Substring(0, loc);
                }
                monster.Name = name;
            }


            //System.Diagnostics.Debug.WriteLine(statsblock);


            String strStatSeparator = ",[ ]+";

            //get stats
            string statsRegStr = HeroLabStatRegexString("Str") + strStatSeparator +
                 HeroLabStatRegexString("Dex") + strStatSeparator +
                  HeroLabStatRegexString("Con") + strStatSeparator +
                   HeroLabStatRegexString("Int") + strStatSeparator +
                    HeroLabStatRegexString("Wis") + strStatSeparator +
                     HeroLabStatRegexString("Cha");


            Regex regStats = new Regex(statsRegStr);



            Match m = regStats.Match(statsblock);
            if (m.Success)
            {
                monster.AbilitiyScores = "Str " + m.Groups["str"].Value +
                                         ", Dex " + m.Groups["dex"].Value +
                                         ", Con " + m.Groups["con"].Value +
                                         ", Int " + m.Groups["int"].Value +
                                         ", Wis " + m.Groups["wis"].Value +
                                         ", Cha " + m.Groups["cha"].Value;
            }
            else
            {
                string StatCollection = "";
                const string RegstatsRegStrength = ("Str ([0-9]+/)?(?<str>([0-9]+|-))");
                const string RegstatsRegDexterity = ("Dex ([0-9]+/)?(?<dex>([0-9]+|-))");
                const string RegstatsRegConstitution = ("Con ([0-9]+/)?(?<con>([0-9]+|-))");
                const string RegstatsRegIntelligence = ("Int ([0-9]+/)?(?<int>([0-9]+|-))");
                const string RegstatsRegWisdom = ("Wis ([0-9]+/)?(?<wis>([0-9]+|-))");
                const string RegstatsRegCharisma = ("Cha ([0-9]+/)?(?<cha>([0-9]+|-))");

                regStats = new Regex(RegstatsRegStrength);
                Match regexMatchStr = regStats.Match(statsblock);
                StatCollection = regexMatchStr.Success ? "Str " + regexMatchStr.Groups["str"].Value : "Str -";

                regStats = new Regex(RegstatsRegDexterity);
                Match regexMatchDex = regStats.Match(statsblock);
                StatCollection = StatCollection + (regexMatchDex.Success ? ", Dex " + regexMatchDex.Groups["dex"].Value : ", Dex -");

                regStats = new Regex(RegstatsRegConstitution);
                Match regexMatchCon = regStats.Match(statsblock);
                StatCollection = StatCollection + (regexMatchCon.Success ? ", Con " + regexMatchCon.Groups["con"].Value : ", Con -");

                regStats = new Regex(RegstatsRegIntelligence);
                Match regexMatchInt = regStats.Match(statsblock);
                StatCollection = StatCollection + (regexMatchInt.Success ? ", Int " + regexMatchInt.Groups["int"].Value : ", Int -");

                regStats = new Regex(RegstatsRegWisdom);
                Match regexMatchWis = regStats.Match(statsblock);
                StatCollection = StatCollection + (regexMatchWis.Success ? ", Wis " + regexMatchWis.Groups["wis"].Value : ", Wis -");

                regStats = new Regex(RegstatsRegCharisma);
                Match regexMatchCha = regStats.Match(statsblock);
                StatCollection = StatCollection + (regexMatchCha.Success ? ", Cha " + regexMatchCha.Groups["cha"].Value : ", Cha -");

                monster.AbilitiyScores = StatCollection;

            }

            Regex regCR = new Regex("CR (?<cr>[0-9]+(/[0-9]+)?)\r\n");
            m = regCR.Match(statsblock);
            if (m.Success)
            {
                monster.CR = m.Groups["cr"].Value;

                monster.XP = GetCRValue(monster.CR).ToString();
            }



            //CN Medium Humanoid (Orc)
            string sizesText = SizesString;

            string typesText = TypesString;



            Regex regeAlSizeType = new Regex("(?<align>" + AlignString + ") (?<size>" + sizesText +
                ") (?<type>" + typesText + ") *(\\(|\r\n)");
            m = regeAlSizeType.Match(statsblock);

            if (m.Success)
            {
                monster.Alignment = m.Groups["align"].Value;
                if (monster.Alignment == "NN")
                {
                    monster.Alignment = "N";
                }
                monster.Size = m.Groups["size"].Value;
                monster.Type = m.Groups["type"].Value.ToLower();
            }
            else
            {
                monster.Alignment = "N";
                monster.Size = "Medium";
                monster.Type = "humanoid";
            }

            string raceClass = GetLine("Male", statsblock, true);
            if (raceClass == null)
            {
                raceClass = GetLine("Female", statsblock, true);
                monster.Gender = "Female";
            }
            else
            {
                monster.Gender = "Male";
            }

            if (raceClass != null)
            {
                m = Regex.Match(raceClass, "(?<race>[-\\p{L}]+) (?<class>.+)?");

                if (m.Success)
                {
                    monster.Race = m.Groups["race"].Value;
                    if (m.Groups["class"].Success)
                    {
                        monster.Class = m.Groups["class"].Value;
                    }
                }

            }



            //init, senses, perception

            //Init +7; Senses Darkvision (60 feet); Perception +2
            Regex regSense = new Regex("Init (?<init>(\\+|-)[0-9]+)(; Senses )((?<senses>.+)(;|,) )?Perception (?<perception>(\\+|-)[0-9]+)");
            m = regSense.Match(statsblock);

            if (m.Groups["init"].Success)
            {
                monster.Init = int.Parse(m.Groups["init"].Value);
            }
            else
            {
                monster.Init = 0;
            }
            monster.Senses = "";
            if (m.Groups["senses"].Success)
            {
                monster.Senses += m.Groups["senses"].Value + "; ";
            }
            int perception = 0;

            if (m.Groups["perception"].Success)
            {
                perception = int.Parse(m.Groups["perception"].Value);
            }

            monster.Senses += "Perception " + CMStringUtilities.PlusFormatNumber(perception);

            Regex regArmor = new Regex("(?<ac>AC -?[0-9]+, touch -?[0-9]+, flat-footed -?[0-9]+) +(?<mods>\\([-\\p{L}0-9, +]+\\))?", RegexOptions.IgnoreCase);
            m = regArmor.Match(statsblock);
            monster.AC = m.Groups["ac"].Value;
            if (m.Groups["mods"].Success)
            {
                monster.AC_Mods = m.Groups["mods"].Value;
            }
            else
            {
                monster.AC_Mods = "";
            }


            Regex regHP = new Regex("hp (?<hp>[0-9]+) ((?<hd>\\([-+0-9d]+\\))|(\\(\\)))");
            m = regHP.Match(statsblock);
            if (m.Groups["hp"].Success)
            {
                monster.HP = int.Parse(m.Groups["hp"].Value);
            }
            else
            {
                monster.HP = 0;
            }
            if (m.Groups["hd"].Success)
            {
                monster.HD = m.Groups["hd"].Value;
            }
            else
            {
                monster.HD = "0d0";
            }

            Regex regSave = new Regex("Fort (?<fort>[-+0-9]+)( \\([-+0-9]+bonus vs. [- \\p{L}]+\\))?, Ref (?<ref>[-+0-9]+), Will (?<will>[-+0-9]+)");
            m = regSave.Match(statsblock);
            if (m.Success)
            {
                monster.Fort = int.Parse(m.Groups["fort"].Value);
                monster.Ref = int.Parse(m.Groups["ref"].Value);
                monster.Will = int.Parse(m.Groups["will"].Value);
            }
            else
            {
                monster.Fort = 0;
                monster.Ref = 0;
                monster.Will = 0;
            }

            int defStart = m.Index + m.Length;

            Regex endLine = new Regex("(?<line>.+)");
            m = endLine.Match(statsblock, defStart + 1);
            String defLine = m.Value;

            //string da = FixHeroLabDefensiveAbilities(GetLine("Defensive Abilities", statsblock, true));
            monster.DefensiveAbilities = GetItem("Defensive Abilities", defLine, true);
            monster.Resist = GetItem("Resist", defLine, false);
            monster.Immune = GetItem("Immune", defLine, false);
            monster.SR = GetItem("SR", defLine, false);

            /*Regex regResist = new Regex("(?<da>.+); Resist (?<resist>.+)");

            if (da != null)
            {
                m = regResist.Match(da);

                if (m.Success)
                {
                    monster.DefensiveAbilities = m.Groups["da"].Value;
                    monster.Resist = m.Groups["resist"].Value;
                }
                else
                {
                    monster.DefensiveAbilities = da;
                }
            }*/

            monster.Weaknesses = GetLine("Weakness", statsblock, false);


            monster.Speed = GetLine("Spd", statsblock, false);
            if (monster.Speed == null)
            {

                monster.Speed = GetLine("Speed", statsblock, false);
            }


            monster.SpecialAttacks = GetLine("Special Attacks", statsblock, true);

            Regex regBase = new Regex("Base Atk (?<bab>[-+0-9]+); CMB (?<cmb>[^;]+); CMD (?<cmd>.+)");
            m = regBase.Match(statsblock);

            if (m.Success)
            {
                monster.BaseAtk = int.Parse(m.Groups["bab"].Value);
                monster.CMB = m.Groups["cmb"].Value;
                monster.CMD = m.Groups["cmd"].Value;
            }
            else
            {
                monster.BaseAtk = 0;
                monster.CMB = "+0";
                monster.CMD = "10";
            }

            monster.Feats = FixHeroLabFeats(GetLine("Feats", statsblock, true));

            monster.Skills = GetLine("Skills", statsblock, true);

            monster.Languages = GetLine("Languages", statsblock, false);

            monster.SQ = GetLine("SQ", statsblock, true);
            if (monster.SQ != null)
            {
                monster.SQ = monster.SQ.ToLower();
            }
            monster.Gear = GetLine("Gear", statsblock, true);
            if (monster.Gear == null)
            {
                monster.Gear = GetLine("Combat Gear", statsblock, true);
            }
            string space = GetLine("Space", statsblock, true);
            if (space != null)
            {
                m = Regex.Match(space, "(?<space>[0-9]+ ?ft\\.)[;,] +Reach +(?<reach>[0-9]+ ?ft\\.)");
                if (m.Success)
                {
                    monster.Space = m.Groups["space"].Value;
                    monster.Reach = m.Groups["reach"].Value;
                }
            }




            Regex regLine = new Regex("(?<text>.+)\r\n");

            Regex regSpecial = new Regex("(SPECIAL ABILITIES|Special Abilities)\r\n(-+)\r\n(?<special>(.|\r|\n)+)((Created With Hero Lab)|(Hero Lab))");
            m = regSpecial.Match(statsblock);
            if (m.Success)
            {
                string special = m.Groups["special"].Value;

                //parse special string
                //fix template text
                special = Regex.Replace(special, "Granted by [- \\p{L}]+ heritage\\.\r\n\r\n", "");

                MatchCollection matches = regLine.Matches(special);

                Regex regWeaponTraining = new Regex("Weapon Training: (?<group>[\\p{L}]+) \\+(?<value>[0-9]+)");
                Regex regSR = new Regex("Spell Resistance \\((?<SR>[0-9]+)\\)");
                Regex regSpecialAbility = new Regex("(?<name>[-\\.+, \\p{L}0-9:]+)"
                    + "( \\((?<mod>[-+][0-9]+)\\))?"
                    + "( [0-9]+')?"
                    + "( \\(CMB (?<CMB>[0-9]+)\\))?"
                    + "( \\((?<AtWill>At will)\\))?"
                    + "( \\((?<daily>[0-9]+)/day\\))?"
                    + "( \\(DC (?<DC>[0-9]+)\\))?"
                    + "( \\((?<othertext>[0-9\\p{L}, /]+)\\))?"
                    + " \\((?<type>(Ex|Su|Sp)(, (?<cp>[0-9]+) CP)?)\\) (?<text>.+)");

                foreach (Match ma in matches)
                {
                    string text = ma.Groups["text"].Value;

                    //check for weapon training
                    Match lm = regWeaponTraining.Match(text);

                    if (lm.Success)
                    {
                        string group = lm.Groups["group"].Value;
                        int val = int.Parse(lm.Groups["value"].Value);

                        monster.AddWeaponTraining(group, val);
                    }
                    if (!lm.Success)
                    {
                        lm = regSR.Match(text);

                        if (lm.Success)
                        {
                            monster.SR = lm.Groups["SR"].Value;

                        }
                    }
                    if (!lm.Success)
                    {
                        lm = regSpecialAbility.Match(text);

                        if (lm.Success)
                        {
                            SpecialAbility sa = new SpecialAbility();
                            sa.Name = lm.Groups["name"].Value;
                            sa.Type = lm.Groups["type"].Value;
                            sa.Text = lm.Groups["text"].Value;

                            if (lm.Groups["AtWill"].Success)
                            {
                                sa.Text += " (at will)";
                            }
                            else if (lm.Groups["Daily"].Success)
                            {
                                sa.Text += " (" + lm.Groups["Daily"].Value + "/day)";
                            }

                            if (lm.Groups["DC"].Success)
                            {
                                sa.Text += " (DC " + lm.Groups["DC"].Value + ")";
                            }


                            monster.SpecialAbilitiesList.Add(sa);

                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine(text);
                        }

                    }


                }
            }

            string endAttacks = "[\\p{L}]+ Spells (Known|Prepared)|Special Attacks|Spell-Like Abilities|-------|Space [0-9]";

            Regex regMelee = new Regex("\r\nMelee (?<melee>(.|\r|\n)+?)\r\n(Ranged|" + endAttacks + ")");

            m = regMelee.Match(statsblock);

            if (m.Success)
            {
                string attacks = m.Groups["melee"].Value;

                monster.Melee = FixHeroLabAttackString(attacks);

            }

            Regex regRanged = new Regex(
                "\r\nRanged (?<ranged>(.|\r|\n)+?)\r\n(" + endAttacks + ")");

            m = regRanged.Match(statsblock);

            if (m.Success)
            {
                string attacks = m.Groups["ranged"].Value;

                monster.Ranged = FixHeroLabAttackString(attacks);
            }

            monster.SpellLikeAbilities = GetLine("Spell-Like Abilities", statsblock, false);

            Regex regSpells = new Regex(
                "\r\n[ \\p{L}]+ (?<spells>Spells Known (.|\r|\n)+?)\r\n------");

            m = regSpells.Match(statsblock);
            if (m.Success)
            {
                string spells = m.Groups["spells"].Value;

                monster.SpellsKnown = spells;
            }
        }
        private static string FixHeroLabAttackString(string text)
        {


            string attacks = text;


            attacks = Weapon.ReplaceOriginalWeaponNames(attacks, false);

            attacks = attacks.ToLower();

            attacks = Regex.Replace(attacks, "and\r\n  ", "or");
            attacks = Regex.Replace(attacks, "/20/", "/");
            attacks = Regex.Replace(attacks, "/20\\)", ")");
            attacks = Regex.Replace(attacks, " \\(from armor\\)", "");

            return attacks;
        }
        private static string FixHeroLabFeats(string text)
        {

            string returnText = text;

            if (returnText != null)
            {
                returnText = Regex.Replace(returnText, " ([-+][0-9]+)(/[-+][0-9]+)*", "");

                returnText = Regex.Replace(returnText, " \\([0-9]+d[0-9]+\\)", "");
                returnText = Regex.Replace(returnText, " \\([0-9]+/day\\)", "");
            }

            return returnText;
        }
        private static string FixHeroLabDefensiveAbilities(string text)
        {
            string returnText = text;

            if (returnText != null)
            {
                returnText = Regex.Replace(returnText, " \\(Lv >=[0-9]+\\)", "");
            }

            return returnText;
        }
        private static string FixHeroLabList(string text)
        {

            string returnText = text;

            if (returnText != null)
            {
                returnText = ReplaceHeroLabSpecialChar(returnText);
                returnText = Weapon.ReplaceOriginalWeaponNames(returnText, false);
                returnText = ReplaceColonItems(returnText);

            }


            return returnText;
        }
        private static string ReplaceHeroLabSpecialChar(string text)
        {
            string returnText = text;

            if (returnText != null)
            {
                returnText = returnText.Replace("&#151;", "-");
            }

            return returnText;

        }
        private static string ReplaceColonItems(string text)
        {
            Regex reg = new Regex(": (?<val>[-+/. \\p{L}0-9]+?)(?<mod> (\\+|-)[0-9]+)?((?<comma>,)|\r|\n|$)");

            return reg.Replace(text, delegate(Match m)
            {
                string val = " (" + m.Groups["val"] + ")";

                if (m.Groups["mod"].Success)
                {
                    val += m.Groups["mod"].Value;
                }


                if (m.Groups["comma"].Success)
                {
                    val += ",";
                }

                return val;
            });

        }




    }
    
}

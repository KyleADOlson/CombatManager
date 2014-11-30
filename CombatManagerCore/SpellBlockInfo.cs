/*
 *  SpellBlockInfo.cs
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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace CombatManager
{
    public class SpellBlockInfo : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;


        private String _Class;
        private int? _MeleeTouch;
        private int? _RangedTouch;
        private int? _Concentration;
        private int? _CasterLevel;
        private bool _SpellLikeAbilities;
        private String _BlockType;
        private int? _SpellFailure;

        private ObservableCollection<SpellLevelInfo> _Levels = new ObservableCollection<SpellLevelInfo>();

        public SpellBlockInfo() { }
        public SpellBlockInfo(SpellBlockInfo old)
        {
            _Class = old._Class;
            _MeleeTouch = old._MeleeTouch;
            _RangedTouch = old._RangedTouch;
            _Concentration = old.Concentration;
            _CasterLevel = old._CasterLevel;
            _SpellLikeAbilities = old._SpellLikeAbilities;
            _BlockType = old._BlockType;
            _SpellFailure = old.SpellFailure;
            foreach (SpellLevelInfo info in old._Levels)
            {
                _Levels.Add(new SpellLevelInfo(info));
            }
        }



        public object Clone()
        {
            return new SpellBlockInfo(this);
        }

        static string _ClassRegexString;
        static SpellBlockInfo()
        {
            
            bool first = true;
            _ClassRegexString = "(";
            foreach (Rule rule in Rule.Rules.Where(a => (a.Type == "Classes"||a.Type =="Races")))
            {
                //remove plural from race name - this is for SLA {Dwarven may be incorrect} may need fixed if dwarves ever get a racial SLA
                if (rule.Type == "Races")
                {
                    rule.Name = rule.Name.TrimEnd('s');
                }
                if (!first)
                {
                    _ClassRegexString += "|";
                }
                first = false;
                _ClassRegexString += rule.Name;
            }
            //Wizard Specialist "as class names"
            _ClassRegexString += "|Abjurer|Conjurer|Diviner|Enchanter|Evoker|Illusionist|Necromancer|Transmuter";
            // special SLA names
            _ClassRegexString += "|Domain" + "|Bloodline" + "|Arcane School";
            
            _ClassRegexString += ")";
               
        }

        private static string spellcountblock = "((?<spellcount>[0-9]+(?=[;,\\)])),?)";
        private static string dcblock = "(DC (?<DC>[0-9]+),?)";
        private static string castblock = "((; )?((?<spellcast>[0-9]+|already)) cast)";
        private static string otherblock = "((; )?(?<othertext>[\\p{Pd}+',/\\p{L}0-9:\\. %\\*\\[\\]]+))";
        private static string countdcblock = "((" + spellcountblock + "|" + dcblock + "|" + castblock + "|" + otherblock + ") *)+";


        private static string spellblock = "((?<spellname>[\\p{Pd}'\\p{L}/ ]+)([*)])? *(?<superscript>\\[[MS]\\])? *(?<countdc>\\(" + countdcblock + "\\))?,?)";
        private static string levelblock = " *((?<level>([0-9]))(st|nd|rd|th)?) *(\\((((?<daily>[0-9]+)/day(; (?<levelcast>[0-9]+) cast)?|at will))\\))?-?(?<spellblocks>" + spellblock + "+) *((?<more>[0-9]+) more)? *";


        public static ObservableCollection<SpellBlockInfo> ParseInfo(string spellBlock)
        {
            ObservableCollection<SpellBlockInfo> blocklist = new ObservableCollection<SpellBlockInfo>();


            Regex group = new Regex("((" + _ClassRegexString + ") )?(?<sla>Spell-Like Abilities)|((" + _ClassRegexString + ") )?Spells (?<blocktype>(Known|Prepared))");

            List<string> list = new List<string>();
            List<string> slablock = new List<string>();
            foreach (Match spellMatch in group.Matches(spellBlock))
            {
                int start = spellMatch.Index;
                int length;
                Match next = spellMatch.NextMatch();

                if (next.Success)
                {
                    length = next.Index - start;
                }
                else
                {
                    length = spellBlock.Length - start;
                }

                string text = spellBlock.Substring(start, length);
                if (spellMatch.Groups["sla"].Success)
                {
                    slablock.Add(text);
                }
                else
                {
                    list.Add(text);
                }

            }

            foreach (String spells in list)
            {


                Regex regSpell = new Regex(spellblock);
                Regex regSpells = new Regex("((?<classname>" + _ClassRegexString + " )?(?<sla>Spell-Like Abilities)|(?<classname>" + _ClassRegexString + " )?(Spells (?<blocktype>(Known|Prepared)) +))(\\(CL ((?<cl>([0-9]+))(st|nd|rd|th)?)(?<altcl> \\[[0-9]+(st|nd|rd|th)? [A-Za-z]+\\.\\])?([,;] *concentration *[\\p{Pd}+]?(?<concentration>[0-9]+)( \\[[\\p{Pd}+][0-9]+ [\\p{L}. ]+\\])?)?([,;] *[\\p{Pd}+]?(?<spellfailure>[0-9]+)% spell failure)?([,;] *[\\p{Pd}+]?(?<meleetouch>[0-9]+) melee touch)?([,;] *[\\p{Pd}+]?(?<rangedtouch>[0-9]+) ranged touch)?\\))[:\r\n]*" +
                    "(?<levelblocks>" + levelblock + "\r?\n?)+");
                Regex regLevel = new Regex(levelblock);


                foreach (Match m in regSpells.Matches(spells))
                {
                    SpellBlockInfo blockInfo = new SpellBlockInfo();
                        blockInfo.ParseBlockHeader(m);
                        

                    foreach (Capture cap in m.Groups["levelblocks"].Captures)
                    {
                            
                        Match levelMatch = regLevel.Match(cap.Value);

                        SpellLevelInfo levelInfo = new SpellLevelInfo();

                        levelInfo.Level = levelMatch.IntValue("level");

                        levelInfo.Cast = levelMatch.IntValue("levelcast");

                        if (levelMatch.Groups["daily"].Success)
                            {
                                if (String.Compare(levelMatch.Groups["daily"].Value.Trim(), "At Will", true) == 0)
                                {
                                    levelInfo.AtWill = true;
                                }
                                else if (String.Compare(levelMatch.Groups["daily"].Value.Trim(), "Constant", true) == 0)
                                {
                                    levelInfo.Constant = true;
                                }
                                else
                                {

                                    levelInfo.PerDay = int.Parse(levelMatch.Groups["daily"].Value);
                                }
                            }

                        levelInfo.More = levelMatch.IntValue("more");

                        SpellInfo prevInfo = null;
                            foreach (Match spell in regSpell.Matches(levelMatch.Groups["spellblocks"].Value))
                            {

                                SpellInfo spellInfo = ParseSpell(spell, prevInfo);

                                if (spellInfo != prevInfo && spellInfo.Name.Length > 0)
                                {
                                    levelInfo.Spells.Add(spellInfo);
                                }

                                prevInfo = spellInfo;
                            }

                        if (levelInfo.Spells.Count > 0)
                            {
                                blockInfo.Levels.Add(levelInfo);
                            }          
                    }

                    if (blockInfo.Levels.Count > 0)
                    {
                        blocklist.Add(blockInfo);
                    }

                }
            }

            foreach (String slatext in slablock)
            {
                ParseSLABlocks(blocklist, slatext);
            }

            return blocklist;
        }

        private void ParseBlockHeader( Match m)
        {
            SpellBlockInfo blockInfo = this;
            if (m.Groups["classname"].Success)
            {
                blockInfo.Class = m.Groups["classname"].Value.Trim();
            }

            if (m.Groups["sla"].Success)
            {
                blockInfo._SpellLikeAbilities = true;
            }

            else if (m.Groups["blocktype"].Success)
            {
                blockInfo.BlockType = m.Groups["blocktype"].Value.Trim();
            }


            blockInfo.Concentration = m.IntValue("concentration");
            blockInfo.MeleeTouch = m.IntValue("meleetouch");
            blockInfo.RangedTouch = m.IntValue("rangedtouch");
            blockInfo.CasterLevel = m.IntValue("cl");
            blockInfo.SpellFailure = m.IntValue("spellfailure");

            
        }


        private static string sladcblock = "((" + dcblock + "|" + otherblock + ") *)+";
        private static string slaspellblock = "((?<spellname>[\\p{Pd}'\\p{L}/ ]+)\\*? *(?<countdc>\\(" + sladcblock + "\\))?,?)";
        private static string slaheader = "(((?<daily>[0-9]+)/day)|(?<constant>[Cc]onstant)|(?<atwill>At will))-";
        private static string slablock = " *" + slaheader + "(?<spellblocks>" + slaspellblock + "+) *";



        private static void ParseSLABlocks(ObservableCollection<SpellBlockInfo> info, string text)
        {
            //DebugTimer t = new DebugTimer("SLA Blocks", false, false);

            Regex regSpell = new Regex(slaspellblock);
            Regex regSpells = new Regex("(?<classname>" + _ClassRegexString + " )?(?<sla>Spell-Like Abilities) *\\(CL ((?<cl>[0-9]+)(st|nd|rd|th)?)(?<altcl> \\[[0-9]+(st|nd|rd|th)? [A-Za-z]+\\.\\])?([,;] *concentration *[\\p{Pd}+]?(?<concentration>[0-9]+)( \\[[\\p{Pd}+][0-9]+ [\\p{L} ]+\\])?)?([,;] *[\\p{Pd}+]?(?<spellfailure>[0-9]+)% spell failure)?([,;] *[\\p{Pd}+]?(?<meleetouch>[0-9]+) melee touch)?([,;] *[\\p{Pd}+]?(?<rangedtouch>[0-9]+) ranged touch)?\\)[:\r\n]*");
            Regex regLevel = new Regex(slablock);

          
            

            Match m = regSpells.Match(text);


            //t.MarkEventIf("First", 20);

            if (m.Success)
            {
                SpellBlockInfo blockInfo = new SpellBlockInfo();
                blockInfo.ParseBlockHeader(m);

                Regex regSlaHeader = new Regex(slaheader, RegexOptions.IgnoreCase);
                string spellBlock = text;
                List<string> spellblockList = new List<string>();
                MatchCollection mc = regSlaHeader.Matches(spellBlock);


                for (int i = 0; i < mc.Count; i++ )
                {
                    Match spellMatch = mc[i];
                    int start = spellMatch.Index;
                    int length;
                    Match next = null;

                    if (i + 1 < mc.Count)
                    {
                        next = mc[i + 1];
                    }

                    if (next != null)
                    {
                        length = next.Index - start;
                    }
                    else
                    {
                        length = spellBlock.Length - start;
                    }


                    string btext = spellBlock.Substring(start, length);
                    spellblockList.Add(btext);


                }

                //t.MarkEventIf("SpellBlocklist", 20);


                foreach (string block in spellblockList)
                {
                    //t.MarkEventIf("levmatch s", 100);
                    Match levelMatch = regLevel.Match(block);
                    //t.MarkEventIf("levmatch: " + block, 10);

                    SpellLevelInfo levelInfo = new SpellLevelInfo();
                  

                    if (levelMatch.Groups["daily"].Success)
                    {
                        if (String.Compare(levelMatch.Groups["daily"].Value.Trim(), "At Will", true) == 0)
                        {
                            levelInfo.AtWill = true;
                        }
                        else if (String.Compare(levelMatch.Groups["daily"].Value.Trim(), "Constant", true) == 0)
                        {
                            levelInfo.Constant = true;
                        }
                        else
                        {

                            levelInfo.PerDay = int.Parse(levelMatch.Groups["daily"].Value);
                        }
                    }
                    else if (levelMatch.Groups["atwill"].Success)
                    {
                        levelInfo.AtWill = true;
                    }
                    else if (levelMatch.Groups["constant"].Success)
                    {
                        levelInfo.Constant = true;
                    }

                    levelInfo.More = levelMatch.IntValue("more");

                    SpellInfo prevInfo = null;
                    Match spell = regSpell.Match(levelMatch.Groups["spellblocks"].Value);
                    while (spell.Success)
                    {

                        SpellInfo spellInfo = ParseSpell(spell, prevInfo);

                        if (spellInfo != prevInfo && spellInfo.Name.Length > 0)
                        {
                            levelInfo.Spells.Add(spellInfo);
                        }

                        prevInfo = spellInfo;
                        spell = spell.NextMatch();
                    }

                    if (levelInfo.Spells.Count > 0)
                    {
                        blockInfo.Levels.Add(levelInfo);
                    }

                }

                if (blockInfo.Levels.Count > 0)
                {
                    info.Add(blockInfo);
                }
            }

            //t.MarkEventIfTotal("Long: " + text , 10);
        }

        private static SpellInfo ParseSpell(Match spell, SpellInfo prevInfo)
        {
           
            string spellname = spell.Groups["spellname"].Value.TrimEnd(new char[] { 'D', ' ' });
            spellname = StringCapitalizer.Capitalize(spellname).Trim();

            SpellInfo spellInfo = null;
            if ((String.Compare(spellname, "lesser", true) == 0 ||
                String.Compare(spellname, "greater", true) == 0) && prevInfo != null)
            {
                spellInfo = prevInfo;
                spellname = spellInfo.Name + ", " + spellname;
            }
            else
            {

                spellInfo = new SpellInfo();
            }

            spellInfo.Count = spell.IntValue("spellcount");

            spellInfo.Name = spellname;

            spellInfo.Spell = Spell.ByName(spellname);

            spellInfo.DC = spell.IntValue("DC");

            if (spell.Groups["spellcast"].Success)
            {
                if (String.Compare(spell.Groups["spellcast"].Value, "already", true) == 0)
                {
                    spellInfo.AlreadyCast = true;
                }
                else
                {
                    spellInfo.Cast = spell.IntValue("spellcast");
                }
            }

            if (spell.Groups["othertext"].Success)
            {
                spellInfo.Other = spell.Groups["othertext"].Value;
            }

            if (spell.Groups["onlytext"].Success)
            {
                spellInfo.Only = spell.Groups["onlytext"].Value;
            }

            
            if (spell.Groups["superscript"].Success)
            {
                string superscript = spell.Groups["superscript"].Value;
                if (superscript.Contains("M"))
                {
                    spellInfo.Mythic = true;
                }
            }

            return spellInfo;
        }


        public String Class
        {
            get { return _Class; }
            set
            {
                if (_Class != value)
                {
                    _Class = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Class")); }
                }
            }
        }
        public int? MeleeTouch
        {
            get { return _MeleeTouch; }
            set
            {
                if (_MeleeTouch != value)
                {
                    _MeleeTouch = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("MeleeTouch")); }
                }
            }
        }
        public int? RangedTouch
        {
            get { return _RangedTouch; }
            set
            {
                if (_RangedTouch != value)
                {
                    _RangedTouch = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("RangedTouch")); }
                }
            }
        }
        public int? Concentration
        {
            get { return _Concentration; }
            set
            {
                if (_Concentration != value)
                {
                    _Concentration = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Concentration")); }
                }
            }
        }
        public int? CasterLevel
        {
            get { return _CasterLevel; }
            set
            {
                if (_CasterLevel != value)
                {
                    _CasterLevel = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("CasterLevel")); }
                }
            }
        }

        public String BlockType
        {
            get { return _BlockType; }
            set
            {
                if (_BlockType != value)
                {
                    _BlockType = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("BlockType")); }
                }
            }
        }


        public ObservableCollection<SpellLevelInfo> Levels
        {
            get { return _Levels; }
            set
            {
                if (_Levels != value)
                {
                    _Levels = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Levels")); }
                }
            }
        }


        public int? SpellFailure
        {
            get { return _SpellFailure; }
            set
            {
                if (_SpellFailure != value)
                {
                    _SpellFailure = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("SpellFailure")); }
                }
            }
        }


        public bool SpellLikeAbilities
        {
            get { return _SpellLikeAbilities; }
            set
            {
                if (_SpellLikeAbilities != value)
                {
                    _SpellLikeAbilities = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("SpellLikeAbilities")); }
                }
            }
        }


    }
}

﻿using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
﻿using System.Diagnostics;
﻿using System.Linq;
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
        private void ParseSkills()
        {
            skillValueDictionary = new SortedDictionary<String, SkillValue>(StringComparer.OrdinalIgnoreCase);
            if (skills != null)
            {
                Regex skillReg = new Regex("([ \\p{L}]+)( )(\\(([- \\p{L}]+)\\) )?((\\+|-)[0-9]+)");

                foreach (Match match in skillReg.Matches(skills))
                {
                    SkillValue value = new SkillValue();


                    value.Name = match.Groups[1].Value.Trim();


                    if (match.Groups[3].Success)
                    {
                        value.Subtype = match.Groups[4].Value;

                    }

                    value.Mod = int.Parse(match.Groups[5].Value);

                    skillValueDictionary[value.FullName] = value;


                }
            }

            skillsParsed = true;
        }

        private void ParseFeats()
        {
            if (featsList != null && featsList.Count > 0)
            {
                FeatsParsed = true;
            }
            else
            {

                featsList = new List<string>();

                if (Feats != null)
                {

                    Regex regFeat = new Regex("(^| )([- \\p{L}0-9]+( \\([- ,\\p{L}0-9]+\\))?)(\\*+)?(\\Z|,)");

                    foreach (Match m in regFeat.Matches(Feats))
                    {
                        string val = m.Groups[2].Value;

                        //remove B end of feat error
                        //hopefully no feat ever ends in a capital B
                        if (val[val.Length - 1] == 'B')
                        {
                            val = val.Substring(0, val.Length - 1);
                        }

                        featsList.Add(val);



                    }
                    featsParsed = true;
                }

                featsList.Sort();

            }


        }
        public int GetSkillMod(string skill, string subtype)
        {


            SkillValue val = new SkillValue(skill);
            val.Subtype = subtype;

            if (SkillValueDictionary.ContainsKey(val.FullName))
            {
                return SkillValueDictionary[val.FullName].Mod;
            }

            else
            {
                Stat stat;
                if (SkillsList.TryGetValue(val.Name, out stat))
                {
                    return AbilityBonus(GetStat(stat));
                }
                else
                {
                    return 0;
                }

            }
        }

        public bool AddOrChangeSkill(string skill, int diff)
        {
            return AddOrChangeSkill(skill, null, diff);
        }


        public bool AddOrChangeSkill(string skill, string subtype, int diff)
        {
            bool added = false;


            SkillValue val = new SkillValue(skill);
            val.Subtype = subtype;

            if (SkillValueDictionary.ContainsKey(val.FullName))
            {
                ChangeSkill(val.FullName, diff);
            }
            else
            {

                val.Mod = diff;

                Stat stat;
                if (SkillsList.TryGetValue(val.Name, out stat))
                {
                    val.Mod += AbilityBonus(GetStat(stat));
                }

                SkillValueDictionary[val.FullName] = val;

                UpdateSkillFields(val);

                added = true;

            }



            return added;
        }



        public bool ChangeSkill(string skill, int diff)
        {

            SkillValue value;

            if (SkillValueDictionary.TryGetValue(skill, out value))
            {
                value.Mod += diff;

                UpdateSkillFields(value);
                return true;
            }
            else
            {
                return false;
            }

        }

        private static string SetSkillStringMod(string text, string skill, int val)
        {

            string returnText = text;

            Regex regName = new Regex(skill + " (\\([-\\p{L} ]+\\) )?(\\+|-)[0-9]+");

            Match match = regName.Match(text);
            if (match.Success)
            {
                Regex regMod = new Regex("(\\+|-)[0-9]+");

                Match modMatch = regMod.Match(match.Value);

                int newVal = val;

                String newText = CMStringUtilities.PlusFormatNumber(newVal);

                returnText = returnText.Remove(match.Index + modMatch.Index, modMatch.Length);
                returnText = returnText.Insert(match.Index + modMatch.Index, newText);
            }

            return returnText;
        }

        private static string ChangeSkillStringMod(string text, string skill, int diff)
        {
            return ChangeSkillStringMod(text, skill, diff, false);
        }

        private static string ChangeSkillStringMod(string text, string skill, int diff, bool add)
        {

            string returnText = text;

            Regex regName = new Regex(skill + " (\\([-\\p{L} ]+\\) )?(\\+|-)[0-9]+");

            bool added = false;

            if (returnText != null)
            {
                Match match = regName.Match(returnText);
                if (match.Success)
                {
                    Regex regMod = new Regex("(\\+|-)[0-9]+");

                    Match modMatch = regMod.Match(match.Value);

                    int oldVal = int.Parse(modMatch.Value);
                    int newVal = oldVal + diff;

                    String newText = CMStringUtilities.PlusFormatNumber(newVal);

                    returnText = returnText.Remove(match.Index + modMatch.Index, modMatch.Length);
                    returnText = returnText.Insert(match.Index + modMatch.Index, newText);
                    added = true;
                }
            }

            if (add && !added)
            {
                returnText = AddToStringList(returnText, skill + " " + CMStringUtilities.PlusFormatNumber(diff));
            }

            return returnText;
        }

        private void AddRacialSkillBonus(string Skilltype, int diff)
        {
            RacialMods = AddPlusModValue(RacialMods, Skilltype, diff);

            AddOrChangeSkill(Skilltype, diff);
        }

        private static string AddPlusModValue(string text, string type, int diff)
        {
            string returnText = text;
            if (returnText == null)
            {
                returnText = "";
            }

            Regex regVal = new Regex("((\\+|-)[0-9]+) " + type);

            bool replaced = false;

            returnText = regVal.Replace(returnText, delegate(Match m)
            {
                int val = int.Parse(m.Groups[1].Value);

                val += diff;
                replaced = true;

                return CMStringUtilities.PlusFormatNumber(val) + " " + type;
            });

            if (!replaced)
            {
                returnText = AddToStringList(returnText,
                    CMStringUtilities.PlusFormatNumber(diff) + " " + type);
            }

            return returnText;

        }

        protected void UpdateSkillFields(SkillValue skill)
        {
            if (skill.Name.CompareTo("Perception") == 0)
            {
                //Adjust perception
                senses = SetSkillStringMod(Senses, skill.FullName, skill.Mod);
            }
        }

        private static string ChangeStartingNumber(string text, int diff)
        {
            string returnText = text;
            if (text != null)
            {

                Regex regName = new Regex("^-?[0-9]+");

                Match match = regName.Match(returnText);

                if (match.Success)
                {
                    int val = int.Parse(match.Value);

                    val += diff;

                    returnText = regName.Replace(returnText, val.ToString(), 1);
                }
            }

            return returnText;
        }

        private static int GetStartingNumber(string text)
        {
            int val = 0;

            if (text != null)
            {
                Regex regName = new Regex("^-?[0-9]+");

                Match match = regName.Match(text);

                if (match.Success)
                {
                    val = int.Parse(match.Value);
                }
            }

            return val;
        }
    }
    
}

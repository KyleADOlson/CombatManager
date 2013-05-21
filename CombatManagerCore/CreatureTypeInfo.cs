/*
 *  CreatureTypeInfo.cs
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

namespace CombatManager
{
    public enum CreatureType
    {
        Aberration = 0,
        Animal = 1,
        Construct = 2,
        Dragon = 3,
        Fey = 4,
        Humanoid = 5,
        MagicalBeast = 6,
        MonstrousHumanoid = 7,
        Ooze = 8,
        Outsider = 9,
        Plant = 10,
        Undead = 11,
        Vermin = 12
    }

    public class CreatureTypeInfo
    {

        public string Name;
        public int HDSize;
        public int BABRate;
        public int SaveVariesCount;
        public bool FortGood;
        public bool RefGood;
        public bool WillGood;
        public int Skills;
        public HashSet<string> ClassSkills;

        public CreatureTypeInfo(string Name, int HDSize, int BABRate, int SaveVariesCount, int Skills, HashSet<string> ClassSkills)
        {
            this.Name = Name;
            this.HDSize = HDSize;
            this.BABRate = BABRate;
            this.SaveVariesCount = SaveVariesCount;
            this.Skills = Skills;
            this.ClassSkills = ClassSkills;
        }

        public CreatureTypeInfo(string Name, int HDSize, int BABRate,  bool FortGood, bool RefGood, bool WillGood, int Skills, HashSet<string> ClassSkills)
        {
            this.Name = Name;
            this.HDSize = HDSize;
            this.BABRate = BABRate;
            this.FortGood = FortGood;
            this.RefGood = RefGood;
            this.WillGood = WillGood;
            this.Skills = Skills;
            this.ClassSkills = ClassSkills;
        }

        static Dictionary<string, CreatureTypeInfo> infoMap;

        static CreatureTypeInfo()
        {
            List<CreatureTypeInfo> list = new List<CreatureTypeInfo>();

            list.Add(new CreatureTypeInfo("aberration", 8, 3, false, false, true, 4,
                new HashSet<string>()
                {
                    "Acrobatics", "Climb", "Escape Artist", "Fly", "Intimidate", "Knowledge", "Perception", "Spellcraft", "Stealth", "Survival", "Swim" 
                }));
            list.Add(new CreatureTypeInfo("animal", 8, 3, true, true, false, 2,
                new HashSet<string>()
                {
                    "Acrobatics", "Climb", "Fly", "Perception", "Stealth", "Swim" 
                }));
            list.Add(new CreatureTypeInfo("construct", 10, 4, false, false, false, 2,
                new HashSet<string>()));
            list.Add(new CreatureTypeInfo("dragon", 12, 4, true, false, true, 6,
                new HashSet<string>()
                {
                    "Appraise", "Bluff", "Climb", "Craft", "Diplomacy", "Fly", "Heal", "Intimidate", "Knowledge", "Linguistics", "Perception", "Sense Motive", "Spellcraft", "Stealth", "Survival", "Swim", "Use Magic Device"
                }));
            list.Add(new CreatureTypeInfo("fey", 6, 2, false, true, true, 6,
                new HashSet<string>()
                {
                    "Acrobatics", "Bluff", "Climb", "Craft", "Diplomacy", "Disguise", "Escape Artist", "Fly", "Knowledge (geography)", "Knowledge (local)", "Knowledge (nature)", "Perception", "Perform", "Sense Motive", "Sleight of Hand", "Stealth", "Swim", "Use Magic Device"
                }));
            list.Add(new CreatureTypeInfo("humanoid", 8, 3, 1, 2,
                new HashSet<string>()
                {
                    "Climb", "Craft", "Handle Animal", "Heal", "Profession", "Ride", "Survival"
                }));
            list.Add(new CreatureTypeInfo("magical beast", 10, 4, true, true, false, 2,
                new HashSet<string>()
                {
                    "Acrobatics", "Climb", "Fly", "Perception", "Stealth", "Swim"
                }));
            list.Add(new CreatureTypeInfo("monstrous humanoid", 10, 4, false, true, false, 4,
                new HashSet<string>()
                {
                    "Climb", "Craft", "Fly", "Intimidate", "Perception", "Ride", "Stealth", "Survival", "Swim"
                }));
            list.Add(new CreatureTypeInfo("ooze", 8, 3, false, false, false, 2,
                new HashSet<string>()));
            list.Add(new CreatureTypeInfo("outsider", 10, 4, 2, 6,
                new HashSet<string>()
                {
                    "Bluff", "Craft", "Knowledge (planes)", "Perception", "Sense Motive", "Stealth"
                }));
            list.Add(new CreatureTypeInfo("plant", 8, 3, true, false, false, 2,
                new HashSet<string>()
                {
                    "Perception", "Stealth"
                }));
            list.Add(new CreatureTypeInfo("undead", 8, 3, false, false, true, 4,
                new HashSet<string>()
                {
                    "Climb", "Disguise", "Fly", "Intimidate", "Knowledge (arcane)", "Knowledge (religion)", "Perception", "Sense Motive", "Spellcraft", "Stealth"
                }));
            list.Add(new CreatureTypeInfo("vermin", 8, 3, true, false, false, 2,
                new HashSet<string>()
                {
                    
                }));
            
            infoMap = new Dictionary<string, CreatureTypeInfo>();

            foreach (CreatureTypeInfo info in list)
            {
                infoMap[info.Name] = info;
            }
            
        }

        public bool IsClassSkill(string skill)
        {
            if (Name == "outsider")
            {
                return true;
            }
            if (skill.Contains("Knowledge") && ClassSkills.Contains("Knowledge"))
            {
                return true;
            }
            return ClassSkills.Contains(skill);
        }

        public static CreatureTypeInfo GetInfo(string name)
        {
            try
            {
                return infoMap[name];

            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine(name);
                throw;
            }
        }

        public static IEnumerable<String> GetTypes()
        {
            return infoMap.Keys;
        }

        public int GetBAB(int hd)
        {
            return (hd * BABRate) / 4;
        }

        public static int GetSave(bool good, int hd)
        {
            if (good)
            {
                return hd / 2 + 2;
            }
            else
            {
                return hd / 3;
            }
        }

        
    }
}

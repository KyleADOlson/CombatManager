/*
 *  SizeMods.cs
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
    public enum MonsterSize
    {
        Fine = 0,
        Diminutive = 1,
        Tiny = 2,
        Small = 3,
        Medium = 4,
        Large = 5,
        Huge = 6,
        Gargantuan = 7,
        Colossal = 8
    }

    public class SizeMods
    {
        private MonsterSize _Size;
        private String _Name;
        private int _Attack;
        private int _Combat;
        private int _Fly;
        private int _Stealth;
        private String _Space;
        private int _Strength;
        private int _Dexterity;
        private int _Constitution;
        private int _NaturalArmor;





        private static Dictionary<String, MonsterSize> nameSizeDictionary;
        private static Dictionary<MonsterSize, SizeMods> sizeModsDictionary;


        static SizeMods()
        {
            nameSizeDictionary = new Dictionary<string, MonsterSize>();

            sizeModsDictionary = new Dictionary<MonsterSize, SizeMods>();

            sizeModsDictionary[MonsterSize.Fine] =
                new SizeMods(MonsterSize.Fine, "Fine", +8, -8, +8, +16, "0 ft.", 
                    0, 0, 0, 0);
            sizeModsDictionary[MonsterSize.Diminutive] =
                new SizeMods(MonsterSize.Diminutive, "Diminutive", +4, -4, +6, +12, "1 ft.",
                    0, -2, 0, 0);
            sizeModsDictionary[MonsterSize.Tiny] =
                new SizeMods(MonsterSize.Tiny, "Tiny", +2, -2, +4, +8, "2-1/2 ft.",
                    +2, -2, 0, 0);
            sizeModsDictionary[MonsterSize.Small] =
                new SizeMods(MonsterSize.Small, "Small", +1, -1, +2, +4, "5 ft.",
                    +4, -2, 0, 0);
            sizeModsDictionary[MonsterSize.Medium] =
                new SizeMods(MonsterSize.Medium, "Medium", 0, 0, 0, 0, "5 ft.",
                    +4, -2, +2, 0);
            sizeModsDictionary[MonsterSize.Large] =
                new SizeMods(MonsterSize.Large, "Large", -1, +1, -2, -4, "10 ft.", 
                    +8, -2, +4, +2);
            sizeModsDictionary[MonsterSize.Huge] =
                new SizeMods(MonsterSize.Huge, "Huge", -2, +2, -4, -8, "15 ft.", 
                    +8, -2, +4, +3);
            sizeModsDictionary[MonsterSize.Gargantuan] =
                new SizeMods(MonsterSize.Gargantuan, "Gargantuan", -4, +4, -6, -12, "20 ft.",
                    +8, 0, +4, +4);
            sizeModsDictionary[MonsterSize.Colossal] =
                new SizeMods(MonsterSize.Colossal, "Colossal", -8, +8, -8, -16, "30 ft.",
                    +8, 0, +4, +5);

            foreach (SizeMods mods in sizeModsDictionary.Values)
            {
                nameSizeDictionary[mods.Name] = mods.Size;
            }


            
        }

        public static MonsterSize GetSize(string name)
        {
            if (name == null || name.Length < 4)
            {
                throw new ArgumentException("Invalid monster size", "name");
            }

            string val = name.ToLower();
            val = val.Substring(0, 1).ToUpper() + val.Substring(1);            

            return nameSizeDictionary[val];
        }
		
		public static string GetSizeText(MonsterSize size)
		{
			return sizeModsDictionary[size].Name;
		}


        public static SizeMods GetMods(MonsterSize size)
        {
            return sizeModsDictionary[size];
        }



        public static MonsterSize ChangeSize(MonsterSize size, int diff)
        {
            int sizeInt = (int)size;

            sizeInt += diff;

            if (sizeInt < 0)
            {
                sizeInt = 0;
            }

            if (sizeInt > 8)
            {
                sizeInt = 8;
            }

            return (MonsterSize)sizeInt;

        }

        public static int StepsFromMedium(MonsterSize size)
        {
            return size-CombatManager.MonsterSize.Medium;
        }

        

        public SizeMods(MonsterSize size, String name, int attack, int combat, int fly, int stealth, string space,
            int strength, int dexterity, int constitution, int naturalArmor)
        {
            _Size = size;
            _Name = name;
            _Attack = attack;
            _Combat = combat;
            _Fly = fly;
            _Stealth = stealth;
            _Space = space;
            _Strength = strength;
            _Dexterity = dexterity;
            _Constitution = constitution;
            _NaturalArmor = naturalArmor;
        }



        public MonsterSize Size
        {
            get { return _Size; }
        }
        public String Name
        {
            get { return _Name; }
            
        }
        public int Attack
        {
            get { return _Attack; }
        }
        public int Combat
        {
            get { return _Combat; }
        }
        public int Fly
        {
            get { return _Fly; }
        }
        public int Stealth
        {
            get { return _Stealth; }
        }
        public string Space
        {
            get { return _Space; }
        }

        //mods for racial die chart
        public int Strength
        {
            get { return _Strength; }
        }
        public int Dexterity
        {
            get { return _Dexterity; }
        }
        public int Constitution
        {
            get { return _Constitution; }
        }
        public int NaturalArmor
        {
            get { return _NaturalArmor; }
        }

    }
}

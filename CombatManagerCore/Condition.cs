/*
 *  Condition.cs
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
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Collections.ObjectModel;

namespace CombatManager
{
    public enum ConditionType
    {
        Condition,
        Spell,
        Afflicition,
        Custom
    }

    public class FavoriteCondition : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ConditionType _Type;
        private String _Name;


        public FavoriteCondition()
        {

        }

        public FavoriteCondition(Condition c)
        {
            this._Name = c.Name;
            this._Type = c.Type;
        }

        public ConditionType Type
        {
            get { return _Type; }
            set
            {
                if (_Type != value)
                {
                    _Type = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Type")); }
                }
            }
        }
        public String Name
        {
            get { return _Name; }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Name")); }
                }
            }
        }



    }
    

    public class Condition : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private static List<Condition> _Conditions;
        private static List<Condition> _CustomConditions;
        private static List<FavoriteCondition> _FavoriteConditions;
        private static List<FavoriteCondition> _RecentCondtions;

        private static bool _MonsterConditionsLoaded;

        private const int RecentLength = 8;

        public static void LoadConditions()
        {
            _Conditions = XmlListLoader<Condition>.Load("Condition.xml");

            LoadSpellConditions();
#if !MONO
            LoadMonsterConditions();
#endif
            LoadCustomConditions();
            LoadFavoriteConditions();
            LoadRecentConditions();

        }

        private static void LoadCustomConditions()
        {

           _CustomConditions = XmlListLoader<Condition>.Load("CustomConditions.xml", true);
           if (_CustomConditions == null)
           {
               _CustomConditions = new List<Condition>();
           }
        }

        public static void SaveCustomConditions()
        {
            XmlListLoader<Condition>.Save(_CustomConditions, "CustomConditions.xml", true);
        }

        private static void LoadRecentConditions()
        {

            _RecentCondtions = XmlListLoader<FavoriteCondition>.Load("RecentConditions.xml", true);
            if (_RecentCondtions == null)
            {
                _RecentCondtions = new List<FavoriteCondition>();
            }
        }

        public static void SaveRecentConditions()
        {
            XmlListLoader<FavoriteCondition>.Save(_RecentCondtions, "RecentConditions.xml", true);
        }


        private static void LoadSpellConditions()
        {

            foreach (Spell spell in Spell.Spells)
            {
                Condition c = new Condition();
                c.Name = spell.name;
                c.Spell = spell;
                if (spell.Bonus != null)
                {
                    c.Image = "scrolleffect";
                }
                else
                {
                    c.Image = "scroll";
                }

                Conditions.Add(c);

            }
        }

        public static void LoadMonsterConditions()
        {
            if (!MonsterConditionsLoaded)
            {
                int success = 0;
                int failure = 0;

                foreach (Monster monster in Monster.Monsters)
                {

                    if (monster.SpecialAbilitiesList != null)
                    {
                        foreach (SpecialAbility sa in monster.SpecialAbilitiesList)
                        {
                            if (sa.Name == "Disease" || sa.Name == "Poison")
                            {
                                Affliction a = Affliction.FromSpecialAbility(monster, sa);


                                if (a == null)
                                {
                                    failure++;
                                    //System.Diagnostics.Debug.WriteLine(monster.Name + " - " + sa.Name);
                                }
                                else
                                {
                                    success++;

                                    Condition c = new Condition();
                                    c.Name = a.Name;
                                    c.Affliction = a;
                                    if (sa.Name == "Disease")
                                    {
                                        c.Image = "disease";
                                    }
                                    else
                                    {
                                        c.Image = "poison";
                                    }

                                    Conditions.Add(c);

                                    monster.UsableConditions.Add(c);
                                }

                            }
                        }
                    }
                }

                if (failure > 0)
                {
                    System.Diagnostics.Debug.WriteLine("Afflictions:  Succeeded: " + success + " Failed: " + failure);
                }

                _MonsterConditionsLoaded = true;
            }

        }

        public static Condition FindCondition(string name)
        {
            Condition c = Conditions.Find(delegate(Condition cond)
                {
                    return cond.Name == name;
                });

            return c;
        }

        private static void LoadFavoriteConditions()
        {
            List<FavoriteCondition> list = XmlListLoader<FavoriteCondition>.Load("FavoriteConditions.xml", true);

            if (list != null)
            {
                _FavoriteConditions = list;
            }
            else
            {
                _FavoriteConditions = new List<FavoriteCondition>();

                //add default entries;
                _FavoriteConditions.Add(new FavoriteCondition(ByName("Flat-Footed")));
                _FavoriteConditions.Add(new FavoriteCondition(ByName("Grappled")));
                _FavoriteConditions.Add(new FavoriteCondition(ByName("Pinned")));
                _FavoriteConditions.Add(new FavoriteCondition(ByName("Prone")));


            }
        }

        public static void SaveFavoriteConditions()
        {
            List<FavoriteCondition> list = new List<FavoriteCondition>(FavoriteConditions);
            XmlListLoader<FavoriteCondition>.Save(list, "FavoriteConditions.xml", true);
        }


        static Condition()
        {
        }


        private String _Name;
        private String _Text;
        private String _Image;
        private Spell _Spell;
        private Affliction _Affliction;
        private ConditionBonus _Bonus;
        private bool _Custom;


        public Condition()
        {

        }

        public Condition(Condition c)
        {
            _Name = c._Name;
            _Text = c._Text;
            _Image = c._Image;
            _Spell = c._Spell;
            _Custom = c._Custom;
            if (c._Bonus != null)
            {
                _Bonus = new ConditionBonus(c._Bonus);
            }
            if (c._Affliction != null)
            {
                _Affliction = (Affliction)c._Affliction.Clone();
            }
        }

        public object Clone()
        {
            return new Condition(this);
        }


        public String Name
        {
            get { return _Name; }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Name")); }
                }
            }
        }
        public String Text
        {
            get { return _Text; }
            set
            {
                if (_Text != value)
                {
                    _Text = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Text")); }
                }
            }
        }

        public String Image
        {
            get { return _Image; }
            set
            {
                if (_Image != value)
                {
                    _Image = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Image")); }
                }
            }
        }

        public Spell Spell
        {
            get { return _Spell; }
            set
            {
                if (_Spell != value)
                {
                    _Spell = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Spell")); }
                }
            }

        }

        public Affliction Affliction
        {
            get { return _Affliction; }
            set
            {
                if (_Affliction != value)
                {
                    _Affliction = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Affliction")); }
                }
            }

        }


        public bool Custom
        {
            get { return _Custom; }
            set
            {
                if (_Custom != value)
                {
                    _Custom = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Custom")); }
                }
            }

        }


        public ConditionBonus Bonus
        {

            get { return _Bonus; }
            set
            {
                if (_Bonus != value)
                {
                    _Bonus = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Bonus")); }
                }
            }
        }


        public static List<Condition> Conditions
        {
            get
            {
                if (_Conditions == null)
                {
                    LoadConditions();
                }
                return _Conditions;
            }
        }
        public static bool MonsterConditionsLoaded
        {
            get
            {
                return _MonsterConditionsLoaded;
            }
        }

        public static List<Condition> CustomConditions
        {
            get
            {
                if (_CustomConditions == null)
                {
                    LoadConditions();
                }
                return _CustomConditions;
            }
        }
        public static List<FavoriteCondition> FavoriteConditions
        {
            get
            {
                if (_FavoriteConditions == null)
                {
                    LoadConditions();
                }
                return _FavoriteConditions;
            }
        }
        public static List<FavoriteCondition> RecentConditions
        {
            get
            {
                if (_RecentCondtions == null)
                {
                    LoadConditions();
                }
                return _RecentCondtions;
            }
        }
        public static void PushRecentCondition(Condition c)
        {
            FavoriteCondition f = new FavoriteCondition(c);

            if (!HasFavorite(f))
            {
                int index = RecentIndex(f);

                if (index == -1)
                {
                    //push back
                    List<FavoriteCondition> list = new List<FavoriteCondition>();
                    list.Add(f);
                    for (int i = 0; i < _RecentCondtions.Count && i < RecentLength - 1; i++)
                    {
                        list.Add(_RecentCondtions[i]);
                    }
                    _RecentCondtions = list;
                }
                else if (index > 0)
                {
                    _RecentCondtions.RemoveAt(index);
                    _RecentCondtions.Insert(0, f);
                }
            }
            SaveRecentConditions();
        }


        public static Condition ByName(string name)
        {
            return Conditions.FirstOrDefault(a => String.Compare(a.Name, name, true) == 0);
        }

        public static Condition ByNameCustom(string name)
        {

            return _CustomConditions.FirstOrDefault(a => String.Compare(a.Name, name, true) == 0);
        }

        public static Condition FromFavorite(FavoriteCondition fc)
        {
            if (fc.Type == ConditionType.Custom)
            {
                return _CustomConditions.FirstOrDefault(a => String.Compare(a.Name, fc.Name, true) == 0);
            }
            else
            {
                return _Conditions.FirstOrDefault(a => (String.Compare(a.Name, fc.Name, true) == 0) &&
                    (fc.Type == a.Type));
            }
        }

        public static bool HasFavorite(FavoriteCondition fc)
        {
            return _FavoriteConditions.FirstOrDefault(a => String.Compare(a.Name, fc.Name, true) == 0 && a.Type == fc.Type) != null;
        }

        public static int RecentIndex(FavoriteCondition fc)
        {
            for (int i = 0; i < _RecentCondtions.Count; i++)
            {
                FavoriteCondition r = _RecentCondtions[i];
                if (String.Compare(r.Name, fc.Name, true) == 0 && r.Type == fc.Type)
                {
                    return i;
                }
            }

            return -1;
        }

        [XmlIgnore]
        public ConditionType Type
        {
            get
            {
                if (Spell != null)
                {
                    return ConditionType.Spell;
                }
                else if (Affliction != null)
                {
                    return ConditionType.Afflicition;
                }
                else
                {
                    return Custom ? ConditionType.Custom : ConditionType.Condition;
                }

            }
        }

    }
}

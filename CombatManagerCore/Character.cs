/*
 *  Character.cs
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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace CombatManager
{
    
    [DataContract]
    public class Character : INotifyPropertyChanged
    {
        [XmlIgnore]
        public object UndoInfo { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private String name;
        private int hp;
        private int maxHP;
        private int nonlethalDamage;
        private int temporaryHP;
        private string notes;
        private ObservableCollection<ActiveResource> resources;

        private Guid id;

        private bool isMonster;
        private Monster monster;
        private bool isBlank;


        private bool isReadying;
        private bool isDelaying;

        private bool _IsHidden;
        private bool _IsIdle;

        private uint? color;


        //unsaved data
        private bool isActive;
        private int currentInitiative;
        private int initiativeTiebreaker;
        private bool hasInitiativeChanged;
        private int initiativeRolled;
        private bool isConditionsOpen;
        private bool isOtherHPOpen;

        private InitiativeCount initiativeCount;

        private Character initiativeLeader;
        private ObservableCollection<Character> initiativeFollowers;
        private Guid? initiativeLeaderID;

        private CharacterAdjuster adjuster;

        private static Random rand = new Random();

        public Character()
        {
            this.InitiativeTiebreaker = rand.Next();
            monster = Monster.BlankMonster();
            HP = monster.HP;
            MaxHP = monster.HP;
            initiativeFollowers = new ObservableCollection<Character>();
            initiativeFollowers.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(initiativeFollowers_CollectionChanged);
            resources = new ObservableCollection<ActiveResource>();

        }

  

        public Character(string name) : this()
        {
            this.name = name;
            monster.Name = name;
			
        }

        public Character(Monster monster, bool rollHP) : this()
        {
            this.monster = (Monster)monster.Clone();
            this.name = monster.Name;
            if (!rollHP || !TryParseHP())
            {
                this.hp = monster.HP;
            }
            this.maxHP = this.hp;
            this.isMonster = true;

            if (this.monster.HasDefensiveAbility("Incorporeal"))
            {
                ActiveCondition ac = new ActiveCondition();
                ac.Condition = Condition.FindCondition("Incorporeal");
                this.monster.AddCondition(ac);
            }

            LoadResources();
			
        }

        public object Clone()
        {
            Character character = new Character();

            character.name = name;
            character.hp = hp;
            character.maxHP = maxHP;
            character.notes = notes;
            character.ID = Guid.NewGuid();
            foreach (ActiveResource r in resources)
            {
                character.resources.Add(new ActiveResource(r));
            }

            character.isMonster = isMonster;

            if (monster == null)
            {
                character.monster = null;
            }
            else
            {
                character.monster = (Monster)monster.Clone();
            }

            character.isActive = isActive;
            character.currentInitiative = currentInitiative;
            character.hasInitiativeChanged = false;
            character.initiativeRolled = initiativeRolled;
            character._IsHidden = _IsHidden;
            character._IsIdle = _IsIdle;
            character.color = color;

            if (initiativeCount != null)
            {
                character.InitiativeCount = new InitiativeCount(InitiativeCount);
            }

            return character;
        }

        public override string ToString()
        {
            return Name;
        }

        private void LoadResources()
        {
            if (monster.SpecialAttacks != null)
            {
                //find rage
                Match m = Regex.Match(monster.SpecialAttacks, "[Rr]age \\((?<count>[0-9]+) rounds?/ ?day\\)");
                if (m.Success)
                {
                    int count = int.Parse(m.Groups["count"].Value);
                    ActiveResource r = new ActiveResource() { Name = "Rage", Max = count, Current = count, Uses = count + " rounds/day" };
                    Resources.Add(r);
                }


            }

            if (monster.SQ != null)
            {
                //find rage
                Match m = Regex.Match(monster.SQ, "[Kk]i [Pp]ool \\((?<count>[0-9]+) points?,");
                if (m.Success)
                {
                    int count = int.Parse(m.Groups["count"].Value);
                    ActiveResource r = new ActiveResource() { Name = "Ki pool", Max = count, Current = count };
                    Resources.Add(r);
                }
            }
        }



        private void Notify(string prop)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        [XmlIgnore]
        public Monster Stats
        {
            get
            {
                return monster;
            }
        }

        [DataMember]
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                
                if (this.name != value)
                {
                    this.name = value;
                    Notify("Name");
                    Notify("HiddenName");

                    if (this.IsBlank)
                    {
                        if (monster != null)
                        {
                            monster.Name = name;
                        }
                    }
                }
            }
        }


        [DataMember]
        public int HP
        {
            get
            {
                return this.hp;
            }
            set
            {
                if (this.hp != value)
                {
                    this.hp = value;
                    Notify("HP");
                }
            }
        }


        [DataMember]
        public int MaxHP
        {
            get
            {
                return this.maxHP;
            }
            set
            {
                if (this.maxHP != value)
                {
                    this.maxHP = value;
                    Notify("MaxHP");

                    if (IsBlank)
                    {
                        if (monster != null)
                        {
                            monster.HP = maxHP;
                        }
                    }
                }
            }
        }

        [DataMember]
        public int NonlethalDamage
        {
            get
            {
                return this.nonlethalDamage;
            }
            set
            {
                if (this.nonlethalDamage != value)
                {
                    this.nonlethalDamage = value;
                    Notify("NonlethalDamage");
                }
            }
        }

        [DataMember]
        public int TemporaryHP
        {
            get
            {
                return this.temporaryHP;
            }
            set
            {
                if (this.temporaryHP != value)
                {
                    this.temporaryHP = value;
                    Notify("TemporaryHP");
                }
            }
        }


        [DataMember]
        public bool IsMonster
        {
            get
            {
                return this.isMonster;
            }
            set
            {
                if (this.isMonster != value)
                {
                    this.isMonster = value;
                    Notify("IsMonster");
                }
            }
        }

        [DataMember]
        public Guid ID
        {
            get
            {
                if (id == Guid.Empty)
                {
                    id = Guid.NewGuid();
                }
                return this.id;
            }
            set
            {
                if (this.id != value)
                {
                    this.id = value;
                    Notify("ID");
                }
            }
        }

        [DataMember]
        public bool IsBlank
        {
            get
            {
                return this.isBlank;
            }
            set
            {
                if (this.isBlank != value)
                {
                    this.isBlank = value;
                    Notify("IsBlank");
                }
            }
        }

        [DataMember]
        [XmlIgnore]
        public bool IsActive
        {
            get
            {
                return this.isActive;
            }
            set
            {
                if (this.isActive != value)
                {
                    this.isActive = value;
                    Notify("IsActive");
                }
            }
        }


        [DataMember]
        public bool IsConditionsOpen
        {
            get
            {
                return this.isConditionsOpen;
            }
            set
            {
                if (this.isConditionsOpen != value)
                {
                    this.isConditionsOpen = value;
                    Notify("IsConditionsOpen");
                }
            }
        }

        [DataMember]
        public bool IsOtherHPOpen
        {
            get
            {
                return this.isOtherHPOpen;
            }
            set
            {
                if (this.isOtherHPOpen != value)
                {
                    this.isOtherHPOpen = value;
                    Notify("IsOtherHPOpen");
                }
            }
        }


        [DataMember]
        public int CurrentInitiative
        {
            get
            {
                return this.currentInitiative;
            }
            set
            {
                if (this.currentInitiative != value)
                {
                    this.currentInitiative = value;
                    Notify("CurrentInitiative");
                }
            }
        }

        [DataMember]
        public int InitiativeTiebreaker
        {
            get
            {
                return this.initiativeTiebreaker;
            }
            set
            {
                if (this.initiativeTiebreaker != value)
                {
                    this.initiativeTiebreaker = value;
                    Notify("InitiativeTiebreaker");
                }
            }
        }

        [DataMember]
        public bool HasInitiativeChanged
        {
            get
            {
                return this.hasInitiativeChanged;
            }
            set
            {
                if (this.hasInitiativeChanged != value)
                {
                    this.hasInitiativeChanged = value;
                    Notify("HasInitiativeChanged");
                }
            }
        }

        [DataMember]
        public int InitiativeRolled
        {
            get
            {
                return this.initiativeRolled;
            }
            set
            {
                if (this.initiativeRolled != value)
                {
                    this.initiativeRolled = value;
                    Notify("InitiativeRolled");
                }
            }
        }

        [DataMember]
        public InitiativeCount InitiativeCount
        {
            get
            {
                return this.initiativeCount;
            }
            set
            {
                if (this.initiativeCount != value)
                {
                    this.initiativeCount = value;
                    Notify("InitiativeCount");
                }
            }
        }

        [XmlIgnore]
        public Character InitiativeLeader
        {
            get
            {
                return this.initiativeLeader;
            }
            set
            {
                if (this.initiativeLeader != value)
                {
                    this.initiativeLeader = value;
                    initiativeLeaderID = null;
                    Notify("InitiativeLeader");
                }
            }
        }


        [DataMember]
        public Guid? InitiativeLeaderID
        {
            get
            {
                if (initiativeLeader != null)
                {
                    return initiativeLeader.ID;
                }
                else
                {
                    return initiativeLeaderID;
                }
            }
            set
            {
                if (this.initiativeLeaderID != value)
                {
                    this.initiativeLeaderID = value;
                    Notify("InitiativeLeaderID");
                }
            }
        }

        [XmlIgnore]
        public ObservableCollection<Character> InitiativeFollowers
        {
            get
            {
                return initiativeFollowers;
            }
        }


        [XmlIgnore]
        public bool HasFollowers
        {
            get
            {
                return InitiativeFollowers.Count > 0;
            }
        }


        [DataMember]
        public bool IsDelaying
        {
            get
            {
                return isDelaying;
            }
            set
            {
                if (this.isDelaying != value)
                {
                    this.isDelaying = value;
                    Notify("IsDelaying");
                    Notify("IsNotReadyingOrDelaying");
                }
            }
        }

        [DataMember]
        public bool IsReadying
        {
            get
            {
                return isReadying;
            }
            set
            {
                if (this.isReadying != value)
                {
                    this.isReadying = value;
                    Notify("IsReadying");
                    Notify("IsNotReadyingOrDelaying");
                }
            }
        }

        [DataMember]
        public uint? Color
        {
            get
            {
                return color;
            }
            set
            {
                if (this.color != value)
                {
                    this.color = value;
                    Notify("Color");
                }
            }
        }
		
		[XmlIgnore]
        public bool IsNotReadyingOrDelaying
        {
            get
            {
                return !isReadying && !isDelaying;
            }
        }

        void initiativeFollowers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Notify("HasFollowers");
        }

        [DataMember]
        public string Notes
        {
            get
            {
                return notes;
            }
            set
            {
                if (notes != value)
                {
                    notes = value;
                    Notify("Notes");
                }
            }
        }


        [DataMember]
        public Monster Monster
        {
            get
            {
                return monster;
            }
            set
            {
                if (this.monster != value)
                {
                    this.monster = value;

                    Notify("Monster");
                }
            }
        }



        public bool TryParseHP()
        {
            DieRoll dr = DieRoll.FromString(monster.HD);
            if (dr != null)
            {
                HP = dr.Roll().Total;
                return true;
            }

            return false;
        }

        public bool HasCondition(string name)
        {
            return FindCondition(name) != null;
        }

        public void AddConditionByName(string name)
        {

            if (!HasCondition(name))
            {
                ActiveCondition c = new ActiveCondition();
                c.Condition = Condition.ByName(name);
                Stats.AddCondition(c);
            }
        }

        public ActiveCondition FindCondition(string name)
        {
            return Stats.ActiveConditions.FirstOrDefault
                (a => String.Compare(a.Condition.Name, name, true) == 0);
        }


        public IEnumerable<ActiveCondition> FindAllConditions(string name)
        {
            return Stats.ActiveConditions.Where<ActiveCondition>
                (a => String.Compare(a.Condition.Name, name, true) == 0);
        }

        public void RemoveConditionByName(string name)
        {
            List<ActiveCondition> list = new List<ActiveCondition>(FindAllConditions(name));
            foreach (ActiveCondition c in list)
            {
                Stats.RemoveCondition(c);
            }
        }

        [XmlIgnore]
        public int MinHP
        {
            get
            {
                int val = 0;

                if (Stats.Constitution != null)
                {
                    val = -Stats.Constitution.Value;
                }
                else if (Stats.Charisma != null)
                {
                    val = -Stats.Charisma.Value;
                }
                return val;

            }
        }

        public int AdjustHP(int val)
        {
            return AdjustHP(val, 0, 0);
        }


        public int AdjustHP(int val, int nlval, int tempval)
        {
            int oldHP = HP + TemporaryHP;
            int adjust = val;

            if (NonlethalDamage > 0)
            {
                if (adjust > 0)
                {
                    NonlethalDamage = (adjust == NonlethalDamage||adjust > NonlethalDamage?0:NonlethalDamage-adjust);
                }
            }
            if (temporaryHP > 0)
            {
                if (adjust < 0)
                {
                    if (-adjust <= temporaryHP)
                    {
                        TemporaryHP += adjust;
                        adjust = 0;
                    }
                    else
                    {
                        adjust = adjust + TemporaryHP;
                        TemporaryHP = 0;
                    }
                }
            }

            TemporaryHP = Math.Max(TemporaryHP + tempval, 0);
            NonlethalDamage = Math.Max(NonlethalDamage +nlval, 0);
            HP += adjust;

            int effectiveHP = HP + TemporaryHP;

            if (oldHP > MinHP && effectiveHP <= MinHP)
            {
                RemoveConditionByName("staggered");
                RemoveConditionByName("disabled");
                RemoveConditionByName("dying");
                RemoveConditionByName("stable");
                RemoveConditionByName("unconscious");
                AddConditionByName("dead");
            }
            else if (oldHP == 0 && effectiveHP == 0)
            {
                RemoveConditionByName("unconscious");
                RemoveConditionByName("staggered");
                AddConditionByName("disabled");
            }
            else if (oldHP > 0 && effectiveHP == 0)
            {
                AddConditionByName("disabled");
                AddConditionByName("staggered");
            }

            else if (oldHP >= 0 && effectiveHP < 0)
            {
                if (!HasCondition("dead"))
                {
                    RemoveConditionByName("staggered");
                    RemoveConditionByName("disabled");
                    RemoveConditionByName("stable");
                    AddConditionByName("unconscious");
                    AddConditionByName("dying");
                }
            }
            else if (oldHP <= MinHP && effectiveHP > MinHP && effectiveHP < 0)
            {
                //AddConditionByName("dying");
                AddConditionByName("unconscious");
                AddConditionByName("stable");
                RemoveConditionByName("dead");
            }

            else if (oldHP < 0 && effectiveHP == 0)
            {
                RemoveConditionByName("unconscious");
                AddConditionByName("disabled");
                AddConditionByName("staggered");
                RemoveConditionByName("dying");
                RemoveConditionByName("dead");
                RemoveConditionByName("stable");
            }

            else if (oldHP <= 0 && effectiveHP > 0)
            {
                RemoveConditionByName("unconscious");
                RemoveConditionByName("disabled");
                RemoveConditionByName("staggered");
                RemoveConditionByName("dying");
                RemoveConditionByName("dead");
                RemoveConditionByName("stable");
            }
            else if(oldHP < 0 && effectiveHP > MinHP && adjust > 0)
            {
                RemoveConditionByName("dying");
                AddConditionByName("unconscious");
                AddConditionByName("stable");
            }

            else if (effectiveHP < oldHP && HasCondition("stable"))
            {
                RemoveConditionByName("stable");
                AddConditionByName("dying");
            }
            else if (effectiveHP > 0)
            {
                RemoveConditionByName("unconscious");
                RemoveConditionByName("disabled");
                RemoveConditionByName("staggered");
                RemoveConditionByName("dying");
                RemoveConditionByName("dead");
                RemoveConditionByName("stable");
            }
            if (nonlethalDamage > 0 && nonlethalDamage >= effectiveHP)
            {
                if ((!HasCondition("dying") || HasCondition("disabled")) && !HasCondition("dead"))
                {
                    if (nonlethalDamage == effectiveHP)
                    {
                        RemoveConditionByName("unconscious");
                        AddConditionByName("staggered");
                    }
                    else
                    {
                        RemoveConditionByName("disabled");
                        RemoveConditionByName("staggered");
                        AddConditionByName("unconscious");
                    }
                }
            }
            //else
            //{
            //    RemoveConditionByName("staggered");
            //    RemoveConditionByName("unconscious");
            //}

            return HP;
        }


        [DataMember]
        public bool IsHidden
        {
            get { return _IsHidden; }
            set
            {
                if (_IsHidden != value)
                {
                    _IsHidden = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("IsHidden")); 
                        PropertyChanged(this, new PropertyChangedEventArgs("HiddenName"));
                    }
                }
            }
        }


        [DataMember]
        public bool IsIdle
        {
            get { return _IsIdle; }
            set
            {
                if (_IsIdle != value)
                {
                    _IsIdle = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("IsIdle")); }
                }
            }
        }

        [DataMember]
        public ObservableCollection<ActiveResource> Resources
        {
            get
            {
                return resources;
            }
            set
            {
                if (resources != value)
                {
                    resources = value;

                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Resources")); }

                }
            }
        }

        [XmlIgnore]
        public string HiddenName
        {
            get
            {
                if (_IsHidden)
                {
                    return "??????";
                }
                else
                {
                    return name;
                }
            }
        }

        [XmlIgnore]
        public CharacterAdjuster Adjuster
        {
            get
            {
                if (adjuster == null)
                {
                    adjuster = new CharacterAdjuster(this);
                }
                return adjuster;
            }
        }        

        public class CharacterAdjuster : INotifyPropertyChanged
        {

            public event PropertyChangedEventHandler PropertyChanged;

            private Character _c;

            public CharacterAdjuster(Character c)
            {
                _c = c;
                c.PropertyChanged += new PropertyChangedEventHandler(c_PropertyChanged);
            }

            void c_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                PropertyChanged.Call(this, e.PropertyName);
                
            }

            public int HP
            {
                get
                {
                    return _c.HP;
                }
                set
                {
                    int change = value - _c.HP;

                    _c.AdjustHP(change);

                }
            }

            public int NonlethalDamage
            {
                get
                {
                    return _c.NonlethalDamage ;
                }
                set
                {

                    int change = value - _c.NonlethalDamage;

                    _c.AdjustHP(0, change, 0);
                }
            }

            public int TemporaryHP
            {
                get
                {
                    return _c.TemporaryHP;
                }
                set
                {

                    int change = value - _c.TemporaryHP;

                    _c.AdjustHP(0, 0, change);
                }
            }



        }

    }

    public static class EventExt
    {
        public static void Call(this PropertyChangedEventHandler PropertyChanged, object sender, string property)
        {
            if (PropertyChanged != null) { PropertyChanged(sender, new PropertyChangedEventArgs(property)); }

        }
    }

}

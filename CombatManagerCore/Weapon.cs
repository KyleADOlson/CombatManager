/*
 *  Weapon.cs
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
using System.Text.RegularExpressions;

namespace CombatManager
{
    public class Weapon : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private String _Name;
        private String _AltName;
        private String _OriginalName;
        private String _Cost;
        private String _DmgS;
        private String _DmgM;
        private String _Critical;
        private String _Range;
        private String _Weight;
        private String _DmgType;
        private String _Special;
        private String _Source;
        private String _Hands;
        private String _Class;
		private String _Plural;
        private List<String> _Groups;
        private bool _IsHand;
        private bool _Throw;
        private int _CritRange;
        private int _CritMultiplier;
        private bool _RangedTouch;
        private bool _AltDamage;
        private Stat _AltDamageStat;
        private bool _AltDamageDrain;
        private string _URL;
        private string _Desc;
        private String _Misfire;
        private String _Capacity;

        
        static Dictionary<string, Weapon> weapons;
        static Dictionary<string, Weapon> weaponsPlural;
        static Dictionary<string, Weapon> weaponsOriginalName;	
        static Dictionary<string, Weapon> weaponsAltName;	

        static Weapon()
        {
            LoadWeapons();		
        }
        public Weapon()
        {
            _CritRange = 20;
            _CritMultiplier = 2;
        }
        //create a blank weapon from an attack
        public Weapon(Attack attack, bool ranged, MonsterSize size)
        {
            

            _Name = attack.Name;

            DieRoll medRoll = DieRoll.StepDie(attack.Damage, ((int)MonsterSize.Medium) - (int)size);
            DieRoll smRoll = DieRoll.StepDie(attack.Damage, -1);

            _DmgM = new DieStep(medRoll).ToString();
            _DmgS = new DieStep(smRoll).ToString();

            _CritRange = attack.CritRange;
            _CritMultiplier = attack.CritMultiplier;

            if (attack.Bonus.Count > 1)
            {
                _Class = "Martial";
            }
            else
            {
                _Class = "Natural";
            }

            if (ranged)
            {
                _Hands = "Ranged";
            }
            else
            {
                _Hands = "One-Handed";
            }

            _RangedTouch = attack.RangedTouch;
            _AltDamage = attack.AltDamage;
            _AltDamageStat = attack.AltDamageStat;
            _AltDamageDrain = attack.AltDamageDrain;          
        }
        public object Clone()
        {
            Weapon weapon = new Weapon();
            weapon._Name = _Name;
            weapon._Cost = _Cost;
            weapon._DmgS = _DmgS;
            weapon._DmgM = _DmgM;
            weapon._Critical = _Critical;
            weapon._Range = _Range;
            weapon._Weight = _Weight;
            weapon._DmgType = _DmgType;
            weapon._Special = _Special;
            weapon._Source = _Source;
            weapon._Hands = _Hands;
            weapon._Class = _Class;
            weapon._Plural = _Plural;
            weapon._IsHand = _IsHand;
            weapon._Throw = _Throw;
            weapon._CritRange = _CritRange;
            weapon._CritMultiplier = _CritMultiplier;
            weapon._RangedTouch = _RangedTouch;
            weapon._AltDamage = _AltDamage;
            weapon._AltDamageStat = _AltDamageStat;
            weapon._AltDamageDrain = _AltDamageDrain;
            weapon._Misfire = _Misfire;
            weapon._Capacity = _Capacity;
            if (_Groups != null)
            {
                weapon._Groups = new List<string>(_Groups);
            }


            return weapon;
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
        public String AltName
        {
            get { return _AltName; }
            set
            {
                if (_AltName != value)
                {
                    _AltName = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("AltName")); }
                }
            }
        }
        [XmlIgnore]
        public String OriginalName
        {
            get { return _OriginalName; }
            set
            {
                if (_OriginalName != value)
                {
                    _OriginalName = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("OriginalName")); }
                }
            }
        }
        public String Cost
        {
            get { return _Cost; }
            set
            {
                if (_Cost != value)
                {
                    _Cost = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Cost")); }
                }
            }
        }
        public String DmgS
        {
            get { return _DmgS; }
            set
            {
                if (_DmgS != value)
                {
                    _DmgS = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("DmgS")); }
                }
            }
        }
        public String DmgM
        {
            get { return _DmgM; }
            set
            {
                if (_DmgM != value)
                {
                    _DmgM = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("DmgM")); }
                }
            }
 
        }		
		[XmlIgnore]
		public DieRoll DamageDie
		{
			get
			{
				return DieRoll.FromString(DmgM, 0);
			}
			
		}
        [XmlIgnore]
        public string DamageText
        {
            get
            {
                if (DamageDie == null)
                {
                    return "";
                }
                else
                {
                    string critText = "";

                    if (CritRange != 20)
                    {
                        critText += "/" + CritRange + "-20";
                    }

                    if (CritMultiplier != 2)
                    {
                        critText += "/x" + CritMultiplier;
                    }



                    return DamageDie.Text + critText;
                }
            }
        }
        public DieRoll SizeDamageDie(MonsterSize size)
        {
            if (DamageDie == null)
            {
                return null;
            }

            return DieRoll.StepDie(DamageDie, (size - MonsterSize.Medium));
        }
        public string SizeDamageText(MonsterSize size)
        {
            DieRoll roll = SizeDamageDie(size);

            if (roll == null)
            {
                return "";
            }

            string critText = "";

            if (CritRange != 20)
            {
                critText += "/" + CritRange + "-20";
            }

            if (CritMultiplier != 2)
            {
                critText += "/x" + CritMultiplier;
            }

            return roll.Text + critText;
        }	
        private const string RegCritString = "((?<critrange>[0-9]+)-[0-9]+)?(/?x(?<critmultiplier>[0-9]+))?";
        public String Critical
        {
            get { return _Critical; }
            set
            {
                if (_Critical != value)
                {
                    _Critical = value;                  
                    _CritRange = 20;
                    _CritMultiplier = 2;
                    if (_Critical != null)
                    {
                        Match m = new Regex(RegCritString).Match(_Critical);

                        if (m.Groups["critrange"].Success)
                        {
                            _CritRange = int.Parse(m.Groups["critrange"].Value);
                        }
                        if (m.Groups["critmultiplier"].Success)
                        {
                            _CritMultiplier = int.Parse(m.Groups["critmultiplier"].Value);
                        }
                        
                    }

                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Critical")); }
                }
            }
        }
        public String Range
        {
            get { return _Range; }
            set
            {
                if (_Range != value)
                {
                    _Range = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Range")); }
                }
            }
        }
        public String Weight
        {
            get { return _Weight; }
            set
            {
                if (_Weight != value)
                {
                    _Weight = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Weight")); }
                }
            }
        }
        public String DmgType
        {
            get { return _DmgType; }
            set
            {
                if (_DmgType != value)
                {
                    _DmgType = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("DmgType")); }
                }
            }
        }
        public String Special
        {
            get { return _Special; }
            set
            {
                if (_Special != value)
                {
                    _Special = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Special")); }
                }
            }
        }
        public String Source
        {
            get { return _Source; }
            set
            {
                if (_Source != value)
                {
                    _Source = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Source")); }
                }
            }
        }
        public String Hands
        {
            get { return _Hands; }
            set
            {
                if (_Hands != value)
                {
                    _Hands = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Hands")); }
                }
            }
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
        public String Plural
        {
            get { return _Plural; }
            set
            {
                if (_Plural != value)
                {
                    _Plural = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Plural")); }
                }
            }
        }
        public bool IsHand
        {
            get { return _IsHand; }
            set
            {
                if (_IsHand != value)
                {
                    _IsHand = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("IsHand")); }
                }
            }
        }
        public bool Throw
        {
            get { return _Throw; }
            set
            {
                if (_Throw != value)
                {
                    _Throw = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Throw")); }
                }
            }
        }
        public bool RangedTouch
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
        public bool AltDamage
        {
            get { return _AltDamage; }
            set
            {
                if (_AltDamage != value)
                {
                    _AltDamage = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("AltDamage")); }
                }
            }
        }
        public Stat AltDamageStat
        {
            get { return _AltDamageStat; }
            set
            {
                if (_AltDamageStat != value)
                {
                    _AltDamageStat = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("AltDamageStat")); }
                }
            }
        }
        public bool AltDamageDrain
        {
            get { return _AltDamageDrain; }
            set
            {
                if (_AltDamageDrain != value)
                {
                    _AltDamageDrain = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("AltDamageDrain")); }
                }
            }
        }
        public string URL
        {
            get { return _URL; }
            set
            {
                if (_URL != value)
                {
                    _URL = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("URL")); }
                }
            }
        }
        public string Desc
        {
            get { return _Desc; }
            set
            {
                if (_Desc != value)
                {
                    _Desc = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Desc")); }
                }
            }
        }        
        public String Misfire
        {
            get { return _Misfire; }
            set
            {
                if (_Misfire != value)
                {
                    _Misfire = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Misfire")); }
                }
            }
        }
        public String Capacity
        {
            get { return _Capacity; }
            set
            {
                if (_Capacity != value)
                {
                    _Capacity = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Capacity")); }
                }
            }
        }
        [XmlArrayItem("Group")]
        public List<String> Groups
        {
            get
            {
                return _Groups;
            }
            set
            {
                if (_Groups != value)
                {
                    _Groups = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Groups")); }
                }

            }
        }
        [XmlIgnore]
        public int CritRange
        {
            get
            {
                return _CritRange;

            }
        }
        [XmlIgnore]
        public int CritMultiplier
        {
            get
            {
                return _CritMultiplier;
            }
        }
        [XmlIgnore]
        public int HandsUsed
        {
            get
            {
                if (Class != "Natural")
                {
                    return (Hands == "Two-Handed" || Hands == "Double" || Hands == "Two-Handed Firearm") ? 2 : 1;
                }
                else
                {
                    return IsHand ? 1 : 0;
                }

            }
        }			
		[XmlIgnore]
        public bool TwoHanded
        {
			
            get
            {
				return HandsUsed == 2;
			}
		}
        [XmlIgnore]
        public bool Natural
        {
            get
            {
                return Class == "Natural";
            }
        }
        [XmlIgnore]
        public bool Ranged
        {
            get
            {
                return Hands == "Ranged" || Hands == "One-Handed Firearm" || Hands == "Two-Handed Firearm";
            }
        }
        [XmlIgnore]
        public bool Firearm
        {
            get
            {
                return Hands == "One-Handed Firearm" || Hands == "Two-Handed Firearm";
            }
        }
        [XmlIgnore]
        public bool Double
        {
            get
            {
                return Hands == "Double";
            }
        }
        [XmlIgnore]
        public bool Light
        {
            get
            {
                return Hands == "Light" || Hands == "Unarmed" || Hands == "One-Handed Firearm"
                    ;
            }
        }
        [XmlIgnore]
        public bool WeaponFinesse
        {
            get
            {
                if (Light || Natural)
                {
                    return true;
                }
                else if (_Special != null)
                {
                    return Regex.Match(_Special, "Weapon Finesse", RegexOptions.IgnoreCase).Success;
                }
                else
                {
                    return false;
                }
            }
        }
        public static Dictionary<String, Weapon> Weapons
        {
            get
            {
                return weapons;
            }
        }
        public static Dictionary<String, Weapon> WeaponsPlural
        {
            get
            {
                return weaponsPlural;
            }
        }
        public static Dictionary<String, Weapon> WeaponsOriginalName
        {
            get
            {
                return weaponsPlural;
            }
        }
        public static Dictionary<String, Weapon> WeaponsAltName
        {
            get
            {
                return weaponsAltName;
            }
        }
        public static Weapon Find(string name)
        {
            if (weapons.ContainsKey(name))
            {
                return weapons[name];
            }
            else if (weaponsPlural.ContainsKey(name))
            {
                return weaponsPlural[name];
            }
            else if (weaponsOriginalName.ContainsKey(name))
            {
                return weaponsOriginalName[name];
            }
            else if (weaponsAltName.ContainsKey(name))
            {
                return weaponsAltName[name];
            }

            return null;
        }
        static void LoadWeapons()
        {
            FileStream fs = null;
            try
            {

                List<Weapon> set = XmlListLoader<Weapon>.Load("Weapons.xml");


                weapons = new Dictionary<string, Weapon>(new InsensitiveEqualityCompararer());
                weaponsPlural = new Dictionary<string, Weapon>(new InsensitiveEqualityCompararer());
                weaponsOriginalName = new Dictionary<string, Weapon>(new InsensitiveEqualityCompararer());
                weaponsAltName = new Dictionary<string, Weapon>(new InsensitiveEqualityCompararer());

                foreach (Weapon weapon in set)
                {
                    Regex reg = new Regex("([-\\. \\p{L}]+), ([-\\. \\p{L}]+)");

                    Match m = reg.Match(weapon.Name);

                    if (m.Success)
                    {
                        weapon.OriginalName = weapon.Name;
                        weapon.Name = m.Groups[2].Value + " " + m.Groups[1].Value;
                        weaponsOriginalName.Add(weapon.OriginalName, weapon);
                    }

                    weapons[weapon.Name] = weapon;

                    if (weapon.Plural != null && weapon.Plural.Length > 0)
                    {
                        weaponsPlural[weapon.Plural] = weapon;
                    }

                    if (weapon.AltName != null && weapon.AltName.Length > 0)
                    {
                        weaponsAltName[weapon.AltName] = weapon;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());

            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }
        public static string ReplaceOriginalWeaponNames(string text, bool uppercase)
        {
            string returnText = text;

            foreach (Weapon weapon in Weapons.Values)
            {
                if (weapon.OriginalName != null)
                {
                    string name = weapon.Name;

                    if (uppercase)
                    {
                        name = StringCapitalizer.Capitalize(name);
                    }
                    else
                    {
                        name = name.ToLower();
                    }

                    returnText = new Regex(Regex.Escape(weapon.OriginalName), RegexOptions.IgnoreCase).Replace(returnText, name);
                }
            }

            return returnText;
        }

    }
}

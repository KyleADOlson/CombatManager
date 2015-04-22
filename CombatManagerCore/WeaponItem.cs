/*
 *  WeaponItem.cs
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
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

namespace CombatManager
{

    public class WeaponItemPlus : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        private string _Name;
        private DieRoll _Roll;

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

        public DieRoll Roll
        {
            get { return _Roll; }
            set
            {
                if (_Roll != value)
                {
                    _Roll = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Roll")); }
                }
            }
        }
    }

    public class WeaponItem : INotifyPropertyChanged
    {

        

        public event PropertyChangedEventHandler PropertyChanged;

        private int _Count;
        private Weapon _Weapon;
        private bool _Masterwork;
        private bool _Broken;
        private int _MagicBonus;
        private String _SpecialAbilities;
        private bool _MainHand;
		private bool _TwoHanded;
        private string _Plus;
        private bool _NoMods;
        private DieStep _Step;
        private Dictionary<string, WeaponItemPlus> _PlusList;

        


        private SortedDictionary<string, string> specialAbilitySet;



        public WeaponItem() 
        {
            specialAbilitySet = new SortedDictionary<string, string>();
            
        }
        public WeaponItem(Attack attack)
        {

            specialAbilitySet = new SortedDictionary<string, string>();
            Weapon = (Weapon)attack.Weapon.Clone();
            Count = attack.Count;
            MagicBonus = attack.MagicBonus;
            Masterwork = attack.Masterwork;
            Broken = attack.Broken;
            SpecialAbilities = attack.SpecialAbilities;
			TwoHanded = attack.TwoHanded;
            

            if (Weapon.Class == "Natural" )
            {

                Step = attack.Damage.Step;
                if (attack.Plus != null)
                {
                    Plus = attack.Plus;
                }
            }
            
        }


        public WeaponItem(Weapon weapon)
        {
            Weapon = (Weapon)weapon.Clone();
            specialAbilitySet = new SortedDictionary<string, string>();
            Count = 1;
            MagicBonus = 0;
            Masterwork = false;
			TwoHanded = weapon.TwoHanded;

            if (Weapon.Class == "Natural")
            {
                Step = weapon.DamageDie.Step;
            }
        }

        public object Clone()
        {
            WeaponItem item = new WeaponItem();

            item.Weapon = (Weapon)Weapon.Clone();
            item.Count = Count;
            item.MagicBonus = MagicBonus;
            item.Masterwork = Masterwork;
            item.Broken = Broken;
            item.SpecialAbilities = SpecialAbilities;
            item.MainHand = MainHand;
            item.Plus = Plus;
            item.Step = Step;
            item.NoMods = NoMods;
			item.TwoHanded = TwoHanded;

            return item;
        }


        public int Count
        {
            get { return _Count; }
            set
            {
                if (_Count != value)
                {
                    _Count = value;
					OnPropertyChanged("Count");
                }
            }
        }
        public Weapon Weapon
        {
            get { return _Weapon; }
            set
            {
                if (_Weapon != value)
                {
                    _Weapon = value;
					OnPropertyChanged("Weapon");
                }
            }
        }
        public bool Masterwork
        {
            get { return _Masterwork; }
            set
            {
                if (_Masterwork != value)
                {
                    _Masterwork = value;
					OnPropertyChanged("Masterwork");
                }
            }
        }
        public bool Broken
        {
            get { return _Broken; }
            set
            {
                if (_Broken != value)
                {
                    _Broken = value;
					OnPropertyChanged("Broken");
                }
            }
        }
        public int MagicBonus
        {
            get { return _MagicBonus; }
            set
            {
                if (_MagicBonus != value)
                {
                    _MagicBonus = value;
					OnPropertyChanged("MagicBonus");
                }
            }
        }
        public bool MainHand
        {
            get { return _MainHand; }
            set
            {
                if (_MainHand != value)
                {
                    _MainHand = value;
					OnPropertyChanged("MainHand");
                }
            }
        }

		public bool TwoHanded
		{
			get { return _TwoHanded; }
			set
			{
				if (_TwoHanded != value)
				{
					_TwoHanded = value;
					OnPropertyChanged("TwoHanded");
				}
			}
		}

		protected void OnPropertyChanged(string propertyName)
		{
			if(PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

        public String SpecialAbilities
        {
            get { return _SpecialAbilities; }
            set
            {
                if (_SpecialAbilities != value)
                {
                    _SpecialAbilities = value;

                    //update special ability set
                    specialAbilitySet.Clear();
                    if (_SpecialAbilities != null)
                    {
						string text = _SpecialAbilities;
                        foreach (WeaponSpecialAbility w in WeaponSpecialAbility.SpecialAbilities)
                        {
                           	if (w.Name.Contains(" "))
							{

                                text = FindSpecial(text, w.Name);

                                if (w.AltName != null && w.AltName.Length > 0)
                                {

                                    text = FindSpecial(text, w.AltName);
                                }
	
							}
                        }
						
						 foreach (WeaponSpecialAbility w in WeaponSpecialAbility.SpecialAbilities)
                        {
                           	if (!w.Name.Contains(" "))
							{

                                text = FindSpecial(text, w.Name);

                                if (w.AltName != null && w.AltName.Length > 0)
                                {

                                    text = FindSpecial(text, w.AltName);
                                }
									
							}
                        }
                    }

					OnPropertyChanged("SpecialAbilities");
					OnPropertyChanged("SpecialAbilitySet");
                }
            }
        }

        private string FindSpecial(string text, string name)
        {

            Regex regSpec = new Regex("(^| )" + name + "( |$)", RegexOptions.IgnoreCase);

            return regSpec.Replace(text, delegate(Match m)
            {

                specialAbilitySet[name] = name;
                return "";
            });
        }

        public string Plus
        {
            get { return _Plus; }
            set
            {
                if (_Plus != value)
                {
                    _Plus = value;
					OnPropertyChanged("Plus");
                    if (_PlusList != null)
                    {
                        _PlusList = null;
                    }
                }
            }
        }

        public bool NoMods
        {
            get { return _NoMods; }
            set
            {
                if (_NoMods != value)
                {
                    _NoMods = value;
                    OnPropertyChanged("NoMods");
                }
            }
        }


        public DieStep Step
        {
            get { return _Step; }
            set
            {
                if (_Step != value)
                {
                    _Step = value;
					OnPropertyChanged("Step");
                }
            }
        }

        [XmlIgnore]
        public string PlusText
        {
            get
            {
                string text = Plus;



                if (SpecialAbilitySet != null)
                {
                    foreach (string ab in SpecialAbilitySet.Values)
                    {
                        WeaponSpecialAbility sp = WeaponSpecialAbility.SpecialAbilities.Find(delegate(WeaponSpecialAbility wp)
                        {
                            return string.Compare(wp.Name, ab) == 0;
                        }
                                    );

                        if (sp != null)
                        {
                            if (sp.Plus != null && sp.Plus.Length > 0)
                            {
                                text = Attack.AddPlus(text, sp.Plus);
                            }
                        }

                    }
                }

                if (Weapon.Special != null)
                {
                    if (Weapon.Special.Contains("grapple"))
                    {
                        text = Attack.AddPlus(text, "grab");
                    }
                    if (Weapon.Special.Contains("trip"))
                    {
                        text = Attack.AddPlus(text, "trip");
                    }
                    if (Weapon.Special.Contains("disarm"))
                    {
                        text = Attack.AddPlus(text, "disarm");
                    }
                }

                return text;
            }
        }

        public bool HasSpecialAbility(string name)
        {
            return SpecialAbilitySet.ContainsKey(name);
        }
            

        [XmlIgnore]
        public SortedDictionary<string, string> SpecialAbilitySet
        {
            get
            {
                return specialAbilitySet;
            }
            set
            {
                specialAbilitySet = value;

                string text = null;

                if (specialAbilitySet != null)
                {
                    bool first = true;
                    foreach (string ab in specialAbilitySet.Values)
                    {
                        if (first)
                        {
                            first = false;
                        }
                        else
                        {
                            text += " ";
                        }

                        text += ab;
                    }
                }

                _SpecialAbilities = text;

				OnPropertyChanged("SpecialAbilitySet");
				OnPropertyChanged("SpecialAbilities");                
            }
        }

        [XmlIgnore]
        public string Name
        {
            get
            {
                return Weapon.Name;
            }

        }


        [XmlIgnore]
        public string FullName
        {
            get
            {
                string text = "";
				
				if (Count != 1)
				{
					text += (Count.ToString()) + " ";
				}

                if (MagicBonus != 0)
                {
                    text += CMStringUtilities.PlusFormatNumber(MagicBonus) + " ";

                    if (SpecialAbilities != null && SpecialAbilities.Length > 0)
                    {
                        text += SpecialAbilities + " ";
                    }
                }
                else if (Masterwork)
                {
                    text += "mwk ";
                }
                else if (Broken)
                {
                    text += "broken ";
                }

                text += Weapon.Name + (Count != 1 ? "s" : "");


                if (Weapon.AltDamage)
                {
                    text += " " + Monster.StatText(Weapon.AltDamageStat) + " " + (Weapon.AltDamageDrain ? "drain" : "damage");
                }


				if (Plus != null && Plus.Length > 0)
				{
					text += " plus " + Plus;	
				}

                return text;

            }
        }



        public Dictionary<string, WeaponItemPlus> PlusList
        {
            get
            {
                if (_PlusList == null)
                {
                    ParsePlusList();
                }
                return _PlusList;
            }
        }

        private void ParsePlusList()
        {
            _PlusList = new Dictionary<string,WeaponItemPlus>();
            if (_Plus != null)
            {
                Regex regex = new Regex("((?<dieroll>([0-9]+)d([0-9]+)((\\+|-)[0-9]+)?) +)?(?<name>[-\\p{L}0-9 ]+?)(( +and +)|($))");

                foreach (Match m in regex.Matches(_Plus))
                {
                    WeaponItemPlus p = new WeaponItemPlus();
                    p.Name = m.Groups["name"].Value;
                    if (m.Groups["dieroll"].Success)
                    {
                        p.Roll = Monster.FindNextDieRoll(m.Groups["dieroll"].Value);
                    }
                    _PlusList[p.Name] = p;

                }
            }
        }


    }

    
}

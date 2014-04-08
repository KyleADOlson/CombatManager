/*
 *  Attack.cs
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
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Globalization;
using System.IO;

namespace CombatManager
{
    public class Attack : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;



        private int _Count;
        private string _Name;
        private List<int> _Bonus;
        private DieRoll _Damage;
        private DieRoll _OffHandDamage;
        private string _Plus;
        private int _CritRange;
        private int _CritMultiplier;
        private Weapon _BaseWeapon;
        private bool _Masterwork;
        private bool _Broken;
        private int _MagicBonus;
        private string _SpecialAbilities;
        private bool _RangedTouch;
        private bool _AltDamage;
        private Stat _AltDamageStat;
        private bool _AltDamageDrain;
		private bool _TwoHanded;

        private static string _SpecialAbilityString;



        public Attack()
        {
        }


        public Attack(int count, string name, int bonus, DieRoll damage, string plus)
        {
            Count = count;
            Name = name;
            Bonus = new List<int>();
            Bonus.Add(bonus);
            Damage = damage;
            Plus = plus;
            CritRange = 20;
            CritMultiplier = 2;
        }

        public Attack(int count, string name, DieRoll damage, string plus)
        {
            Count = count;
            Name = name;
            Bonus = new List<int>();
            Damage = damage;
            Plus = plus;
            CritRange = 20;
            CritMultiplier = 2;
        }


        public object Clone()
        {
            Attack atk = new Attack();

            atk._Count = _Count;
            atk._Name = _Name;
            atk._Bonus = new List<int>(_Bonus);
            atk._Damage = _Damage;
            atk._OffHandDamage = _OffHandDamage;
            atk._Plus = _Plus;
            atk._CritRange = _CritRange;
            atk._CritMultiplier = _CritMultiplier;
            atk._BaseWeapon = _BaseWeapon;
            atk._Masterwork = _Masterwork;
            atk._Broken = _Broken;
            atk._MagicBonus = _MagicBonus;
            atk._SpecialAbilities = _SpecialAbilities;
            atk._RangedTouch = _RangedTouch;
            atk._AltDamage = _AltDamage;
            atk._AltDamageStat = _AltDamageStat;
            atk._AltDamageDrain = _AltDamageDrain;
			atk._TwoHanded = _TwoHanded;

            return atk;
        }
        public static string AddPlus(string plus, string newPlus)
        {
            string text = plus;

            if (text == null || text.Trim().Length == 0)
            {
                text = newPlus;
            }
            else
            {
                text = text.Trim();

                Regex reg = new Regex(Regex.Escape(newPlus), RegexOptions.IgnoreCase);

                if (!(reg.Match(text).Success))
                {
                    text += " and " + newPlus;
                }
            }
            return text;
        }
        public int Count
        {
            get { return _Count; }
            set
            {
                if (_Count != value)
                {
                    _Count = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Count")); }
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
        public List<int> Bonus
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
        public DieRoll Damage
        {
            get { return _Damage; }
            set
            {
                if (_Damage != value)
                {
                    _Damage = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Damage")); }
                }
            }
        }
        public DieRoll OffHandDamage
        {
            get { return _OffHandDamage; }
            set
            {
                if (_OffHandDamage != value)
                {
                    _OffHandDamage = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("OffHandDamage")); }
                }
            }
        }
        public String Plus
        {
            get { return _Plus; }
            set
            {
                if (_Plus != value)
                {
                    _Plus = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Plus")); }
                }
            }
        }
        public int CritRange
        {
            get { return _CritRange; }
            set
            {
                if (_CritRange != value)
                {
                    _CritRange = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("CritRange")); }
                }
            }
        }
        public int CritMultiplier
        {
            get { return _CritMultiplier; }
            set
            {
                if (_CritMultiplier != value)
                {
                    _CritMultiplier = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("CritMultiplier")); }
                }
            }
        }
        public Weapon Weapon
        {
            get { return _BaseWeapon; }
            set
            {
                if (_BaseWeapon != value)
                {
                    _BaseWeapon = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("BaseWeapon")); }

					this.TwoHanded = _BaseWeapon.TwoHanded;
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Masterwork")); }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Broken")); }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("MagicBonus")); }
                }
            }
        }
        public String SpecialAbilities
        {
            get { return _SpecialAbilities; }
            set
            {
                if (_SpecialAbilities != value)
                {
                    _SpecialAbilities = value;
					
                    if (PropertyChanged != null) 
					{ 
						PropertyChanged(this, new PropertyChangedEventArgs("SpecialAbilities")); 	
					}
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
		public bool TwoHanded
		{
			get { return _TwoHanded; }
			set
			{
				if (_TwoHanded != value)
				{
					_TwoHanded = value;
					if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("TwoHanded")); }
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
        [XmlIgnore]
        public string Text
        {
            get {return AttackText(this); }
        }
        public override string ToString()
        {
            return Text;
        }
        public static string DieRegexString
        {
            get
            {
                return "([0-9/]+)(d)([0-9]+)((\\+|-)[0-9]+)?";
            }
        }
        public static string RegexString(string attackName)
        {
            string text =
            "((and )|(or ))?(?<count>[0-9]+( )+)?((?<magicbonus>(\\+|-)[0-9]+) +(?<specialabilities>((" + SpecialAbilityString + ") )*)?)?(?<broken>broken )?(?<mwk>((mwk)|(masterwork)) )?(?<name>";

            if (attackName == null)
            {
                text += "[\\p{L}][- \\p{L}]*)(s?";
            }

            else
            {
                text += Regex.Escape(attackName) + ")(s?";
            }

            text += ")( )((?<incorrectmagicbonus>\\+[0-9]+ +)?(\\([- \\p{L}]+\\) )?(?<bonus>[-\\+0-9/]+))( melee)?(?<rangedtouch> ranged touch)?( +\\()(?<damage>" + DieRegexString + ")(/(?<offhanddamage>" + DieRegexString + "))?(?<critrange>/[0-9]+-[0-9]+)?(?<critmultiplier>/x[0-9]+)?(?<altdamage> (?<altdamagestat>(Strength)|(Dexterity)|(Constitution)|(Intelligence)|(Wisdom)|(Charisma)) (?<altdamagetype>(damage)|(drain)))?(, (?<savetype>((Fort)|(Ref)|(Will)))\\. DC (?<saveval>[0-9]+) (?<saveresult>((half)|(negates))) ?)?(?<allplus> (plus|and) (?<plus>[- \\p{L}0-9\\.\\+;]+))?(\\))";

            return text;
        }
        private static string SpecialAbilityString
        {
            get
            {
                if (_SpecialAbilityString == null)
                {
                    StringBuilder bld = new StringBuilder();

                    bool first = true;

                    foreach (WeaponSpecialAbility w in WeaponSpecialAbility.SpecialAbilities)
                    {
                        bld.Append(first ? w.Name : ("|" + w.Name));

                        if (w.AltName != null && w.AltName.Length > 0)
                        {

                            bld.Append("|" + w.AltName);
                        }

                        first = false;
                    }

                    _SpecialAbilityString = bld.ToString();
                }

                return _SpecialAbilityString;
            }
        }
        public static Attack ParseAttack(Match m)
        {
            Attack info = new Attack();

            if (m.Groups["count"].Success)
            {
                info.Count = int.Parse(m.Groups["count"].Value);
            }
            else
            {
                info.Count = 1;
            }

            info.Name = m.Groups["name"].Value.Trim() ;

            if (info.Count > 1 && info.Name[info.Name.Length - 1] == 's')
            {
                info.Name = info.Name.Substring(0, info.Name.Length - 1);
            }


            info.Bonus = new List<int>();

            Regex bonus = new Regex("((-|\\+)[0-9]+)(/)?");

            foreach (Match b in bonus.Matches(m.Groups["bonus"].Value))
            {
                info.Bonus.Add(int.Parse(b.Groups[1].Value));
            };


            if (m.Groups["rangedtouch"].Success)
            {
                info.RangedTouch = true;
            }


            info.Damage = Monster.FindNextDieRoll(m.Groups["damage"].Value, 0);

			if (m.Groups["offhanddamage"].Success)
            {
                info.OffHandDamage = Monster.FindNextDieRoll(m.Groups["offhanddamage"].Value, 0);
            }

            if (m.Groups["altdamage"].Success)
            {
                info.AltDamage = true;
                info.AltDamageStat = Monster.StatFromName(m.Groups["altdamagestat"].Value);
                info.AltDamageDrain = (String.Compare(m.Groups["altdamagetype"].Value, "drain", true) == 0);
            }

            info.Plus = null;

            if (m.Groups["critrange"].Success)
            {
                info.CritRange = FindNextNum(m.Groups["critrange"].Value);
            }
            else
            {
                info.CritRange = 20;
            }

            if (m.Groups["critmultiplier"].Success)
            {
                info.CritMultiplier = FindNextNum(m.Groups["critmultiplier"].Value);
            }
            else
            {
                info.CritMultiplier = 2;
            }

            info.Masterwork = m.Groups["mwk"].Success;
            info.Broken = m.Groups["broken"].Success;

            if (m.Groups["magicbonus"].Success)
            {
                info.MagicBonus = int.Parse(m.Groups["magicbonus"].Value);

                if (m.Groups["specialabilities"].Success)
                {
                    info.SpecialAbilities = m.Groups["specialabilities"].Value.Trim();
                }
            
            }
            else if (m.Groups["incorrectmagicbonus"].Success)
            {
                info.MagicBonus = int.Parse(m.Groups["incorrectmagicbonus"].Value);                
            }

            if (m.Groups["allplus"].Success)
            {
                info.Plus = m.Groups["plus"].Value;
            }

            if (Weapon.Weapons.ContainsKey(info.Name))
            {
                info.Weapon = Weapon.Weapons[info.Name];
            }
            else if (Weapon.WeaponsAltName.ContainsKey(info.Name))
            {
                info.Weapon = Weapon.WeaponsAltName[info.Name];
            }
            else if (info.Name[info.Name.Length - 1] == 's' && Weapon.Find(info.Name.Substring(0, info.Name.Length - 1)) != null)
            {
                info.Weapon = Weapon.Find(info.Name.Substring(0, info.Name.Length - 1));
            }
            else if (Weapon.WeaponsPlural.ContainsKey(info.Name + "s"))
            {
                info.Weapon = Weapon.WeaponsPlural[info.Name + "s"];
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Unknown weapon: \"" + info.Name + "\"");                
            }


            return info;
        }
        private static int FindNextNum(string text)
        {
            int val = 0;

            Regex regEx = new Regex("-?[0-9]+");

            Match m = regEx.Match(text);

            if (m.Success)
            {
                val = int.Parse(m.Value);
            }

            return val;

        }
        private static string AttackText(Attack info)
        {
            string text = "";

            if (info.Count != 1)
            {
                text += (info.Count.ToString()) + " ";
            }

            if (info.MagicBonus != 0)
            {
                text += info.MagicBonus.PlusFormat() +" ";

                if (info.SpecialAbilities != null && info.SpecialAbilities.Length > 0)
                {
                    text += info.SpecialAbilities + " ";
                }
            }
            else if (info.Masterwork)
            {
                text += "mwk ";
            }
            else if (info.Broken)
            {
                text += "broken ";
            }

            text += info.Name + (info.Count != 1 ? "s" : "");

            if (info.RangedTouch)
            {
                text += " ranged touch";
            }

            text += " " + AttackBonusText(info.Bonus) + " (";

            text += Monster.DieRollText(info.Damage);

            if (info.OffHandDamage != null)
            {
                text += "/" + Monster.DieRollText(info.OffHandDamage);
            }

            if (info.CritRange != 20)
            {
                text += "/" + info.CritRange + "-20";
            }
            if (info.CritMultiplier != 2)
            {
                text += "/x" + info.CritMultiplier;
            }

            if (info.AltDamage)
            {
                text += " " + Monster.StatText(info.AltDamageStat) + " " + (info.AltDamageDrain ? "drain" : "damage");
            }

            if (info.Plus != null && info.Plus.Length > 0)
            {
                text += " plus " + info.Plus;
            }


            text += ")";


            return text;
        }
        private static string AttackBonusText(List<int> bonus)
        {
            string text = "";

            for (int i = 0; i < bonus.Count; i++)
            {
                if (i > 0)
                {
                    text += "/";

                }
                text += bonus[i].PlusFormat();
            }

            return text;
        }		
		[XmlIgnore]		
		private string FullName
		{
			get
			{
				string text = "";
	
				if (MagicBonus != 0)
				{
					text += MagicBonus.PlusFormat() + " ";
	
					if (SpecialAbilities != null && SpecialAbilities.Length > 0)
					{
						text += SpecialAbilities + " ";
					}
				}
				else if (Masterwork)
				{
					text += "mwk ";
				}
	
				text += Name;
				
				return text;
			}
		}
    }
}

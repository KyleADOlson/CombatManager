/*
 *  ConditionBonus.cs
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
using System.ComponentModel;

namespace CombatManager
{
    public class ConditionBonus : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private int? _Str;
        private int? _Dex;
        private int? _Con;
        private int? _Int;
        private int? _Wis;
        private int? _Cha;
        private int? _StrSkill;
        private int? _DexSkill;
        private int? _ConSkill;
        private int? _IntSkill;
        private int? _WisSkill;
        private int? _ChaSkill;
        private int? _Dodge;
        private int? _Armor;
        private int? _Shield;
        private int? _NaturalArmor;
        private int? _Deflection;
        private int? _AC;
        private int? _Initiative;
        private int? _AllAttack;
        private int? _MeleeAttack;
        private int? _RangedAttack;
        private int? _AttackDamage;
        private int? _MeleeDamage;
        private int? _RangedDamage;
        private int? _Perception;
        private bool _LoseDex;//*
        private int? _Size;
        private int? _Fort;
        private int? _Ref;
        private int? _Will;
        private int? _AllSaves;
        private int? _AllSkills; 
        private int? _CMB;
        private int? _CMD;
        private bool _StrZero; 
        private bool _DexZero; 




        public ConditionBonus()
        {
        }

        public ConditionBonus(ConditionBonus bonus)
        {
            _Str = bonus._Str;
            _Dex = bonus._Dex;
            _Con = bonus._Con;
            _Int = bonus._Int;
            _Wis = bonus._Wis;
            _Cha = bonus._Cha;
            _StrSkill = bonus._StrSkill;
            _DexSkill = bonus._DexSkill;
            _ConSkill = bonus._ConSkill;
            _IntSkill = bonus._IntSkill;
            _WisSkill = bonus._WisSkill;
            _ChaSkill = bonus._ChaSkill;
            _Dodge = bonus._Dodge;
            _Armor = bonus._Armor;
            _Shield = bonus._Shield;
            _NaturalArmor = bonus._NaturalArmor;
			_Deflection = bonus._Deflection;
            _Initiative = bonus._Initiative;
            _AllAttack = bonus._AllAttack;
            _AC = bonus._AC;
            _MeleeAttack = bonus._MeleeAttack;
            _RangedAttack = bonus._RangedAttack;
            _AttackDamage = bonus._AttackDamage;
            _MeleeDamage = bonus._MeleeDamage;
            _RangedDamage = bonus._RangedDamage;
            _Perception = bonus._Perception;
            _LoseDex = bonus._LoseDex;
            _Size = bonus._Size;
            _Fort = bonus._Fort;
            _Ref = bonus._Ref;
            _Will = bonus._Will;
            _AllSaves = bonus._AllSaves;
            _AllSkills = bonus._AllSkills;
            _CMB = bonus.CMB;
            _CMD = bonus.CMD;
            _StrZero = bonus._StrZero;
            _DexZero = bonus._DexZero; 
        }


        public int? Str
        {
            get { return _Str; }
            set
            {
                if (_Str != value)
                {
                    _Str = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Str")); }
                }
            }
        }
        public int? Dex
        {
            get { return _Dex; }
            set
            {
                if (_Dex != value)
                {
                    _Dex = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Dex")); }
                }
            }
        }
        public int? Con
        {
            get { return _Con; }
            set
            {
                if (_Con != value)
                {
                    _Con = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Con")); }
                }
            }
        }
        public int? Int
        {
            get { return _Int; }
            set
            {
                if (_Int != value)
                {
                    _Int = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Int")); }
                }
            }
        }
        public int? Wis
        {
            get { return _Wis; }
            set
            {
                if (_Wis != value)
                {
                    _Wis = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Wis")); }
                }
            }
        }
        public int? Cha
        {
            get { return _Cha; }
            set
            {
                if (_Cha != value)
                {
                    _Cha = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Cha")); }
                }
            }
        }
        public int? StrSkill
        {
            get { return _StrSkill; }
            set
            {
                if (_StrSkill != value)
                {
                    _StrSkill = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("StrSkill")); }
                }
            }
        }
        public int? DexSkill
        {
            get { return _DexSkill; }
            set
            {
                if (_DexSkill != value)
                {
                    _DexSkill = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("DexSkill")); }
                }
            }
        }
        public int? ConSkill
        {
            get { return _ConSkill; }
            set
            {
                if (_ConSkill != value)
                {
                    _ConSkill = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("ConSkill")); }
                }
            }
        }
        public int? IntSkill
        {
            get { return _IntSkill; }
            set
            {
                if (_IntSkill != value)
                {
                    _IntSkill = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("IntSkill")); }
                }
            }
        }
        public int? WisSkill
        {
            get { return _WisSkill; }
            set
            {
                if (_WisSkill != value)
                {
                    _WisSkill = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("WisSkill")); }
                }
            }
        }
        public int? ChaSkill
        {
            get { return _ChaSkill; }
            set
            {
                if (_ChaSkill != value)
                {
                    _ChaSkill = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("ChaSkill")); }
                }
            }
        }

        public int? Dodge
        {
            get { return _Dodge; }
            set
            {
                if (_Dodge != value)
                {
                    _Dodge = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Dodge")); }
                }
            }
        }
        public int? Armor
        {
            get { return _Armor; }
            set
            {
                if (_Armor != value)
                {
                    _Armor = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Armor")); }
                }
            }
        }
        public int? Shield
        {
            get { return _Shield; }
            set
            {
                if (_Shield != value)
                {
                    _Shield = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Shield")); }
                }
            }
        }
        public int? NaturalArmor
        {
            get { return _NaturalArmor; }
            set
            {
                if (_NaturalArmor != value)
                {
                    _NaturalArmor = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("NaturalArmor")); }
                }
            }
        }
        public int? Deflection
        {
            get { return _Deflection; }
            set
            {
                if (_Deflection != value)
                {
                    _Deflection = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Deflection")); }
                }
            }
        }
        public int? AC
        {
            get { return _AC; }
            set
            {
                if (_AC != value)
                {
                    _AC = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("AC")); }
                }
            }
        }
        public int? Initiative
        {
            get { return _Initiative; }
            set
            {
                if (_Initiative != value)
                {
                    _Initiative = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Initiative")); }
                }
            }
        }
        public int? AllAttack
        {
            get { return _AllAttack; }
            set
            {
                if (_AllAttack != value)
                {
                    _AllAttack = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("AllAttack")); }
                }
            }
        }
        public int? MeleeAttack
        {
            get { return _MeleeAttack; }
            set
            {
                if (_MeleeAttack != value)
                {
                    _MeleeAttack = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("MeleeAttack")); }
                }
            }
        }
        public int? RangedAttack
        {
            get { return _RangedAttack; }
            set
            {
                if (_RangedAttack != value)
                {
                    _RangedAttack = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("RangedAttack")); }
                }
            }
        }
        public int? AttackDamage
        {
            get { return _AttackDamage; }
            set
            {
                if (_AttackDamage != value)
                {
                    _AttackDamage = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("AttackDamage")); }
                }
            }
        }

        public int? MeleeDamage
        {
            get { return _MeleeDamage; }
            set
            {
                if (_MeleeDamage != value)
                {
                    _MeleeDamage = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("MeleeDamage")); }
                }
            }
        }
        public int? RangedDamage
        {
            get { return _RangedDamage; }
            set
            {
                if (_RangedDamage != value)
                {
                    _RangedDamage = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("RangedDamage")); }
                }
            }
        }


        public int? Perception
        {
            get { return _Perception; }
            set
            {
                if (_Perception != value)
                {
                    _Perception = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Perception")); }
                }
            }
        }
        public bool LoseDex
        {
            get { return _LoseDex; }
            set
            {
                if (_LoseDex != value)
                {
                    _LoseDex = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("LoseDex")); }
                }
            }
        }
        public int? Size
        {
            get { return _Size; }
            set
            {
                if (_Size != value)
                {
                    _Size = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Size")); }
                }
            }
        }
        public int? Fort
        {
            get { return _Fort; }
            set
            {
                if (_Fort != value)
                {
                    _Fort = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Fort")); }
                }
            }
        }
        public int? Ref
        {
            get { return _Ref; }
            set
            {
                if (_Ref != value)
                {
                    _Ref = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Ref")); }
                }
            }
        }
        public int? Will
        {
            get { return _Will; }
            set
            {
                if (_Will != value)
                {
                    _Will = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Will")); }
                }
            }
        }


        public int? AllSaves
        {
            get { return _AllSaves; }
            set
            {
                if (_AllSaves != value)
                {
                    _AllSaves = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("AllSaves")); }
                }
            }
        }
        public int? AllSkills
        {
            get { return _AllSkills; }
            set
            {
                if (_AllSkills != value)
                {
                    _AllSkills = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("AllSkills")); }
                }
            }
        }



        public int? CMB
        {
            get { return _CMB; }
            set
            {
                if (_CMB != value)
                {
                    _CMB = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("CMB")); }
                }
            }
        }
        public int? CMD
        {
            get { return _CMD; }
            set
            {
                if (_CMD != value)
                {
                    _CMD = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("CMD")); }
                }
            }
        }

        public bool StrZero
        {
            get { return _StrZero; }
            set
            {
                if (_StrZero != value)
                {
                    _StrZero = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("StrZero")); }
                }
            }
        }
        public bool DexZero
        {
            get { return _DexZero; }
            set
            {
                if (_DexZero != value)
                {
                    _DexZero = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("DexZero")); }
                }
            }
        }

        public object Clone()
        {
            return new ConditionBonus(this);
        }

    }
}

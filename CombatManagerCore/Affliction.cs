/*
 *  Affliction.cs
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
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace CombatManager
{
    public class Affliction : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private String _Name;
        private String _Type;
        private String _Cause;
        private String _SaveType;
        private int _Save;
        private DieRoll _Onset;
        private String _OnsetUnit;
        private bool _Immediate;
        private int _Frequency;
        private String _FrequencyUnit;
        private int _Limit;
        private String _LimitUnit;
        private String _SpecialEffectName;
        private DieRoll _SpecialEffectTime;
        private String _SpecialEffectUnit;
        private String _OtherEffect;
        private bool _Once;
        private DieRoll _DamageDie;
        private String _DamageType;
        private bool _IsDamageDrain;
        private DieRoll _SecondaryDamageDie;
        private String _SecondaryDamageType;
        private bool _IsSecondaryDamageDrain;
        private String _DamageExtra;
        private String _Cure;
        private String _Details;
        private String _Cost;

        public Affliction()
        {

        }

        public Affliction(Affliction a)
        {
            _Name = a._Name;
            _Type = a._Type;
            _Cause = a._Cause;
            _SaveType = a._SaveType;
            _Save = a._Save;
            if (a._Onset != null)
            {
                _Onset = (DieRoll)a._Onset.Clone();
            }
            _OnsetUnit = a._OnsetUnit;
            _Immediate = a._Immediate;
            _Frequency = a._Frequency;
            _FrequencyUnit = a._FrequencyUnit;
            _Limit = a._Limit;
            _LimitUnit = a._LimitUnit;
            _Once = a._Once;
            if (a._DamageDie != null)
            {
                _DamageDie = (DieRoll)a._DamageDie.Clone();
            }
            _DamageType = a._DamageType;
            _IsDamageDrain = a.IsDamageDrain;
            if (a._SecondaryDamageDie != null)
            {
                _SecondaryDamageDie = (DieRoll)a._SecondaryDamageDie.Clone();
            }
            _SecondaryDamageType = a._SecondaryDamageType;
            _IsSecondaryDamageDrain = a._IsSecondaryDamageDrain;
            _DamageExtra = a._DamageExtra;
            _SpecialEffectName = a._SpecialEffectName;
            if (a._SpecialEffectTime != null)
            {
                _SpecialEffectTime = (DieRoll)a._SpecialEffectTime.Clone();
            }
            _SpecialEffectUnit = a._SpecialEffectUnit;
            _OtherEffect = a.OtherEffect;
            _Cure = a._Cure;
            _Details = a._Details;
            _Cost = a._Cost;

        }

        public object Clone()
        {
            return new Affliction(this);
        }

        public static Affliction FromSpecialAbility(Monster monster, SpecialAbility sa)
        {
            Affliction a = null;

            Regex reg = new Regex(RegexString, RegexOptions.IgnoreCase);

            Match m = reg.Match(sa.Text);

            if (m.Success)
            {
                a = new Affliction();

                a.Type = sa.Name;

                if (m.Groups["afflictionname"].Success && m.Groups["afflictionname"].Value.Trim().Length > 0)
                {
                    a.Name = m.Groups["afflictionname"].Value.Trim();

                    a.Name = a.Name.Capitalize();

                    if (a.Name == "Filth Fever")
                    {
                        a.Name += " - " + monster.Name;
                    }
                }
                else
                {
                    a.Name = monster.Name + " " + a.Type;
                }

                if (m.Groups["cause"].Success)
                {
                    a.Cause = m.Groups["cause"].Value.Trim();
                }

                a.SaveType = m.Groups["savetype"].Value;
                a.Save = int.Parse(m.Groups["savedc"].Value);
                
                if (m.Groups["onset"].Success)
                {
                    if (m.Groups["immediateonset"].Success)
                    {
                        a.Immediate = true;
                    }
                    else
                    {
                        a.Onset = GetDie(m.Groups["onsetdie"].Value);

                        a.OnsetUnit = m.Groups["onsetunit"].Value;
                    }
                }
                if (m.Groups["once"].Success)
                {
                    a.Once = true;
                }
                else
                {
                    a.Frequency = int.Parse(m.Groups["frequencycount"].Value);

                    a.FrequencyUnit = m.Groups["frequencyunit"].Value;

                    if (m.Groups["Limit"].Success)
                    {
                        a.Limit = int.Parse(m.Groups["limitcount"].Value);
                        a.LimitUnit = m.Groups["limittype"].Value;

                    }
                }

                if (m.Groups["damageeffect"].Success)
                {

                    a.DamageDie = GetDie(m.Groups["damagedie"].Value);
                    a.DamageType = m.Groups["damagetype"].Value;

                    if (m.Groups["secondarydamagedie"].Success)
                    {

                        a.SecondaryDamageDie = GetDie(m.Groups["secondarydamagedie"].Value);
                        a.SecondaryDamageType = m.Groups["secondarydamagetype"].Value;
                    }
                }
                else if (m.Groups["specialeffect"].Success)
                {

                    a.SpecialEffectTime = GetDie(m.Groups["specialeffectdie"].Value);
                    a.SpecialEffectName = m.Groups["specialeffectname"].Value;
                    a.SpecialEffectUnit = m.Groups["specialeffectunit"].Value;
                }
                else if (m.Groups["othereffect"].Success)
                {
                    a.OtherEffect = m.Groups["othereffect"].Value.Trim();
                }

                a.Cure = m.Groups["cure"].Value;

                if (m.Groups["details"].Success)
                {
                    a.Details = m.Groups["details"].Value.Trim();
                }
                

            }

            return a;
        }


        public static string RegexString
        {
            get
            {
                return "((^)|((?<afflictionname>[\\p{L} ]+): ))(?<cause>[- \\p{L}]+)" + 
                    "; save (?<savetype>((Fort)|(Fortitude)|(Ref)|(Reflex)|(Will))) DC (?<savedc>[0-9]+)" +

                    "(?<onset>; onset (((?<onsetdie>([0-9]+)(d[0-9]+)?) ((?<onsetunit>[\\p{L}]+?)s?))|(?<immediateonset>immediate)))?" + 
                    "; frequency (((?<frequencycount>[0-9]+)([/ ]+)((?<frequencyunit>[\\p{L}]+?)s?)(?<limit> for (?<limitcount>[0-9]+) (?<limittype>[\\p{L}]+)s?)?)|(?<once>once))" +
                    "; effect (" + 
                    "(?<damageeffect>(?<damagedie>([0-9]+)(d[0-9]+)?) (?<damagetype>[\\p{L}]+)( and (?<secondarydamagedie>([0-9]+)(d[0-9]+)?) (?<secondarydamagetype>[\\p{L}]+))?( damage)?([-\\(\\) ,\\.\\p{L}0-9]+)?)|" +
                    "(?<specialeffect>(?<specialeffectname>[\\p{L}]+) for (?<specialeffectdie>([0-9]+)(d[0-9]+)?) (?<specialeffectunit>[\\p{L}]+?)s?)|" +
                    "(?<othereffect>[- '\\p{L}0-9]+)"+
                     ")" +
                    "(; cure (?<cure>([- +\\(\\),\\p{L}0-9]+)))?(\\.)?( (?<details>.+$))?";

            }
        }


        private static DieRoll GetDie(string text)
        {
            DieRoll dieroll;
            dieroll = Monster.FindNextDieRoll(text);

            if (dieroll == null)
            {
                dieroll = new DieRoll(int.Parse(text), 1, 0);
            }

            return dieroll;

        }

        private static string DieText(DieRoll roll)
        {
            if (roll.die == 1)
            {
                return roll.count.ToString();
            }
            else
            {
                return roll.Text;
            }
        }

        [XmlIgnore]
        public string Text
        {
            get
            {
                string text = "";

                text += Type;

                if (Cause != null)
                {
                    text += ", " + Cause;
                }

                text += "; ";

                text += "save " + SaveType + " DC " + Save + "; ";

                if (Immediate)
                {
                    text += "onset immediate; ";
                }
                else if (Onset != null)
                {
                    text += "onset " + DieText(Onset) + " " + OnsetUnit + ((Onset.count == 1 && Onset.die == 1) ? "" : "s") + "; ";
                }

                if (Once)
                {
                    text += "frequency once; ";
                }
                else
                {
                    text += "frequency " + Frequency + " " + FrequencyUnit + ((Frequency==1) ? "" : "s") + "; ";
                }

                text += "effect ";

                if (DamageDie != null)
                {
                    text += DieText(DamageDie) + " " + DamageType;

                    if (SecondaryDamageDie != null)
                    {

                        text += " " + DieText(DamageDie) + " " + DamageType;
                    }
                }
                else if (SpecialEffectTime != null)
                {
                    text += SpecialEffectName + " for " + DieText(SpecialEffectTime) + " " + SpecialEffectUnit;
                }
                else if (OtherEffect != null)
                {
                    text += OtherEffect;
                }

                if (DamageExtra != null)
                {
                    text += " " + DamageExtra;
                }

                text += "; ";

                text += "cure " + Cure;

                if (Details != null)
                {
                    text += ". " + Details;
                }

                return text;
            }
        }

        public override string ToString()
        {
            string text = "";

            if (Name != null)
            {
                text += Name + ": ";
            }

            text += Text;

            return text;
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
        public String Type
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

        public String Cause
        {
            get { return _Cause; }
            set
            {
                if (_Cause != value)
                {
                    _Cause = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Cause")); }
                }
            }
        }
        public String SaveType
        {
            get { return _SaveType; }
            set
            {
                if (_SaveType != value)
                {
                    _SaveType = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("SaveType")); }
                }
            }
        }
        public int Save
        {
            get { return _Save; }
            set
            {
                if (_Save != value)
                {
                    _Save = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Save")); }
                }
            }
        }
        public DieRoll Onset
        {
            get { return _Onset; }
            set
            {
                if (_Onset != value)
                {
                    _Onset = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Onset")); }
                }
            }
        }
        public String OnsetUnit
        {
            get { return _OnsetUnit; }
            set
            {
                if (_OnsetUnit != value)
                {
                    _OnsetUnit = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("OnsetUnit")); }
                }
            }
        }
        public bool Immediate
        {
            get { return _Immediate; }
            set
            {
                if (_Immediate != value)
                {
                    _Immediate = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Immediate")); }
                }
            }
        }
        public int Frequency
        {
            get { return _Frequency; }
            set
            {
                if (_Frequency != value)
                {
                    _Frequency = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Frequency")); }
                }
            }
        }
        public String FrequencyUnit
        {
            get { return _FrequencyUnit; }
            set
            {
                if (_FrequencyUnit != value)
                {
                    _FrequencyUnit = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("FrequencyUnit")); }
                }
            }
        }

        public int Limit
        {
            get { return _Limit; }
            set
            {
                if (_Limit != value)
                {
                    _Limit = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Limit")); }
                }
            }
        }
        public String LimitUnit
        {
            get { return _LimitUnit; }
            set
            {
                if (_LimitUnit != value)
                {
                    _LimitUnit = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("LimitUnit")); }
                }
            }
        }

        public String SpecialEffectName
        {
            get { return _SpecialEffectName; }
            set
            {
                if (_SpecialEffectName != value)
                {
                    _SpecialEffectName = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("SpecialEffectName")); }
                }
            }
        }

        public DieRoll SpecialEffectTime
        {
            get { return _SpecialEffectTime; }
            set
            {
                if (_SpecialEffectTime != value)
                {
                    _SpecialEffectTime = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("SpecialEffectTime")); }
                }
            }
        }
        public String SpecialEffectUnit
        {
            get { return _SpecialEffectUnit; }
            set
            {
                if (_SpecialEffectUnit != value)
                {
                    _SpecialEffectUnit = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("SpecialEffectUnit")); }
                }
            }
        }

        public String OtherEffect
        {
            get { return _OtherEffect; }
            set
            {
                if (_OtherEffect != value)
                {
                    _OtherEffect = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("OtherEffect")); }
                }
            }
        }

        public bool Once
        {
            get { return _Once; }
            set
            {
                if (_Once != value)
                {
                    _Once = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Once")); }
                }
            }
        }
        public DieRoll DamageDie
        {
            get { return _DamageDie; }
            set
            {
                if (_DamageDie != value)
                {
                    _DamageDie = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("DamageDie")); }
                }
            }
        }
        public String DamageType
        {
            get { return _DamageType; }
            set
            {
                if (_DamageType != value)
                {
                    _DamageType = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("DamageType")); }
                }
            }
        }
        public bool IsDamageDrain
        {
            get { return _IsDamageDrain; }
            set
            {
                if (_IsDamageDrain != value)
                {
                    _IsDamageDrain = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("IsDamageDrain")); }
                }
            }
        }
        public DieRoll SecondaryDamageDie
        {
            get { return _SecondaryDamageDie; }
            set
            {
                if (_SecondaryDamageDie != value)
                {
                    _SecondaryDamageDie = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("SecondaryDamageDie")); }
                }
            }
        }
        public String SecondaryDamageType
        {
            get { return _SecondaryDamageType; }
            set
            {
                if (_SecondaryDamageType != value)
                {
                    _SecondaryDamageType = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("SecondaryDamageType")); }
                }
            }
        }
        public bool IsSecondaryDamageDrain
        {
            get { return _IsSecondaryDamageDrain; }
            set
            {
                if (_IsSecondaryDamageDrain != value)
                {
                    _IsSecondaryDamageDrain = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("IsSecondaryDamageDrain")); }
                }
            }
        }
        public String DamageExtra
        {
            get { return _DamageExtra; }
            set
            {
                if (_DamageExtra != value)
                {
                    _DamageExtra = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("DamageExtra")); }
                }
            }
        }
        public String Cure
        {
            get { return _Cure; }
            set
            {
                if (_Cure != value)
                {
                    _Cure = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Cure")); }
                }
            }
        }
        public String Details
        {
            get { return _Details; }
            set
            {
                if (_Details != value)
                {
                    _Details = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Details")); }
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


    }
}

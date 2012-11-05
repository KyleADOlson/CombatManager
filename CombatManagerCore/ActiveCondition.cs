/*
 *  ActiveCondition.cs
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


namespace CombatManager
{
    public class ActiveCondition : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private Condition _Condition;
        private int? _Turns;
        private int? _EndTurn;
        private string _Details;
        private InitiativeCount _InitiativeCount;

        public ActiveCondition()
        {

        }


        public ActiveCondition(ActiveCondition a)
        {
            if (a._Condition != null)
            {
                _Condition = (Condition)a._Condition.Clone();
            }
            _Turns = a._Turns;
            _EndTurn = a._EndTurn;
            if (a._InitiativeCount != null)
            {
                _InitiativeCount = (InitiativeCount)a._InitiativeCount.Clone();
            }
            _Details = a._Details;
        }

        public object Clone()
        {
            return new ActiveCondition(this);
        }

        public Condition Condition
        {
            get { return _Condition; }
            set
            {
                if (_Condition != value)
                {
                    _Condition = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Condition")); }
                }
            }
        }

        public int? Turns
        {
            get { return _Turns; }
            set
            {
                if (_Turns != value)
                {
                    _Turns = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Turns")); }
                }
            }
        }

        [XmlIgnore]
        public int? EndTurn
        {
            get { return _EndTurn; }
            set
            {
                if (_EndTurn != value)
                {
                    _EndTurn = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Turns")); }
                }
            }
        }

        [XmlIgnore]
        public InitiativeCount InitiativeCount
        {
            get { return _InitiativeCount; }
            set
            {
                if (_InitiativeCount != value)
                {
                    _InitiativeCount = new InitiativeCount((InitiativeCount)value) ;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("InitiativeCount")); }
                }
            }
        }

        [XmlIgnore]
        public ConditionBonus Bonus
        {
            get
            {
                if (Condition != null)
                {
                    if (Condition.Spell != null)
                    {
                        return Condition.Spell.Bonus;
                    }

                    return Condition.Bonus;
                }
                return null;

            }
        }

        public string Details
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

    }
}

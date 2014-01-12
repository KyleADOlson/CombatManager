/*
 *  InitiativeCount.cs
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

namespace CombatManager
{
    public class InitiativeCount : INotifyPropertyChanged, IComparable<InitiativeCount>, IComparable
    {
        
        public event PropertyChangedEventHandler PropertyChanged;

        private int _Base;
        private int _Dex;
        private int _Tiebreaker;

        public InitiativeCount()
        {
        
        }

        public InitiativeCount(int baseval, int dex, int tiebreaker)
        {
            _Base = baseval;
            _Dex = dex;
            _Tiebreaker = tiebreaker;
        }

        public InitiativeCount(InitiativeCount count) : this(count._Base, count._Dex, count._Tiebreaker)
        {

        }

        public object Clone()
        {

            return new InitiativeCount(this);
        }

        public override bool Equals(object obj)
        {
            if (typeof(InitiativeCount) == obj.GetType())
            {
                return (this == (InitiativeCount)obj);
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return (this.Base << 8) ^ (this.Dex << 4) ^ (this.Tiebreaker);
        }

        public static bool operator == (InitiativeCount counta, InitiativeCount countb)
        {
            Object a = (object)counta;
            Object b = (object)countb;
            if (a == null || b == null)
            {
                return (a == null && b == null);
            }

            return counta.CompareTo(countb) == 0;
        }


        public static bool operator !=(InitiativeCount counta, InitiativeCount countb)
        {

            return !(counta == countb);
        }

        public static bool operator > (InitiativeCount counta, InitiativeCount countb)
        {
            return counta.CompareTo(countb) > 0;
        }

        public static bool operator < (InitiativeCount counta, InitiativeCount countb)
        {
            return counta.CompareTo(countb) < 0;
        }

        public static bool operator >= (InitiativeCount counta, InitiativeCount countb)
        {
            return counta.CompareTo(countb) >= 0;
        }

        public static bool operator <= (InitiativeCount counta, InitiativeCount countb)
        {
            return counta.CompareTo(countb) <= 0;
        }



        public int CompareTo(InitiativeCount other)
        {
            if (Base != other.Base)
            {
                return Base.CompareTo(other.Base);
            }

            if (Dex != other.Dex)
            {
                return Dex.CompareTo(other.Dex);
            }

            return Tiebreaker.CompareTo(other.Tiebreaker);

        }



        public int Base
        {
            get { return _Base; }
            set
            {
                if (_Base != value)
                {
                    _Base = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Base")); }
                }
            }
        }
        public int Dex
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
        public int Tiebreaker
        {
            get { return _Tiebreaker; }
            set
            {
                if (_Tiebreaker != value)
                {
                    _Tiebreaker = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Tiebreaker")); }
                }
            }
        }

        public string Text
        {
            get
            {
                return ToString();
            }
        }

        public override string ToString()
        {
            return _Base + "-" + _Dex + "-" + _Tiebreaker;
        }



        public int CompareTo(object obj)
        {
            if (obj.GetType() != typeof(InitiativeCount))
            {
                throw new ArgumentException("Cannot compare", "obj");
            }
            else
            {
                return CompareTo((InitiativeCount)obj);
            }
        }
    }
}

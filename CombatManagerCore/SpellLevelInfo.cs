/*
 *  SpellLevelInfo.cs
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
using System.Collections.ObjectModel;

namespace CombatManager
{
    public class SpellLevelInfo : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private int? _Level;
        private int? _PerDay;
        private bool _AtWill;
        private int? _More;
        private int? _Cast; 
        private bool _Constant;


        private ObservableCollection<SpellInfo> _Spells = new ObservableCollection<SpellInfo>();

        public SpellLevelInfo() { }

        public SpellLevelInfo (SpellLevelInfo old)
        {
            _Level = old._Level;
            _PerDay = old._PerDay;
            _AtWill = old._AtWill;
            _More = old._More;
            _Cast = old._Cast;
            _Constant = old._Constant;

            foreach (SpellInfo info in old.Spells)
            {
                _Spells.Add(new SpellInfo(info));
            }
        }

        public object Clone()
        {
            return new SpellLevelInfo(this);
        }

        public int? Level
        {
            get { return _Level; }
            set
            {
                if (_Level != value)
                {
                    _Level = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Level")); }
                }
            }
        }
        public int? PerDay
        {
            get { return _PerDay; }
            set
            {
                if (_PerDay != value)
                {
                    _PerDay = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("PerDay")); }
                }
            }
        }
        public bool AtWill
        {
            get { return _AtWill; }
            set
            {
                if (_AtWill != value)
                {
                    _AtWill = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("AtWill")); }
                }
            }
        }
        public int? More
        {
            get { return _More; }
            set
            {
                if (_More != value)
                {
                    _More = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("More")); }
                }
            }
        }
        public int? Cast
        {
            get { return _Cast; }
            set
            {
                if (_Cast != value)
                {
                    _Cast = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Cast")); }
                }
            }
        }


        public bool Constant
        {
            get { return _Constant; }
            set
            {
                if (_Constant != value)
                {
                    _Constant = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Constant")); }
                }
            }
        }

        public ObservableCollection<SpellInfo> Spells
        {
            get { return _Spells; }
            set
            {
                if (_Spells != value)
                {
                    _Spells = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Spells")); }
                }
            }
        }



    }
}

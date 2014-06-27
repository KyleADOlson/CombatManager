/*
 *  SpellInfo.cs
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
    public class SpellInfo : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private String _Name;
        private int? _DC;
        private Spell _Spell;
        private int? _Count;
        private int? _Cast;
        private bool _AlreadyCast;
        private String _Only;
        private String _Other;
        private bool _Mythic;

        public SpellInfo() { }

        public SpellInfo(SpellInfo old)
        {
            _Name = old._Name;
            _DC = old._DC;
            _Spell = old._Spell;
            _Count = old._Count;
            _Cast = old._Cast;
            _AlreadyCast = old._AlreadyCast;
            _Only = old._Only;
            _Other = old._Other;
            _Mythic = old._Mythic;
        }

        public object Clone()
        {
            return new SpellInfo(this);
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
        public int? DC
        {
            get { return _DC; }
            set
            {
                if (_DC != value)
                {
                    _DC = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("DC")); }
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
        public int? Count
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
        public bool AlreadyCast
        {
            get { return _AlreadyCast; }
            set
            {
                if (_AlreadyCast != value)
                {
                    _AlreadyCast = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("AlreadyCast")); }
                }
            }
        }
        public String Only
        {
            get { return _Only; }
            set
            {
                if (_Only != value)
                {
                    _Only = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Only")); }
                }
            }
        }

        public String Other
        {
            get { return _Other; }
            set
            {
                if (_Other != value)
                {
                    _Other = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Other")); }
                }
            }
        }



        public bool Mythic
        {
            get { return _Mythic; }
            set
            {
                if (_Mythic != value)
                {
                    _Mythic = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Mythic")); }
                }
            }
        }


    }
}

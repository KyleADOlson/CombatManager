/*
 *  ExportData.cs
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
    public class ExportData : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private List<Monster> _Monsters = new List<Monster>();
        private List<Spell> _Spells = new List<Spell>();
        private List<Feat> _Feats = new List<Feat>();
        private List<Condition> _Conditions = new List<Condition>();

        public List<Monster> Monsters
        {
            get { return _Monsters; }
            set
            {
                if (_Monsters != value)
                {
                    _Monsters = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Monsters")); }
                }
            }
        }
        public List<Spell> Spells
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
        public List<Feat> Feats
        {
            get { return _Feats; }
            set
            {
                if (_Feats != value)
                {
                    _Feats = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Feats")); }
                }
            }
        }
        public List<Condition> Conditions
        {
            get { return _Conditions; }
            set
            {
                if (_Conditions != value)
                {
                    _Conditions = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Conditions")); }
                }
            }
        }

    }
}

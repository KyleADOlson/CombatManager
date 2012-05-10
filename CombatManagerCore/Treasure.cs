/*
 *  Treasure.cs
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
  

    public class Good : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private String _Name;
        private int _Value;

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
        public int Value
        {
            get { return _Value; }
            set
            {
                if (_Value != value)
                {
                    _Value = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Value")); }
                }
            }
        }

    }

    public class Treasure : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private int _Level;
        private Coin _Coin;
        private List<Good> _Goods;
        private List<TreasureItem> _Items;

        public Treasure()
        {
            _Goods = new List<Good>();
            _Items = new List<TreasureItem>();
        }

        public int Level
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
        public Coin Coin
        {
            get { return _Coin; }
            set
            {
                if (_Coin != value)
                {
                    _Coin = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Coin")); }
                }
            }
        }
        public List<Good> Goods
        {
            get { return _Goods; }
            set
            {
                if (_Goods != value)
                {
                    _Goods = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Goods")); }
                }
            }
        }

        public List<TreasureItem> Items
        {
            get { return _Items; }
            set
            {
                if (_Items != value)
                {
                    _Items = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Items")); }
                }
            }
        }

        public decimal TotalValue
        {
            get
            {
                decimal val = 0;

                if (_Coin != null)
                {
                    val += _Coin.GPValue;
                }

                foreach (Good good in _Goods)
                {
                    val += (decimal)good.Value;
                }

                foreach (TreasureItem item in Items)
                {
                    val += (decimal)item.Value;
                }

                return val;
            }
        }

    }
}
